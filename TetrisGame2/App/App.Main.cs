using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace TetrisGame2
{
	public partial class App : Form
	{
		private void BTN_START_Click(object sender, EventArgs e)
		{
			BTN_EXIT.Hide();
			BTN_START.Hide();

			hasStack = true;
			DrawStackSpace();
		}

		private void BTN_EXIT_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show("프로그램을 종료하시겠습니다?", BTN_EXIT.Text, MessageBoxButtons.OKCancel);

			if (result == DialogResult.OK)
			{
				Application.Exit();
			}
		}
	}
}