using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunch.Helper
{
    public class LogHelper
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("LOGGER");

        public static void Log(string message)
        {
            logger.Debug(message);
        }
    }
}