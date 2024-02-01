using System.IO;
using System.Text.Json;
using System.Windows;

namespace Keypad_Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ApplicationData appData = new();
        public static ApplicationData AppData
        {
            get
            {
                return appData;
            }
        }

        public App()
        {
            try
            {
                var data = JsonSerializer.Deserialize<ApplicationData>(File.ReadAllText(Path.GetFullPath("data/Config.json")));
                if (data is not null)
                    appData = data;
            }
            catch { }
        }
    }

    public class ApplicationData
    {
        // The values that are set to filds below is the settings by default
        public byte NumberOfKeys { get; set; } = 8;
        public string Language { get; set; } = "EN";
        public bool Cache { get; set; } = true; // Should I leave it?
        public string InitialGroupName { get; set; } = "Main";
    }
}
