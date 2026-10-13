    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace SoundScenesOpenAL_Library.Models
{
    public class Scene
    {
        public List<Source> Sources { get; set; } = new();
        public Listener Listener { get; set; } = new();
        public string Name { get; set; } = "DefaultScene";
        [JsonIgnore]
        public float Duration
        {
            get
            {
                return Sources
                    .SelectMany(s => s.Path)
                    .Select(p => p.Time)
                    .DefaultIfEmpty(0f)
                    .Max();
            }
        }

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
            // Disallow: nieznane pola (np. stare TimeStart/TimeEnd) dają błąd zamiast cicho zerować czasy
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
            };

            string json = File.ReadAllText(path);
            var scene = JsonSerializer.Deserialize<Scene>(json, options)
                ?? throw new InvalidDataException($"Plik {path} nie zawiera sceny.");
            scene.Validate();
            return scene;
        }

        // Sprawdza, czy każde źródło ma głośność 0..1 i ścieżkę z nieujemnymi, niemalejącymi czasami
        public void Validate()
        {
            foreach (var source in Sources)
            {
                // OpenAL obcina wynikową głośność do AL_MAX_GAIN (domyślnie 1), więc Gain > 1 działałby tylko z daleka
                if (source.Gain < 0f || source.Gain > 1f)
                    throw new InvalidDataException($"Źródło '{source.Name}': Gain {source.Gain} musi być w zakresie 0..1.");

                if (source.Path == null || source.Path.Count == 0)
                    throw new InvalidDataException($"Źródło '{source.Name}' nie ma żadnego punktu ruchu.");

                for (int i = 0; i < source.Path.Count; i++)
                {
                    float time = source.Path[i].Time;
                    if (time < 0f)
                        throw new InvalidDataException($"Źródło '{source.Name}', punkt {i}: czas {time} jest ujemny.");
                    if (i > 0 && time < source.Path[i - 1].Time)
                        throw new InvalidDataException($"Źródło '{source.Name}', punkt {i}: czas {time} jest mniejszy niż w poprzednim punkcie ({source.Path[i - 1].Time}).");
                }
            }
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
