using OpenTK.Audio.OpenAL;
using SoundScenesOpenAL_Library.Audio;
using SoundScenesOpenAL_Library.Models;
using System.Numerics;

namespace SoundScenesOpenAL_Library
{
    public class ALSource : Source, IDisposable
    {

        //: base(src.Name,src.SoundFilePath,src.StartPosition,src.Velocity,src.Path,src.Gain, src.Pitch,src.Loop)   
        public ALSource(Source src) 
        :base(src.Name,src.SoundFilePath,src.Path,src.Gain, src.Pitch,src.Loop)   
        {
            LoadBuffer();
        }
        public int BufferId { get; private set; }
        public int SourceId { get; private set; }
      //  public double StartTime { get; set; } // czas w sekundach, kiedy odpalić

        public void LoadBuffer()
        {
            // SoundLoader zawsze zwraca mono 16-bit, niezależnie od formatu pliku
            byte[] data = SoundLoader.LoadMono16FromFile(SoundFilePath, out int rate, out var originalFormat);
            Console.WriteLine($"Loaded {data.Length} bytes from {SoundFilePath} (original: {originalFormat}, played as: 16 bit mono, rate: {rate})");
            if (data.Length == 0)
                throw new InvalidDataException($"Plik {SoundFilePath} nie zawiera próbek dźwięku.");

            BufferId = AL.GenBuffer();
            AL.BufferData(BufferId, ALFormat.Mono16, ref data[0], data.Length, rate);
            var error = AL.GetError();
            if (error != ALError.NoError)
                Console.WriteLine($"OpenAL error after BufferData: {error}");

            SourceId = AL.GenSource();
            AL.Source(SourceId, ALSourcei.Buffer, BufferId);
            AL.Source(SourceId, ALSource3f.Position, GetStartPosition().X, GetStartPosition().Y, GetStartPosition().Z);
            AL.Source(SourceId, ALSource3f.Velocity, 0f, 0f, 0f); // Set velocity to zero at initialization
            AL.Source(SourceId, ALSourcef.Gain, Gain);
            AL.Source(SourceId, ALSourcef.Pitch, Pitch);
            AL.Source(SourceId, ALSourceb.Looping, Loop);
        }

        public void Play() => AL.SourcePlay(SourceId);
        public void Stop() => AL.SourceStop(SourceId);  
        public void SetPositionAndVelocity(Vector3 position, Vector3 velocity)
        {
            AL.Source(SourceId, ALSource3f.Position, position.X, position.Y, position.Z);
            AL.Source(SourceId, ALSource3f.Velocity, velocity.X, velocity.Y, velocity.Z);
        }

        public void Dispose()
        {
            AL.SourceStop(SourceId);
            AL.DeleteSource(SourceId);
            AL.DeleteBuffer(BufferId);
        }
    }
}