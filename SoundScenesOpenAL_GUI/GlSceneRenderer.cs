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
        public void Render(Scene? scene, float currentTime, int width, int height,
                           float yawDeg, float pitchDeg, float distance)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            float aspect = width > 0 && height > 0 ? width / (float)height : 1f;
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), aspect, 0.1f, 2000f);

            var target = scene?.Listener.Position ?? ModelVector3.Zero;

            // konwersja k¹tów do radianów
            float yaw = MathHelper.DegreesToRadians(yawDeg);
            float pitch = MathHelper.DegreesToRadians(pitchDeg);

            // sferycznie: obrót wokó³ celu
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

            DrawAxes(15f);
            if (scene == null) return;

            DrawListener(scene.Listener);
            foreach (var src in scene.Sources)
            {
                DrawPath(src);
                var p = InterpolatePosition(src, currentTime);
                DrawSourcePoint(p, currentTime, src);
            }
        }

        private void DrawAxes(float len)
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1f, 0f, 0f); GL.Vertex3(0, 0, 0); GL.Vertex3(len, 0, 0);     // X
            GL.Color3(0f, 1f, 0f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, len, 0);     // Y
            GL.Color3(0f, 0.5f, 1f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, len);   // Z
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
            foreach (var mp in src.Path.OrderBy(x => x.TimeStart))
                GL.Vertex3(mp.Position.X, mp.Position.Y, mp.Position.Z);
            GL.End();
        }

        private ModelVector3 InterpolatePosition(Source src, float time)
        {
            if (src.Path == null || src.Path.Count == 0)
                return src.GetStartPosition();
            if (src.Path.Count == 1)
                return src.Path[0].Position;

            int segIdx = src.Path.FindIndex(p => time >= p.TimeStart && time < p.TimeEnd);
            if (segIdx >= 0 && segIdx < src.Path.Count - 1)
            {
                var a = src.Path[segIdx];
                var b = src.Path[segIdx + 1];
                float dur = b.TimeStart - a.TimeStart;
                if (dur <= 0f) return b.Position;
                float t = Math.Clamp(time - a.TimeStart, 0, dur);
                var vel = (b.Position - a.Position) / dur;
                return a.Position + vel * t;
            }
            return src.Path.OrderBy(p => p.TimeEnd).Last().Position;
        }

        private void DrawSourcePoint(ModelVector3 p, float time, Source src)
        {
            bool active = time <= (src.Path?.Max(m => m.TimeEnd) ?? 0f);
            GL.PointSize(8f);
            GL.Begin(PrimitiveType.Points);
            if (active) GL.Color3(1f, 0.5f, 0.2f); else GL.Color3(0.5f, 0.5f, 0.5f);
            GL.Vertex3(p.X, p.Y, p.Z);
            GL.End();
        }
    }
}