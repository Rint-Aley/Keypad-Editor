using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Keypad_Editor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Forms.NotifyIcon notifyIcon = new Forms.NotifyIcon();

        public static string Language { get; set; }
        public static ushort NumberOfKeys { get; set; }
        public static bool Cache { get; set; }
        public static short Theme { get; set; }
        public static short InitalWindow { get; set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            IniFile iniFile = new IniFile("Data\\Config.ini");

            Language = iniFile.Read("Language", "ApplicationSettings");
            if (iniFile.Read("Cache", "ApplicationSettings") == "true") Cache = true;
            else Cache = false;
            Theme = Convert.ToInt16(iniFile.Read("Theme", "ApplicationSettings"));
            InitalWindow = Convert.ToInt16(iniFile.Read("InitalWindow", "ApplicationSettings"));
            NumberOfKeys = Convert.ToInt32(iniFile.Read("NumberOfKeys", "ApplicationSettings"));



            if (!File.Exists(FileEditor.path))
                FileEditor.CreateFile(NumberOfKeys);
            if (!File.Exists("Data\\port.txt"))
                File.Create("Data\\port.txt").Close();


            Forms.ContextMenuStrip menu = new Forms.ContextMenuStrip();
            
            notifyIcon.Icon = new Icon("Data\\Keypad.ico");
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = menu;
            menu.Items.Add("Show window", null, OnButton1Clicked);
            menu.Items.Add("Hide window", null, OnButton2Clicked);
            menu.Items.Add("Exit", null, CloseClicked);

            notifyIcon.Click += NotifyIcon_Click;

            base.OnStartup(e);
        }
        void OnButton1Clicked(object sender, EventArgs e)
        {
            MainWindow.Show();
        }
        void OnButton2Clicked(object sender, EventArgs e)
        {
            MainWindow.Hide();
        }
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            //MainWindow.Show();
            //throw new NotImplementedException();
        }
        void CloseClicked(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Current.Shutdown();
        }
    }
}
