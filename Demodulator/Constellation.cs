using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mitov.PlotLab;
using Mitov.SignalLab;
using System.Numerics;

namespace demodulation
{
    public partial class Constellation : Form
    {
        public Demodulator dem_functions;
        public Constellation()
        {
            InitializeComponent();
            this.Top = 518;
            this.Left = 0;
        }

        private void checkBox_constellation_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.display_constellation = checkBox_constellation.Checked;
            //timer_constellation.Enabled = checkBox_constellation.Checked;
        }        

        private void Constellation_FormClosing(object sender, FormClosingEventArgs e)
        {
            //dem_functions.display_constellation = false;
        }

        private void Constellation_Load(object sender, EventArgs e)
        {
            checkBox_constellation.Checked = dem_functions.display_constellation;
        }

        private void timer_constellation_Tick(object sender, EventArgs e)
        {
            try
            {
                timer_constellation.Interval = dem_functions.display_Tick;
                ComplexBuffer buffer = new ComplexBuffer(dem_functions.IQ_filtered.bytes.Length / 4);
                RealBuffer real = new RealBuffer(dem_functions.IQ_filtered.bytes.Length / 4);
                RealBuffer imaginary = new RealBuffer(dem_functions.IQ_filtered.bytes.Length / 4);
                for (int k = 0; k < dem_functions.IQ_filtered.bytes.Length / 4; k++)
                {
                    real[k] = dem_functions.IQ_filtered.iq[k].i;
                    imaginary[k] = dem_functions.IQ_filtered.iq[k].q;
                }
                buffer.Set(real, imaginary);           
                genericComplex_constellation.SendData(buffer);
            }
            catch (Exception exception)
            {
                dem_functions.warningMessage = string.Format("{0}.{1}: {2}", exception.Source, exception.TargetSite, exception.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            checkBox_constellation.Checked = dem_functions.display_constellation;
            timer_constellation.Enabled = dem_functions.display_constellation;
        }
    }
}
