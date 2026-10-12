using System.Numerics;

namespace SoundScenesOpenAL_Library.Models
{
    // Wspólna logika ruchu po ścieżce - używana przez ScenePlayer (dźwięk) i renderer w GUI (obraz).
    public static class PathInterpolator
    {
        // Zwraca true, jeśli źródło jest aktywne w chwili time.
        // Aktywność: od Time pierwszego punktu do Time ostatniego; jeden punkt = źródło stoi od Time do końca sceny.
        // position/velocity są ustawione zawsze: przed startem = pierwszy punkt, po końcu = ostatni punkt (prędkość 0).
        public static bool TryGetState(IReadOnlyList<MovementPoint>? path, float time, out Vector3 position, out Vector3 velocity)
        {
            velocity = Vector3.Zero;

            if (path == null || path.Count == 0)
            {
                position = Vector3.Zero;
                return false;
            }

            var first = path[0];
            if (time < first.Time)
            {
                position = first.Position;
                return false;
            }

            if (path.Count == 1)
            {
                position = first.Position;
                return true;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];
                if (time > b.Time)
                    continue;

                float duration = b.Time - a.Time;
                if (duration <= 0f)
                {
                    // Dwa punkty z tym samym czasem - przeskok do drugiego punktu
                    position = b.Position;
                    return true;
                }

                velocity = (b.Position - a.Position) / duration;
                position = a.Position + velocity * (time - a.Time);
                return true;
            }

            position = path[path.Count - 1].Position;
            return false;
        }
    }
}
