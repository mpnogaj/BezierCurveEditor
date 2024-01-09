using System;
using System.Windows.Forms;
using Common;

namespace BezierFontTextBoxDemo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void loadFontBtn_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog()
			{
				Title = "Pick font pack",
				DefaultExt = FileExtension.FontPackExtension,
				Filter = $"Font pack (*{FileExtension.FontPackExtension})|*{FileExtension.FontPackExtension}|All files (*.*)|*.*",
				Multiselect = false
			};

			if(ofd.ShowDialog() != DialogResult.OK) return;

			bezierTextBox.FontPath = ofd.FileName;
		}

		private void textBox_TextChanged(object sender, EventArgs e)
		{
			bezierTextBox.Text = textBox.Text;
		}
	}
}
