using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Flint.Core
{
    public class SettingsService : ObservableObject
    {
        private const string SETTING_NAME_APPEARANCEINDEX = "AppearanceIndex";
        private const string SETTING_NAME_SEARCHPREVIEWMODE = "SearchPreviewMode";
        private const string SETTING_NAME_ENABLEHISTORY = "EnableHistory";

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

        // 搜索预览 0-简洁 1-详细
        private int _searchPreviewMode = -1;
        public int SearchPreviewMode
        {
            get
            {
                try
                {
                    if (_searchPreviewMode < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_SEARCHPREVIEWMODE] == null)
                        {
                            _searchPreviewMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHPREVIEWMODE]?.ToString() == "0")
                        {
                            _searchPreviewMode = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHPREVIEWMODE]?.ToString() == "1")
                        {
                            _searchPreviewMode = 1;
                        }
                        else
                        {
                            _searchPreviewMode = 0;
                        }
                    }
                }
                catch { }
                if (_searchPreviewMode < 0) _searchPreviewMode = 0;
                return _searchPreviewMode < 0 ? 0 : _searchPreviewMode;
            }
            set
            {
                SetProperty(ref _searchPreviewMode, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_SEARCHPREVIEWMODE] = _searchPreviewMode;
            }
        }

        // 是否开启历史记录
        private bool? _enableHistory = null;
        public bool EnableHistory
        {
            get
            {
                try
                {
                    if (_enableHistory is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_ENABLEHISTORY] == null)
                        {
                            _enableHistory = true;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ENABLEHISTORY]?.ToString() == "True")
                        {
                            _enableHistory = true;
                        }
                        else
                        {
                            _enableHistory = false;
                        }
                    }
                }
                catch { }
                if (_enableHistory is null) _enableHistory = true;
                return _enableHistory != false;
            }
            set
            {
                SetProperty(ref _enableHistory, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ENABLEHISTORY] = _enableHistory;
            }
        }
    }
}
