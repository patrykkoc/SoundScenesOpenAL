using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SoundScenesOpenAL_Library
{
 

    public class Source
    {
        public string Name { get; set; }
        public string SoundFilePath { get; set; }
        public Vector3 StartPosition { get; set; } // [x, y, z]
        public Vector3 Velocity { get; set; } // [x, y, z]
        public List<MovementPoint> Path { get; set; } = new(); // lista punktów z prędkościami
 
        public float Gain { get; set; } = 1.0f; // Głośność źródła dźwięku 1.0 = 100%

        public float Pitch { get; set; } = 1.0f; // Wysokość dźwięku 1.0 = normalna wysokość 


        public bool Loop { get; set; } = false; 
    }
}
