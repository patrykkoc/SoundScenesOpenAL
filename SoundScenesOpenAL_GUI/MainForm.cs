using System.Xml.Serialization;
using SoundScenesOpenAL_Library.Models;

namespace SoundScenesOpenAL_GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

      

        private void jSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pliki sceny (*.json)|*.json|Wszystkie pliki (*.*)|*.*";
            openFileDialog.Title = "Wybierz plik sceny";
            openFileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources" , "Sceny");

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Wczytaj scenê z pliku
                var scene = new  Scene();
                scene.InitializeFromJson(openFileDialog.FileName);

                // Mo¿esz zapamiêtaæ scenê w polu klasy, np. this._scene = scene;
                MessageBox.Show($"Wczytano scenê: {scene.Name}", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tutaj mo¿esz dodaæ logikê do uruchamiania ScenePlayer lub wyœwietlania informacji o scenie
            }
        }
    }
}
