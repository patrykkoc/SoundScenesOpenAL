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
        private Listener _currentListener;

        private volatile float _currentTime;
        public float CurrentTime => _currentTime;

        // flaga z GUI 
        private volatile bool _stopRequested;

        
        public ScenePlayer(Scene scene)
        {
            _scene = scene;
            _currentListener = scene.Listener;
            
        }

        public void Play(float stepSeconds = 0.05f)
        {
            _currentTime = 0f;
            _stopRequested = false;

            float sceneDuration = _scene.Duration;
            _device = new SoundDevice();
            ALListenerHelper.Apply(_scene.Listener);

            // Utworzenie ALSource dla każdego Source
            foreach (var src in _scene.Sources)
            {
                var alSource = new ALSource(src);
                _alSources.Add(alSource);
              
            }

            // Czy źródło zostało już uruchomione - każde startuje raz, w chwili aktywacji
            var started = new bool[_alSources.Count];
            float currentTime = 0f;

            while (currentTime <= _scene.Duration)
            {
                _currentTime = currentTime;
                if (_stopRequested) break;

                for (int i = 0; i < _alSources.Count; i++)
                {
                    var alSource = _alSources[i];

                    if (PathInterpolator.TryGetState(alSource.Path, currentTime, out Vector3 position, out Vector3 velocity))
                    {
                        alSource.SetPositionAndVelocity(position, velocity);

                        // Loop=true zapętla się w OpenAL, Loop=false gra jednorazowo (bez restartu po zakończeniu)
                        if (!started[i])
                        {
                            alSource.Play();
                            started[i] = true;
                        }
                    }
                    else if (started[i])
                    {
                        alSource.Stop();
                    }
                }
                Console.WriteLine("Current time: " + currentTime);
                Thread.Sleep((int)(stepSeconds * 1000));
                currentTime += stepSeconds;
            }

            _currentTime = currentTime;

            Dispose();
        }
        public void Stop()
        {
            _stopRequested = true;
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
