using SoundScenesOpenAL_Library;
using SoundScenesOpenAL_Library.Models;

namespace SoundScenesOpenAL_Console
{
    public class EntryPoints
    {
        public static void Main(string[] args)
        {
            // Uruchomienie testu sceny (zapis/odczyt JSON)
              SceneCreateTest.CreateTestScene();
             
            // Odtwarzanie sceny wczytanej z pliku JSON przez ScenePlayer
            var scene = new Scene();
            scene.InitializeFromJson("scene_test.json");
            var player = new ScenePlayer(scene);
            player.Play();
        }
    }
}