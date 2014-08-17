using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FanfouWP.Storage
{
    public sealed class SettingManager
    {
        private FanfouWP.API.Items.User _currentUser;
        public FanfouWP.API.Items.User currentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                settings["currentUser"] = value;

                is_setting_changed = true;
            }
        }

        private List<FanfouWP.API.Items.User> _currentList;
        public List<FanfouWP.API.Items.User> currentList
        {
            get
            {
                return _currentList;
            }
            set
            {
                _currentList = value;
                settings["currentList"] = value;

                is_setting_changed = true;
            }
        }

        private Boolean _quit_confirm;
        public Boolean quit_confirm
        {
            get
            {
                return _quit_confirm;
            }
            set
            {
                _quit_confirm = value;
                settings["quit_confirm"] = value;

                is_setting_changed = true;
            }
        }

        private string _username;
        public string username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                settings["username"] = value;

                is_setting_changed = true;
            }
        }

        private string _password;
        public string password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                settings["password"] = value;

                is_setting_changed = true;
            }
        }
        private string _oauthToken;
        public string oauthToken
        {
            get
            {
                return _oauthToken;
            }
            set
            {
                _oauthToken = value;
                settings["oauthToken"] = value;

                is_setting_changed = true;
            }
        }

        private string _oauthSecret;
        public string oauthSecret
        {
            get
            {
                return _oauthSecret;
            }
            set
            {
                _oauthSecret = value;
                settings["oauthSecret"] = value;

                is_setting_changed = true;
            }
        }

        private bool _displayImage;
        public bool displayImage
        {
            get
            {
                return _displayImage;
            }
            set
            {
                _displayImage = value;
                settings["displayImage"] = value;

                is_setting_changed = true;
            }
        }

        private bool _enableLocation;
        public bool enableLocation
        {
            get
            {
                return _enableLocation;
            }
            set
            {
                _enableLocation = value;
                settings["enableLocation"] = value;

                is_setting_changed = true;
            }
        }

        private int _imageQuality;
        public int imageQuality
        {
            get
            {
                return _imageQuality;
            }
            set
            {
                _imageQuality = value;
                settings["imageQuality"] = value;

                is_setting_changed = true;
            }
        }

        private int _cacheSize;
        public int cacheSize
        {
            get
            {
                return _cacheSize;
            }
            set
            {
                _cacheSize = value;
                settings["cacheSize"] = value;

                is_setting_changed = true;
            }
        }

        private int _backgroundFeq2;
        public int backgroundFeq2
        {
            get
            {
                return _backgroundFeq2;
            }
            set
            {
                _backgroundFeq2 = value;
                settings["backgroundFeq2"] = value;

                is_setting_changed = true;
            }
        }

        private int _defaultCount2;
        public int defaultCount2
        {
            get
            {
                return _defaultCount2;
            }
            set
            {
                _defaultCount2 = value;
                settings["defaultCount2"] = value;

                is_setting_changed = true;
            }
        }

        private bool _reverseContext;
        public bool reverseContext
        {
            get
            {
                return _reverseContext;
            }
            set
            {
                _reverseContext = value;
                settings["reverseContext"] = value;

                is_setting_changed = true;
            }
        }

        private int _refreshFreq;
        public int refreshFreq
        {
            get
            {
                return _refreshFreq;
            }
            set
            {
                _refreshFreq = value;
                settings["refreshFreq"] = value;

                is_setting_changed = true;
            }
        }

        private bool _alwaysTop;
        public bool alwaysTop
        {
            get
            {
                return _alwaysTop;
            }
            set
            {
                _alwaysTop = value;
                settings["alwaysTop"] = value;

                is_setting_changed = true;
            }
        }

        private bool _largeImage;
        public bool largeImage
        {
            get
            {
                return _largeImage;
            }
            set
            {
                _largeImage = value;
                settings["largeImage"] = value;

                is_setting_changed = true;
            }
        }

        private bool _voiceError;
        public bool voiceError
        {
            get
            {
                return _voiceError;
            }
            set
            {
                _voiceError = value;
                settings["voiceError"] = value;

                is_setting_changed = true;
            }
        }
        public Boolean is_setting_changed { get; set; }

        private static SettingManager instance = new SettingManager();

        public IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        private SettingManager()
        {
            RestoreSettings();
        }

        public static SettingManager GetInstance()
        {
            return instance;
        }

        public void SaveSettings()
        {

            settings["quit_confirm"] = this.quit_confirm;
            settings["currentUser"] = this.currentUser;
            settings["currentList"] = this.currentList;
            settings["username"] = this.username;
            settings["password"] = this.password;
            settings["oauthToken"] = this.oauthToken;
            settings["oauthSecret"] = this.oauthSecret;

            settings["displayImage"] = this.displayImage;
            settings["enableLocation"] = this.enableLocation;
            settings["imageQuality"] = this.imageQuality;
            settings["cacheSize"] = this.cacheSize;
            settings["backgroundFeq2"] = this.backgroundFeq2;
            settings["defaultCount2"] = this.defaultCount2;
            settings["reverseContext"] = this.reverseContext;
            settings["refreshFreq"] = this.refreshFreq;
            settings["alwaysTop"] = this.alwaysTop;
            settings["largeImage"] = this.largeImage;
            settings["voiceError"] = this.voiceError;
            try
            {
                settings.Save();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                Thread.Sleep(1000);
                settings.Save();
            }
        }

        public void RestoreSettings()
        {
            Object currentUser, currentList, quit_confirm, username, password, oauthToken, oauthSecret, displayImage, enableLocation, imageQuality, cacheSize, backgroundFeq2, defaultCount2, reverseContext, refreshFreq, alwaysTop, largeImage,voiceError;
            if (settings.TryGetValue("defaultCount2", out defaultCount2) && defaultCount2 != null)
                this.defaultCount2 = (int)defaultCount2;
            else
                this.defaultCount2 = 0;
            if (settings.TryGetValue("currentList", out currentList) && currentList != null)
                this.currentList = (List<FanfouWP.API.Items.User>)currentList;
            else
                this.currentList = new List<FanfouWP.API.Items.User>();
            if (settings.TryGetValue("currentUser", out currentUser) && currentUser != null)
                this.currentUser = (FanfouWP.API.Items.User)currentUser;
            else
                this.currentUser = null;
            if (settings.TryGetValue("quit_confirm", out quit_confirm) && quit_confirm != null)
                this.quit_confirm = (Boolean)quit_confirm;
            else
                this.quit_confirm = true;
            if (settings.TryGetValue("username", out username) && username != null)
                this.username = (string)username;
            else
                this.username = null;
            if (settings.TryGetValue("password", out password) && password != null)
                this.password = (string)password;
            else
                this.password = null;
            if (settings.TryGetValue("oauthToken", out oauthToken) && oauthToken != null)
                this.oauthToken = (string)oauthToken;
            else
                this.oauthToken = null;
            if (settings.TryGetValue("oauthSecret", out oauthSecret) && oauthSecret != null)
                this.oauthSecret = (string)oauthSecret;
            else
                this.oauthSecret = null;

            if (settings.TryGetValue("displayImage", out displayImage) && displayImage != null)
                this.displayImage = (Boolean)displayImage;
            else
                this.displayImage = true;
            if (settings.TryGetValue("enableLocation", out enableLocation) && enableLocation != null)
                this.enableLocation = (Boolean)enableLocation;
            else
                this.enableLocation = true;
            if (settings.TryGetValue("imageQuality", out imageQuality) && imageQuality != null)
                this.imageQuality = (int)imageQuality;
            else
                this.imageQuality = 0;
            if (settings.TryGetValue("cacheSize", out cacheSize) && cacheSize != null)
                this.cacheSize = (int)cacheSize;
            else
                this.cacheSize = 0;
            if (settings.TryGetValue("backgroundFeq2", out backgroundFeq2) && backgroundFeq2 != null)
                this.backgroundFeq2 = (int)backgroundFeq2;
            else
                this.backgroundFeq2 = 0;
            if (settings.TryGetValue("reverseContext", out reverseContext) && reverseContext != null)
                this.reverseContext = (bool)reverseContext;
            else
                this.reverseContext = false;
            if (settings.TryGetValue("refreshFreq", out refreshFreq) && refreshFreq != null)
                this.refreshFreq = (int)refreshFreq;
            else
                this.refreshFreq = 2;
            if (settings.TryGetValue("alwaysTop", out alwaysTop) && alwaysTop != null)
                this.alwaysTop = (bool)alwaysTop;
            else
                this.alwaysTop = false;
            if (settings.TryGetValue("largeImage", out largeImage) && largeImage != null)
                this.largeImage = (bool)largeImage;
            else
                this.largeImage = false;
            if (settings.TryGetValue("voiceError", out voiceError) && voiceError != null)
                this.voiceError = (bool)voiceError;
            else
                this.voiceError = false;
        }

    }

}
