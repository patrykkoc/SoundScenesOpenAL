using System;
using OpenTK.Audio.OpenAL;

namespace SoundScenesOpenAL_Library.Audio
{
    public class SoundDevice : IDisposable
    {
        public ALDevice Device { get; }
        public ALContext Context { get; }

        // Faktyczny stan HRTF zgłoszony przez OpenAL Soft; null, gdy brak rozszerzenia ALC_SOFT_HRTF
        public HrtfStatus? HrtfStatus { get; }

        public SoundDevice(string? deviceName = null, HrtfMode hrtf = HrtfMode.Default)
        {
            Device = ALC.OpenDevice(deviceName);
            if (Device == ALDevice.Null)
                throw new InvalidOperationException("Failed to open OpenAL device.");

            int[]? attributes;
            try
            {
                attributes = SoftHrtf.BuildContextAttributes(Device, hrtf);
            }
            catch
            {
                ALC.CloseDevice(Device);
                throw;
            }

            Context = ALC.CreateContext(Device, attributes);
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

            HrtfStatus = SoftHrtf.ReadStatus(Device);
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