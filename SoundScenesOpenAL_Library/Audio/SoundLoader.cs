using System;
using System.IO;

namespace SoundScenesOpenAL_Library.Audio
{
    // Loads only wav fiels
    public static class SoundLoader
    {
        public static byte[] LoadSOundFromFile(string path, out int channels, out int bits, out int rate)
        {
            using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return LoadSound(fs, out channels, out bits, out rate);
        }

        public static byte[] LoadSound(Stream stream, out int channels, out int bits, out int rate)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new Exception("Invalid WAV file");

                reader.ReadInt32(); // riff chunk size

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new Exception("Invalid WAV file");

                // Szukaj chunków
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
                        reader.ReadInt32(); // byte rate
                        reader.ReadInt16(); // block align
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
                if (data == null)
                    throw new Exception("No data chunk found in WAV file.");
                return data;
            }
        }
    }
}