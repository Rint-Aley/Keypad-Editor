using System.Windows;

namespace Keypad_Editor
{
    public class MainWindowLogic
    {
        private MainWindow Window; // Pointer to main window
        public enum KeypadActions : byte
        {
            none,
            open,
            type,
            pressCombination
        }

        private List<Group> Groups; // The list of all groups in settings file
        private Group? currentGroup; // "Pointer" to the current group

        // 255 (0xFF) is reserved. If this variables have 255 it will mean key isn't selected
        public const byte KEY_DONT_SELECTED = 255;
        public byte selectedKey = KEY_DONT_SELECTED, lastSelectedKey = KEY_DONT_SELECTED;

        public KeypadActions selectedAction;

        // Cache system
        bool cache = true;
        private KeypadActions[] ActionsInFile = new KeypadActions[App.NumberOfKeys];
        private string[] ParametrsInFile = new string[App.NumberOfKeys];
        private KeypadActions[] newActions = new KeypadActions[App.NumberOfKeys];
        private string[] newParametrs = new string[App.NumberOfKeys];

        // Combination system
        struct CombinationUnit
        {
            public string keys;
            public int delay;
        }

        List<CombinationUnit> combination;

        public MainWindowLogic (MainWindow owner)
        {
            Window = owner;
        }

        /// <summary>
        /// Reads data about actions for device and fills arrays to get fast access to it.
        /// </summary>
        public void ReadDataFromFile()
        {
            Groups = new List<Group>(JsonReader.Read());

            //TODO: Reading Inital group from App and using it
            SwitchGroup("main");

            if (currentGroup == null)
            {
                // There isn't inital group in file
                // TODO: Window to choose new inital group from the avialable
            }

            // TODO: Check if array of actions have the same number of elements as in config file 
            IntArrayToEmum(currentGroup.Actions, out ActionsInFile);
            currentGroup.Parametrs?.CopyTo(ParametrsInFile, 0);
            ActionsInFile.CopyTo(newActions, 0);
            ParametrsInFile.CopyTo(newParametrs, 0);
        }

        private void IntArrayToEmum(int[] intArr, out KeypadActions[] enumArr)
        {
            enumArr = new KeypadActions[App.NumberOfKeys];
            for (int i = 0; i < intArr.Length; i++)
            {
                enumArr[i] = (KeypadActions)intArr[i];
            }

        }

        public void RetunToOldSettings()
        {
            ActionsInFile.CopyTo(newActions, 0);
            ParametrsInFile.CopyTo(newParametrs, 0);
            HideControls(true);
            selectedAction = ActionsInFile[selectedKey];
            ChangeDisplayedGrid(true);
        }

        /// <summary>
        /// Handles changing key event.
        /// </summary>
        /// <param name="newKey">The key changes to...</param>
        public void ChangeKey(short newKey)
        {
            selectedKey = (byte)(newKey - 1);
            if(lastSelectedKey != selectedKey)
            {
                if ((lastSelectedKey != KEY_DONT_SELECTED) & cache)
                    AddDataToCache();
                else if (lastSelectedKey == KEY_DONT_SELECTED)
                    Window.Apply.Visibility = Visibility.Visible;
                HideControls(true);
                selectedAction = newActions[selectedKey];
                ChangeDisplayedGrid(true);
                lastSelectedKey = selectedKey;
            }
        }

        private void AddDataToCache()
        {
            newActions[lastSelectedKey] = selectedAction;
            switch (selectedAction)
            {
                case KeypadActions.open:
                    newParametrs[lastSelectedKey] = Window.PathToFileOrWebsite.Text;
                    break;

                case KeypadActions.type:
                    newParametrs[lastSelectedKey] = Window.TextToType.Text;
                    break;

                case KeypadActions.pressCombination:
                    //TODO: Fill it
                    break;
            }
        }

