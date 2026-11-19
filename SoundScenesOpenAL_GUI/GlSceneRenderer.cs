using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SoundScenesOpenAL_Library.Models;
using System.Linq;
using System.Numerics;

// Oddzielne aliasy: modelowe wektory (System.Numerics) vs wektory OpenTK do macierzy
using ModelVector3 = System.Numerics.Vector3;
using TKVector3 = OpenTK.Mathematics.Vector3;

namespace SoundScenesOpenAL_GUI
{
    public class GlSceneRenderer
    {
        // Długość każdej połowy osi w metrach (od 0 do +15 i od 0 do -15)
        private const float AxisLength = 15f;

        // Wysokość litery przy końcu osi w metrach - przy domyślnej odległości kamery (70 m) dobrze widoczna
        private const float AxisLetterSize = 1.5f;

        // Ujemne połowy osi rysowane ciemniej: mnożnik jasności koloru
        private const float NegativeAxisDim = 0.4f;

        // Litery z odcinków (stary OpenGL nie ma czcionek). Współrzędne w kwadracie 0..1:
        // u = w prawo, v = w górę; każde 4 liczby to jeden odcinek (u1, v1, u2, v2)
        private static readonly Dictionary<char, float[]> Glyphs = new()
        {
            ['X'] = new[] { 0f, 0f, 1f, 1f,   0f, 1f, 1f, 0f },
            ['Y'] = new[] { 0f, 1f, 0.5f, 0.5f,   1f, 1f, 0.5f, 0.5f,   0.5f, 0.5f, 0.5f, 0f },
            ['Z'] = new[] { 0f, 1f, 1f, 1f,   1f, 1f, 0f, 0f,   0f, 0f, 1f, 0f },
            ['-'] = new[] { 0f, 0.5f, 0.7f, 0.5f },
        };
        // listener: bieżący słuchacz (w trakcie odtwarzania przesuwany klawiszami), a nie ten zapisany w scenie
        public void Render(Scene? scene, Listener? listener, float currentTime, int width, int height,
                           float yawDeg, float pitchDeg, float distance)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            float aspect = width > 0 && height > 0 ? width / (float)height : 1f;
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), aspect, 0.1f, 2000f);

            // Kamera krąży wokół słuchacza, więc podąża za nim, gdy się porusza
            var target = listener?.Position ?? ModelVector3.Zero;

            // konwersja kątów do radianów
            float yaw = MathHelper.DegreesToRadians(yawDeg);
            float pitch = MathHelper.DegreesToRadians(pitchDeg);

            // sferycznie: obrót wokół celu
            var camOffset = new ModelVector3(
                distance * (float)(Math.Cos(pitch) * Math.Cos(yaw)),
                distance * (float)(Math.Sin(pitch)),
                distance * (float)(Math.Cos(pitch) * Math.Sin(yaw))
            );
            var camPos = target + camOffset;

            var view = Matrix4.LookAt(
                new TKVector3(camPos.X, camPos.Y, camPos.Z),
                new TKVector3(target.X, target.Y, target.Z),
                TKVector3.UnitY);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref view);

            // Kierunki "w prawo" i "w górę" ekranu - litery osi rysowane w tej płaszczyźnie są zawsze zwrócone do kamery
            var camForward = ModelVector3.Normalize(target - camPos);
            var screenRight = ModelVector3.Normalize(ModelVector3.Cross(camForward, ModelVector3.UnitY));
            var screenUp = ModelVector3.Cross(screenRight, camForward);

            DrawAxes(screenRight, screenUp);
            if (scene == null) return;

            if (listener != null)
                DrawListener(listener);
            foreach (var src in scene.Sources)
            {
                DrawPath(src);
                bool active = PathInterpolator.TryGetState(src.Path, currentTime, out var p, out _);
                DrawSourcePoint(p, active);
            }
        }

        // Osie OpenAL: X - prawo (czerwona), Y - góra (zielona), Z - tył (niebieska); przód słuchacza to -Z
        private void DrawAxes(ModelVector3 screenRight, ModelVector3 screenUp)
        {
            DrawAxis(ModelVector3.UnitX, "X", 1f, 0f, 0f, screenRight, screenUp);
            DrawAxis(ModelVector3.UnitY, "Y", 0f, 1f, 0f, screenRight, screenUp);
            DrawAxis(ModelVector3.UnitZ, "Z", 0f, 0.5f, 1f, screenRight, screenUp);
        }

        // Dodatnia połowa w pełnym kolorze, ujemna przyciemniona; podpisy "X" i "-X" przy końcach
        private void DrawAxis(ModelVector3 direction, string name, float r, float g, float b,
                              ModelVector3 screenRight, ModelVector3 screenUp)
        {
            var positiveEnd = direction * AxisLength;
            var negativeEnd = -positiveEnd;

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(r, g, b);
            GL.Vertex3(0, 0, 0); GL.Vertex3(positiveEnd.X, positiveEnd.Y, positiveEnd.Z);
            GL.Color3(r * NegativeAxisDim, g * NegativeAxisDim, b * NegativeAxisDim);
            GL.Vertex3(0, 0, 0); GL.Vertex3(negativeEnd.X, negativeEnd.Y, negativeEnd.Z);
            GL.End();

            DrawText(name, positiveEnd, r, g, b, screenRight, screenUp);
            DrawText("-" + name, negativeEnd, r * NegativeAxisDim, g * NegativeAxisDim, b * NegativeAxisDim, screenRight, screenUp);
        }

        // Tekst z odcinków; lewy dolny róg pierwszej litery w punkcie anchor
        private void DrawText(string text, ModelVector3 anchor, float r, float g, float b,
                              ModelVector3 screenRight, ModelVector3 screenUp)
        {
            // Litera węższa niż wysoka (proporcje zwykłej czcionki), z odstępem między literami
            float letterWidth = AxisLetterSize * 0.6f;
            float advance = AxisLetterSize * 0.9f;

            GL.Color3(r, g, b);
            GL.Begin(PrimitiveType.Lines);
            float offset = 0f;
            foreach (char c in text)
            {
                if (Glyphs.TryGetValue(c, out var segments))
                {
                    for (int i = 0; i < segments.Length; i += 2)
                    {
                        var p = anchor
                              + screenRight * (offset + segments[i] * letterWidth)
                              + screenUp * (segments[i + 1] * AxisLetterSize);
                        GL.Vertex3(p.X, p.Y, p.Z);
                    }
                }
                offset += advance;
            }
            GL.End();
        }

        private void DrawListener(Listener l)
        {
            var p = l.Position;
            GL.PointSize(10f);
            GL.Begin(PrimitiveType.Points);
            GL.Color3(1f, 1f, 0.2f);
            GL.Vertex3(p.X, p.Y, p.Z);
            GL.End();

            var fwd = l.OrientationForward;
            var end = new ModelVector3(p.X + fwd.X, p.Y + fwd.Y, p.Z + fwd.Z);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1f, 1f, 0.2f);
            GL.Vertex3(p.X, p.Y, p.Z);
            GL.Vertex3(end.X, end.Y, end.Z);
            GL.End();
        }

        private void DrawPath(Source src)
        {
            if (src.Path == null || src.Path.Count == 0) return;
            GL.Begin(PrimitiveType.LineStrip);
            GL.Color3(0.9f, 0.7f, 0.3f);
            foreach (var mp in src.Path)
                GL.Vertex3(mp.Position.X, mp.Position.Y, mp.Position.Z);
            GL.End();
        }

        private void DrawSourcePoint(ModelVector3 p, bool active)
        {
            GL.PointSize(8f);
            GL.Begin(PrimitiveType.Points);
            if (active) GL.Color3(1f, 0.5f, 0.2f); else GL.Color3(0.5f, 0.5f, 0.5f);
            GL.Vertex3(p.X, p.Y, p.Z);
            GL.End();
        }
    }
}