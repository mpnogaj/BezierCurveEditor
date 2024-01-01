namespace BezierCurveEditor
{
	partial class MainWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.canvasModeStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.canvasSizeStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.canvasErrorLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.curvesView = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.newFileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.openFileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.saveFileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsFileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.exportFileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.exitFileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showHelpMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.canvasPanel = new BezierCurveEditor.CustomPanel();
			this.canvas = new BezierCurveEditor.Controls.Canvas();
			this.statusStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.canvasPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.canvasModeStatusLabel,
            this.canvasSizeStatusLabel,
            this.canvasErrorLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 484);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
			this.statusStrip1.Size = new System.Drawing.Size(1018, 24);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// canvasModeStatusLabel
			// 
			this.canvasModeStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.canvasModeStatusLabel.Name = "canvasModeStatusLabel";
			this.canvasModeStatusLabel.Size = new System.Drawing.Size(103, 19);
			this.canvasModeStatusLabel.Text = "Test123 status bar";
			// 
			// canvasSizeStatusLabel
			// 
			this.canvasSizeStatusLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.canvasSizeStatusLabel.Name = "canvasSizeStatusLabel";
			this.canvasSizeStatusLabel.Size = new System.Drawing.Size(4, 19);
			// 
			// canvasErrorLabel
			// 
			this.canvasErrorLabel.ForeColor = System.Drawing.Color.Red;
			this.canvasErrorLabel.Name = "canvasErrorLabel";
			this.canvasErrorLabel.Size = new System.Drawing.Size(0, 19);
			// 
			// curvesView
			// 
			this.curvesView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.curvesView.HideSelection = false;
			this.curvesView.Location = new System.Drawing.Point(9, 38);
			this.curvesView.Margin = new System.Windows.Forms.Padding(2);
			this.curvesView.Name = "curvesView";
			this.curvesView.Size = new System.Drawing.Size(92, 447);
			this.curvesView.TabIndex = 1;
			this.curvesView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.curvesView_AfterSelect);
			this.curvesView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.curvesView_KeyUp);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 23);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Curves";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(102, 23);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Canvas";
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(1018, 24);
			this.menuStrip1.TabIndex = 6;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileMenu
			// 
			this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileMenu,
            this.toolStripSeparator1,
            this.openFileMenu,
            this.toolStripSeparator2,
            this.saveFileMenu,
            this.saveAsFileMenu,
            this.toolStripSeparator3,
            this.exportFileMenu,
            this.toolStripSeparator4,
            this.exitFileMenu});
			this.fileMenu.Name = "fileMenu";
			this.fileMenu.Size = new System.Drawing.Size(37, 20);
			this.fileMenu.Text = "File";
			// 
			// newFileMenu
			// 
			this.newFileMenu.Name = "newFileMenu";
			this.newFileMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newFileMenu.Size = new System.Drawing.Size(184, 22);
			this.newFileMenu.Text = "New";
			this.newFileMenu.Click += new System.EventHandler(this.newFileMenu_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
			// 
			// openFileMenu
			// 
			this.openFileMenu.Name = "openFileMenu";
			this.openFileMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openFileMenu.Size = new System.Drawing.Size(184, 22);
			this.openFileMenu.Text = "Open";
			this.openFileMenu.Click += new System.EventHandler(this.openFileMenu_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(181, 6);
			// 
			// saveFileMenu
			// 
			this.saveFileMenu.Name = "saveFileMenu";
			this.saveFileMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveFileMenu.Size = new System.Drawing.Size(184, 22);
			this.saveFileMenu.Text = "Save";
			this.saveFileMenu.Click += new System.EventHandler(this.saveFileMenu_Click);
			// 
			// saveAsFileMenu
			// 
			this.saveAsFileMenu.Name = "saveAsFileMenu";
			this.saveAsFileMenu.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.saveAsFileMenu.Size = new System.Drawing.Size(184, 22);
			this.saveAsFileMenu.Text = "Save as";
			this.saveAsFileMenu.Click += new System.EventHandler(this.saveAsFileMenu_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(181, 6);
			// 
			// exportFileMenu
			// 
			this.exportFileMenu.Name = "exportFileMenu";
			this.exportFileMenu.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
			this.exportFileMenu.Size = new System.Drawing.Size(184, 22);
			this.exportFileMenu.Text = "Export";
			this.exportFileMenu.Click += new System.EventHandler(this.exportFileMenu_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(181, 6);
			// 
			// exitFileMenu
			// 
			this.exitFileMenu.Name = "exitFileMenu";
			this.exitFileMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitFileMenu.Size = new System.Drawing.Size(184, 22);
			this.exitFileMenu.Text = "Exit";
			this.exitFileMenu.Click += new System.EventHandler(this.exitFileMenu_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHelpMenu,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// showHelpMenu
			// 
			this.showHelpMenu.Name = "showHelpMenu";
			this.showHelpMenu.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.showHelpMenu.Size = new System.Drawing.Size(148, 22);
			this.showHelpMenu.Text = "Show help";
			this.showHelpMenu.Click += new System.EventHandler(this.showHelpMenu_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// canvasPanel
			// 
			this.canvasPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.canvasPanel.AutoScroll = true;
			this.canvasPanel.Controls.Add(this.canvas);
			this.canvasPanel.Location = new System.Drawing.Point(104, 38);
			this.canvasPanel.Margin = new System.Windows.Forms.Padding(2);
			this.canvasPanel.Name = "canvasPanel";
			this.canvasPanel.Size = new System.Drawing.Size(914, 446);
			this.canvasPanel.TabIndex = 3;
			// 
			// canvas
			// 
			this.canvas.BackColor = System.Drawing.Color.White;
			this.canvas.Location = new System.Drawing.Point(2, 2);
			this.canvas.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.canvas.Name = "canvas";
			this.canvas.SelectedItem = null;
			this.canvas.Size = new System.Drawing.Size(656, 349);
			this.canvas.TabIndex = 0;
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1018, 508);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.curvesView);
			this.Controls.Add(this.canvasPanel);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "MainWindow";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyUp);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.canvasPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Controls.Canvas canvas;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel canvasModeStatusLabel;
		private CustomPanel canvasPanel;
		private System.Windows.Forms.TreeView curvesView;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileMenu;
		private System.Windows.Forms.ToolStripMenuItem openFileMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem saveFileMenu;
		private System.Windows.Forms.ToolStripMenuItem saveAsFileMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem exitFileMenu;
		private System.Windows.Forms.ToolStripMenuItem newFileMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripStatusLabel canvasErrorLabel;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showHelpMenu;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportFileMenu;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripStatusLabel canvasSizeStatusLabel;
	}
}

