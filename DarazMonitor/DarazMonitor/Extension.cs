using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarazMonitor
{
    public static class Extension
    {
        public static IWebElement GetElement(this IWebElement element, By by)
        {
            try
            {
                return element.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IReadOnlyCollection<IWebElement> GetElements(this IWebElement element, By by)
        {
            try
            {
                return element.FindElements(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DateTime? AsDateTime(this string darazText)
        {
            if (string.IsNullOrWhiteSpace(darazText))
            {
                return null;
            }
            var Text = darazText.Trim().ToLowerInvariant().Split(' ');
            if (Text.Length != 3)
            {
                return null;
            }
            DateTime value;
            int.TryParse(Text[0], out int scalarValue);
            switch (Text[1])
            {
                case "day":
                case "days":
                    value = DateTime.Now.AddDays(-scalarValue);
                    break;
                case "week":
                case "weeks":
                    value = DateTime.Now.AddDays(-scalarValue * 7);
                    break;
                default:
                    if (!DateTime.TryParse(darazText, out value))
                    {
                        return null;
                    }
                    break;
            }
            return value;
        }
    }
}
