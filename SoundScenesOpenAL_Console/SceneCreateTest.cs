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

            // Car in front: right to left, in front of listener (0 s -> 6 s)
            var carFrontPath = new List<MovementPoint>
            {
                new MovementPoint { Position = new Vector3(20.0f, 0.0f, -10.0f), Time = 0f },
                new MovementPoint { Position = new Vector3(-20.0f, 0.0f, -10.0f), Time = 6f }
            };

            scene.AddSource(new Source
            {
                Name = "CarFront",
                SoundFilePath = "Resources/dzwiekiMono/sound2.wav",
                Path = carFrontPath,
                Gain = 1.0f,
                Loop = true
            });

           // Samochód z tyłu z lewej do prawej, równocześnie z CarFront (0 s -> 6 s)
            var carBackPath = new List<MovementPoint>
            {
                new MovementPoint { Position = new Vector3(-20.0f, 0.0f, 5.0f), Time = 0f },
                new MovementPoint { Position = new Vector3(20.0f, 0.0f, 5.0f), Time = 6f }
            };

            scene.AddSource(new Source
            {
                Name = "CarBack",
                SoundFilePath = "Resources/dzwiekiMono/samochod-ruszajacy-str.lewa.wav",
                Path = carBackPath,
                Gain = 1.0f,
                Loop = true
            });

            // Train station in front - stoi w miejscu przez całą zapowiedź, jednorazowo (dwa punkty z tą samą pozycją).
            // 20.26 s = długość pociagzapowiedz-Siedlce.wav (1 944 960 B danych / (48 000 Hz * 2 B na próbkę mono 16-bit));
            // scena kończy się razem z zapowiedzią.
            var trainPath = new List<MovementPoint>
            {
                new MovementPoint { Position = new Vector3(0.0f, 0.0f, -20.0f), Time = 0f },
                new MovementPoint { Position = new Vector3(0.0f, 0.0f, -20.0f), Time = 20.26f }
            };
            scene.AddSource(new Source
            {
                Name = "TrainAnnouncement",
                SoundFilePath = "Resources/dzwiekiMono/pociagzapowiedz-Siedlce.wav",
                Path = trainPath,
                Gain = 1.0f,
                Loop = false
            });

            scene.SetListener(new Listener
            {
                Position = new Vector3(0.0f, 0.0f, 0.0f),
                Velocity = Vector3.Zero,
            });

            string path = "Resources/scene_test.json";
            scene.SaveToJson(path);
            Console.WriteLine($"Scene saved to {path}");

            var loadedScene = new Scene();
            loadedScene.InitializeFromJson(path);
            Console.WriteLine($"Loaded scene name: {loadedScene.Name}");
            Console.WriteLine($"Listener position: {loadedScene.Listener.Position}");
            foreach (var src in loadedScene.Sources)
            {
                Console.WriteLine($"Source: {src.Name}, File: {src.SoundFilePath}, Position: {src.GetStartPosition()}, Gain: {src.Gain}, Pitch: {src.Pitch}, Loop: {src.Loop}");
                if (src.Path != null && src.Path.Count > 0)
                {
                    Console.WriteLine("  Path:");
                    foreach (var point in src.Path)
                        Console.WriteLine($"Pos: {point.Position}");
                }
            }
            return scene;
        }
    }
}

