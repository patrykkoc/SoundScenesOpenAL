using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySoundOpenAL_Library
{
    public class MovementPoint
    {
        public float[] Position { get; set; } // [x, y, z]
        public float Speed { get; set; }
        public float TimeStart { get; set; }   // czas rozpoczęcia ruchu do punktu
        public float TimeEnd { get; set; }     // czas zakończenia ruchu w punkcie
    }
}
