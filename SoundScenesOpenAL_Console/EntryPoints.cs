using SoundScenesOpenAL_Library;

namespace SoundScenesOpenAL_Console
{
    public class EntryPoints
    {
        public static void Main(string[] args)
        {
            // Uruchomienie testu sceny (zapis/odczyt JSON)
            SceneTest.CreateTestScene();

            // Uruchomienie testu odtwarzania sceny (generowanie i odtwarzanie)
            // ScenePlayTest.PlayTestScene();

            // Odtwarzanie sceny wczytanej z pliku JSON przez ScenePlayer
            var scene = new Scene();
            scene.InitializeFromJson("scene_test.json");
            var player = new ScenePlayer(scene);
            player.Play();
        }
    }
}