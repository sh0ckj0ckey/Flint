﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Flint3.Core
{
    public class SettingsService : ObservableObject
    {
        private const string FLINT_MAINWINDOW_WIDTH = "FlintMainWindowWidth";
        private const string FLINT_MAINWINDOW_HEIGHT = "FlintMainWindowHeight";

        private const string SETTING_NAME_APPEARANCEINDEX = "AppearanceIndex";
        private const string SETTING_NAME_BACKDROPINDEX = "BackdropIndex";
        private const string SETTING_NAME_ENABLEENGDEF = "EnableEngDefinition";
        private const string SETTING_NAME_ENABLEGLOSSARY = "EnableGlossary";
        private const string SETTING_NAME_WINDOWMODE = "WindowMode";
        private const string SETTING_NAME_AUTOCLEARLASTINPUT = "AutoClearLastInput";
        private const string SETTING_NAME_CLOSEBUTTONMODE = "CloseButtonMode";
        private const string SETTING_NAME_SEARCHBOXSTYLE = "SearchBoxStyle";

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        private double _mainWindowWidth = -1;

        private double _mainWindowHeight = -1;

        private int _appearanceIndex = -1;

        private int _backdropIndex = -1;

        private bool? _enableEngDefinition = null;

        private bool? _enableGlossary = null;

        private int _windowMode = -1;

        private bool? _autoClearLastInput = null;

        private int _closeButtonMode = -1;

        private int _searchBoxStyle = -1;

        public event Action<int> OnAppearanceSettingChanged = null;

        public event Action<int> OnBackdropSettingChanged = null;

        /// <summary>
        /// 主窗口的宽度
        /// </summary>
        public double MainWindowWidth
        {
            get
            {
                try
                {
                    if (_mainWindowWidth < 0)
                    {
                        if (double.TryParse(_localSettings.Values[FLINT_MAINWINDOW_WIDTH]?.ToString(), out double width))
                        {
                            _mainWindowWidth = width;
                        }
                        else
                        {
                            _mainWindowWidth = 580;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_mainWindowWidth <= 0) _mainWindowWidth = 580;
                return _mainWindowWidth <= 0 ? 580 : _mainWindowWidth;
            }
            set
            {
                SetProperty(ref _mainWindowWidth, value);
                ApplicationData.Current.LocalSettings.Values[FLINT_MAINWINDOW_WIDTH] = _mainWindowWidth;
            }
        }

        /// <summary>
        /// 主窗口的高度
        /// </summary>
        public double MainWindowHeight
        {
            get
            {
                try
                {
                    if (_mainWindowHeight < 0)
                    {
                        if (double.TryParse(_localSettings.Values[FLINT_MAINWINDOW_HEIGHT]?.ToString(), out double height))
                        {
                            _mainWindowHeight = height;
                        }
                        else
                        {
                            _mainWindowHeight = 386;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_mainWindowHeight <= 0) _mainWindowHeight = 580;
                return _mainWindowHeight <= 0 ? 580 : _mainWindowHeight;
            }
            set
            {
                SetProperty(ref _mainWindowHeight, value);
                ApplicationData.Current.LocalSettings.Values[FLINT_MAINWINDOW_HEIGHT] = _mainWindowHeight;
            }
        }

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
        /// 快捷键唤起的窗口模式 0-完整 1-简洁
        /// </summary>
        public int WindowMode
        {
            get
            {
                try
                {
                    if (_windowMode < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_WINDOWMODE] == null)
                        {
                            _windowMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_WINDOWMODE]?.ToString() == "0")
                        {
                            _windowMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_WINDOWMODE]?.ToString() == "1")
                        {
                            _windowMode = 1;
                        }
                        else
                        {
                            _windowMode = 0;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_windowMode < 0) _windowMode = 0;
                return _windowMode;
            }
            set
            {
                SetProperty(ref _windowMode, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_WINDOWMODE] = _windowMode;
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
