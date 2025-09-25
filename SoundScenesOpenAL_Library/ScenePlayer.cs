using OpenTK.Audio.OpenAL;
using System.Numerics;
using System.Collections.Generic;

namespace SoundScenesOpenAL_Library
{
    public class ScenePlayer
    {
        private Scene _scene;
        private object _device;   // Użyj var lub odpowiedniego typu (np. ALDevice lub ALDevice*)
        private object _context;  // Użyj var lub odpowiedniego typu (np. ALContext lub ALContext*)
        private List<int> _buffers = new(); // Dodaj to pole
        private List<int> _sources = new(); // Dodaj to pole

        public ScenePlayer(Scene scene)
        {
            _scene = scene;
        }

        public void Play()
        {
            // OpenAL init
            var device = ALC.OpenDevice(null);
            var context = ALC.CreateContext(device, (int[])null);
            ALC.MakeContextCurrent(context);

            _device = device;
            _context = context;

            // Listener
            var l = _scene.Listener;
            AL.Listener(ALListener3f.Position, l.Position.X, l.Position.Y, l.Position.Z);
            AL.Listener(ALListener3f.Velocity, l.Velocity.X, l.Velocity.Y, l.Velocity.Z);

            // Sources
            foreach (var src in _scene.Sources)
            {
                int buffer = AL.GenBuffer();
                int source = AL.GenSource();
                _buffers.Add(buffer);
                _sources.Add(source);
                    
                // Load WAV
                int channels, bits, rate;
                byte[] data;
                using (var fs = File.Open(src.SoundFilePath, FileMode.Open))
                {
                    data = LoadWave(fs, out channels, out bits, out rate);
                }
                AL.BufferData(buffer, GetSoundFormat(channels, bits), ref data[0], data.Length, rate);
                AL.Source(source, ALSourcei.Buffer, buffer);
                AL.Source(source, ALSource3f.Position, src.StartPosition.X, src.StartPosition.Y, src.StartPosition.Z);
                AL.Source(source, ALSource3f.Velocity, src.Velocity.X, src.Velocity.Y, src.Velocity.Z);
                AL.Source(source, ALSourcef.Gain, src.Gain);
                AL.Source(source, ALSourcef.Pitch, src.Pitch);
                AL.Source(source, ALSourceb.Looping, src.Loop);

                AL.SourcePlay(source);
            }

            Console.WriteLine("Playing scene. Press Enter to stop...");
            Console.ReadLine();

            // Cleanup
            //foreach (var s in _sources) AL.SourceStop(s);
            //foreach (var s in _sources) AL.DeleteSource(s);
            //foreach (var b in _buffers) AL.DeleteBuffer(b);
            //ALC.DestroyContext(_context);
            //ALC.CloseDevice(_device);
        }

        // Helper methods (skopiuj z Program.cs)
        private static ALFormat GetSoundFormat(int channels, int bits)
        {
            if (channels == 1)
                return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
            else
                return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
        }

        private static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            using var reader = new BinaryReader(stream);
            string riff = new string(reader.ReadChars(4));
            if (riff != "RIFF") throw new NotSupportedException("Not a WAV file.");
            reader.ReadInt32();
            string wave = new string(reader.ReadChars(4));
            if (wave != "WAVE") throw new NotSupportedException("Not a WAV file.");

            channels = bits = rate = 0;
            byte[]? data = null;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                string chunkId = new string(reader.ReadChars(4));
                int chunkSize = reader.ReadInt32();
                if (chunkId == "fmt ")
                {
                    int audioFormat = reader.ReadInt16();
                    channels = reader.ReadInt16();
                    rate = reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt16();
                    bits = reader.ReadInt16();
                    if (chunkSize > 16)
                        reader.ReadBytes(chunkSize - 16);
                    if (audioFormat != 1)
                        throw new NotSupportedException("Only PCM WAV files are supported.");
                }
                else if (chunkId == "data")
                {
                    data = reader.ReadBytes(chunkSize);
                    break;
                }
                else
                {
                    reader.ReadBytes(chunkSize);
                }
            }
            if (data == null) throw new NotSupportedException("No data chunk found in WAV file.");
            return data;
        }
    }
}
