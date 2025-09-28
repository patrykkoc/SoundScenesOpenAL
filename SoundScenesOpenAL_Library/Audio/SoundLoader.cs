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
            byte[] data;
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string signature = new string(reader.ReadChars(4));
                
                if (signature != "RIFF")
                    throw new Exception("Invalid WAV file");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new Exception("Invalid WAV file");

                string subchunk1_id = new string(reader.ReadChars(4));
                int subchunk1_size = reader.ReadInt32();
                channels = reader.ReadInt16();
                rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                bits = reader.ReadInt16();

                string subchunk2_id = new string(reader.ReadChars(4));
                int subchunk2_size = reader.ReadInt32();
                data = reader.ReadBytes(subchunk2_size);
            }

            return data;
        }
    }
}