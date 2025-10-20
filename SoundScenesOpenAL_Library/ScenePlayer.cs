using OpenTK.Audio.OpenAL;
using SoundScenesOpenAL_Library.Audio;
using SoundScenesOpenAL_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

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
            float sceneDuration = _scene.Duration;
            _device = new SoundDevice();
            ALListenerHelper.Apply(_scene.Listener);

            // Utworzenie ALSource dla każdego Source
            foreach (var src in _scene.Sources)
            {
                var alSource = new ALSource(src);
                _alSources.Add(alSource);
                alSource.Stop();
            }

            float currentTime = 0f;

            while (currentTime <= _scene.Duration)
            {
                for (int i = 0; i < _scene.Sources.Count; i++)
                {
                    var src = _scene.Sources[i];
                    var alSource = _alSources[i];

                    // Find the current segment (between two MovementPoints)
                    var path = src.Path;
                    if (path == null || path.Count == 0)
                    {
                        alSource.Stop();
                        continue;
                    }
                    else if (path.Count == 1)
                    {
                        var point = path[0];
                        alSource.SetPositionAndVelocity(point.Position, Vector3.Zero);

                        if (!IsSourcePlaying(alSource.SourceId))
                            alSource.Play();

                        continue;
                    }

                    // Find the segment for currentTime
                    int segIdx = path.FindIndex(p => currentTime >= p.TimeStart && currentTime < p.TimeEnd);
                    if (segIdx >= 0 && segIdx < path.Count - 1)
                    {
                        var startPoint = path[segIdx];
                        var endPoint = path[segIdx + 1];

                        float segmentDuration = endPoint.TimeStart - startPoint.TimeStart;
                        if (segmentDuration <= 0f)
                        {
                            alSource.Stop();
                            continue;
                        }

                        // Calculate velocity for this segment
                        Vector3 velocity = (endPoint.Position - startPoint.Position) / segmentDuration;
                        float t = Math.Clamp(currentTime - startPoint.TimeStart, 0, segmentDuration);
                        Vector3 newPosition = startPoint.Position + velocity * t;

                        alSource.SetPositionAndVelocity(newPosition, velocity);

                        if (!IsSourcePlaying(alSource.SourceId))
                            alSource.Play();
                    }
                    else
                    {
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
