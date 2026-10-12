using System.Numerics;

namespace SoundScenesOpenAL_Library.Models
{
    // Klatka kluczowa ruchu: w chwili Time źródło znajduje się w Position.
    // Pomiędzy kolejnymi punktami źródło porusza się liniowo, postój = dwa punkty z tą samą pozycją.
    // Układ osi (OpenAL): X - prawo, Y - góra, Z - tył (słuchacz patrzy w -Z), jednostka = metr.
    public class MovementPoint
    {
        public Vector3 Position { get; set; } // [x, y, z]
        public float Time { get; set; }       // czas w sekundach od początku sceny
    }
}
