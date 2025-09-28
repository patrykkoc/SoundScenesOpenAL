using System;
using OpenTK.Audio.OpenAL;

namespace SoundScenesOpenAL_Library.Audio
{
    public class SoundDevice : IDisposable
    {
        public ALDevice Device { get; }
        public ALContext Context { get; }

        public SoundDevice(string? deviceName = null)
        {
            Device = ALC.OpenDevice(deviceName);
            if (Device == ALDevice.Null)
                throw new InvalidOperationException("Failed to open OpenAL device.");

            Context = ALC.CreateContext(Device, (int[]?)null);
            if (Context == ALContext.Null)
            {
                ALC.CloseDevice(Device);
                throw new InvalidOperationException("Failed to create OpenAL context.");
            }

            if (!ALC.MakeContextCurrent(Context))
            {
                ALC.DestroyContext(Context);
                ALC.CloseDevice(Device);
                throw new InvalidOperationException("Failed to make OpenAL context current.");
            }
        }

        public void Dispose()
        {
            // Detach if current
            if (ALC.GetCurrentContext() == Context)
            {
                ALC.MakeContextCurrent(ALContext.Null);
            }

            ALC.DestroyContext(Context);
            ALC.CloseDevice(Device);
        }
    }
}