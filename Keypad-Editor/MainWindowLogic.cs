﻿using System.Windows;
using System.Text;

namespace Keypad_Editor
{
    public class MainWindowLogic
    {
        private readonly MainWindow Window; // Pointer to main window

        public enum KeypadActions : byte
        {
            none,
            open,
            type,
            pressCombination
        }

        // 255 (0xFF) is reserved. If this variables have 255 it will mean key isn't selected
        public const byte KEY_DONT_SELECTED = 255;
        public byte selectedKey = KEY_DONT_SELECTED;
        private byte lastSelectedKey = KEY_DONT_SELECTED;
        public KeypadActions selectedAction;

        // Group system
        private List<Group> Groups = []; // The list of all groups in settings file
        private Group currentGroup; // "Pointer" to the current group

        // Cache system
        private KeypadActions[] ActionsInFile;
        private string[] ParametrsInFile;
        private KeypadActions[] newActions;
        private string[] newParametrs;

        // Combination system
        struct CombinationUnit(string keys, uint delay)
        {
            public string keys = keys;
            public uint delay = delay;
        }

        List<CombinationUnit> combination = [];
        private int currentCombination = 1; // Index to current keys combination. Starts with 1

        public MainWindowLogic (MainWindow owner)
        {
            Window = owner;

            // Reading data from the file
            try { Groups = new List<Group>(GroupsLogic.ReadFile()); }
            catch
            {
                // File doesn't exist or has incompatible object format
                // TODO: Error messege with offers to fix it
            }

            try { currentGroup = GroupsLogic.FindGroup(Groups, App.AppData.InitialGroupName); }
            catch
            {
                // There isn't inital group in file
                // TODO: Window to choose new inital group from the avialable
                currentGroup = new Group();
            }

            GroupsLogic.ParseGroup(currentGroup, out ActionsInFile, out ParametrsInFile);

            newActions = (KeypadActions[])ActionsInFile.Clone();
            newParametrs = (string[])ParametrsInFile.Clone();
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
                if ((lastSelectedKey != KEY_DONT_SELECTED) & App.AppData.Cache)
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
                    SaveCombinationUnit();
                    StringBuilder param = new StringBuilder();
                    for (int i = 0; i < combination.Count; i++)
                    {
                        param.Append(combination[i].keys.Replace(' ', '|') + " ");
                        param.Append(combination[i].delay + " ");
                    }
                    newParametrs[lastSelectedKey] = param.Remove(param.Length - 1, 1).ToString();
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

                currentCombination = 1;
                Window.NumberOfGroupTextBlock.Text = "1";
                Window.KeysTextBlock.Text = "";
                Window.DelayTextBlock.Text = "0";
                Window.DeleateGroupCombnations.IsEnabled = false;
                combination = [new CombinationUnit("", 0)];
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
                                keys = parts[i].Replace('|', ' '),
                                delay = Convert.ToUInt32(parts[i + 1])
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
                        JumpToGroup(1, false);

                        if (combination.Count == 1)
                            Window.DeleateGroupCombnations.IsEnabled = false;
                        else
                            Window.DeleateGroupCombnations.IsEnabled = true;
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

        /// <summary>
        /// Deletes the last one key in the combination.
        /// </summary>
        public void DeleateKeyFromCombination()
        {
            var keys = Window.KeysTextBlock.Text.Split(' ');
            StringBuilder text = new StringBuilder(Window.KeysTextBlock.Text.Length);
            // (keys.Length - 2) because the last one element is empty string
            for (int i = 0; i < keys.Length - 2; i++)
            {
                text.Append(keys[i] + ' ');
            }
            Window.KeysTextBlock.Text = text.ToString();
        }

        /// <summary>
        /// Adds new group of combination
        /// </summary>
        public void AddGropOfCombination()
        {
            SaveCombinationUnit();
            // We new to insert new combination unit to the (index of current combination + 1)
            // But currentCombination is already index + 1 so we insert to currentCombination
            combination.Insert(currentCombination, new CombinationUnit("", 0));
            JumpToGroup(currentCombination + 1);
            Window.DeleateGroupCombnations.IsEnabled = true;
        }

        /// <summary>
        /// Deletes new group of combination
        /// </summary>
        public void DeleteGropOfCombination()
        {
            combination.RemoveAt(currentCombination - 1);
            JumpToGroup(currentCombination, false);
            if (combination.Count == 1)
                Window.DeleateGroupCombnations.IsEnabled = false;
        }

        /// <summary>
        /// Switches group of combination to the next one
        /// </summary>
        public void ToTheNextGroupOfCombination()
        {
            SaveCombinationUnit();
            JumpToGroup(currentCombination + 1);
        }

        /// <summary>
        /// Switches group of combination to the previous one
        /// </summary>
        public void ToThePreviousGroupOfCombination()
        {
            SaveCombinationUnit();
            JumpToGroup(currentCombination - 1);
        }

        /// <summary>
        /// Jumps to the group at index. If index is out of number of group, it will jump to the last one, or to the first one.
        /// </summary>
        /// <param name="index">Number of group jump to.</param>
        /// <param name="optimization">Enable optimization when switching to the same combination group.</param>
        public void JumpToGroup(int index, bool optimization = true)
        {
            if (index < 1)
            {
                JumpToGroup(1);
            }
            else if (index > combination.Count)
            {
                JumpToGroup(combination.Count);
            }
            // If combination wasn't changed, it will skip this function
            else if (optimization && currentCombination == index)
            {
                return;
            }
            else
            {
                Window.NumberOfGroupTextBlock.Text = index.ToString();
                // If keys is empty string it will not add extra space
                if (combination[index - 1].keys != "")
                    Window.KeysTextBlock.Text = combination[index - 1].keys + ' ';
                else
                    Window.KeysTextBlock.Text = "";
                Window.DelayTextBlock.Text = combination[index - 1].delay.ToString();
                currentCombination = index;
            }
        }

        /// <summary>
        /// Saves data from the window to the combination list.
        /// </summary>
        private void SaveCombinationUnit()
        {
            string keys;
            if (Window.KeysTextBlock.Text == "")
                keys = "";
            else
                keys = Window.KeysTextBlock.Text.Remove(Window.KeysTextBlock.Text.Length - 1);

            combination[currentCombination - 1] = new CombinationUnit(
                keys,
                Convert.ToUInt32(Window.DelayTextBlock.Text)
                );
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

            currentGroup.Actions = ActionsInFile.Select(i => (int)i).ToArray();
            currentGroup.Parametrs = ParametrsInFile;

            GroupsLogic.WriteToFile(Groups);
        }
    }
}
