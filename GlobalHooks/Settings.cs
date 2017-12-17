using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GlobalHooks
{
    class Settings
    {
        private string eMail = "email@gmail.com";
        private int maxLogFileSize = 1024;
        private bool autoStart = false;

        public string EMail
        {
            get { return eMail; }
            set { eMail = value; }
        }

        public int MaxLogFileSize
        {
            get { return maxLogFileSize; }
            set { maxLogFileSize = value; }
        }

        public bool AutoStart
        {
            get { return autoStart; }
            set { autoStart = value; }
        }

        public Settings()
        {
            string[] settings;

            try
            {
                settings = File.ReadAllLines(@"C:\Andrey\VS2017\GlobalHooks\files\settings");                               
            }
            catch (Exception)
            {
                return;
            }

            if (settings.Length == 3)
            {
                string eMail = Coder.Decode(settings[0]);
                string maxSize = Coder.Decode(settings[1]);
                string autoStart = Coder.Decode(settings[2]);

                if (CheckMail(eMail) && CheckNumber(maxSize) && (autoStart.Equals("true") || autoStart.Equals("false")))
                {
                    this.eMail = Coder.Decode(settings[0]);
                    this.maxLogFileSize = int.Parse(Coder.Decode(settings[1]));
                    this.autoStart = Coder.Decode(settings[2]).Equals("true");
                }
            }
        }

        public bool SaveSettings()
        {
            if (CheckMail())
            {
                File.Delete(@"C:\Andrey\VS2017\GlobalHooks\files\settings");                                                
                File.AppendAllLines(@"C:\Andrey\VS2017\GlobalHooks\files\settings", new List<string>() 
                    { Coder.Code(eMail), 
                      Coder.Code(maxLogFileSize.ToString()), 
                      Coder.Code(autoStart ? "true" : "false") });

                return true;
            }
            return false;
        }

        public bool CheckMail(string mail)
        {
            if (mail == null)
            {
                return false;
            }

            System.Text.RegularExpressions.Regex rEMail =
                new System.Text.RegularExpressions.Regex(@"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
           
            if (mail.Length > 0)
            {
                return (rEMail.IsMatch(mail));
            }

            return false;
        }

        public bool CheckMail()
        {
            return CheckMail(eMail);
        }

        public bool CheckNumber(string number)
        {
            foreach (var digit in number.ToArray())
            {
                if (!Char.IsDigit(digit))
                {
                    return false;
                }
            }
            return true;
        }
    }
}