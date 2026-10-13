using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

namespace SoundScenesOpenAL_GUI
{
    public partial class MainForm
    {
        private GLControl _gl;
        private System.Windows.Forms.Timer _glTimer;
        private GlSceneRenderer _renderer = new GlSceneRenderer();

        // pola kamery
        private float _camYaw = 30f;      // stopnie
        private float _camPitch = 25f;    // stopnie
        private float _camDistance = 70f;
        private bool _dragging;
        private Point _lastMouse;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _gl = new GLControl(new GLControlSettings
            {
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Compatability, // potrzebne dla GL.Begin/End 
                NumberOfSamples = 4
            })
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(_gl);

            _gl.Load += (_, __) =>
            {
                GL.ClearColor(0.08f, 0.08f, 0.1f, 1f);
                GL.Enable(EnableCap.DepthTest);
            };

            _gl.Paint += (_, __) =>
            {
                // w Paint przed wywo³aniem renderer
                float t = _scenePlayer?.CurrentTime ?? 0f;
                _renderer.Render(_scene, t, _gl.ClientSize.Width, _gl.ClientSize.Height,
                                _camYaw, _camPitch, _camDistance);
                _gl.SwapBuffers();

               // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
              //  _gl.SwapBuffers();
            };

            _glTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            _glTimer.Tick += (_, __) => _gl.Invalidate();
            _glTimer.Start();

            // w OnLoad (po utworzeniu _gl):
            _gl.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    _dragging = true;
                    _lastMouse = e.Location;
                }
            };
            _gl.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    _dragging = false;
            };
            _gl.MouseMove += (s, e) =>
            {
                if (_dragging)
                {
                    var dx = e.X - _lastMouse.X;
                    var dy = e.Y - _lastMouse.Y;
                    _camYaw   += dx * 0.4f;
                    _camPitch -= dy * 0.4f;
                    _camPitch = Math.Clamp(_camPitch, -85f, 85f);
                    _lastMouse = e.Location;
                }
            };
            _gl.MouseWheel += (s, e) =>
            {
                _camDistance -= e.Delta * 0.1f;
                _camDistance = Math.Clamp(_camDistance, 10f, 300f);
            };
        }
    }
}