using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Globalization;
using IDC.LeadCapture.DAL;
using IDC.LeadCapture.Models;

namespace IDC.LeadCapture.BLL
{
    public class ResourceCache
    {
        static ResourceCache()
        {
            // preload all localized text resources into cache
            var db = new ResourceRepo();
            var list = db.GetResourceValues(ResourceType.UI, CultureInfo.CurrentUICulture.TextInfo.CultureName);
            foreach (var item in list) if (HttpRuntime.Cache.Get(item.Key) == null) HttpRuntime.Cache.Insert(item.Key, item.Value);
        }

        public static string Localize(string key)
        {
            if (HttpRuntime.Cache.Get(key) == null)
            {
                string value = ResourceRepo.GetResourceValue(key, CultureInfo.CurrentUICulture.TextInfo.CultureName);

                if (!string.IsNullOrEmpty(value))
                {
                    HttpRuntime.Cache.Insert(key, value);
                }
            }

            var val = HttpRuntime.Cache.Get(key);
            return val != null ? val.ToString() : null;
        }
    }


}