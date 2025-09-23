using OpenTK.Audio.OpenAL;

 
// ================== OpenAL Initialization ==================
// Open the default audio device
var device = ALC.OpenDevice(null);

// Create and activate an OpenAL context for the device
var context = ALC.CreateContext(device, (int[])null);
ALC.MakeContextCurrent(context);

// ================== Buffer and Source Preparation ==================
// Generate two OpenAL buffers and sources for audio playback
int buffer1 = AL.GenBuffer();
int buffer2 = AL.GenBuffer();
int source1 = AL.GenSource();
int source2 = AL.GenSource();

// ================== WAV File Loading ==================
// Load the same WAV file for both sources (you can use different files if you want)
int channels, bitsPerSample, sampleRate;
byte[] soundData;
using (var fs = File.Open("sound2.wav", FileMode.Open))
{
    soundData = LoadWave(fs, out channels, out bitsPerSample, out sampleRate);
}

// Print WAV file properties at the beginning
Console.WriteLine($"Channels: {channels}");
Console.WriteLine($"Bits per sample: {bitsPerSample}");
Console.WriteLine($"Sample rate: {sampleRate} Hz");

// Upload the loaded WAV data to both OpenAL buffers
AL.BufferData(buffer1, GetSoundFormat(channels, bitsPerSample), ref soundData[0], soundData.Length, sampleRate);
AL.BufferData(buffer2, GetSoundFormat(channels, bitsPerSample), ref soundData[0], soundData.Length, sampleRate);

// Attach the buffers to the sources for playback
AL.Source(source1, ALSourcei.Buffer, buffer1);
AL.Source(source2, ALSourcei.Buffer, buffer2);

// ================== Listener Settings ==================
// Place the listener at the origin (center)
AL.Listener(ALListener3f.Position, 0.0f, 0.0f, 0.0f);
AL.Listener(ALListener3f.Velocity, 0.0f, 0.0f, 0.0f);

// Set a realistic distance model for 3D attenuation
AL.DistanceModel(ALDistanceModel.InverseDistanceClamped);

// ================== 3D Source Settings ==================
// Initial positions: source1 at (left, top), source2 at (right, bottom)
float source1X = -10.0f, source1Y = 10.0f;
float source2X = 10.0f, source2Y = -10.0f;
AL.Source(source1, ALSource3f.Position, source1X, source1Y, 0.0f);
AL.Source(source2, ALSource3f.Position, source2X, source2Y, 0.0f);
AL.Source(source1, ALSource3f.Velocity, 0.0f, -2.0f, 0.0f); // Moving down
AL.Source(source2, ALSource3f.Velocity, 0.0f, 2.0f, 0.0f);  // Moving up
AL.Source(source1, ALSourcef.Gain, 0.5f); // Lower volume
AL.Source(source2, ALSourcef.Gain, 0.5f); // Lower volume
AL.Source(source1, ALSourcef.Pitch, 1.0f);
AL.Source(source2, ALSourcef.Pitch, 1.0f);

// Set reference and max distance for attenuation
AL.Source(source1, ALSourcef.ReferenceDistance, 2.0f);
AL.Source(source2, ALSourcef.ReferenceDistance, 2.0f);
AL.Source(source1, ALSourcef.MaxDistance, 50.0f);
AL.Source(source2, ALSourcef.MaxDistance, 50.0f);
AL.Source(source1, ALSourceb.Looping, true); // <-- Looping enabled for source1
AL.Source(source2, ALSourceb.Looping, true); // <-- Looping enabled for source2

// ================== Playback ==================
// Start both cars, but delay the right one (source2) by 1 second
Console.WriteLine("Left car (top-to-bottom) is starting...");
AL.SourcePlay(source1);

for (int t = 0; t < 100; t++)
{
    source1Y -= 0.15f;
    AL.Source(source1, ALSource3f.Position, source1X, source1Y, 0.0f);

    // Start right car after 1 second (20 * 50ms = 1000ms)
    if (t == 20)
    {
        Console.WriteLine("Right car (bottom-to-top) is starting...");
        AL.SourcePlay(source2);
    }
    if (t >= 20)
    {
        source2Y += 0.2f;
        AL.Source(source2, ALSource3f.Position, source2X, source2Y, 0.0f);
    }
    Thread.Sleep(50);
}
AL.SourceStop(source1);
AL.SourceStop(source2);
Console.WriteLine("Both cars stopped.");

Console.WriteLine("Done. Press Enter to exit.");
Console.ReadLine();

// ================== Cleanup ==================
// Delete the OpenAL sources and buffers, destroy context, and close device
AL.DeleteSource(source1);
AL.DeleteSource(source2);
AL.DeleteBuffer(buffer1);
AL.DeleteBuffer(buffer2);
ALC.DestroyContext(context);
ALC.CloseDevice(device);

// ================== Helpers ==================
// Helper to determine OpenAL format from WAV properties
static ALFormat GetSoundFormat(int channels, int bits)
{
    if (channels == 1)
        return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
    else
        return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
}

// Helper to load WAV file data and extract format info
static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
{
    using var reader = new BinaryReader(stream);
    // Read RIFF header
    string riff = new string(reader.ReadChars(4));
    if (riff != "RIFF") throw new NotSupportedException("Not a WAV file.");
    reader.ReadInt32(); // file size
    string wave = new string(reader.ReadChars(4));
    if (wave != "WAVE") throw new NotSupportedException("Not a WAV file.");

    // Read chunks
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
            // Skip unknown chunk
            reader.ReadBytes(chunkSize);
        }
    }
    if (data == null) throw new NotSupportedException("No data chunk found in WAV file.");
    return data;
}