using System.Numerics;
using System.Collections.Generic;
using SoundScenesOpenAL_Library.Models;

namespace SoundScenesOpenAL_Console
{
    public static class SceneCreateTest
    {
        public static Scene CreateTestScene()
        {
            Scene scene = new Scene { Name = "TestScene" };

            var carPath = new List<MovementPoint>
            {
                new MovementPoint { Position = new Vector3(-20.0f, 0.0f, 0.0f), Velocity = new Vector3(2.0f, 0.0f, 0.0f) },
                new MovementPoint { Position = new Vector3(-10.0f, 0.0f, 0.0f), Velocity = new Vector3(2.0f, 0.0f, 0.0f) },
                new MovementPoint { Position = new Vector3(0.0f, 0.0f, 0.0f),   Velocity = new Vector3(2.0f, 0.0f, 0.0f) },
                new MovementPoint { Position = new Vector3(10.0f, 0.0f, 0.0f),  Velocity = new Vector3(2.0f, 0.0f, 0.0f) },
                new MovementPoint { Position = new Vector3(20.0f, 0.0f, 0.0f),  Velocity = new Vector3(0.0f, 0.0f, 0.0f) }
            };

            scene.AddSource(new Source
            {
                Name = "CarSound",
                SoundFilePath = "Resources/dzwiekiMono/samochod-ruszajacy-str.lewa.wav",
                StartPosition = carPath[0].Position,
                Velocity = carPath[0].Velocity,
                Path = carPath,
                Gain = 1.0f,
                Loop = false
            });

            scene.AddSource(new Source
            {
                Name = "TrainAnnouncement",
                SoundFilePath = "Resources/dzwiekiMono/pociagzapowiedz-Siedlce.wav",
                StartPosition = new Vector3(20.0f, 0.0f, 0.0f),
                Velocity = Vector3.Zero,
                Gain = 1.0f,
                Loop = true
            });

            scene.SetListener(new Listener
            {
                Position = new Vector3(0.0f, 0.0f, 0.0f),
                Velocity = Vector3.Zero,
            });

            string path = "scene_test.json";
            scene.SaveToJson(path);
            Console.WriteLine($"Scene saved to {path}");

            var loadedScene = new Scene();
            loadedScene.InitializeFromJson(path);
            Console.WriteLine($"Loaded scene name: {loadedScene.Name}");
            Console.WriteLine($"Listener position: {loadedScene.Listener.Position}");
            foreach (var src in loadedScene.Sources)
            {
                Console.WriteLine($"Source: {src.Name}, File: {src.SoundFilePath}, Position: {src.StartPosition}, Gain: {src.Gain}, Pitch: {src.Pitch}, Loop: {src.Loop}");
                if (src.Path != null && src.Path.Count > 0)
                {
                    Console.WriteLine("  Path:");
                    foreach (var point in src.Path)
                        Console.WriteLine($"Pos: {point.Position}, Vel: {point.Velocity}");
                }
            }
            return scene;
        }
    }
}

