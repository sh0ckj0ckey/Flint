using System;
using Flint3.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.Resources;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Controls.ShortcutControl
{
    public sealed partial class ShortcutControl : UserControl, IDisposable
    {
        private readonly UIntPtr ignoreKeyEventFlag = (UIntPtr)0x5555;
        private bool _shiftKeyDownOnEntering;
        private bool _shiftToggled;
        private bool _enabled;
        private HotkeySettings hotkeySettings;
        private HotkeySettings internalSettings;
        private HotkeySettings lastValidSettings;
        private HotkeySettingsControlHook hook;
        private bool _isActive;
        private bool disposedValue;

        public string Header { get; set; }

        public string Keys { get; set; }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(ShortcutControl), null);
        public static readonly DependencyProperty HotkeySettingsProperty = DependencyProperty.Register("HotkeySettings", typeof(HotkeySettings), typeof(ShortcutControl), null);

        private ShortcutDialogContentControl c = new ShortcutDialogContentControl();
        private ContentDialog shortcutDialog;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                SetValue(IsActiveProperty, value);
                _enabled = value;

                if (value)
                {
                    EditButton.IsEnabled = true;
                }
                else
                {
                    EditButton.IsEnabled = false;
                }
            }
        }

        public HotkeySettings HotkeySettings
        {
            get
            {
                return hotkeySettings;
            }

            set
            {
                if (hotkeySettings != value)
                {
                    hotkeySettings = value;
                    SetValue(HotkeySettingsProperty, value);
                    PreviewKeysControl.ItemsSource = HotkeySettings.GetKeysList();
                    AutomationProperties.SetHelpText(EditButton, HotkeySettings.ToString());
                    c.Keys = HotkeySettings.GetKeysList();
                }
            }
        }

        public ShortcutControl()
        {
            InitializeComponent();
            internalSettings = new HotkeySettings();

            this.Unloaded += ShortcutControl_Unloaded;
            hook = new HotkeySettingsControlHook(Hotkey_KeyDown, Hotkey_KeyUp, Hotkey_IsActive, FilterAccessibleKeyboardEvents);
            ResourceLoader resourceLoader = ResourceLoader.GetForViewIndependentUse();

            if (App.MainWindow != null)
            {
                App.MainWindow.Activated += ShortcutDialog_SettingsWindow_Activated;
            }

            // We create the Dialog in C# because doing it in XAML is giving WinUI/XAML Island bugs when using dark theme.
            shortcutDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = resourceLoader.GetString("Activation_Shortcut_Title"),
                Content = c,
                PrimaryButtonText = resourceLoader.GetString("Activation_Shortcut_Save"),
                SecondaryButtonText = resourceLoader.GetString("Activation_Shortcut_Reset"),
                CloseButtonText = resourceLoader.GetString("Activation_Shortcut_Cancel"),
                DefaultButton = ContentDialogButton.Primary,
            };
            shortcutDialog.PrimaryButtonClick += ShortcutDialog_PrimaryButtonClick;
            shortcutDialog.SecondaryButtonClick += ShortcutDialog_Reset;
            shortcutDialog.Opened += ShortcutDialog_Opened;
            shortcutDialog.Closing += ShortcutDialog_Closing;
            AutomationProperties.SetName(EditButton, resourceLoader.GetString("Activation_Shortcut_Title"));
        }

        private void ShortcutControl_Unloaded(object sender, RoutedEventArgs e)
        {
            shortcutDialog.PrimaryButtonClick -= ShortcutDialog_PrimaryButtonClick;
            shortcutDialog.Opened -= ShortcutDialog_Opened;
            shortcutDialog.Closing -= ShortcutDialog_Closing;

            if (App.MainWindow != null)
            {
                App.MainWindow.Activated -= ShortcutDialog_SettingsWindow_Activated;
            }

            // Dispose the HotkeySettingsControlHook object to terminate the hook threads when the textbox is unloaded
            if (hook != null)
            {
                hook.Dispose();
            }

            hook = null;
        }

        private void KeyEventHandler(int key, bool matchValue, int matchValueCode)
        {
            switch ((VirtualKey)key)
            {
                case VirtualKey.LeftWindows:
                case VirtualKey.RightWindows:
                    internalSettings.Win = matchValue;
                    break;
                case VirtualKey.Control:
                case VirtualKey.LeftControl:
                case VirtualKey.RightControl:
                    internalSettings.Ctrl = matchValue;
                    break;
                case VirtualKey.Menu:
                case VirtualKey.LeftMenu:
                case VirtualKey.RightMenu:
                    internalSettings.Alt = matchValue;
                    break;
                case VirtualKey.Shift:
                case VirtualKey.LeftShift:
                case VirtualKey.RightShift:
                    _shiftToggled = true;
                    internalSettings.Shift = matchValue;
                    break;
                case VirtualKey.Escape:
                    internalSettings = new HotkeySettings();
                    shortcutDialog.IsPrimaryButtonEnabled = false;
                    return;
                default:
                    internalSettings.Code = matchValueCode;
                    break;
            }
        }

        // Function to send a single key event to the system which would be ignored by the hotkey control.
        private void SendSingleKeyboardInput(short keyCode, uint keyStatus)
        {
            NativeKeyboardHelper.INPUT inputShift = new NativeKeyboardHelper.INPUT
            {
                type = NativeKeyboardHelper.INPUTTYPE.INPUT_KEYBOARD,
                data = new NativeKeyboardHelper.InputUnion
                {
                    ki = new NativeKeyboardHelper.KEYBDINPUT
                    {
                        wVk = keyCode,
                        dwFlags = keyStatus,

                        // Any keyevent with the extraInfo set to this value will be ignored by the keyboard hook and sent to the system instead.
                        dwExtraInfo = ignoreKeyEventFlag,
                    },
                },
            };

            NativeKeyboardHelper.INPUT[] inputs = new NativeKeyboardHelper.INPUT[] { inputShift };

            _ = NativeMethods.SendInput(1, inputs, NativeKeyboardHelper.INPUT.Size);
        }

        private bool FilterAccessibleKeyboardEvents(int key, UIntPtr extraInfo)
        {
            // A keyboard event sent with this value in the extra Information field should be ignored by the hook so that it can be captured by the system instead.
            if (extraInfo == ignoreKeyEventFlag)
            {
                return false;
            }

            // If the current key press is tab, based on the other keys ignore the key press so as to shift focus out of the hotkey control.
            if ((VirtualKey)key == VirtualKey.Tab)
            {
                // Shift was not pressed while entering and Shift is not pressed while leaving the hotkey control, treat it as a normal tab key press.
                if (!internalSettings.Shift && !_shiftKeyDownOnEntering && !internalSettings.Win && !internalSettings.Alt && !internalSettings.Ctrl)
                {
                    return false;
                }

                // Shift was not pressed while entering but it was pressed while leaving the hotkey, therefore simulate a shift key press as the system does not know about shift being pressed in the hotkey.
                else if (internalSettings.Shift && !_shiftKeyDownOnEntering && !internalSettings.Win && !internalSettings.Alt && !internalSettings.Ctrl)
                {
                    // This is to reset the shift key press within the control as it was not used within the control but rather was used to leave the hotkey.
                    internalSettings.Shift = false;

                    SendSingleKeyboardInput((short)VirtualKey.Shift, (uint)NativeKeyboardHelper.KeyEventF.KeyDown);

                    return false;
                }

                // Shift was pressed on entering and remained pressed, therefore only ignore the tab key so that it can be passed to the system.
                // As the shift key is already assumed to be pressed by the system while it entered the hotkey control, shift would still remain pressed, hence ignoring the tab input would simulate a Shift+Tab key press.
                else if (!internalSettings.Shift && _shiftKeyDownOnEntering && !_shiftToggled && !internalSettings.Win && !internalSettings.Alt && !internalSettings.Ctrl)
                {
                    return false;
                }

                // Shift was pressed on entering but it was released and later pressed again.
                // Ignore the tab key and the system already has the shift key pressed, therefore this would simulate Shift+Tab.
                // However, since the last shift key was only used to move out of the control, reset the status of shift within the control.
                else if (internalSettings.Shift && _shiftKeyDownOnEntering && _shiftToggled && !internalSettings.Win && !internalSettings.Alt && !internalSettings.Ctrl)
                {
                    internalSettings.Shift = false;

                    return false;
                }

                // Shift was pressed on entering and was later released.
                // The system still has shift in the key pressed status, therefore pass a Shift KeyUp message to the system, to release the shift key, therefore simulating only the Tab key press.
                else if (!internalSettings.Shift && _shiftKeyDownOnEntering && _shiftToggled && !internalSettings.Win && !internalSettings.Alt && !internalSettings.Ctrl)
                {
                    SendSingleKeyboardInput((short)VirtualKey.Shift, (uint)NativeKeyboardHelper.KeyEventF.KeyUp);

                    return false;
                }
            }

            // Either the cancel or save button has keyboard focus.
            if (FocusManager.GetFocusedElement(LayoutRoot.XamlRoot).GetType() == typeof(Button))
            {
                return false;
            }

            return true;
        }

        private void Hotkey_KeyDown(int key)
        {
            KeyEventHandler(key, true, key);

            c.Keys = internalSettings.GetKeysList();

            if (internalSettings.GetKeysList().Count == 0)
            {
                // Empty, disable save button
                shortcutDialog.IsPrimaryButtonEnabled = false;
            }
            else if (internalSettings.GetKeysList().Count == 1)
            {
                // 1 key, disable save button
                shortcutDialog.IsPrimaryButtonEnabled = false;

                // Check if the one key is a hotkey
                if (internalSettings.Shift || internalSettings.Win || internalSettings.Alt || internalSettings.Ctrl)
                {
                    c.IsError = false;
                }
                else
                {
                    c.IsError = true;
                }
            }

            // Tab and Shift+Tab are accessible keys and should not be displayed in the hotkey control.
            if (internalSettings.Code > 0 && !internalSettings.IsAccessibleShortcut())
            {
                lastValidSettings = internalSettings.Clone();

                if (!ComboIsValid(lastValidSettings))
                {
                    DisableKeys();
                }
                else
                {
                    EnableKeys();
                }
            }
        }

        private void EnableKeys()
        {
            shortcutDialog.IsPrimaryButtonEnabled = true;
            c.IsError = false;

            // WarningLabel.Style = (Style)App.Current.Resources["SecondaryTextStyle"];
        }

        private void DisableKeys()
        {
            shortcutDialog.IsPrimaryButtonEnabled = false;
            c.IsError = true;

            // WarningLabel.Style = (Style)App.Current.Resources["SecondaryWarningTextStyle"];
        }

        private void Hotkey_KeyUp(int key)
        {
            KeyEventHandler(key, false, 0);
        }

        private bool Hotkey_IsActive()
        {
            return _isActive;
        }

        private void ShortcutDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            if (!ComboIsValid(hotkeySettings))
            {
                DisableKeys();
            }
            else
            {
                EnableKeys();
            }

            // Reset the status on entering the hotkey each time.
            _shiftKeyDownOnEntering = false;
            _shiftToggled = false;

            // To keep track of the shift key, whether it was pressed on entering.
            if ((NativeMethods.GetAsyncKeyState((int)VirtualKey.Shift) & 0x8000) != 0)
            {
                _shiftKeyDownOnEntering = true;
            }

            _isActive = true;
        }

        private async void OpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            c.Keys = null;
            c.Keys = HotkeySettings.GetKeysList();

            shortcutDialog.XamlRoot = this.XamlRoot;
            shortcutDialog.RequestedTheme = this.ActualTheme;
            await shortcutDialog.ShowAsync();
        }

        private void ShortcutDialog_Reset(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            hotkeySettings = null;

            SetValue(HotkeySettingsProperty, hotkeySettings);
            PreviewKeysControl.ItemsSource = HotkeySettings.GetKeysList();

            lastValidSettings = hotkeySettings;

            AutomationProperties.SetHelpText(EditButton, HotkeySettings.ToString());
            shortcutDialog.Hide();
        }

        private void ShortcutDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (ComboIsValid(lastValidSettings))
            {
                HotkeySettings = lastValidSettings.Clone();
            }

            PreviewKeysControl.ItemsSource = hotkeySettings.GetKeysList();
            AutomationProperties.SetHelpText(EditButton, HotkeySettings.ToString());
            shortcutDialog.Hide();
        }

        private static bool ComboIsValid(HotkeySettings settings)
        {
            if (settings != null && (settings.IsValid() || settings.IsEmpty()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ShortcutDialog_SettingsWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            args.Handled = true;
            if (args.WindowActivationState != WindowActivationState.Deactivated && (hook == null || hook.GetDisposedState() == true))
            {
                // If the PT settings window gets focussed/activated again, we enable the keyboard hook to catch the keyboard input.
                hook = new HotkeySettingsControlHook(Hotkey_KeyDown, Hotkey_KeyUp, Hotkey_IsActive, FilterAccessibleKeyboardEvents);
            }
            else if (args.WindowActivationState == WindowActivationState.Deactivated && hook != null && hook.GetDisposedState() == false)
            {
                // If the PT settings window lost focus/activation, we disable the keyboard hook to allow keyboard input on other windows.
                hook.Dispose();
                hook = null;
            }
        }

        private void ShortcutDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            _isActive = false;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (hook != null)
                    {
                        hook.Dispose();
                    }

                    hook = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
