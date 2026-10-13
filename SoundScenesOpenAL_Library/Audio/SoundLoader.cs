using System;
using System.IO;
using NAudio.Wave;

namespace SoundScenesOpenAL_Library.Audio
{
    // Wczytuje pliki WAV (PCM 8/16/24/32-bit, float, WAVE_FORMAT_EXTENSIBLE) przez NAudio
    // i zawsze zwraca dźwięk mono 16-bit - jedyny format, który OpenAL umie spozycjonować w 3D.
    public static class SoundLoader
    {
        public static byte[] LoadMono16FromFile(string path, out int rate, out WaveFormat originalFormat)
        {
            using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return LoadMono16(fs, out rate, out originalFormat);
        }

        public static byte[] LoadMono16(Stream stream, out int rate, out WaveFormat originalFormat)
        {
            using var reader = new WaveFileReader(stream);
            originalFormat = reader.WaveFormat;
            rate = originalFormat.SampleRate;
            int channels = originalFormat.Channels;

            ISampleProvider samples;
            try
            {
                samples = reader.ToSampleProvider();
            }
            catch (ArgumentException ex)
            {
                throw new NotSupportedException($"Nieobsługiwany format WAV: {originalFormat}.", ex);
            }

            using var output = new MemoryStream();
            using var writer = new BinaryWriter(output);

            // Bufor o długości 1 s; kanały są przeplatane, więc liczba próbek jest wielokrotnością channels
            var buffer = new float[rate * channels];
            int read;
            while ((read = samples.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int frame = 0; frame + channels <= read; frame += channels)
                {
                    // Downmix do mono: średnia ze wszystkich kanałów
                    float sum = 0f;
                    for (int c = 0; c < channels; c++)
                        sum += buffer[frame + c];
                    float mono = Math.Clamp(sum / channels, -1f, 1f);

                    writer.Write((short)(mono * short.MaxValue));
                }
            }

            writer.Flush();
            return output.ToArray();
        }
    }
}
