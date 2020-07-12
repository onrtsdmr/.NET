using Newtonsoft.Json;
using shopapp.webui.Models;

namespace shopapp.webui.Helper
{
    public class AppHelper
    {
        public static string CreateMessage(string message, string alertType)
        {
            return JsonConvert.SerializeObject(
                new AlertMessage()
                {
                    Message = message,
                    Type = alertType
                }
            );
        }
    }
}