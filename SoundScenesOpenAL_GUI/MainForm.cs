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
           
             changeButtonState(toolStripButtonStart); 
            changeButtonState(toolStripButtonStop); 
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
                // Wczytaj scenê z pliku
                _scene = new Scene();
                _scene.InitializeFromJson(openFileDialog.FileName);


                // MessageBox.Show($"Wczytano scenê: {_scene.Name}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if(toolStripButtonStart.Enabled ==false)
                    changeButtonState(toolStripButtonStart);
               
                 
            }
        }

        private void uruchomSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonStart.PerformClick();
        }

        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            
             
                changeButtonState(toolStripButtonStart);
                changeButtonState(toolStripButtonStop);
                // Request to START playback
                if (_scene == null)
                {
                    MessageBox.Show("Brak wczytanej sceny. U¿yj Wczytaj najpierw.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                  
                    return;
                }
 
                // Stwórz player i uruchom  
                _scenePlayer = new ScenePlayer(_scene);
            
            Task.Run(() =>
                {
                    try
                    {
                        _scenePlayer?.Play();  
                    }
                    catch (Exception ex)
                    {
                        BeginInvoke(() =>
                        {
                            MessageBox.Show($"B³¹d odtwarzania sceny: {ex.Message}", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                    finally
                    {
                        // Przywróæ UI na w¹tku GUI
                        BeginInvoke(() =>
                        {
                           // _scenePlayer = null;
                          
                        }); 

                    }
                });
            }
      

        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            _scenePlayer?.Stop();


            changeButtonState(toolStripButtonStart);
            changeButtonState(toolStripButtonStop);

        }

        private void changeButtonState(ToolStripButton button)
        {
            if (button.Enabled)
            {
                button.Enabled = false;
              //  button.Checked = false;

            }
            else
            {
                button.Enabled = true;
             //   button.Checked = true;
            }
        }

        private void openDefaultScene()
        {
            string defaultScenePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sceny", "scene_test.json");
            if (File.Exists(defaultScenePath))
            {
                _scene = new Scene();
                _scene.InitializeFromJson(defaultScenePath);
                 if (toolStripButtonStart.Enabled == false)
                    changeButtonState(toolStripButtonStart);
            }
            else
            {
                MessageBox.Show("Nie znaleziono domyœlnej sceny.", "B³¹d", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
