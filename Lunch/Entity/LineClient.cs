using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using Lunch.Helper;

namespace Lunch.Entity
{
    public class LineClient
    {
        private readonly string _Token = ConfigurationManager.AppSettings["LineToken"];
        private readonly string _GroupName = ConfigurationManager.AppSettings["Group"];
        private readonly bool _IsLineEnabled = ConfigurationManager.AppSettings["IsLineEnabled"].ToLower() == "y";

        public void SendMessage(string message)
        {
            if (!_IsLineEnabled) return;
            var fileName = HttpContext.Current.Server.MapPath("~/Scripts/LineClient.py");
            var start = new ProcessStartInfo()
            {
                FileName = @"D:\Python27\python.exe",
                Arguments =
                    string.Format("{0} {1} {2} {3}", fileName, _Token, _GroupName, string.Format("\"{0}\"", message)),
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            try
            {
                using (var process = Process.Start(start))
                {
                    using (var reader = process.StandardOutput)
                    {
                        var result = reader.ReadToEnd();
                        LogHelper.Log(result);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(string.Format("{0} {1}", ex.Message, ex.StackTrace));
            }
        }
    }
}