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

            // Car in front: right to left, in front of listener
            // Samochód przód z prawej do lewej 
            var carFrontPath = new List<MovementPoint>
            {
                new MovementPoint { Position = new Vector3(20.0f, 0.0f, -10.0f), Velocity = new Vector3(-4.0f, 0.0f, 0.0f), TimeStart = 0f, TimeEnd = 3f },
                new MovementPoint { Position = new Vector3(-20.0f, 0.0f, -10.0f), Velocity = new Vector3(0.0f, 0.0f, 0.0f), TimeStart = 3f, TimeEnd = 6f }
            };

            scene.AddSource(new Source
            {
                Name = "CarFront",
              //  SoundFilePath = "Resources/dzwiekiMono/samochod-ruszajacy-str.lewa.wav",
                 SoundFilePath = "Resources/sound2.wav",
                Path = carFrontPath,
                Gain = 5.0f,
                Loop = true
            });

            // Samochód z tyłu z lewej do prawej 
            var carBackPath = new List<MovementPoint>
            {
                new MovementPoint { Position = new Vector3(-20.0f, 0.0f, 5.0f), Velocity = new Vector3(4.0f, 0.0f, 0.0f), TimeStart = 3f, TimeEnd = 6f },
                new MovementPoint { Position = new Vector3(20.0f, 0.0f, 5.0f), Velocity = new Vector3(0.0f, 0.0f, 0.0f), TimeStart = 6f, TimeEnd = 9f }
            };

            scene.AddSource(new Source
            {
                Name = "CarBack",
                SoundFilePath = "Resources/dzwiekiMono/samochod-ruszajacy-str.lewa.wav", 

                //SoundFilePath = "Resources/sound2.wav", 
                Path = carBackPath,
                Gain = 2.0f,
                Loop = true
            });

            // Train station in front
            var trainPath = new List<MovementPoint>
            {
                new MovementPoint { Position = new Vector3(0.0f, 0.0f, -20.0f), Velocity = Vector3.Zero, TimeStart = 0f, TimeEnd = 50f }
            };
            scene.AddSource(new Source
            {
                Name = "TrainAnnouncement",
                SoundFilePath = "Resources/dzwiekiMono/pociagzapowiedz-Siedlce.wav",
                Path = trainPath,
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
                Console.WriteLine($"Source: {src.Name}, File: {src.SoundFilePath}, Position: {src.GetStartPosition}, Gain: {src.Gain}, Pitch: {src.Pitch}, Loop: {src.Loop}");
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

