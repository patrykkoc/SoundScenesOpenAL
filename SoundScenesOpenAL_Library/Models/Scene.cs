using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace SoundScenesOpenAL_Library.Models
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
            var options = new JsonSerializerOptions { WriteIndented = true , IncludeFields = true };
            File.WriteAllText(path, JsonSerializer.Serialize(this, options));
        }

        // Wczytywanie z JSON
        public static Scene LoadFromJson(string path)
        {
            var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Scene>(json, options);
        }

        public void InitializeFromJson(string path)
        {
            var loaded = LoadFromJson(path);
            Name = loaded.Name;
            Listener = loaded.Listener;
            Sources = loaded.Sources;
        }
    }

}
