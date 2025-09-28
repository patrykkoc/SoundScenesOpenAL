using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SoundScenesOpenAL_Library.Models
{
    public class MovementPoint
    {
        public Vector3 Position { get; set; } // [x, y, z]
        //Kierunek ruchu 
        public Vector3 Velocity { get; set; } // [x, y, z] 
        public float Speed { get; set; }
        public float TimeStart { get; set; }   // czas rozpoczęcia ruchu do punktu
        public float TimeEnd { get; set; }     // czas zakończenia ruchu w punkcie
    }
}
 