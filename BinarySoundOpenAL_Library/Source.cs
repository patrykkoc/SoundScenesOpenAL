using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySoundOpenAL_Library
{
 

    public class Source
    {
        public string Name { get; set; }
        public string SoundFile { get; set; }
        public float[] StartPosition { get; set; } // [x, y, z]
        public List<MovementPoint> Path { get; set; } = new(); // lista punktów z prędkościami
    }
}