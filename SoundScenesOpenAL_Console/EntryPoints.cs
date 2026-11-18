using SoundScenesOpenAL_Library;
using SoundScenesOpenAL_Library.Audio;
using SoundScenesOpenAL_Library.Models;

namespace SoundScenesOpenAL_Console
{
    public class EntryPoints
    {
        // Ścieżki względem katalogu roboczego (katalog wyjściowy programu)
        private const string ScenesDirectory = "Resources/Sceny";

        // Pozycja menu: plik sceny i opis wyświetlany użytkownikowi
        private record SceneEntry(string Path, string Label);

        // Tryb HRTF dla kolejnych odtwarzań; przełączany w menu klawiszem "h"
        private static HrtfMode _hrtfMode = HrtfMode.Default;

        public static void Main(string[] args)
        {
            // Ścieżki scen i dźwięków są względne - katalog roboczy = folder programu,
            // żeby działało niezależnie od tego, skąd uruchomiono .exe
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            while (true)
            {
                var scenes = FindScenes();

                Console.WriteLine();
                Console.WriteLine("=== SoundScenesOpenAL - demo ===");
                for (int i = 0; i < scenes.Count; i++)
                    Console.WriteLine($"{i + 1}. {scenes[i].Label}");
                Console.WriteLine($"h. HRTF: {DescribeMode(_hrtfMode)} (zmień)");
                Console.WriteLine("0. Wyjście");
                Console.Write("Wybierz scenę: ");

                // null = koniec strumienia wejścia (np. przekierowane wejście)
                string? input = Console.ReadLine();
                if (input == null || input.Trim() == "0")
                    return;

                if (input.Trim().Equals("h", StringComparison.OrdinalIgnoreCase))
                {
                    _hrtfMode = NextMode(_hrtfMode);
                    continue;
                }

                if (!int.TryParse(input, out int choice) || choice < 1 || choice > scenes.Count)
                {
                    Console.WriteLine("Nieprawidłowy wybór.");
                    continue;
                }

                try
                {
                    var scene = Scene.LoadFromJson(scenes[choice - 1].Path);
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
            var player = new ScenePlayer(scene, _hrtfMode);
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
                // Faktyczny stan HRTF w linii postępu - znany dopiero po otwarciu urządzenia w wątku odtwarzania
                string hrtf = player.DeviceOpened ? $" | HRTF: {DescribeStatus(player.HrtfStatus)}" : "";
                Console.Write($"\rt = {player.CurrentTime,5:0.0} / {scene.Duration:0.0} s{hrtf}");
            }
            Console.WriteLine();

            if (playError != null)
                Console.WriteLine($"Błąd odtwarzania: {playError.Message}");
            else
                Console.WriteLine(stopped ? "Zatrzymano." : "Koniec sceny.");
        }

        // Kolejność przełączania: domyślny -> włączony -> wyłączony -> domyślny
        private static HrtfMode NextMode(HrtfMode mode) => mode switch
        {
            HrtfMode.Default => HrtfMode.Enabled,
            HrtfMode.Enabled => HrtfMode.Disabled,
            _ => HrtfMode.Default
        };

        // Żądany tryb (to, o co prosimy OpenAL)
        private static string DescribeMode(HrtfMode mode) => mode switch
        {
            HrtfMode.Enabled => "włączony",
            HrtfMode.Disabled => "wyłączony",
            _ => "domyślny (decyduje OpenAL)"
        };

        // Faktyczny stan zgłoszony przez OpenAL (opisy wg specyfikacji SOFT_HRTF)
        private static string DescribeStatus(HrtfStatus? status) => status switch
        {
            null => "brak obsługi (urządzenie nie ma ALC_SOFT_HRTF)",
            HrtfStatus.Disabled => "wyłączony",
            HrtfStatus.Enabled => "włączony",
            HrtfStatus.Denied => "wyłączony (niedozwolony na urządzeniu)",
            HrtfStatus.Required => "włączony (wymagany przez urządzenie lub konfigurację)",
            HrtfStatus.HeadphonesDetected => "włączony automatycznie (wykryto słuchawki)",
            HrtfStatus.UnsupportedFormat => "wyłączony (wyjście nie jest stereo lub ma złą częstotliwość)",
            _ => $"nieznany status {(int)status}"
        };
    }
}
