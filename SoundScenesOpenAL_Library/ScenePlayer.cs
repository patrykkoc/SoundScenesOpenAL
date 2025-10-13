using OpenTK.Audio.OpenAL;
using SoundScenesOpenAL_Library.Audio;
using SoundScenesOpenAL_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SoundScenesOpenAL_Library
{
    public class ScenePlayer
    {
        private readonly Scene _scene;
        private SoundDevice _device;
        private List<ALSource> _alSources = new();

        public ScenePlayer(Scene scene)
        {
            _scene = scene;
        }

        public void Play(float stepSeconds = 0.05f)
        {
            float sceneDuration = _scene.Duration; // automatycznie wyliczane na podstawie ścieżek
            _device = new SoundDevice();
            ALListenerHelper.Apply(_scene.Listener);

            // Utworzenie ALSource dla każdego Source
            foreach (var src in _scene.Sources)
            {
                var alSource = new ALSource(src);
                _alSources.Add(alSource);
                alSource.Stop(); // źródło nie gra na starcie
            }

            float currentTime = 0f;
            bool running = true;

            while (currentTime <= _scene.Duration)
            {
                for (int i = 0; i < _scene.Sources.Count; i++)
                {
                    var src = _scene.Sources[i];
                    var alSource = _alSources[i];

                    // Znajdź aktywny MovementPoint dla aktualnego czasu
                    var activePoint = src.Path?.FirstOrDefault(p => currentTime >= p.TimeStart && currentTime < p.TimeEnd);

                    if (activePoint != null)
                    {
                        // Oblicz nową pozycję na podstawie velocity i upływu czasu
                        float t = currentTime - activePoint.TimeStart;
                        Vector3 newPosition = activePoint.Position + activePoint.Velocity * t;

                        AL.Source(alSource.SourceId, ALSource3f.Position, newPosition.X, newPosition.Y, newPosition.Z);
                        AL.Source(alSource.SourceId, ALSource3f.Velocity, activePoint.Velocity.X, activePoint.Velocity.Y, activePoint.Velocity.Z);

                        // Jeśli źródło nie gra, uruchom je
                        if (!IsSourcePlaying(alSource.SourceId))
                            alSource.Play();
                    }
                    else
                    {
                        // Poza aktywnym segmentem zatrzymaj źródło
                        alSource.Stop();
                    }
                    Console.WriteLine("Current time: " + currentTime);
                }

                Thread.Sleep((int)(stepSeconds * 1000));
                currentTime += stepSeconds;
            }

            Console.WriteLine("Press Enter to stop...");
            Console.ReadLine();
            Dispose();
        }

        private bool IsSourcePlaying(int sourceId)
        {
            int state;
            AL.GetSource(sourceId, ALGetSourcei.SourceState, out state);
            return (ALSourceState)state == ALSourceState.Playing;
        }

        public void Dispose()
        {
            foreach (var alSource in _alSources)
            {
                alSource.Dispose();
            }
            _alSources.Clear();

            _device?.Dispose();
        }
    }
}
