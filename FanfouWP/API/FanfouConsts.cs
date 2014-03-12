﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanfouWP.API
{
    public sealed class FanfouConsts
    {
        public const string CONSUMER_KEY = "ca01f862370339af2dc80eca5e9fa8cd";
        public const string CONSUMER_SECRET = "6faef3388beac280fc1830a7797c4e55";
        public const string BASE_URL = "http://fanfou.com";
        public const string API_URL = "http://api.fanfou.com";
        public const string ACCESS_TOKEN = "oauth/access_token";
        public const string VERIFY_CREDENTIALS = "account/verify_credentials.json";
        public const string STATUS_HOME_TIMELINE = "statuses/home_timeline.json";
        public const string STATUS_PUBLIC_TIMELINE = "statuses/public_timeline.json";
        public const string STATUS_USER_TIMELINE = "statuses/user_timeline.json";
        public const string STATUS_MENTION_TIMELINE = "statuses/mentions.json";
        public const string STATUS_UPDATE = "statuses/update.json";      
    }
}
