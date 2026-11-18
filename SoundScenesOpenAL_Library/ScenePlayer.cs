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
        private readonly HrtfMode _hrtfMode;

        private volatile float _currentTime;
        public float CurrentTime => _currentTime;

        // flaga z GUI
        private volatile bool _stopRequested;

        // Stan HRTF zapisywany w wątku odtwarzania, czytany z innego wątku (konsola/GUI) - stąd lock
        private readonly object _hrtfLock = new();
        private bool _deviceOpened;
        private HrtfStatus? _hrtfStatus;

        // true od chwili otwarcia urządzenia w Play(); dopiero wtedy HrtfStatus ma sens
        public bool DeviceOpened { get { lock (_hrtfLock) return _deviceOpened; } }

        // Faktyczny stan HRTF; null, gdy urządzenie nie obsługuje ALC_SOFT_HRTF
        public HrtfStatus? HrtfStatus { get { lock (_hrtfLock) return _hrtfStatus; } }


        public ScenePlayer(Scene scene, HrtfMode hrtfMode = HrtfMode.Default)
        {
            _scene = scene;
            _hrtfMode = hrtfMode;
        }

        public void Play(float stepSeconds = 0.05f)
        {
            _currentTime = 0f;
            SetDeviceState(opened: false, status: null);

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
            using var device = new SoundDevice(hrtf: _hrtfMode);
            SetDeviceState(opened: true, status: device.HrtfStatus);
            try
            {
                // Model odległości: głośność maleje odwrotnie proporcjonalnie do odległości (domyślny model OpenAL).
                // Ustawiony jawnie, żeby zachowanie sceny nie zależało od domyślnych ustawień implementacji.
                AL.DistanceModel(ALDistanceModel.InverseDistanceClamped);

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

        private void SetDeviceState(bool opened, HrtfStatus? status)
        {
            lock (_hrtfLock)
            {
                _deviceOpened = opened;
                _hrtfStatus = status;
            }
        }
    }
}
