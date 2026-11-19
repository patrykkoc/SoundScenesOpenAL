using SoundScenesOpenAL_Library.Models;
using System;
using System.Numerics;

namespace SoundScenesOpenAL_Library
{
    // Sterowanie słuchaczem w trakcie odtwarzania (np. z klawiatury).
    // Metody wołane z wątku interfejsu, a stan czytany przez wątek odtwarzania - stąd lock.
    // Obrót tylko w poziomie (wokół osi Y): kąt 0 = patrzenie w -Z (przód), kąt dodatni = obrót w prawo.
    public class ListenerControl
    {
        private readonly object _lock = new();
        private Vector3 _position;
        private float _yawDegrees;

        // Prędkość ze sceny przepisywana bez zmian (dla Dopplera); ruch z klawiatury jej nie zmienia
        private readonly Vector3 _velocity;

        public ListenerControl(Listener start)
        {
            _position = start.Position;
            _velocity = start.Velocity;

            // Kąt z kierunku patrzenia sceny; składowa Y (patrzenie w górę/dół) jest pomijana
            _yawDegrees = RadiansToDegrees(MathF.Atan2(start.OrientationForward.X, -start.OrientationForward.Z));
        }

        public Vector3 Position { get { lock (_lock) return _position; } }

        // Kąt w zakresie (-180, 180]
        public float YawDegrees { get { lock (_lock) return _yawDegrees; } }

        // Ruch wzdłuż kierunku patrzenia; wartość ujemna = do tyłu
        public void MoveForward(float metres)
        {
            lock (_lock)
                _position += Forward(_yawDegrees) * metres;
        }

        // Ruch w bok względem kierunku patrzenia; wartość ujemna = w lewo
        public void MoveRight(float metres)
        {
            lock (_lock)
                _position += Right(_yawDegrees) * metres;
        }

        // Obrót głowy; wartość dodatnia = w prawo, ujemna = w lewo
        public void Turn(float degrees)
        {
            lock (_lock)
                _yawDegrees = NormalizeDegrees(_yawDegrees + degrees);
        }

        // Bieżący stan jako Listener - do ustawienia w OpenAL przez ALListenerHelper.Apply
        public Listener ToListener()
        {
            lock (_lock)
            {
                return new Listener
                {
                    Position = _position,
                    OrientationForward = Forward(_yawDegrees),
                    OrientationUp = Vector3.UnitY,
                    Velocity = _velocity
                };
            }
        }

        // Kąt 0 -> (0, 0, -1) przód; 90 -> (1, 0, 0) prawo; 180 -> (0, 0, 1) tył
        private static Vector3 Forward(float yawDegrees)
        {
            float yaw = DegreesToRadians(yawDegrees);
            return new Vector3(MathF.Sin(yaw), 0f, -MathF.Cos(yaw));
        }

        // Kierunek "w prawo" = kierunek patrzenia obrócony o 90 stopni w prawo; kąt 0 -> (1, 0, 0)
        private static Vector3 Right(float yawDegrees) => Forward(yawDegrees + 90f);

        private static float NormalizeDegrees(float degrees)
        {
            degrees %= 360f;
            if (degrees > 180f) degrees -= 360f;
            if (degrees <= -180f) degrees += 360f;
            return degrees;
        }

        private static float DegreesToRadians(float degrees) => degrees * MathF.PI / 180f;
        private static float RadiansToDegrees(float radians) => radians * 180f / MathF.PI;
    }
}
