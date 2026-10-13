using SoundScenesOpenAL_Library;
using SoundScenesOpenAL_Library.Models;

namespace SoundScenesOpenAL_Console
{
    public class EntryPoints
    {
        // Ścieżki względem katalogu roboczego (katalog wyjściowy programu)
        private const string ScenesDirectory = "Resources/Sceny";
        private const string TestScenePath = "Resources/scene_test.json";

        // Pozycja menu: plik sceny i opis wyświetlany użytkownikowi
        private record SceneEntry(string Path, string Label);

        public static void Main(string[] args)
        {
            while (true)
            {
                var scenes = FindScenes();
                int testOption = scenes.Count + 1;

                Console.WriteLine();
                Console.WriteLine("=== SoundScenesOpenAL - demo ===");
                for (int i = 0; i < scenes.Count; i++)
                    Console.WriteLine($"{i + 1}. {scenes[i].Label}");
                Console.WriteLine($"{testOption}. Scena testowa z kodu (SceneCreateTest)");
                Console.WriteLine("0. Wyjście");
                Console.Write("Wybierz scenę: ");

                // null = koniec strumienia wejścia (np. przekierowane wejście)
                string? input = Console.ReadLine();
                if (input == null || input.Trim() == "0")
                    return;

                if (!int.TryParse(input, out int choice) || choice < 1 || choice > testOption)
                {
                    Console.WriteLine("Nieprawidłowy wybór.");
                    continue;
                }

                try
                {
                    Scene scene;
                    if (choice == testOption)
                    {
                        // Zapis sceny testowej do JSON i ponowny odczyt (dotychczasowe działanie konsoli)
                        SceneCreateTest.CreateTestScene();
                        scene = Scene.LoadFromJson(TestScenePath);
                    }
                    else
                    {
                        scene = Scene.LoadFromJson(scenes[choice - 1].Path);
                    }

                    PlayWithProgress(scene);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd: {ex.Message}");
                }
            }
        }

        // Wszystkie pliki .json z katalogu scen; nazwa i czas trwania czytane z pliku
        private static List<SceneEntry> FindScenes()
        {
            var entries = new List<SceneEntry>();
            if (!Directory.Exists(ScenesDirectory))
                return entries;

            foreach (var path in Directory.GetFiles(ScenesDirectory, "*.json").OrderBy(p => p))
            {
                try
                {
                    var scene = Scene.LoadFromJson(path);
                    entries.Add(new SceneEntry(path, $"{scene.Name} ({scene.Duration:0.#} s)"));
                }
                catch (Exception ex)
                {
                    // Błędna scena zostaje na liście - wybranie jej pokaże powód błędu
                    entries.Add(new SceneEntry(path, $"{Path.GetFileName(path)} (błąd: {ex.Message})"));
                }
            }
            return entries;
        }

        // Odtwarzanie w osobnym wątku; postęp w jednej linii, dowolny klawisz zatrzymuje scenę
        private static void PlayWithProgress(Scene scene)
        {
            var player = new ScenePlayer(scene);
            Exception? playError = null;
            var playThread = new Thread(() =>
            {
                try { player.Play(); }
                catch (Exception ex) { playError = ex; }
            });

            Console.WriteLine($"Odtwarzanie: {scene.Name}. Naciśnij dowolny klawisz, aby zatrzymać.");
            playThread.Start();

            bool stopped = false;
            while (!playThread.Join(100))
            {
                // KeyAvailable nie działa przy przekierowanym wejściu
                if (!Console.IsInputRedirected && Console.KeyAvailable)
                {
                    Console.ReadKey(intercept: true);
                    player.Stop();
                    stopped = true;
                }
                Console.Write($"\rt = {player.CurrentTime,5:0.0} / {scene.Duration:0.0} s");
            }
            Console.WriteLine();

            if (playError != null)
                Console.WriteLine($"Błąd odtwarzania: {playError.Message}");
            else
                Console.WriteLine(stopped ? "Zatrzymano." : "Koniec sceny.");
        }
    }
}
