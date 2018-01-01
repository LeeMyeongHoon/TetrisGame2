using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

public partial class App
{
	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);

		if (hasStack)
		{
			DrawStackSpace();
		}
	}
}
