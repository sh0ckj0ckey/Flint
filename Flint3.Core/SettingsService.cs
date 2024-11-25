using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Flint3.Core
{
    public class SettingsService : ObservableObject
    {
        private const string SETTING_NAME_APPEARANCEINDEX = "AppearanceIndex";
        private const string SETTING_NAME_BACKDROPINDEX = "BackdropIndex";
        private const string SETTING_NAME_ACRYLICOPACITY = "AcrylicOpacity";
        private const string SETTING_NAME_ENABLEENGDEF = "EnableEngDefinition";
        private const string SETTING_NAME_ENABLEGLOSSARY = "EnableGlossary";
        private const string SETTING_NAME_USELITEWINDOW = "UseLiteWindow";
        private const string SETTING_NAME_AUTOCLEARLASTINPUT = "AutoClearLastInput";
        private const string SETTING_NAME_CLOSEBUTTONMODE = "CloseButtonMode";
        private const string SETTING_NAME_SEARCHBOXSTYLE = "SearchBoxStyle";

        private const string SETTING_NAME_MAINSIZE_HEIGHT = "MainWindowHeight";
        private const string SETTING_NAME_MAINSIZE_WIDTH = "MainWindowWidth";

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        private int _appearanceIndex = -1;

        private int _backdropIndex = -1;

        private double _acrylicOpacity = -1;

        private bool? _enableEngDefinition = null;

        private bool? _enableGlossary = null;

        private bool? _useLiteWindow = null;

        private bool? _autoClearLastInput = null;

        private int _closeButtonMode = -1;

        private int _searchBoxStyle = -1;

        public event Action<int> OnAppearanceSettingChanged = null;

        public event Action<int> OnBackdropSettingChanged = null;

        public event Action<double> OnAcrylicOpacitySettingChanged = null;

        /// <summary>
        /// 设置的应用程序的主题 0-System 1-Dark 2-Light
        /// </summary>
        public int AppearanceIndex
        {
            get
            {
                try
                {
                    if (_appearanceIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX] == null)
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "0")
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "1")
                        {
                            _appearanceIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "2")
                        {
                            _appearanceIndex = 2;
                        }
                        else
                        {
                            _appearanceIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_appearanceIndex < 0) _appearanceIndex = 0;
                return _appearanceIndex < 0 ? 0 : _appearanceIndex;
            }
            set
            {
                SetProperty(ref _appearanceIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_APPEARANCEINDEX] = _appearanceIndex;
                OnAppearanceSettingChanged?.Invoke(_appearanceIndex);
            }
        }

        /// <summary>
        /// 设置的应用程序的背景材质 0-Mica 1-Acrylic
        /// </summary>
        public int BackdropIndex
        {
            get
            {
                try
                {
                    if (_backdropIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX] == null)
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX]?.ToString() == "0")
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX]?.ToString() == "1")
                        {
                            _backdropIndex = 1;
                        }
                        else
                        {
                            _backdropIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_backdropIndex < 0) _backdropIndex = 0;
                return _backdropIndex < 0 ? 0 : _backdropIndex;
            }
            set
            {
                SetProperty(ref _backdropIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_BACKDROPINDEX] = _backdropIndex;
                OnBackdropSettingChanged?.Invoke(_backdropIndex);
            }
        }

        /// <summary>
        /// 亚克力背景透明度
        /// </summary>
        public double AcrylicOpacity
        {
            get
            {
                try
                {
                    if (_acrylicOpacity < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_ACRYLICOPACITY] == null)
                        {
                            _acrylicOpacity = 1.0;
                        }
                        else
                        {
                            string opacityStr = _localSettings.Values[SETTING_NAME_ACRYLICOPACITY]?.ToString();
                            if (double.TryParse(opacityStr, out double opacity))
                            {
                                _acrylicOpacity = opacity;
                            }
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_acrylicOpacity < 0) _acrylicOpacity = 1.0;
                return _acrylicOpacity < 0 ? 1.0 : _acrylicOpacity;
            }
            set
            {
                SetProperty(ref _acrylicOpacity, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ACRYLICOPACITY] = _acrylicOpacity;
                OnAcrylicOpacitySettingChanged?.Invoke(_acrylicOpacity);
            }
        }

        /// <summary>
        /// 是否显示英英释义
        /// </summary>
        public bool EnableEngDefinition
        {
            get
            {
                try
                {
                    if (_enableEngDefinition is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_ENABLEENGDEF] == null)
                        {
                            _enableEngDefinition = true;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ENABLEENGDEF]?.ToString() == "False")
                        {
                            _enableEngDefinition = false;
                        }
                        else
                        {
                            _enableEngDefinition = true;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_enableEngDefinition is null) _enableEngDefinition = true;
                return _enableEngDefinition != false;
            }
            set
            {
                SetProperty(ref _enableEngDefinition, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ENABLEENGDEF] = _enableEngDefinition;
            }
        }

        /// <summary>
        /// 是否启用生词本
        /// </summary>
        public bool EnableGlossary
        {
            get
            {
                try
                {
                    if (_enableGlossary is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_ENABLEGLOSSARY] == null)
                        {
                            _enableGlossary = true;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ENABLEGLOSSARY]?.ToString() == "False")
                        {
                            _enableGlossary = false;
                        }
                        else
                        {
                            _enableGlossary = true;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_enableGlossary is null) _enableGlossary = true;
                return _enableGlossary != false;
            }
            set
            {
                SetProperty(ref _enableGlossary, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ENABLEGLOSSARY] = _enableGlossary;
            }
        }

        /// <summary>
        /// 是否启用精简搜索窗口
        /// </summary>
        public bool UseLiteWindow
        {
            get
            {
                try
                {
                    if (_useLiteWindow is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_USELITEWINDOW] == null)
                        {
                            _useLiteWindow = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_USELITEWINDOW]?.ToString() == "True")
                        {
                            _useLiteWindow = true;
                        }
                        else
                        {
                            _useLiteWindow = false;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_useLiteWindow is null) _useLiteWindow = false;
                return _useLiteWindow == true;
            }
            set
            {
                SetProperty(ref _useLiteWindow, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_USELITEWINDOW] = _useLiteWindow;
            }
        }

        /// <summary>
        /// 是否自动清除上次输入
        /// </summary>
        public bool AutoClearLastInput
        {
            get
            {
                try
                {
                    if (_autoClearLastInput is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_AUTOCLEARLASTINPUT] == null)
                        {
                            _autoClearLastInput = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_AUTOCLEARLASTINPUT]?.ToString() == "True")
                        {
                            _autoClearLastInput = true;
                        }
                        else
                        {
                            _autoClearLastInput = false;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_autoClearLastInput is null) _autoClearLastInput = false;
                return _autoClearLastInput == true;
            }
            set
            {
                SetProperty(ref _autoClearLastInput, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_AUTOCLEARLASTINPUT] = _autoClearLastInput;
            }
        }

        /// <summary>
        /// 设置关闭按钮的作用 0-隐藏 1-退出
        /// </summary>
        public int CloseButtonMode
        {
            get
            {
                try
                {
                    if (_closeButtonMode < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_CLOSEBUTTONMODE] == null)
                        {
                            _closeButtonMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_CLOSEBUTTONMODE]?.ToString() == "0")
                        {
                            _closeButtonMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_CLOSEBUTTONMODE]?.ToString() == "1")
                        {
                            _closeButtonMode = 1;
                        }
                        else
                        {
                            _closeButtonMode = 0;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_closeButtonMode < 0) _closeButtonMode = 0;
                return _closeButtonMode < 0 ? 0 : _closeButtonMode;
            }
            set
            {
                SetProperty(ref _closeButtonMode, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_CLOSEBUTTONMODE] = _closeButtonMode;
            }
        }

        /// <summary>
        /// 搜索框样式
        /// </summary>
        public int SearchBoxStyle
        {
            get
            {
                try
                {
                    if (_searchBoxStyle < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_SEARCHBOXSTYLE] == null)
                        {
                            _searchBoxStyle = 2;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHBOXSTYLE]?.ToString() == "0")
                        {
                            _searchBoxStyle = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHBOXSTYLE]?.ToString() == "1")
                        {
                            _searchBoxStyle = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHBOXSTYLE]?.ToString() == "2")
                        {
                            _searchBoxStyle = 2;
                        }
                        else
                        {
                            _searchBoxStyle = 2;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_searchBoxStyle < 0) _searchBoxStyle = 2;
                return _searchBoxStyle < 0 ? 2 : _searchBoxStyle;
            }
            set
            {
                SetProperty(ref _searchBoxStyle, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_SEARCHBOXSTYLE] = _searchBoxStyle;
            }
        }
    }
}
