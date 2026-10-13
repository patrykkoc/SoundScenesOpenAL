using OpenTK.Audio.OpenAL;
using SoundScenesOpenAL_Library.Audio;
using SoundScenesOpenAL_Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace SoundScenesOpenAL_Library
{
    public class ScenePlayer
    {
        // Kontekst OpenAL jest wspólny dla całego procesu, więc naraz gra tylko jeden player.
        // Nowe Play() czeka, aż poprzednie odtwarzanie zwolni źródła i urządzenie (szybkie Stop→Start w GUI).
        private static readonly object PlaybackLock = new();

        private readonly Scene _scene;

        private volatile float _currentTime;
        public float CurrentTime => _currentTime;

        // flaga z GUI
        private volatile bool _stopRequested;


        public ScenePlayer(Scene scene)
        {
            _scene = scene;
        }

        public void Play(float stepSeconds = 0.05f)
        {
            _currentTime = 0f;

            lock (PlaybackLock)
            {
                try
                {
                    // Stop() wywołane w trakcie czekania na poprzednie odtwarzanie - nie startujemy
                    if (!_stopRequested)
                        PlayLocked(stepSeconds);
                }
                finally
                {
                    // Reset na końcu, a nie na początku - Stop() przed startem nie może zginąć
                    _stopRequested = false;
                }
            }
        }

        private void PlayLocked(float stepSeconds)
        {
            float sceneDuration = _scene.Duration;
            var alSources = new List<ALSource>();

            // using + finally: źródła i urządzenie są zwalniane także po wyjątku (np. brak pliku WAV)
            using var device = new SoundDevice();
            try
            {
                ALListenerHelper.Apply(_scene.Listener);

                // Utworzenie ALSource dla każdego Source
                foreach (var src in _scene.Sources)
                    alSources.Add(new ALSource(src));

                // Czy źródło zostało już uruchomione - każde startuje raz, w chwili aktywacji
                var started = new bool[alSources.Count];
                var toStart = new List<int>();

                // Czas sceny z zegara, a nie z sumy Sleep() - Sleep na Windows trwa dłużej niż zadano
                var clock = Stopwatch.StartNew();

                while (!_stopRequested)
                {
                    float currentTime = (float)clock.Elapsed.TotalSeconds;
                    if (currentTime > sceneDuration)
                    {
                        _currentTime = sceneDuration;
                        break;
                    }
                    _currentTime = currentTime;

                    toStart.Clear();
                    for (int i = 0; i < alSources.Count; i++)
                    {
                        var alSource = alSources[i];

                        if (PathInterpolator.TryGetState(alSource.Path, currentTime, out Vector3 position, out Vector3 velocity))
                        {
                            alSource.SetPositionAndVelocity(position, velocity);

                            // Loop=true zapętla się w OpenAL, Loop=false gra jednorazowo (bez restartu po zakończeniu)
                            if (!started[i])
                            {
                                toStart.Add(alSource.SourceId);
                                started[i] = true;
                            }
                        }
                        else if (started[i])
                        {
                            alSource.Stop();
                        }
                    }

                    // Źródła aktywowane w tym kroku startują jednym wywołaniem - w tym samym cyklu miksera
                    if (toStart.Count > 0)
                        AL.SourcePlay(toStart.Count, toStart.ToArray());

                    Console.WriteLine("Current time: " + currentTime);
                    Thread.Sleep((int)(stepSeconds * 1000));
                }
            }
            finally
            {
                foreach (var alSource in alSources)
                    alSource.Dispose();
            }
        }

        public void Stop()
        {
            _stopRequested = true;
        }
    }
}
