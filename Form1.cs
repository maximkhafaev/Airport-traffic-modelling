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
    public partial class Form1 : Form
    {
        public static double idle;

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            
            chart1.Series["Workload of the runways, %"].Points.Clear();
            chart2.Series["Waiting time in queue to land, h."].Points.Clear();
            chart2.Series["Waiting time in queue to takeoff, h."].Points.Clear();

            Modelling.HourModelling = (double)numericTimeModeling.Value;
            Modelling.NumberRunways4Landing = (int)numericLanesLanding.Value;
            Modelling.NumberRunways4Takeoff = (int)numericLanesTakeoff.Value;
            Modelling.ExpectationAppearanceLanding = (double)numericExpectationAppearanceLanding.Value;
            Modelling.DeviationAppearanceLanding = (double)numericDeviationAppearancLandinge.Value;
            Modelling.ExpectationLanding = (double)numericExpectationLanding.Value;
            Modelling.DeviationLanding = (double)numericDeviationLanding.Value;
            Modelling.ExpectationAppearanceTakeoff = (double)numericExpectationAppearanceTakeoff.Value;
            Modelling.DeviationAppearanceTakeoff = (double)numericDeviationAppearancTakeoff.Value;
            Modelling.ExpectationTakeoff = (double)numericExpectationTakeoff.Value;
            Modelling.DeviationTakeoff = (double)numericDeviationTakeoff.Value;

            if (numericLanesLanding.Value >= 0 | numericLanesTakeoff.Value >= 0)
            {
                Modelling.Start();

                int counter = 0;
                double idletime = 0;
                foreach (var val in Modelling.LoadOfRunways)
                {
                    chart1.Series["Workload of the runways, %"].Points.AddXY(val.Item1, val.Item2);
                    idletime += val.Item2;
                    counter += 1;
                }
                idle = 100 - (idletime / counter);

                foreach (var val in Modelling.AverageWaitngLanding)
                {
                    chart2.Series["Waiting time in queue to land, h."].Points.AddXY(val.Item1, val.Item2);
                }

                foreach (var val in Modelling.AverageWaitngTakeoff)
                {
                    chart2.Series["Waiting time in queue to takeoff, h."].Points.AddXY(val.Item1, val.Item2);
                }

                Form2 statistics = new Form2();
                statistics.Show();
            }
        }
    }
}
