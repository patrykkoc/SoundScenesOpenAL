using SoundScenesOpenAL_Library;
using System.Numerics;
using System.Collections.Generic;

namespace SoundScenesOpenAL_Console
{
    public static class ScenePlayTest
    {
        // Przyk³adowa metoda do utworzenia i odtworzenia sceny
        public static void PlayTestScene()
        {
            var scene = new Scene { Name = "PlayTestScene" };

            var carPath = GenerateCarPath();

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

            scene.SetListener(new Listener
            {
                Position = new Vector3(0.0f, 0.0f, 0.0f),
                Velocity = new Vector3(0.0f, 0.0f, 0.0f),
            });

            var player = new ScenePlayer(scene);
            player.Play();
        }

        private static List<MovementPoint> GenerateCarPath()
        {
            var path = new List<MovementPoint>();
            for (float y = 10.0f; y >= -10.0f; y -= 2.0f)
            {
                path.Add(new MovementPoint
                {
                    Position = new Vector3(-20.0f, y, 0.0f),
                    Velocity = new Vector3(0.0f, -2.0f, 0.0f)
                });
            }
            return path;
        }
    }
}