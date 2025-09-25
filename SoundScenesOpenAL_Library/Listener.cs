using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SoundScenesOpenAL_Library
{
    public class Listener
    {
        public Vector3 Position { get; set; } = new(0, 0, 0);
        public Vector3 OrientationForward { get; set; } = new(0, 0, -1); // kierunek patrzenia
        public Vector3 OrientationUp { get; set; } = new(0, 1, 0); // kierunek do góry
        public Vector3 Velocity { get; set; } = new(0, 0, 0);
    }

}
