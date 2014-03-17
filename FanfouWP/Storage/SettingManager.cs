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

            public Boolean is_setting_changed { get; set; }

            private static readonly SettingManager instance = new SettingManager();

            private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            private SettingManager()
            {
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
                settings.Save();
            }

            public void RestoreSettings()
            {
                Object username, password, oauthToken, oauthSecret;
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
            }
       
    }

}
