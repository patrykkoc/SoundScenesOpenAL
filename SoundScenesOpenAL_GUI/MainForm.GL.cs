using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;

namespace SoundScenesOpenAL_GUI
{
    public partial class MainForm
    {
        private GLControl _gl;
        private System.Windows.Forms.Timer _glTimer;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _gl = new GLControl( 
            )
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
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                _gl.SwapBuffers();
            };

            _glTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            _glTimer.Tick += (_, __) => _gl.Invalidate();
            _glTimer.Start();
        }
    }
}