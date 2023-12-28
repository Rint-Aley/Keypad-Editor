using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.IO;
using Microsoft.Win32;
namespace Keypad_Editor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        MainWindowLogic logic;

        string? DontSelectedAnyButtons;

        //Logic for combinations
        private short selectedHotkey = 1;

        string[] hotkeys = new string[1];
        int[] delays = new int[1];

        public MainWindow()
        {
            logic = new MainWindowLogic(this);
            InitializeComponent();
            //Localizate();
            logic.ReadDataFromFile();
        }

        public void Localizate()
        {
            IniFile iniFile = new IniFile();

            if (App.Language == "RU")
                iniFile.Path = new FileInfo("Data\\locale\\RU.ini").FullName;
            if (App.Language == "EN")
                iniFile.Path = new FileInfo("Data\\locale\\EN.ini").FullName;

            Title.Text = iniFile.Read("MainWindow", "WindowTitles");
            SettingsButtonText.Text = iniFile.Read("SettingsButton.Text", "DeviceBlock");
            ToCurrentSettings.Content = iniFile.Read("ToCurrentSettingsButton.Text", "DeviceBlock");
            Back.Content = iniFile.Read("Back.Text", "DeviceBlock");
            open.Content = iniFile.Read("Opening", "ActivityButtons");
            type.Content = iniFile.Read("Typing", "ActivityButtons");
            pressCombination.Content = iniFile.Read("Combination", "ActivityButtons");
            Apply.Content = iniFile.Read("Apply.Text", "ActivityGrids");
            selectDirectory.Content = iniFile.Read("SelectDirectory", "ActivityGrids");
            TipTypingGrid.Text = iniFile.Read("TipTypingGrid", "ActivityGrids");
            CombinationTip1.Text = iniFile.Read("Tip1", "CombinationGrid");
            CombinationTip2.Text = iniFile.Read("Tip2", "CombinationGrid");
            AddKeyToCombination.ToolTip = iniFile.Read("AddKeyToCombination.ToolTip", "CombinationGrid");
            DeleateKeyButton.ToolTip = iniFile.Read("DeleateKeyButton.ToolTip", "CombinationGrid");
            ToPreviousGroupOfCombination.ToolTip = iniFile.Read("LastGroupKeys.ToolTip", "CombinationGrid");
            ToNextGroupOfCombination.ToolTip = iniFile.Read("NextGroupKeys.ToolTip", "CombinationGrid");
            DelayTextBlock.ToolTip = iniFile.Read("DelayTextBlock.ToolTip", "CombinationGrid");
            DeleateGroupCombnations.ToolTip = iniFile.Read("DeleateGroupCombnations.ToolTip", "CombinationGrid");
            AddGroupCombimations.ToolTip = iniFile.Read("AddGroupCombimations.ToolTip", "CombinationGrid");

            SettingsButton.Width = Convert.ToDouble(iniFile.Read("SettingsButton.Width", "DeviceBlock"));
            ToCurrentSettings.Width = Convert.ToDouble(iniFile.Read("ToCurrentSettingsButton.Width", "DeviceBlock"));

            DontSelectedAnyButtons = iniFile.Read("ErrorKeyIsNotSelected", "DeviceBlock");
        }
        
        /// <summary>
        /// Close window.
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Changes window position.
        /// </summary>
        private void TopMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }


        /// <summary>
        /// Changes subwindows
        /// </summary>
        private void BigDevice_Click(object sender, RoutedEventArgs e)
        {
            BigDeviceGrid.Visibility = Visibility.Hidden;
            MainWindowGrid.Visibility = Visibility.Visible;
            //Animation
        }

        /// <summary>
        /// Sets commands to last saved state.
        /// </summary>
        private void ToCurrentSettings_Click(object sender, RoutedEventArgs e)
        {
            logic.RetunToOldSettings();
        }

        /// <summary>
        /// Handles perssing to button on device.
        /// </summary>
        private void Key_Click(object sender, RoutedEventArgs e)
        {
            var selectedKeyTag = ((ToggleButton)sender).Tag;
            logic.ChangeKey(Convert.ToInt16(selectedKeyTag));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //if (selectedKey != 0) WriteNewValues();
            //if(parametrNew.SequenceEqual(parametrInFile) == false || activityNew.SequenceEqual(activityInFile) == false)
            //{
            //    YesNoForm yesNo = new YesNoForm("Doesn't save changes", "Are you going to save changes?");
            //    yesNo.ShowDialog();
            //    //Если выбрано нет
            //    if (!yesNo.Result)
            //    {
            //        Array.Copy(parametrInFile, parametrNew, parametrInFile.Length);
            //        Array.Copy(activityInFile, activityNew, activityInFile.Length);
            //    }
            //    else
            //        Apply_Click(Apply, null);
                
            //}
            //BigDeviceGrid.Visibility = Visibility.Visible;
            //MainWindowGrid.Visibility = Visibility.Hidden;
            //selectedKey = 0;
            //lastSelectedKey = 0;
            ////Очищаем экран
            //HideControls();
            //DeleteActivity();
            //foreach (System.Windows.Controls.RadioButton radioButton in RadioButtonsOfDevice.Children)
            //{
            //    radioButton.IsChecked = false;
            //}
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            var currentAction = (ToggleButton)sender;
            // If user doesn't select any keys it will notify about it
            if (logic.selectedKey == MainWindowLogic.KEY_DONT_SELECTED)
            {
                Thread clearErrorTextBlock = new Thread(runTimer);
                currentAction.IsChecked = false;
                ErrorTextBlock.Text = DontSelectedAnyButtons;
                clearErrorTextBlock.Start();
            }
            // If user selected active action it will make it unactive
            else if (currentAction.Name == logic.selectedAction.ToString())
            {
                logic.ChangeAction("none");
            }
            // Changes action
            else
            {
                logic.ChangeAction(currentAction.Name);
            }
        }

        /// <summary>
        /// Clear text in text box after 2 seconds
        /// </summary>
        private async void runTimer()
        {
            await Task.Delay(2000);
            Dispatcher.BeginInvoke(new Action(() => ErrorTextBlock.Text = ""));
        }

        // Opens menu to choose the path to the file or to the folder
        private void selectDirectory_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            if (PathToFileOrWebsite.Text != string.Empty)
            {
                openFileDialog.InitialDirectory = PathToFileOrWebsite.Text; //Открывает нужную деррикторию
                //openFileDialog.FileName = Path.GetFileName(PathToFileOrWebsite.Text); //Пишет в имя уже выбранный файл
            }
            else openFileDialog.InitialDirectory = @"C:\";

            bool? result = openFileDialog.ShowDialog();

            if (result == true) PathToFileOrWebsite.Text = openFileDialog.FileName;
        }

        // Applies settings
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            logic.SaveSettings();
        }


        //Система комбинцаций

        //Удаляет последнню клавишу
        private void DeleateKeyButton_Click(object sender, RoutedEventArgs e)
        {
            string[] keys = KeysTextBlock.Text.Split(' ');
            KeysTextBlock.Text = String.Empty;
            for (int i = 0; i < keys.Length - 2; i++)//-2 т.к. один элемент - пустая строка
            {
                KeysTextBlock.Text += keys[i] + " ";
            }
        }

        //Переходит в предыдущую группу комбинаций
        private void LastGroupKeysButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHotkey > 1)
            {
                ChangeSelectedGroupHotKey(Convert.ToInt16(selectedHotkey - 1));
            }
        }

        //Переходит в следующую группу комбинаций
        private void NextGroupKeysButton_Click(object sender, RoutedEventArgs e)
        {
            if(selectedHotkey < hotkeys.Length)
            {
                ChangeSelectedGroupHotKey(Convert.ToInt16(selectedHotkey + 1));
            }
        }

        //Удаляет группу
        private void DeleateGroupCombinations_Click(object sender, RoutedEventArgs e)
        {
            //Когда удаляется остётся всего одна группа - блокирум кнопки
            if (hotkeys.Length == 2)
            {
                DeleateGroupCombnations.IsEnabled = false;
                DelayTextBlock.IsEnabled = false;
                //NextGroupKeys.IsEnabled = false;
                //LastGroupKeys.IsEnabled = false;
            }
            //Смещение (с выбранного элемента) элементов массива на элемент назад
            for (int i = selectedHotkey; i < hotkeys.Length; i++)
            {
                hotkeys[i - 1] = hotkeys[i];
                delays[i - 1] = delays[i];
            }
            Array.Resize(ref hotkeys, hotkeys.Length - 1);
            Array.Resize(ref delays, delays.Length - 1);
            //Если в момент удаления выбранна последняя группа, то смещаем выделение на группу назад
            if (selectedHotkey == hotkeys.Length + 1)
                ChangeSelectedGroupHotKey(Convert.ToInt16(selectedHotkey - 1), false);
            else
                ChangeSelectedGroupHotKey(selectedHotkey, false);
        }

        //Создаёт группу
        private void AddGroupCombinations_Click(object sender, RoutedEventArgs e)
        {
            //Можно добавить защиту от превышения предела short, но это 32767 знаков, так что защита не очень то и нужна)
            Array.Resize(ref hotkeys, hotkeys.Length + 1);
            Array.Resize(ref delays, delays.Length + 1);
            //Ограничение, чтобы не было ошибки
            if (hotkeys.Length > 2)
            {
                //Сдвигаем все элементы массива после выбранного на элемент вперёд
                for (int i = hotkeys.Length - 2; i > selectedHotkey - 2; i--)
                {
                    hotkeys[i + 1] = hotkeys[i];
                    delays[i + 1] = delays[i];
                }
                //Онуляют значения для новой группы
                hotkeys[selectedHotkey] = String.Empty;
                delays[selectedHotkey] = 0;
            }
            //Когда только 1 группа комбинаций
            else
            {
                DeleateGroupCombnations.IsEnabled = true;
                DelayTextBlock.IsEnabled = true;
                //NextGroupKeys.IsEnabled = true;
                //LastGroupKeys.IsEnabled = true;
            }
            ChangeSelectedGroupHotKey(Convert.ToInt16(selectedHotkey + 1));
        }

        //Событие смены группы
        private void ChangeSelectedGroupHotKey(short newValueOfSelectedHotKey, bool save = true)
        {
            //При желании можно не сохранять настройки предыдуще группы, а просто сменить её
            if (save)
            {
                hotkeys[selectedHotkey - 1] = KeysTextBlock.Text;
                try
                {
                    delays[selectedHotkey - 1] = Convert.ToInt32(DelayTextBlock.Text);
                }
                catch { }
                
            }
            selectedHotkey = newValueOfSelectedHotKey;
            //Когда вы,ранны последняя группа, задерка не активна и равна нулю
            if (selectedHotkey == hotkeys.Length)
            {
                DelayTextBlock.IsEnabled = false;
                delays[selectedHotkey - 1] = 0;
            }
            else
                DelayTextBlock.IsEnabled = true;
            NumberOfGroupTextBlock.Text = Convert.ToString(selectedHotkey);
            KeysTextBlock.Text = hotkeys[selectedHotkey - 1];
            DelayTextBlock.Text = Convert.ToString(delays[selectedHotkey - 1]);
        }

        //Изменение текста задержки
        private void DelayTextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToUInt64(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            //sw.Localizate += Localizate;
            //sw.Owner = this;
            sw.Show();
        }


        //Нажатие клавиш в форме
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (AddKeyToCombination.IsChecked == true)
            {
                //Отключает стандартные действия windows для ниже указанных клавиш
                if(e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.Tab ||
                   e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.Up || e.Key == Key.Down)
                {
                    e.Handled = true;
                }
                KeysTextBlock.Text += e.Key.ToString() + " ";
                AddKeyToCombination.IsChecked = false;
            }
        }

        //Конец системы комбинацйий
    }
}
