using SoundScenesOpenAL_Library;
using System.Numerics;

namespace SoundScenesOpenAL_Console
{
    public static class SceneTest
    {
        public static void Main(string[] args)
        {
            CreateTestScene();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        public static Scene CreateTestScene()
        {
            var scene = new Scene { Name = "TestScene" };
            
            scene.AddSource(new Source
            {
                Name = "CarSound",
                SoundFilePath = "Resources/dzwiekiMono/samochod-ruszajacy-str.lewa.wav",
                StartPosition = new Vector3(-20.0f, 10.0f, 0.0f),
                Velocity = new Vector3(0.0f, -2.0f, 0.0f),
                Gain = 1.0f,
                Loop = true
            });
            scene.AddSource(new Source
            {
                Name = "TrainAnnouncement",
                SoundFilePath = "Resources/dzwiekiMono/pociagzapowiedz-Siedlce.wav",
                StartPosition = new Vector3(10.0f, -10.0f, 0.0f),
                Velocity = new Vector3(0.0f, 2.0f, 0.0f),
                Gain = 1.0f,
                Loop = true
            });
            scene.SetListener(new Listener
            {
                Position = new Vector3(0.0f, 0.0f, 0.0f),
                Velocity = new Vector3(0.0f, 0.0f, 0.0f),
            });

            // Save to JSON
            string path = "scene_test.json";
            scene.SaveToJson(path);
            Console.WriteLine($"Scene saved to {path}");

            // Load from JSON
            var loadedScene = Scene.LoadFromJson(path);
            Console.WriteLine($"Loaded scene name: {loadedScene.Name}");
            Console.WriteLine($"Listener position: {loadedScene.Listener.Position}");
            foreach (var src in loadedScene.Sources)
            {
                Console.WriteLine($"Source: {src.Name}, File: {src.SoundFilePath}, Position: {src.StartPosition}, Gain: {src.Gain}, Pitch: {src.Pitch}, Loop: {src.Loop}");
            }
            return scene;
        }
    }
}

