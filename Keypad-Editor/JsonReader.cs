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
    public static class JsonReader
    {
        public static string path = Path.GetFullPath("data/Settings.json");
        public static Group[] Read()
        {
            var groups = JsonSerializer.Deserialize<Group[]>(File.ReadAllText(path));
            if (groups != null)
                return groups;
            else
                return new Group[0];
        }
        public static void Write(List<Group> list)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(list));
        }
        public static void Parse()
        {
            
        }
    }
}
