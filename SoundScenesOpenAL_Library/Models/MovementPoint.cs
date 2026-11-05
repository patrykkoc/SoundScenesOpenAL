using System.Numerics;

namespace SoundScenesOpenAL_Library.Models
{
    // Klatka kluczowa ruchu: w chwili Time źródło znajduje się w Position.
    // Pomiędzy kolejnymi punktami źródło porusza się liniowo, postój = dwa punkty z tą samą pozycją.
    // Układ osi (OpenAL, prawoskrętny), jednostka = metr, słuchacz domyślnie w (0, 0, 0):
    //   +X = prawo,              -X = lewo
    //   +Y = góra,               -Y = dół      (Y = 0 to wysokość uszu słuchacza)
    //   +Z = tył (za słuchaczem), -Z = przód   (słuchacz patrzy w -Z)
    // Przykład: (-3, 0, 20) = 3 m w lewo i 20 m za słuchaczem.
    // Źródło: specyfikacja OpenAL 1.1, rozdział o układzie współrzędnych i domyślne AL_ORIENTATION.
    public class MovementPoint
    {
        public Vector3 Position { get; set; } // [x, y, z]
        public float Time { get; set; }       // czas w sekundach od początku sceny
    }
}
