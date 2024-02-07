using System.IO;
using System.Text.Json;

namespace Keypad_Editor
{
    public class Group
    {
        public string? Name { get; set; }
        public int[]? Actions{ get; set; }
        public string[]? Parametrs { get; set; }
    }

    public static class GroupsLogic
    {
        public static string FilePath = Path.GetFullPath("data/Settings.json");
        public static Group[] ReadFile()
        {
            var groups = JsonSerializer.Deserialize<Group[]>(File.ReadAllText(FilePath));
            if (groups is null)
                throw new Exception("Unreadable objects format.");
            return groups;
        }

        //Done
        public static void ParseGroup(Group group, out MainWindowLogic.KeypadActions[] keypadActions, out string[] parametrs) 
        {
            keypadActions = new MainWindowLogic.KeypadActions[App.AppData.NumberOfKeys];
            parametrs = new string[App.AppData.NumberOfKeys];

            if (group.Actions is not null)
            {
                for (int i = 0; i < App.AppData.NumberOfKeys; i++)
                {
                    try { keypadActions[i] = (MainWindowLogic.KeypadActions)group.Actions[i]; }
                    catch { }
                }
            }

            if (group.Parametrs is not null)
            {
                for (int i = 0; i < App.AppData.NumberOfKeys; i++)
                {
                    try { parametrs[i] = group.Parametrs[i]; }
                    catch { parametrs[i] = ""; }
                }
            }
            else
                for (int i = 0; i < App.AppData.NumberOfKeys; i++)
                    parametrs[i] = "";
        }

        public static Group FindGroup(List<Group> groups, string name)
        {
            foreach (var group in groups)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }
            throw new Exception("Object wasn't found.");
            // TODO: Perhaps I should to add a handler if currentGroup is null
            //TODO: clear window
        }

        public static void WriteToFile(List<Group> list)
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(list));
        }
    }
}