        /// <summary>
        /// Hides grid of the selected action.
        /// </summary>
        /// <param name="clearData">Whether it will clear control's content.</param>
        private void HideControls(bool clearData)
        {
            switch (selectedAction)
            {
                case KeypadActions.open:
                    Window.open.IsChecked = false;
                    Window.OpenGrid.Visibility = Visibility.Collapsed;
                    break;

                case KeypadActions.type:
                    Window.type.IsChecked = false;
                    Window.TypeGrid.Visibility = Visibility.Collapsed;
                    break;

                case KeypadActions.pressCombination:
                    Window.pressCombination.IsChecked = false;
                    Window.AddKeyToCombination.IsChecked = false;
                    Window.CombinationGrid.Visibility = Visibility.Collapsed;
                    break;
            }
            if (clearData)
            {
                Window.PathToFileOrWebsite.Text = String.Empty;
                Window.TextToType.Text = String.Empty;

                Window.NumberOfGroupTextBlock.Text = "1";
                Window.KeysTextBlock.Text = "";
                Window.DelayTextBlock.Text = "0";
                combination = new List<CombinationUnit> { new CombinationUnit() { keys = "", delay = 0 } };
            }
        }

        /// <summary>
        /// Shows grid of the current selected action.
        /// </summary>
        /// <param name="setValues">Whether it will set parameter's value to selected.</param>
        private void ChangeDisplayedGrid(bool setValues)
        {
            switch (selectedAction)
            {
                case KeypadActions.open:
                    Window.open.IsChecked = true;
                    Window.OpenGrid.Visibility = Visibility.Visible;
                    if (setValues) Window.PathToFileOrWebsite.Text = newParametrs[selectedKey];
                    break;

                case KeypadActions.type:
                    Window.type.IsChecked = true;
                    Window.TypeGrid.Visibility = Visibility.Visible;
                    if (setValues) Window.TextToType.Text = newParametrs[selectedKey];
                    break;

                case KeypadActions.pressCombination:
                    Window.pressCombination.IsChecked = true;
                    Window.CombinationGrid.Visibility = Visibility.Visible;
                    if (setValues)
                    {
                        string[] parts = newParametrs[selectedKey].Split(' ');
                        
                        combination = new List<CombinationUnit>(parts.Length / 2 + 1);
                        // comment to "i < parts.Length - 1"
                        // If parts.Length is even number it will add all values
                        // If it isn't it will add all values except the last one
                        for (int i = 0; i < parts.Length - 1; i += 2)
                        {
                            combination.Add(new CombinationUnit()
                            {
                                keys = parts[i],
                                delay = Convert.ToInt32(parts[i + 1])
                            });
                        }
                        // The last one elements will be added here (if it's necessary)
                        if (parts.Length % 2 != 0)
                        {
                            combination.Add(new CombinationUnit()
                            {
                                keys = parts[parts.Length - 1],
                                delay = 0
                            });
                        }

                        // Showing the first group on the screen
                        Window.NumberOfGroupTextBlock.Text = "1";
                        Window.KeysTextBlock.Text = combination[0].keys;
                        Window.DelayTextBlock.Text = combination[0].delay.ToString();
                    }
                    break;
            }
        }
        
        public void ChangeAction(string sNewAction)
        {
            KeypadActions newAction = (KeypadActions)Enum.Parse(typeof(KeypadActions), sNewAction);
            HideControls(false);
            selectedAction = newAction;
            ChangeDisplayedGrid(false);
        }

        // Combination logic

        public void DeleateKeyFromCombination()
        {

        }
        public void AddGrop()
        {

        }

        public void ToTheNextGroupOfCombination()
        {

        }

        public void ToThePreviousGroupOfCombination()
        {

        }

        // Combination logic

        /// <summary>
        /// Saves all settings to a file
        /// </summary>
        public void SaveSettings()
        {
            //First of all we save command for current button
            AddDataToCache();

            newActions.CopyTo(ActionsInFile, 0);
            newParametrs.CopyTo(ParametrsInFile, 0);

            Group newGroup = new() {
                Name = currentGroup?.Name,
                Actions = ActionsInFile.Select(i => (int)i).ToArray(),
                Parametrs = ParametrsInFile
            };
            Groups.Remove(currentGroup);
            Groups.Add(newGroup);

            JsonReader.Write(Groups);
        }

        private void SwitchGroup(string name)
        {
            foreach (var group in Groups)
            {
                if (group.Name == name)
                {
                    currentGroup = group;
                }
            }
            // TODO: Perhaps I should to add a handler if currentGroup is null
            //TODO: clear window
        }
    }
}
