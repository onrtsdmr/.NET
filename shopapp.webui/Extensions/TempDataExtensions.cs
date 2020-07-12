using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace shopapp.webui.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempDataDictionary, string key, T value) where T: class
        {
            tempDataDictionary[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempDataDictionary, string key) where T : class
        {
            object obj;
            tempDataDictionary.TryGetValue(key, out obj);
            return obj==null?null:JsonConvert.DeserializeObject<T>((string) obj);
        }
    }
}