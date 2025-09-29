using OpenTK.Audio.OpenAL;
using SoundScenesOpenAL_Library.Audio;
using SoundScenesOpenAL_Library.Models;
using System.Numerics;

namespace SoundScenesOpenAL_Library
{
    public class ALSource : Source, IDisposable
    {
        public ALSource(Source src)
       : base(src.Name,src.SoundFilePath,src.StartPosition,src.Velocity,src.Path,src.Gain, src.Pitch,src.Loop)   
        {
            LoadBuffer();
        }
        public int BufferId { get; private set; }
        public int SourceId { get; private set; }
        public double StartTime { get; set; } // czas w sekundach, kiedy odpaliæ

        public void LoadBuffer()
        {
            int channels, bits, rate;
            byte[] data = SoundLoader.LoadSOundFromFile(SoundFilePath, out channels, out bits, out rate);
            Console.WriteLine($"Loaded {data.Length} bytes from {SoundFilePath} (channels: {channels}, bits: {bits}, rate: {rate})");
            Console.WriteLine($"WAV header: channels={channels}, bits={bits}, rate={rate}");

            BufferId = AL.GenBuffer();
            ALFormat format = (channels == 1)
                ? (bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16)
                : (bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16);

            AL.BufferData(BufferId, format, ref data[0], data.Length, rate);
            var error = AL.GetError();
            if (error != ALError.NoError)
                Console.WriteLine($"OpenAL error after BufferData: {error}");

            SourceId = AL.GenSource();
            AL.Source(SourceId, ALSourcei.Buffer, BufferId);
            AL.Source(SourceId, ALSource3f.Position, StartPosition.X, StartPosition.Y, StartPosition.Z);
            AL.Source(SourceId, ALSource3f.Velocity, Velocity.X, Velocity.Y, Velocity.Z);
            AL.Source(SourceId, ALSourcef.Gain, Gain);
            AL.Source(SourceId, ALSourcef.Pitch, Pitch);
            AL.Source(SourceId, ALSourceb.Looping, Loop);
        }

        public void Play() => AL.SourcePlay(SourceId);
        public void Stop() => AL.SourceStop(SourceId);

        public void Dispose()
        {
            AL.SourceStop(SourceId);
            AL.DeleteSource(SourceId);
            AL.DeleteBuffer(BufferId);
        }
    }
}