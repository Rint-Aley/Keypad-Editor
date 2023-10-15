using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Keypad_Editor
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            //Localizate();

            //set current settings
            IniFile iniFile = new IniFile("Data\\Config.ini");

            if(iniFile.Read("Cache", "ApplicationSettings") == "true") CacheToggleButton.IsChecked = true;
            else if(iniFile.Read("Cache", "ApplicationSettings") == "false") CacheToggleButton.IsChecked = false;

            if (iniFile.Read("Language", "ApplicationSettings") == "RU") LanguageComboBox.SelectedItem = RussianLanguage;
            else if (iniFile.Read("Language", "ApplicationSettings") == "EN") LanguageComboBox.SelectedItem = EnglishLanguage;
        }

        public void Localizate()
        {
            IniFile iniFile = new IniFile();

            if (App.Language == "RU")
                iniFile.Path = new FileInfo("Data\\locale\\RU.ini").FullName;
            if (App.Language == "EN")
                iniFile.Path = new FileInfo("Data\\locale\\EN.ini").FullName;

            Title = iniFile.Read("Settings", "WindowTitles");
            CacheTextBlock.Text = iniFile.Read("Cache", "SettingsItems");
            ThemeTextBlock.Text = iniFile.Read("Theme", "SettingsItems");
            InitialWindowTextBlock.Text = iniFile.Read("InitialWindow", "SettingsItems");
            LanguageTextBlock.Text = iniFile.Read("Language", "SettingsItems");
            CancelButton.Content = iniFile.Read("Cancel.Text", "SettingsItems");
            OKButton.Content = iniFile.Read("OK.Text", "SettingsItems");
            ApplyButton.Content = iniFile.Read("Apply.Text", "SettingsItems");
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TopMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyButton_Click(ApplyButton, null);
            this.Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            IniFile iniFile = new IniFile("Data\\Config.ini");

            if (CacheToggleButton.IsChecked == true)
            {
                iniFile.Write("Cache", "true", "ApplicationSettings");
                App.Cache = true;
            }
                
            else if (CacheToggleButton.IsChecked == false)
            {
                iniFile.Write("Cache", "false", "ApplicationSettings");
                App.Cache = false;
            }
                

            if(LanguageComboBox.SelectedItem == RussianLanguage)
            {
                iniFile.Write("Language", "RU", "ApplicationSettings");
                App.Language = "RU";
            }
                
            if (LanguageComboBox.SelectedItem == EnglishLanguage)
            {
                iniFile.Write("Language", "EN", "ApplicationSettings");
                App.Language = "EN";
            }
            //Localizate();
        }
    }
}
