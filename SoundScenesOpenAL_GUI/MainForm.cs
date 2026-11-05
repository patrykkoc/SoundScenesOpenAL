using SoundScenesOpenAL_Library;
using SoundScenesOpenAL_Library.Models;
using System.Xml.Serialization;

namespace SoundScenesOpenAL_GUI
{
    public partial class MainForm : Form
    {
        private Scene _scene;
        private ScenePlayer _scenePlayer;


        public MainForm()
        {
            InitializeComponent();

            // Start dost�pny dopiero po wczytaniu sceny
            toolStripButtonStart.Enabled = false;
            toolStripButtonStop.Enabled = false;
            openDefaultScene();
        }



        private void jSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki sceny (*.json)|*.json|Wszystkie pliki (*.*)|*.*";
            openFileDialog.Title = "Wybierz plik sceny";
            openFileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sceny");



            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Wczytanie nowej sceny zatrzymuje odtwarzanie poprzedniej
                _scenePlayer?.Stop();

                // Wczytaj scen� z pliku
                _scene = new Scene();
                _scene.InitializeFromJson(openFileDialog.FileName);


                // MessageBox.Show($"Wczytano scen�: {_scene.Name}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                setPlaybackButtons(isPlaying: false);
            }
        }

        private void uruchomSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonStart.PerformClick();
        }

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            // Request to START playback
            if (_scene == null)
            {
                MessageBox.Show("Brak wczytanej sceny. U�yj Wczytaj najpierw.", "B��d", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Stw�rz player i uruchom
            var player = new ScenePlayer(_scene);
            _scenePlayer = player;
            setPlaybackButtons(isPlaying: true);

            Task.Run(() =>
            {
                try
                {
                    player.Play();
                }
                catch (Exception ex)
                {
                    BeginInvoke(() =>
                    {
                        MessageBox.Show($"B��d odtwarzania sceny: {ex.Message}", "B��d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                finally
                {
                    // Przywr�� UI na w�tku GUI po ko�cu sceny (tak�e naturalnym, bez Stop).
                    // Tylko bie��cy player - stary, zako�czony po szybkim Stop->Start, nie mo�e nadpisa� przycisk�w nowego.
                    BeginInvoke(() =>
                    {
                        if (_scenePlayer == player)
                            setPlaybackButtons(isPlaying: false);
                    });
                }
            });
        }


        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            _scenePlayer?.Stop();
            setPlaybackButtons(isPlaying: false);
        }

        // Scena gra: Start wy��czony, Stop w��czony; nie gra: odwrotnie
        private void setPlaybackButtons(bool isPlaying)
        {
            toolStripButtonStart.Enabled = !isPlaying;
            toolStripButtonStop.Enabled = isPlaying;
        }

        private void openDefaultScene()
        {
            string defaultScenePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sceny", "scene_test.json");
            if (File.Exists(defaultScenePath))
            {
                _scene = new Scene();
                _scene.InitializeFromJson(defaultScenePath);
                setPlaybackButtons(isPlaying: false);
            }
            else
            {
                MessageBox.Show("Nie znaleziono domyślnej sceny.", "B��d", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
