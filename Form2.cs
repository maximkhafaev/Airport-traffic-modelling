using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirportSimulation
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            labelTakenOff.Text = Modelling.countTakeoff.ToString();
            labelLanded.Text = Modelling.countLanding.ToString();
            double total = Modelling.countLanding + Modelling.countTakeoff;
            labelTotal.Text = total.ToString();
            labelIdle.Text = Form1.idle.ToString("0.00");
        }
    }
}
