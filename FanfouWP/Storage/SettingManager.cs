using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.Storage
{
    public sealed class SettingManager
    {

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

        private int _backgroundFeq;
        public int backgroundFeq
        {
            get
            {
                return _backgroundFeq;
            }
            set
            {
                _backgroundFeq = value;
                settings["backgroundFeq"] = value;

                is_setting_changed = true;
            }
        }

        public Boolean is_setting_changed { get; set; }

        private static readonly SettingManager instance = new SettingManager();

        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

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
            settings["username"] = this.username;
            settings["password"] = this.password;
            settings["oauthToken"] = this.oauthToken;
            settings["oauthSecret"] = this.oauthSecret;

            settings["displayImage"] = this.displayImage;
            settings["enableLocation"] = this.enableLocation;
            settings["imageQuality"] = this.imageQuality;
            settings["cacheSize"] = this.cacheSize;
            settings["backgroundFeq"] = this.backgroundFeq;


            settings.Save();
        }

        public void RestoreSettings()
        {
            Object quit_confirm, username, password, oauthToken, oauthSecret, displayImage, enableLocation, imageQuality, cacheSize, backgroundFeq;
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
            if (settings.TryGetValue("backgroundFeq", out backgroundFeq) && backgroundFeq != null)
                this.backgroundFeq = (int)backgroundFeq;
            else
                this.backgroundFeq = 0;
        }

    }

}
