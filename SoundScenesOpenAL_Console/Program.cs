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
// Load two different WAV files for each source
int channels1, bitsPerSample1, sampleRate1;
int channels2, bitsPerSample2, sampleRate2;
byte[] soundData1, soundData2;

using (var fs1 = File.Open("Resources/dzwiekiMono/samochod-ruszajacy-str.lewa.wav", FileMode.Open))
{
    soundData1 = LoadWave(fs1, out channels1, out bitsPerSample1, out sampleRate1);
}
using (var fs2 = File.Open("Resources/dzwiekiMono/pociagzapowiedz-Siedlce.wav", FileMode.Open))
{
    soundData2 = LoadWave(fs2, out channels2, out bitsPerSample2, out sampleRate2);
}

// Print WAV file properties at the beginning
Console.WriteLine($"Source1: Channels: {channels1}, Bits per sample: {bitsPerSample1}, Sample rate: {sampleRate1} Hz");
Console.WriteLine($"Source2: Channels: {channels2}, Bits per sample: {bitsPerSample2}, Sample rate: {sampleRate2} Hz");

// Upload the loaded WAV data to both OpenAL buffers
AL.BufferData(buffer1, GetSoundFormat(channels1, bitsPerSample1), ref soundData1[0], soundData1.Length, sampleRate1);
AL.BufferData(buffer2, GetSoundFormat(channels2, bitsPerSample2), ref soundData2[0], soundData2.Length, sampleRate2);

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
float source1X = -20.0f, source1Y = 10.0f;
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
//AL.Source(source1, ALSourcef.MaxDistance, 50.0f);
//AL.Source(source2, ALSourcef.MaxDistance, 50.0f);
AL.Source(source1, ALSourceb.Looping, true); // <-- Looping enabled for source1
AL.Source(source2, ALSourceb.Looping, true); // <-- Looping enabled for source2

// ================== Playback ==================
// Start both cars at the same moment
Console.WriteLine("Both cars are starting (left top-to-bottom, right bottom-to-top)...");
AL.SourcePlay(source1);
AL.SourcePlay(source2);

while(true)
{
    source1Y -= 0.15f;
    source2Y += 0.2f;
    AL.Source(source1, ALSource3f.Position, source1X, source1Y, 0.0f);
    AL.Source(source2, ALSource3f.Position, source2X, source2Y, 0.0f);
    Thread.Sleep(50);

    Console.WriteLine("source1Y " + source1Y + "source2Y" + source2Y);
    if(source1Y < -40.0f && source2Y > 40.0f)
        break;
}


while (true)
{
    source1Y += 0.15f;
    source2Y -= 0.2f;
    AL.Source(source1, ALSource3f.Position, source1X, source1Y, 0.0f);
    AL.Source(source2, ALSource3f.Position, source2X, source2Y, 0.0f);
    Thread.Sleep(50);

    Console.WriteLine("source1Y " + source1Y + "source2Y" + source2Y);
    if (source1Y < -40.0f && source2Y > 40.0f)
        break;
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