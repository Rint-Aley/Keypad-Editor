using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Diagnostics;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.IO;
using System.IO.Ports;

namespace WPF_test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : System.Windows.Window
    {

        ///Объявление переменных
        private short selectedKey = 0, lastSelectedKey = 0;
        string DontSelectedAnyButtons;

        //Кэш
        bool cache = true;
        string[] activityInFile = new string[App.NumberOfKeys];
        string[] parametrInFile = new string[App.NumberOfKeys];
        string[] activityNew = new string[App.NumberOfKeys];
        string[] parametrNew = new string[App.NumberOfKeys];
        //Кэш
        string selectedActivity = String.Empty;

        //Комбинации
        private short selectedHotkey = 1;

        string[] hotkeys = new string[1];
        int[] delays = new int[1];
        ///Объявление переменных
        FileEditor settingsEditor = new FileEditor();
        

        public MainWindow()
        {
            InitializeComponent();
            Localizate();
            UpdateListButton_Click(UpdateListButton, null);

            FillingInArrays();
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
            pressCombimation.Content = iniFile.Read("Combination", "ActivityButtons");
            Apply.Content = iniFile.Read("Apply.Text", "ActivityGrids");
            selectDirectory.Content = iniFile.Read("SelectDirectory", "ActivityGrids");
            TipTypingGrid.Text = iniFile.Read("TipTypingGrid", "ActivityGrids");
            CombinationTip1.Text = iniFile.Read("Tip1", "CombinationGrid");
            CombinationTip2.Text = iniFile.Read("Tip2", "CombinationGrid");
            AddKeyToCombination.ToolTip = iniFile.Read("AddKeyToCombination.ToolTip", "CombinationGrid");
            DeleateKeyButton.ToolTip = iniFile.Read("DeleateKeyButton.ToolTip", "CombinationGrid");
            LastGroupKeys.ToolTip = iniFile.Read("LastGroupKeys.ToolTip", "CombinationGrid");
            NextGroupKeys.ToolTip = iniFile.Read("NextGroupKeys.ToolTip", "CombinationGrid");
            DelayTextBlock.ToolTip = iniFile.Read("DelayTextBlock.ToolTip", "CombinationGrid");
            DeleateGroupCombnations.ToolTip = iniFile.Read("DeleateGroupCombnations.ToolTip", "CombinationGrid");
            AddGroupCombimations.ToolTip = iniFile.Read("AddGroupCombimations.ToolTip", "CombinationGrid");

            SettingsButton.Width = Convert.ToDouble(iniFile.Read("SettingsButton.Width", "DeviceBlock"));
            ToCurrentSettings.Width = Convert.ToDouble(iniFile.Read("ToCurrentSettingsButton.Width", "DeviceBlock"));

            DontSelectedAnyButtons = iniFile.Read("ErrorKeyIsNotSelected", "DeviceBlock");
        }

        //Window
        
        //Кнопка закрытия
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        //Перемещение окна
        private void TopMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        //Window.End

        //WorkerPart

        private void BigDevice_Click(object sender, RoutedEventArgs e)
        {
            BigDeviceGrid.Visibility = Visibility.Hidden;
            MainWindowGrid.Visibility = Visibility.Visible;
            //Анимация смены
        }

        //Заполняет массивы
        private void FillingInArrays()
        {
            for (short i = 0; i < App.NumberOfKeys; i++)
            {
                string[] kommands = settingsEditor.readLine(i + 1).Split(' ');
                if (kommands.Length > 1) activityInFile[i] = kommands[1];
                else activityInFile[i] = "None";
                if (kommands.Length > 2)
                {
                    if (activityInFile[i] == "type")
                    {
                        int a = kommands.Length; //сколько слов в массиве
                        string text = String.Empty; // Финальный параметр
                        for (int j = 2; j < a; j++) text += kommands[j] + " "; //Пока не переберёт все слова
                        parametrInFile[i] = text.Remove(text.Length - 1);
                    }
                    else if (activityInFile[i] == "pressCombimation")
                    {
                        int a = kommands.Length; //сколько слов в массиве
                        string text = String.Empty; // Финальный параметр
                        for (int j = 2; j < a; j++) text += kommands[j] + " "; //Пока не переберёт все слова
                        parametrInFile[i] = text.Remove(text.Length - 1);
                    }
                    else if (activityInFile[i] == "open")
                    {
                        int a = kommands.Length; //сколько слов в массиве
                        string text = String.Empty; // Финальный параметр
                        for (int j = 2; j < a; j++) text += kommands[j] + " "; //Пока не переберёт все слова
                        parametrInFile[i] = text;

                    }
                }
                else parametrInFile[i] = "";
            }
            Array.Copy(activityInFile, activityNew, App.NumberOfKeys);
            Array.Copy(parametrInFile, parametrNew, App.NumberOfKeys);

        }

        //Переводит значения массивов к текущим
        private void ToCurrentSettings_Click(object sender, RoutedEventArgs e)
        {
            // Изменяет значяение массивов
            for (short i = 0; i < App.NumberOfKeys; i++)
            {
                activityNew[i] = activityInFile[i];
                parametrNew[i] = parametrInFile[i];
            }
            //Изменяет текущее окно
            if (selectedKey != 0) changedSelectedKey();
        }

        //Нажатите на кнопку устройства
        private void Key_Click(object sender, RoutedEventArgs e)
        {
            var selectedKeyTag = ((ToggleButton)sender).Tag;
            selectedKey = Convert.ToInt16(selectedKeyTag);
            if (lastSelectedKey != selectedKey) changedSelectedKey(); //При повторном нажатии на одну и ту же кнопку ничего не произойдёт
            lastSelectedKey = selectedKey;
        }

        //Очиска предыдущих значений
        private void ClearFillingItems()
        {
            selectedActivity = String.Empty;
            PathToFileOrWebsite.Text = String.Empty;
            TextToType.Text = String.Empty;

            KeysTextBlock.Text = String.Empty;
            DelayTextBlock.Text = String.Empty;
            selectedHotkey = 1;
            Array.Resize(ref hotkeys, 1);
            Array.Resize(ref delays, 1);
            hotkeys[0] = String.Empty;
            delays[0] = 0;
            NumberOfGroupTextBlock.Text = "1";

            DeleateGroupCombnations.IsEnabled = false;
            DelayTextBlock.IsEnabled = false;
            NextGroupKeys.IsEnabled = false;
            LastGroupKeys.IsEnabled = false;
        }

        //Обрабатывает смену настраиваемой клавиши
        void changedSelectedKey()
        {
            //Перезапись настроек, когда мы сменили настраиваемую клавишу и у нас включены закладки
            if ((lastSelectedKey != 0) & cache & (lastSelectedKey != selectedKey)) WriteNewValues();

            ClearFillingItems();

            //Ставит значения параметров из файла
            switch (activityNew[selectedKey - 1])
            {
                case "open":
                    MovingButton_Click(open, null);
                    PathToFileOrWebsite.Text = parametrNew[selectedKey - 1];
                    break;
                case "type":
                    MovingButton_Click(type, null);
                    TextToType.Text = parametrNew[selectedKey - 1];
                    break;
                case "pressCombimation":
                    MovingButton_Click(pressCombimation, null);
                    //Параметры комбинации клавиш
                    if (parametrNew[selectedKey - 1] != "" & parametrNew[selectedKey - 1] != null)
                    {
                        string[] parametr = parametrNew[selectedKey - 1].Split(' ');
                        //Заполнение массивов
                        int numberOfCombinationInt = (parametr.Length + 1) / 2;
                        Array.Resize(ref hotkeys, numberOfCombinationInt);
                        Array.Resize(ref delays, numberOfCombinationInt);
                        for(int i = 0; i < numberOfCombinationInt; i++)
                        {
                            hotkeys[i] = parametr[i*2].Replace('|', ' ') + " ";
                            if(i+1 != numberOfCombinationInt)
                                delays[i] = Convert.ToInt32(parametr[i*2+1]);
                        }
                        if(numberOfCombinationInt != 1)
                        {
                            DeleateGroupCombnations.IsEnabled = true;
                            DelayTextBlock.IsEnabled = true;
                            NextGroupKeys.IsEnabled = true;
                            LastGroupKeys.IsEnabled = true;
                        }
                        //Визуализация информации
                        ChangeSelectedGroupHotKey(1, false);
                    }
                    break;
                default:
                    DeleteActivity();
                    Apply.Visibility = Visibility.Hidden;
                    break;
            }
            //selectedActivity нужно изменять после установки значений параметров
            selectedActivity = activityNew[selectedKey - 1];
        }

        //Записывает значения прошлой настраиваемолй кнопки
        void WriteNewValues()
        {
            activityNew[lastSelectedKey - 1] = selectedActivity;
            switch (selectedActivity)
            {
                case "open":
                    parametrNew[lastSelectedKey - 1] = PathToFileOrWebsite.Text;
                    break;
                case "type":
                    parametrNew[lastSelectedKey - 1] = TextToType.Text;
                    break;
                case "pressCombimation":
                    //Сохраняем текущую выделенную группу комбинаций
                    hotkeys[selectedHotkey - 1] = KeysTextBlock.Text;
                    try
                    {
                        delays[selectedHotkey - 1] = Convert.ToInt32(DelayTextBlock.Text);
                    }
                    catch { delays[selectedHotkey - 1] = 0; }
                    
                    string result = String.Empty;

                    for (int i = 0; i < hotkeys.Length; i++)
                    {
                        //Record keys
                        if(hotkeys[i].Length > 0)
                            result += hotkeys[i].Replace(' ', '|').Remove(hotkeys[i].Length - 1) + " ";
                        else
                            result += " ";
                        //Record delays
                        //Если не последняя итерация цикла то
                        if (i + 1 != hotkeys.Length)
                            result += delays[i] + " ";
                    }
                    parametrNew[lastSelectedKey - 1] = result.Remove(result.Length - 1);
                    break;
                default:
                    parametrNew[lastSelectedKey - 1] = String.Empty;
                    break;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (selectedKey != 0) WriteNewValues();
            if(parametrNew.SequenceEqual(parametrInFile) == false || activityNew.SequenceEqual(activityInFile) == false)
            {
                YesNoForm yesNo = new YesNoForm("Doesn't save changes", "Are you going to save changes?");
                yesNo.ShowDialog();
                //Если выбрано нет
                if (!yesNo.Result)
                {
                    Array.Copy(parametrInFile, parametrNew, parametrInFile.Length);
                    Array.Copy(activityInFile, activityNew, activityInFile.Length);
                }
                else
                    Apply_Click(Apply, null);
                
            }
            BigDeviceGrid.Visibility = Visibility.Visible;
            MainWindowGrid.Visibility = Visibility.Hidden;
            selectedKey = 0;
            lastSelectedKey = 0;
            //Очищаем экран
            ClearFillingItems();
            DeleteActivity();
            foreach (System.Windows.Controls.RadioButton radioButton in RadioButtonsOfDevice.Children)
            {
                radioButton.IsChecked = false;
            }
        }



        //Изменение действия
        private void MovingButton_Click(object sender, RoutedEventArgs e)
        {
            //selectedMove - указатель на выбранное действие
            ToggleButton selectedMove = sender as ToggleButton;
            Thread clearErrorTextBlock = new Thread(runTimer);
            //Остановка потока
            if (selectedKey == 0)
            {
                selectedMove.IsChecked = false;
                //Вывод ошибки
                ErrorTextBlock.Text = DontSelectedAnyButtons;
                clearErrorTextBlock.Start();
            }
            //При двойом нажатии на одну и ту же кнопку, она отключается
            else if (selectedMove.Name == selectedActivity)
            {
                DeleteActivity();
                Apply.Visibility = Visibility.Hidden;
                //selectedMove.IsChecked = false;
            }
            else
            {
                if(selectedMove.IsChecked != true) selectedMove.IsChecked = true;
                DeleteActivity();
                selectedMove.IsChecked = true;
                if (Apply.Visibility == Visibility.Hidden) Apply.Visibility = Visibility.Visible;
                switch (selectedMove.Name)
                {
                    case "open":
                        OpenGrid.Visibility = Visibility.Visible;
                        selectedActivity = "open";
                        break;
                    case "type":
                        TypeGrid.Visibility = Visibility.Visible;
                        selectedActivity = "type";
                        break;
                    case "pressCombimation":
                        CombinationGrid.Visibility = Visibility.Visible;
                        selectedActivity = "pressCombimation";
                        break;
                }
            }
        }

        //Метод для исчезнвения текста
        private async void runTimer()
        {
            await Task.Delay(2000);
            Dispatcher.BeginInvoke(new Action(() => updateScreen()));
        }

        private void updateScreen()
        {
            ErrorTextBlock.Text = "";
        }

        //Метод для исчезнвения текста



        //Скрывает все контейнеры действий 
        private void DeleteActivity()
        {
            OpenGrid.Visibility = Visibility.Hidden;
            TypeGrid.Visibility = Visibility.Hidden;
            CombinationGrid.Visibility = Visibility.Hidden;

            open.IsChecked = false;
            type.IsChecked = false;
            pressCombimation.IsChecked = false;

            selectedActivity = "";
        }

        //Кнопка выбора файла
        private void selectDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                if (PathToFileOrWebsite.Text != string.Empty)
                {
                    openFileDialog.InitialDirectory = PathToFileOrWebsite.Text; //Открывает нужную деррикторию
                    openFileDialog.FileName = System.IO.Path.GetFileName(PathToFileOrWebsite.Text); //Пишет в имя уже выбранный файл
                }
                else openFileDialog.InitialDirectory = @"C:\";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) PathToFileOrWebsite.Text = openFileDialog.FileName;
            }
        }

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

        //Применить
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            //Сохраняет значения текущей кнопки
            WriteNewValues();

            for (short i = 0; i < App.NumberOfKeys; i++)
            {
                if (activityNew[i] == "combination")
                {
                    settingsEditor.ChangeText(i + 1, "hotKey", parametrNew[i]);
                }
                else
                    settingsEditor.ChangeText(i + 1, activityNew[i], parametrNew[i]);
            }

            FillingInArrays();
        }


        //Система комбинцаций

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
                NextGroupKeys.IsEnabled = false;
                LastGroupKeys.IsEnabled = false;
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
                NextGroupKeys.IsEnabled = true;
                LastGroupKeys.IsEnabled = true;
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

        //Port settings
        
        private void UpdateListButton_Click(object sender, RoutedEventArgs e)
        {
            SerialPort serialPort = new SerialPort();
            foreach(var portName in SerialPort.GetPortNames())
            {
                ListOfPorts.Items.Add(portName);
            }
        }

        private void AutomaticSearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ApplyPortButton_Click(object sender, RoutedEventArgs e)
        {

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
