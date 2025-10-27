namespace SoundScenesOpenAL_GUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            plikToolStripMenuItem1 = new ToolStripMenuItem();
            uruchomSceneToolStripMenuItem = new ToolStripMenuItem();
            otwórzToolStripMenuItem1 = new ToolStripMenuItem();
            jSONToolStripMenuItem = new ToolStripMenuItem();
            zamknijToolStripMenuItem1 = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            toolStripButtonStart = new ToolStripButton();
            toolStripButtonStop = new ToolStripButton();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { plikToolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // plikToolStripMenuItem1
            // 
            plikToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { uruchomSceneToolStripMenuItem, otwórzToolStripMenuItem1, zamknijToolStripMenuItem1 });
            plikToolStripMenuItem1.Name = "plikToolStripMenuItem1";
            plikToolStripMenuItem1.Size = new Size(38, 20);
            plikToolStripMenuItem1.Text = "Plik";
            // 
            // uruchomSceneToolStripMenuItem
            // 
            uruchomSceneToolStripMenuItem.Name = "uruchomSceneToolStripMenuItem";
            uruchomSceneToolStripMenuItem.Size = new Size(124, 22);
            uruchomSceneToolStripMenuItem.Text = "Uruchom";
            uruchomSceneToolStripMenuItem.Click += uruchomSceneToolStripMenuItem_Click;
            // 
            // otwórzToolStripMenuItem1
            // 
            otwórzToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { jSONToolStripMenuItem });
            otwórzToolStripMenuItem1.Name = "otwórzToolStripMenuItem1";
            otwórzToolStripMenuItem1.Size = new Size(124, 22);
            otwórzToolStripMenuItem1.Text = "Wczytaj";
            // 
            // jSONToolStripMenuItem
            // 
            jSONToolStripMenuItem.Name = "jSONToolStripMenuItem";
            jSONToolStripMenuItem.Size = new Size(102, 22);
            jSONToolStripMenuItem.Text = "JSON";
            jSONToolStripMenuItem.Click += jSONToolStripMenuItem_Click;
            // 
            // zamknijToolStripMenuItem1
            // 
            zamknijToolStripMenuItem1.Name = "zamknijToolStripMenuItem1";
            zamknijToolStripMenuItem1.Size = new Size(124, 22);
            zamknijToolStripMenuItem1.Text = "Zamknij";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonStart, toolStripButtonStop });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonStart
            // 
            toolStripButtonStart.CheckOnClick = true;
            toolStripButtonStart.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonStart.Image = (Image)resources.GetObject("toolStripButtonStart.Image");
            toolStripButtonStart.ImageTransparentColor = Color.Magenta;
            toolStripButtonStart.Name = "toolStripButtonStart";
            toolStripButtonStart.Size = new Size(23, 22);
            toolStripButtonStart.Text = "Start";
            toolStripButtonStart.Click += toolStripButtonStart_Click;
            // 
            // toolStripButtonStop
            // 
            toolStripButtonStop.CheckOnClick = true;
            toolStripButtonStop.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonStop.Image = (Image)resources.GetObject("toolStripButtonStop.Image");
            toolStripButtonStop.ImageTransparentColor = Color.Magenta;
            toolStripButtonStop.Name = "toolStripButtonStop";
            toolStripButtonStop.Size = new Size(23, 22);
            toolStripButtonStop.Text = "Stop";
            toolStripButtonStop.ToolTipText = "Stop";
            toolStripButtonStop.Click += toolStripButtonStop_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "SoundScenesGUI";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem plikToolStripMenuItem1;
        private ToolStripMenuItem uruchomSceneToolStripMenuItem;
        private ToolStripMenuItem otwórzToolStripMenuItem1;
        private ToolStripMenuItem jSONToolStripMenuItem;
        private ToolStripMenuItem zamknijToolStripMenuItem1;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonStart;
        private ToolStripButton toolStripButtonStop;
    }
}
