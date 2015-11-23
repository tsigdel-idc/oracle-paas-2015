using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace IDC.Common
{
    public class Helper
    {
        private static CultureInfo _culture_US = CultureInfo.CreateSpecificCulture("en-US");

        // Capitalize first letter
        public static string ToTitleCase(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            else if (s.Length == 1) return s.ToUpper();
            else return char.ToUpper(s[0]) + s.Substring(1);
        }

        // string to int
        public static int IntValue(string value)
        {
            int val = 0;
            int.TryParse(value, out val);
            return val;
        }

        // string to float
        public static float FloatValue(string value)
        {
            float val = 0;
            float.TryParse(value, NumberStyles.Float, _culture_US, out val);
            return val;
        }

        // string to decimal
        public static decimal DecimalValue(string value)
        {
            decimal val = 0;
            decimal.TryParse(value, NumberStyles.Float, _culture_US, out val);
            return val;
        }

        public static string GetUrlPath(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return string.Empty;

            int inx = uri.IndexOf("//");
            if (inx >= 0 && uri.Length > inx + 2) return uri.Substring(inx + 2);

            return uri;
        }
    }
}
