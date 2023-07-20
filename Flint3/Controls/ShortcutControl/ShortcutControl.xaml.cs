using System;
using System.Collections.Generic;
using System.Diagnostics;
using Flint3.Helpers;
using Flint3.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using static System.Runtime.CompilerServices.RuntimeHelpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Flint3.Controls.ShortcutControl
{
    public sealed partial class ShortcutControl : UserControl
    {
        private readonly UIntPtr ignoreKeyEventFlag = (UIntPtr)0x5555;
        private Dictionary<VirtualKey, bool> modifierKeysOnEntering = new Dictionary<VirtualKey, bool>();
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
            _settingShortcutDialog.SecondaryButtonClick -= ShortcutDialog_Reset;
            _settingShortcutDialog.Opened -= ShortcutDialog_Opened;
            _settingShortcutDialog.Closing -= ShortcutDialog_Closing;
        }

        // Function to send a single key event to the system which would be ignored by the hotkey control.
        private void SendSingleKeyboardInput(short keyCode, uint keyStatus)
        {
            KeyboardHelper.INPUT inputShift = new KeyboardHelper.INPUT
            {
                type = KeyboardHelper.INPUTTYPE.INPUT_KEYBOARD,
                data = new KeyboardHelper.InputUnion
                {
                    ki = new KeyboardHelper.KEYBDINPUT
                    {
                        wVk = keyCode,
                        dwFlags = keyStatus,

                        // Any keyevent with the extraInfo set to this value will be ignored by the keyboard hook and sent to the system instead.
                        dwExtraInfo = ignoreKeyEventFlag,
                    },
                },
            };

            KeyboardHelper.INPUT[] inputs = new KeyboardHelper.INPUT[] { inputShift };

            _ = KeyboardHelper.SendInput(1, inputs, KeyboardHelper.INPUT.Size);

            Debug.WriteLine($"SendSingleKeyboardInput: {keyCode}");
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
                if (!internalSettings.Shift && !modifierKeysOnEntering.ContainsKey(VirtualKey.Shift) && !internalSettings.Win && !internalSettings.Alt && !internalSettings.Ctrl)
                {
                    return false;
                }

                // Shift was not pressed while entering but it was pressed while leaving the hotkey, therefore simulate a shift key press as the system does not know about shift being pressed in the hotkey.
                else if (internalSettings.Shift && !modifierKeysOnEntering.ContainsKey(VirtualKey.Shift) && !internalSettings.Win && !internalSettings.Alt && !internalSettings.Ctrl)
                {
                    // This is to reset the shift key press within the control as it was not used within the control but rather was used to leave the hotkey.
                    internalSettings.Shift = false;

                    SendSingleKeyboardInput((short)VirtualKey.Shift, (uint)KeyboardHelper.KeyEventF.KeyDown);

                    return false;
                }

                // Shift was pressed on entering and remained pressed, therefore only ignore the tab key so that it can be passed to the system.
                // As the shift key is already assumed to be pressed by the system while it entered the hotkey control, shift would still remain pressed, hence ignoring the tab input would simulate a Shift+Tab key press.
                else if (!internalSettings.Shift && modifierKeysOnEntering.ContainsKey(VirtualKey.Shift) && !internalSettings.Win && !internalSettings.Alt && !internalSettings.Ctrl)
                {
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

        private void Hotkey_KeyUp(int key)
        {
            KeyEventHandler(key, false, 0);
        }

        private void KeyEventHandler(int key, bool matchValue, int matchValueCode)
        {
            VirtualKey virtualKey = (VirtualKey)key;
            switch (virtualKey)
            {
                case VirtualKey.LeftWindows:
                case VirtualKey.RightWindows:
                    if (!matchValue && (modifierKeysOnEntering.ContainsKey(virtualKey) || !internalSettings.Win))
                    {
                        SendSingleKeyboardInput((short)virtualKey, (uint)KeyboardHelper.KeyEventF.KeyUp);
                        modifierKeysOnEntering.Remove(virtualKey);
                        Debug.WriteLine("Simulate Win KeyUp");
                    }
                    internalSettings.Win = matchValue;
                    break;
                case VirtualKey.Control:
                case VirtualKey.LeftControl:
                case VirtualKey.RightControl:
                    if (!matchValue && (modifierKeysOnEntering.ContainsKey(VirtualKey.Control) || !internalSettings.Ctrl))
                    {
                        SendSingleKeyboardInput((short)VirtualKey.Control, (uint)KeyboardHelper.KeyEventF.KeyUp);
                        modifierKeysOnEntering.Remove(VirtualKey.Control);
                        Debug.WriteLine("Simulate Control KeyUp");
                    }
                    internalSettings.Ctrl = matchValue;
                    break;
                case VirtualKey.Menu:
                case VirtualKey.LeftMenu:
                case VirtualKey.RightMenu:
                    if (!matchValue && (modifierKeysOnEntering.ContainsKey(VirtualKey.Menu) || !internalSettings.Alt))
                    {
                        SendSingleKeyboardInput((short)VirtualKey.Menu, (uint)KeyboardHelper.KeyEventF.KeyUp);
                        modifierKeysOnEntering.Remove(VirtualKey.Menu);
                        Debug.WriteLine("Simulate Alt KeyUp");
                    }
                    internalSettings.Alt = matchValue;
                    break;
                case VirtualKey.Shift:
                case VirtualKey.LeftShift:
                case VirtualKey.RightShift:
                    if (!matchValue && (modifierKeysOnEntering.ContainsKey(VirtualKey.Shift) || !internalSettings.Shift))
                    {
                        SendSingleKeyboardInput((short)VirtualKey.Shift, (uint)KeyboardHelper.KeyEventF.KeyUp);
                        modifierKeysOnEntering.Remove(VirtualKey.Shift);
                        Debug.WriteLine("Simulate Shift KeyUp");
                    }
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

        private bool Hotkey_IsActive()
        {
            return _isActive;
        }

        // 打开快捷键设置对话框
        private async void OpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            _shortcutDialogContent.Keys = null;
            _shortcutDialogContent.Keys = HotkeySettings.GetKeysList();

            _settingShortcutDialog.XamlRoot = this.XamlRoot;
            _settingShortcutDialog.RequestedTheme = this.ActualTheme;
            await _settingShortcutDialog.ShowAsync();
        }

        // 重置快捷键并关闭快捷键设置对话框
        private void ShortcutDialog_Reset(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _hotkeySettings = null;

            SetValue(HotkeySettingsProperty, _hotkeySettings);
            PreviewKeysControl.ItemsSource = HotkeySettings.GetKeysList();

            lastValidSettings = _hotkeySettings;

            _settingShortcutDialog.Hide();
        }

        // 保存快捷键并关闭快捷键设置对话框
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
            return settings != null && (settings.IsValid() || settings.IsEmpty());
        }

        // 窗口失去焦点时禁用钩子，防止拦截用户在其他窗口的输入，再次获得焦点则重新启用钩子
        private void ShortcutDialog_SettingsWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            args.Handled = true;
            if (args.WindowActivationState != WindowActivationState.Deactivated && (hook == null || hook.GetDisposedState() == true))
            {
                // If settings window gets focussed/activated again, we enable the keyboard hook to catch the keyboard input.
                hook = new HotkeySettingsControlHook(Hotkey_KeyDown, Hotkey_KeyUp, Hotkey_IsActive, FilterAccessibleKeyboardEvents);
                Debug.WriteLine($"Add Hook");
            }
            else if (args.WindowActivationState == WindowActivationState.Deactivated && hook != null && hook.GetDisposedState() == false)
            {
                // If settings window lost focus/activation, we disable the keyboard hook to allow keyboard input on other windows.
                hook?.Dispose();
                hook = null;
                Debug.WriteLine($"Delete Hook");
            }
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
            modifierKeysOnEntering.Clear();

            // To keep track of the modifier keys, whether it was pressed on entering.
            if ((KeyboardHelper.GetAsyncKeyState((int)VirtualKey.Shift) & 0x8000) != 0)
            {
                modifierKeysOnEntering.Add(VirtualKey.Shift, true);
            }
            if ((KeyboardHelper.GetAsyncKeyState((int)VirtualKey.Control) & 0x8000) != 0)
            {
                modifierKeysOnEntering.Add(VirtualKey.Control, true);
            }
            if ((KeyboardHelper.GetAsyncKeyState((int)VirtualKey.Menu) & 0x8000) != 0)
            {
                modifierKeysOnEntering.Add(VirtualKey.Menu, true);
            }
            if ((KeyboardHelper.GetAsyncKeyState((int)VirtualKey.LeftWindows) & 0x8000) != 0)
            {
                modifierKeysOnEntering.Add(VirtualKey.LeftWindows, true);
            }
            if ((KeyboardHelper.GetAsyncKeyState((int)VirtualKey.RightWindows) & 0x8000) != 0)
            {
                modifierKeysOnEntering.Add(VirtualKey.RightWindows, true);
            }

            _isActive = true;
        }

        private void ShortcutDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            _isActive = false;

            // Modifier keys remains pressed on dialog closing, system will receive a KeyUp message at some time in the future, therefore pass a KeyDown message to the system.
            if (internalSettings.Win)
            {
                SendSingleKeyboardInput((short)VirtualKey.LeftWindows, (uint)KeyboardHelper.KeyEventF.KeyDown);
                internalSettings.Win = false;
                Debug.WriteLine("Simulate Win KeyDown");
            }
            if (internalSettings.Ctrl)
            {
                SendSingleKeyboardInput((short)VirtualKey.Control, (uint)KeyboardHelper.KeyEventF.KeyDown);
                internalSettings.Ctrl = false;
                Debug.WriteLine("Simulate Control KeyDown");
            }
            if (internalSettings.Alt)
            {
                SendSingleKeyboardInput((short)VirtualKey.Menu, (uint)KeyboardHelper.KeyEventF.KeyDown);
                internalSettings.Alt = false;
                Debug.WriteLine("Simulate Menu KeyDown");
            }
            if (internalSettings.Shift)
            {
                SendSingleKeyboardInput((short)VirtualKey.Shift, (uint)KeyboardHelper.KeyEventF.KeyDown);
                internalSettings.Shift = false;
                Debug.WriteLine("Simulate Shift KeyDown");
            }
        }

    }
}
