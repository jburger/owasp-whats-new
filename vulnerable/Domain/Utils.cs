using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using vulnerable.Models;

namespace vulnerable.Domain {
    public class Utils {
        public static User GetIdentity(HttpRequest request) 
        {
            var cookie = request.Cookies["PiggyBankCo"];
            if(string.IsNullOrWhiteSpace(cookie))
                return null;
            var decoded = Convert.FromBase64String(cookie);
            var json = Encoding.UTF8.GetString(decoded);

            return JsonConvert.DeserializeObject<User>(json);
        }

        public static bool IsAdmin(HttpRequest request) 
        {
            var user = GetIdentity(request);
            if (!IsUser(request)) return false;

            return user.Type == UserType.Admin;
        }

        public static bool IsUser(HttpRequest request) 
        {
            var user = GetIdentity(request);
            return user != null;
        }
    }
}