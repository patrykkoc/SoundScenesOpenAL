using OpenTK.Audio.OpenAL;
using SoundScenesOpenAL_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundScenesOpenAL_Library
{
    public static class ALListenerHelper
    {
        public static void Apply(Listener listener)
        {
            AL.Listener(ALListener3f.Position, listener.Position.X, listener.Position.Y, listener.Position.Z);
            AL.Listener(ALListener3f.Velocity, listener.Velocity.X, listener.Velocity.Y, listener.Velocity.Z);
            AL.Listener(ALListenerfv.Orientation, new float[]
            {
            listener.OrientationForward.X, listener.OrientationForward.Y, listener.OrientationForward.Z,
            listener.OrientationUp.X, listener.OrientationUp.Y, listener.OrientationUp.Z
            });
        }
    }
}
