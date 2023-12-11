using System.IO;
using System.Windows;

namespace Keypad_Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string? Language { get; set; }
        public static ushort NumberOfKeys { get; set; }
        public static string? initialGroupName {
            get { return initialGroupName; }
            set { initialGroupName = value;
                  //Setting new value to a settings file
                }
        }
        public static bool Cache { get; set; } // Should I leave it?
        public static short Theme { get; set; }
        public static short InitalWindow { get; set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            IniFile iniFile = new IniFile("data\\Config.ini");

            Language = iniFile.Read("Language", "ApplicationSettings");
            if (iniFile.Read("Cache", "ApplicationSettings") == "true") Cache = true;
            else Cache = false;
            //Theme = Convert.ToInt16(iniFile.Read("Theme", "ApplicationSettings"));
            //InitalWindow = Convert.ToInt16(iniFile.Read("InitalWindow", "ApplicationSettings"));
            NumberOfKeys = Convert.ToUInt16(iniFile.Read("NumberOfKeys", "ApplicationSettings"));

            base.OnStartup(e);
        }
    }
}
