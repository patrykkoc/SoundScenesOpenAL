using SoundScenesOpenAL_Library;
using SoundScenesOpenAL_Library.Models;
using System.Xml.Serialization;

namespace SoundScenesOpenAL_GUI
{
    public partial class MainForm : Form
    {
        private Scene _scene;
        private ScenePlayer _scenePlayer;

        // Czy scena gra - tylko wtedy klawisze sterują słuchaczem (ustawiane w setPlaybackButtons)
        private bool _isPlaying;


        public MainForm()
        {
            InitializeComponent();

            // Start dostępny dopiero po wczytaniu sceny
            toolStripButtonStart.Enabled = false;
            toolStripButtonStop.Enabled = false;
            uruchomSceneToolStripMenuItem.Enabled = false;
            zatrzymajSceneToolStripMenuItem.Enabled = false;
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

        private void zatrzymajSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonStop.PerformClick();
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

        // Scena gra: Start wyłączony, Stop włączony; nie gra: odwrotnie.
        // Menu ma zawsze ten sam stan co pasek - nieaktywne pozycje są wyszarzone, nie ukryte.
        private void setPlaybackButtons(bool isPlaying)
        {
            _isPlaying = isPlaying;
            toolStripButtonStart.Enabled = !isPlaying;
            toolStripButtonStop.Enabled = isPlaying;
            uruchomSceneToolStripMenuItem.Enabled = !isPlaying;
            zatrzymajSceneToolStripMenuItem.Enabled = isPlaying;
        }

        // Sterowanie słuchaczem z klawiatury, te same klawisze co w konsoli.
        // ProcessCmdKey, a nie KeyDown: strzałki są w WinForms klawiszami nawigacji (przenoszą fokus między
        // kontrolkami) i nie docierają do KeyDown formularza. Tutaj formularz dostaje je przed kontrolkami.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Poza odtwarzaniem i przy otwartym menu klawisze działają normalnie (np. strzałki i Esc w menu)
            if (!_isPlaying || _scenePlayer == null || isMenuOpen())
                return base.ProcessCmdKey(ref msg, keyData);

            var listener = _scenePlayer.ListenerControl;
            // keyData zawiera też modyfikatory - np. Shift+F5 czy Ctrl+W nie pasują do żadnego przypadku
            switch (keyData)
            {
                case Keys.Escape:
                    toolStripButtonStop.PerformClick();
                    return true;
                case Keys.W:
                case Keys.Up:
                    listener.MoveForward(ListenerControl.StepMetres);
                    return true;
                case Keys.S:
                case Keys.Down:
                    listener.MoveForward(-ListenerControl.StepMetres);
                    return true;
                case Keys.D:
                    listener.MoveRight(ListenerControl.StepMetres);
                    return true;
                case Keys.A:
                    listener.MoveRight(-ListenerControl.StepMetres);
                    return true;
                case Keys.Right:
                    listener.Turn(ListenerControl.TurnDegrees);
                    return true;
                case Keys.Left:
                    listener.Turn(-ListenerControl.TurnDegrees);
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        // Czy rozwinięte jest któreś menu na pasku menu
        private bool isMenuOpen()
        {
            return menuStrip1.Items.OfType<ToolStripMenuItem>().Any(item => item.DropDown.Visible);
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
