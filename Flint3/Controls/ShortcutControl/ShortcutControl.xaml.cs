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
        private HotkeySettings internalSettings;
        private HotkeySettings lastValidSettings;
        private HotkeySettingsControlHook hook;
        private bool _isActive;
        private bool disposedValue;

        private ShortcutDialogContentControl _shortcutDialogContent = new ShortcutDialogContentControl();
        private ContentDialog _settingShortcutDialog = null;


        public static readonly DependencyProperty HotkeySettingsProperty = DependencyProperty.Register("HotkeySettings", typeof(HotkeySettings), typeof(ShortcutControl), null);

        private HotkeySettings _hotkeySettings;
        public HotkeySettings HotkeySettings
        {
            get { return _hotkeySettings; }
            set
            {
                if (_hotkeySettings != value)
                {
                    _hotkeySettings = value;
                    SetValue(HotkeySettingsProperty, value);
                    PreviewKeysControl.ItemsSource = HotkeySettings.GetKeysList();
                    _shortcutDialogContent.Keys = HotkeySettings.GetKeysList();
                }
            }
        }

        public ShortcutControl()
        {
            InitializeComponent();
            internalSettings = new HotkeySettings();

            this.Unloaded += ShortcutControl_Unloaded;
            hook = new HotkeySettingsControlHook(Hotkey_KeyDown, Hotkey_KeyUp, Hotkey_IsActive, FilterAccessibleKeyboardEvents);

            if (App.MainWindow != null)
            {
                App.MainWindow.Activated += ShortcutDialog_SettingsWindow_Activated;
            }

            // We create the Dialog in C# because doing it in XAML is giving WinUI/XAML Island bugs when using dark theme.
            _settingShortcutDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "激活快捷键",
                Content = _shortcutDialogContent,
                PrimaryButtonText = "保存",
                SecondaryButtonText = "重置",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
            };
            _settingShortcutDialog.PrimaryButtonClick += ShortcutDialog_PrimaryButtonClick;
            _settingShortcutDialog.SecondaryButtonClick += ShortcutDialog_Reset;
            _settingShortcutDialog.Opened += ShortcutDialog_Opened;
            _settingShortcutDialog.Closing += ShortcutDialog_Closing;
        }

        private void ShortcutControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Dispose the HotkeySettingsControlHook object to terminate the hook threads when the textbox is unloaded
            hook?.Dispose();
            hook = null;

            if (App.MainWindow != null)
            {
                App.MainWindow.Activated -= ShortcutDialog_SettingsWindow_Activated;
            }

            _settingShortcutDialog.PrimaryButtonClick -= ShortcutDialog_PrimaryButtonClick;
            _settingShortcutDialog.Opened -= ShortcutDialog_Opened;
            _settingShortcutDialog.Closing -= ShortcutDialog_Closing;
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
                    _settingShortcutDialog.IsPrimaryButtonEnabled = false;
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

            _shortcutDialogContent.Keys = internalSettings.GetKeysList();

            if (internalSettings.GetKeysList().Count == 0)
            {
                // Empty, disable save button
                _settingShortcutDialog.IsPrimaryButtonEnabled = false;
            }
            else if (internalSettings.GetKeysList().Count == 1)
            {
                // 1 key, disable save button
                _settingShortcutDialog.IsPrimaryButtonEnabled = false;

                // Check if the one key is a hotkey
                if (internalSettings.Shift || internalSettings.Win || internalSettings.Alt || internalSettings.Ctrl)
                {
                    _shortcutDialogContent.IsError = false;
                }
                else
                {
                    _shortcutDialogContent.IsError = true;
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
            _settingShortcutDialog.IsPrimaryButtonEnabled = true;
            _shortcutDialogContent.IsError = false;
        }

        private void DisableKeys()
        {
            _settingShortcutDialog.IsPrimaryButtonEnabled = false;
            _shortcutDialogContent.IsError = true;
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
            if (!ComboIsValid(_hotkeySettings))
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
            _shortcutDialogContent.Keys = null;
            _shortcutDialogContent.Keys = HotkeySettings.GetKeysList();

            _settingShortcutDialog.XamlRoot = this.XamlRoot;
            _settingShortcutDialog.RequestedTheme = this.ActualTheme;
            await _settingShortcutDialog.ShowAsync();
        }

        private void ShortcutDialog_Reset(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _hotkeySettings = null;

            SetValue(HotkeySettingsProperty, _hotkeySettings);
            PreviewKeysControl.ItemsSource = HotkeySettings.GetKeysList();

            lastValidSettings = _hotkeySettings;

            _settingShortcutDialog.Hide();
        }

        private void ShortcutDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (ComboIsValid(lastValidSettings))
            {
                HotkeySettings = lastValidSettings.Clone();
            }

            PreviewKeysControl.ItemsSource = _hotkeySettings.GetKeysList();
            _settingShortcutDialog.Hide();
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

        // 失去焦点时禁用钩子，获得焦点则重新启用钩子
        private void ShortcutDialog_SettingsWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            args.Handled = true;
            if (args.WindowActivationState != WindowActivationState.Deactivated && (hook == null || hook.GetDisposedState() == true))
            {
                // If settings window gets focussed/activated again, we enable the keyboard hook to catch the keyboard input.
                hook = new HotkeySettingsControlHook(Hotkey_KeyDown, Hotkey_KeyUp, Hotkey_IsActive, FilterAccessibleKeyboardEvents);
            }
            else if (args.WindowActivationState == WindowActivationState.Deactivated && hook != null && hook.GetDisposedState() == false)
            {
                // If settings window lost focus/activation, we disable the keyboard hook to allow keyboard input on other windows.
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
