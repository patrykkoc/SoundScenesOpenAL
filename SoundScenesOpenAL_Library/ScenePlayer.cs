using OpenTK.Audio.OpenAL;
using SoundScenesOpenAL_Library.Audio;
using SoundScenesOpenAL_Library.Models;
using System;
using System.Collections.Generic;
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

        public void Play()
        {
            // Inicjalizacja urządzenia audio
            _device = new SoundDevice();

            // Ustawienie listenera 
            ALListenerHelper.Apply(_scene.Listener);
            // Utworzenie i uruchomienie źródeł dźwięku
            foreach (var src in _scene.Sources)
            {
                    var alSource = new ALSource(src);
                
                alSource.Play();
                _alSources.Add(alSource);
            }

            Console.WriteLine("Playing scene. Press Enter to stop...");
            Console.ReadLine();

            // Zatrzymaj i wyczyść źródła
            Dispose();
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
