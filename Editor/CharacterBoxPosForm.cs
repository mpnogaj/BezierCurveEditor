using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BezierCurveEditor
{
	public partial class CharacterBoxPosForm : Form
	{
		public CharacterBoxPosForm()
		{
			InitializeComponent();
		}

		public int PosX
		{
			get => int.Parse(textBoxX.Text);
			set => textBoxX.Text = value.ToString();
		}

		public int PosY
		{
			get => int.Parse(textBoxY.Text);
			set => textBoxY.Text = value.ToString();
		}

		private void IntTextBox_Validating(object sender, CancelEventArgs e)
		{
			var textBox = (TextBox)sender;
			if (!int.TryParse(textBox.Text, out _))
			{
				e.Cancel = true;
				errorProvider.SetError(textBox, "Please enter a valid number");
			}
			else
			{
				e.Cancel = false;
			}
		}

		private void IntTextBox_Validated(object sender, EventArgs e)
		{
			errorProvider.Clear();
		}
	}
}
