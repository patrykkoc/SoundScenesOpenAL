namespace SoundScenesOpenAL_GUI
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            contextMenuStripMain = new ContextMenuStrip(components);
            plikToolStripMenuItem = new ToolStripMenuItem();
            otwórzToolStripMenuItem = new ToolStripMenuItem();
            grajToolStripMenuItem = new ToolStripMenuItem();
            zamknijToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1 = new MenuStrip();
            plikToolStripMenuItem1 = new ToolStripMenuItem();
            uruchomSceneToolStripMenuItem = new ToolStripMenuItem();
            otwórzToolStripMenuItem1 = new ToolStripMenuItem();
            jSONToolStripMenuItem = new ToolStripMenuItem();
            zamknijToolStripMenuItem1 = new ToolStripMenuItem();
            contextMenuStripMain.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStripMain
            // 
            contextMenuStripMain.AccessibleName = "Menu";
            contextMenuStripMain.Items.AddRange(new ToolStripItem[] { plikToolStripMenuItem, grajToolStripMenuItem, zamknijToolStripMenuItem });
            contextMenuStripMain.Name = "contextMenuStrip1";
            contextMenuStripMain.Size = new Size(118, 70);
            // 
            // plikToolStripMenuItem
            // 
            plikToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { otwórzToolStripMenuItem });
            plikToolStripMenuItem.Name = "plikToolStripMenuItem";
            plikToolStripMenuItem.Size = new Size(117, 22);
            plikToolStripMenuItem.Text = "Plik";
            // 
            // otwórzToolStripMenuItem
            // 
            otwórzToolStripMenuItem.Name = "otwórzToolStripMenuItem";
            otwórzToolStripMenuItem.Size = new Size(112, 22);
            otwórzToolStripMenuItem.Text = "Otwórz";
            // 
            // grajToolStripMenuItem
            // 
            grajToolStripMenuItem.Name = "grajToolStripMenuItem";
            grajToolStripMenuItem.Size = new Size(117, 22);
            grajToolStripMenuItem.Text = "Graj";
            // 
            // zamknijToolStripMenuItem
            // 
            zamknijToolStripMenuItem.Name = "zamknijToolStripMenuItem";
            zamknijToolStripMenuItem.Size = new Size(117, 22);
            zamknijToolStripMenuItem.Text = "Zamknij";
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
            uruchomSceneToolStripMenuItem.Size = new Size(180, 22);
            uruchomSceneToolStripMenuItem.Text = "Uruchom scene";
            // 
            // otwórzToolStripMenuItem1
            // 
            otwórzToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { jSONToolStripMenuItem });
            otwórzToolStripMenuItem1.Name = "otwórzToolStripMenuItem1";
            otwórzToolStripMenuItem1.Size = new Size(180, 22);
            otwórzToolStripMenuItem1.Text = "Otwórz";
            // 
            // jSONToolStripMenuItem
            // 
            jSONToolStripMenuItem.Name = "jSONToolStripMenuItem";
            jSONToolStripMenuItem.Size = new Size(180, 22);
            jSONToolStripMenuItem.Text = "JSON";
            // 
            // zamknijToolStripMenuItem1
            // 
            zamknijToolStripMenuItem1.Name = "zamknijToolStripMenuItem1";
            zamknijToolStripMenuItem1.Size = new Size(180, 22);
            zamknijToolStripMenuItem1.Text = "Zamknij";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ContextMenuStrip = contextMenuStripMain;
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "SoundScenesGUI";
            contextMenuStripMain.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ContextMenuStrip contextMenuStripMain;
        private ToolStripMenuItem plikToolStripMenuItem;
        private ToolStripMenuItem otwórzToolStripMenuItem;
        private ToolStripMenuItem grajToolStripMenuItem;
        private ToolStripMenuItem zamknijToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem plikToolStripMenuItem1;
        private ToolStripMenuItem uruchomSceneToolStripMenuItem;
        private ToolStripMenuItem otwórzToolStripMenuItem1;
        private ToolStripMenuItem jSONToolStripMenuItem;
        private ToolStripMenuItem zamknijToolStripMenuItem1;
    }
}
