using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Napkin
{
    public class RowInformation
    {
        public string Content { get; set; }
        public int Tab()
        {
            return Content.Length - Content.TrimStart().Length;
        }
        public int RowNumber { get; set; }
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Content);
        }
        public string[] Split()
        {
            return Content.Trim().Split(' ');
        }
        public string HeaderName()
        {
            if (IsEmpty()) return "";
            return Split()[0];
        }
        public Dictionary<string, string> HeaderAttributes()
        {
            return Split().Where(s => s.Contains("=")).ToDictionary(t => t.Split('=')[0], t => t.Split('=')[1]);
        }
        public KeyValuePair<string, string> Property()
        {
            if (Split().Count() == 1 && Content.Contains("="))
            {
                var splitByKeyValueSeparator = Split()[0].Split('=');
                return new KeyValuePair<string, string>(splitByKeyValueSeparator[0], splitByKeyValueSeparator[1]);
            }
            return new KeyValuePair<string, string>();
        }

        public string HeaderBody()
        {
            if (IsEmpty()) return "";
            if (Content.Trim().Contains(" "))
                return Content.Trim().Split(new[] { ' ' }, 2)[1];
            return "";
        }
        public string HeaderIndentation()
        {
            return Content.Substring(0, Content.Length - Content.TrimStart().Length);
        }
    }

}
