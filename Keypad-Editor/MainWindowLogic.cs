using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Keypad_Editor
{
    public class MainWindowLogic
    {
        MainWindow Window;

        //Fields decloration
        private short selectedKey, lastSelectedKey;
        private string[] actions = new string[] { "none", "open", "type", "pressCombination" };
        public enum action : byte
        {
            none = 0,
            open = 1,
            type = 2,
            pressCombination = 3
        }
        action selectedAction;
        
        struct pieceOfCombination
        {
            uint Delay;
            string Keys;
        }

        //Cache system
        bool cache = true;
        action[] actionInFile = new action[App.NumberOfKeys];
        action[] actionNew = new action[App.NumberOfKeys];
        string[] parametrInFile = new string[App.NumberOfKeys];
        string[] parametrNew = new string[App.NumberOfKeys];

        public MainWindowLogic (MainWindow owner)
        {
            Window = owner;
        }

        /// <summary>
        /// Reads data about actions for device and fills arrays to get fast access to it.
        /// </summary>
        public void ReadDataFromFile(ushort pointer)
        { 
            for(int i = 0; i < App.NumberOfKeys; i++)
            {
                string line = FileEditor.readLine(i + 1);

                if (line == null) //if it's the last line
                {
                    //calls settings parser to "repair" file
                }

                string[] commandUnit = line.Split(' ');

                //Checks for invalid number of commands
                if (commandUnit.Length == 2)
                {
                    for (int j = 0; j < actions.Length; j++)
                    {
                        if (commandUnit[1] == actions[j])
                        {
                            actionInFile[i] = (action)j;
                            break;
                        }
                    }
                    parametrInFile[i] = String.Empty;
                    continue;
                }
                else if (commandUnit.Length < 2)
                {
                    actionInFile[i] = 0;
                    parametrInFile[i] = String.Empty;
                    continue;
                }

                //Assigns action
                for (int j = 0; j < actions.Length; j++)
                {
                    if (commandUnit[1] == actions[j])
                    {
                        actionInFile[i] = (action) j;
                        break;
                    }
                }

                //Assigns parametr
                if (actionInFile[i] == action.none)
                    parametrInFile[i] = "";
                else
                {
                    //Insert all text after command into param
                    string param = string.Empty;
                    for (int j = 2; j < commandUnit.Length; j++)
                    {
                        param += commandUnit[j] + " ";
                    }
                    param.Remove(param.Length - 1);

                    parametrInFile[i] = param;
                }
            }
            actionInFile.CopyTo(actionNew, 0);
            parametrInFile.CopyTo(parametrNew, 0);
        }

        public void retunToOldSettings()
        {
            actionInFile.CopyTo(actionNew, 0);
            parametrInFile.CopyTo(parametrNew, 0);
            HideControls(true);
            selectedAction = actionInFile[selectedKey];
            changeDisplayedGrid(true);
        }

        public void changeKey(short newKey)
        {
            selectedKey = (short)(newKey - 1);
            if(lastSelectedKey != selectedKey)
            {
                if ((selectedKey != 0) & cache)
                    addDataToCache();
                HideControls(true);
                selectedAction = actionNew[selectedKey];
                changeDisplayedGrid(true);
                lastSelectedKey = selectedKey;
            }
        }

        private void addDataToCache()
        {
            actionNew[lastSelectedKey] = selectedAction;
            switch (selectedAction)
            {
                case action.open:
                    parametrNew[lastSelectedKey] = Window.PathToFileOrWebsite.Text;
                    break;

                case action.type:
                    parametrNew[lastSelectedKey] = Window.TextToType.Text;
                    break;

                case action.pressCombination:
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
                case action.open:
                    Window.OpenGrid.Visibility = Visibility.Collapsed;
                    break;

                case action.type:
                    Window.TypeGrid.Visibility = Visibility.Collapsed;
                    break;

                case action.pressCombination:
                    Window.CombinationGrid.Visibility = Visibility.Collapsed;
                    //TODO: Fill it
                    break;
            }
            if (clearData)
            {
                Window.PathToFileOrWebsite.Text = String.Empty;
                Window.TextToType.Text = String.Empty;
                //TODO: pressCombination
            }
        }

        /// <summary>
        /// Shows grid of the current selected action.
        /// </summary>
        /// <param name="setValues">Whether it will set parameter's value to selected.</param>
        private void changeDisplayedGrid(bool setValues)
        {
            switch (selectedAction)
            {
                case action.open:
                    Window.OpenGrid.Visibility = Visibility.Visible;
                    if (setValues) Window.PathToFileOrWebsite.Text = parametrNew[selectedKey];
                    break;

                case action.type:
                    Window.TypeGrid.Visibility = Visibility.Visible;
                    if (setValues) Window.TextToType.Text = parametrNew[selectedKey];
                    break;

                case action.pressCombination:
                    Window.CombinationGrid.Visibility = Visibility.Visible;
                    //TODO: setValues 
                    break;
            }
        }

        
        public void changeAction(action newAction)
        {
            HideControls(false);
            selectedAction = newAction;
            changeDisplayedGrid(false);
        }

        public void SaveSettings()
        {
            //First of all we save command for current button
            addDataToCache();

            actionNew.CopyTo(actionInFile, 0);
            parametrNew.CopyTo(parametrInFile, 0);

            for(short i = 0; i < App.NumberOfKeys; i++)
            {

            }
        }
    }
}
