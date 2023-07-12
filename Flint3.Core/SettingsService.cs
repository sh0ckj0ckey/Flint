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
        private const string SETTING_NAME_ENABLEENGDEF = "EnableEngDefinition";
        private const string SETTING_NAME_SEARCHBOXSTYLE = "SearchBoxStyle";

        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        // 设置的应用程序的主题 0-System 1-Dark 2-Light
        private int _appearanceIndex = -1;
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
                catch { }
                if (_appearanceIndex < 0) _appearanceIndex = 0;
                return _appearanceIndex < 0 ? 0 : _appearanceIndex;
            }
            set
            {
                SetProperty(ref _appearanceIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_APPEARANCEINDEX] = _appearanceIndex;
            }
        }

        // 是否显示英英释义
        private bool? _enableEngDefinition = null;
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
                        else if (_localSettings.Values[SETTING_NAME_ENABLEENGDEF]?.ToString() == "True")
                        {
                            _enableEngDefinition = true;
                        }
                        else
                        {
                            _enableEngDefinition = false;
                        }
                    }
                }
                catch { }
                if (_enableEngDefinition is null) _enableEngDefinition = true;
                return _enableEngDefinition != false;
            }
            set
            {
                SetProperty(ref _enableEngDefinition, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ENABLEENGDEF] = _enableEngDefinition;
            }
        }

        // 搜索框样式
        private int _searchBoxStyle = -1;
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
                catch { }
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
