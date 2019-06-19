using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphic
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		private void Form_Load(object sender, EventArgs e)
		{
			chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
			chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
			chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
			chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

			chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
			chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
			chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;


		}

		private void button1_Click(object sender, EventArgs e)
		{
			double xMin = Convert.ToInt32(textBox2.Text);
			double xMax = Convert.ToInt32(textBox3.Text);
			double interval = Convert.ToDouble(textBox4.Text);
			chart1.Series[0].Points.Clear();
			string Expression = textBox1.Text;
			for(double i = xMin;i < xMax; i+=interval)
			{
				chart1.Series[0].Points.AddXY(i, Parser1.Calculate(Expression, i));
			}
			chart1.Series[0].ToolTip = "X = #VALX, Y = #VALY";
		}
	}
}
