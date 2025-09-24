using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace BinarySoundOpenAL_Library
{
    public class Scene
    {
        public List<Source> Sources { get; set; } = new();
        public Listener Listener { get; set; } = new();
        public string Name { get; set; } = "DefaultScene";

        public void AddSource(Source source) => Sources.Add(source);
        public void RemoveSource(Source source) => Sources.Remove(source);
        public void SetListener(Listener listener) => Listener = listener;

        // Zapis do JSON
        public void SaveToJson(string path)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(path, JsonSerializer.Serialize(this, options));
        }

        // Wczytywanie z JSON
        public static Scene LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Scene>(json);
        }
    }

}
