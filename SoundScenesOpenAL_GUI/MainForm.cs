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

            // Start dostępny dopiero po wczytaniu sceny
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

                // Wczytaj scenę z pliku
                _scene = new Scene();
                _scene.InitializeFromJson(openFileDialog.FileName);


                // MessageBox.Show($"Wczytano scenę: {_scene.Name}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                MessageBox.Show("Brak wczytanej sceny. Użyj Wczytaj najpierw.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Stwórz player i uruchom
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
                        MessageBox.Show($"Błąd odtwarzania sceny: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                finally
                {
                    // Przywróć UI na wątku GUI po końcu sceny (także naturalnym, bez Stop).
                    // Tylko bieżący player - stary, zakończony po szybkim Stop->Start, nie może nadpisać przycisków nowego.
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

        // Scena gra: Start wyłączony, Stop włączony; nie gra: odwrotnie
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
                MessageBox.Show("Nie znaleziono domyślnej sceny.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
