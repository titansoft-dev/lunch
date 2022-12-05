using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Lunch.Helper
{
    public class ConfigHelper
    {
        private static ConfigHelper _configHelper;
        private static XDocument _XDocument;
        private readonly string root = "config/";

        private ConfigHelper()
        {
        }

        public static ConfigHelper Instance
        {
            get
            {
                return _configHelper ?? (_configHelper = new ConfigHelper());
            }
        }

        public static void LoadConfig(string path)
        {
            _XDocument = XDocument.Load(path);
        }
        public List<string> GetConfig(string xPath)
        {
            return _XDocument.XPathSelectElements(string.Format("{0}{1}", root, xPath)).Select(x => x.Value).ToList();
        }
        public string GetSingleConfig(string xPath)
        {
            if (_XDocument.XPathSelectElement(string.Format("{0}{1}", root, xPath)) == null)
                return string.Empty;
            return _XDocument.XPathSelectElement(string.Format("{0}{1}", root, xPath)).Value;
        }
    }
}