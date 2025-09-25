using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SoundScenesOpenAL_Library
{
    public class MovementPoint
    {
        public Vector3 Position { get; set; } // [x, y, z]
        public float Speed { get; set; }
        public float TimeStart { get; set; }   // czas rozpoczęcia ruchu do punktu
        public float TimeEnd { get; set; }     // czas zakończenia ruchu w punkcie
    }
}
 