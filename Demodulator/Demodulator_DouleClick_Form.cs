using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using signal_tester;

namespace demodulation
{
    public partial class Demodulator_DoubleClick_Form : Form
    {
        public Demodulator demodulation_functions = new Demodulator();

        public Demodulator_DoubleClick_Form()
        {
            InitializeComponent();
        }

        private void FirFilterForm_Shown(object sender, EventArgs e)
        {
            SRvalue.Text = String.Format("{0} МГц", demodulation_functions.SR/ 1000000.0);
            Fvalue.Text = String.Format("{0}", demodulation_functions.F / 1000000.0);
            //button1.Text = String.Format("x{0}", Quadrature_AM_detector.x);
            Fvalue.Text = String.Format("{0}", demodulation_functions.F);
            if (demodulation_functions.show) { Show.Checked = true; } else { Show.Checked = false; }
            label4.Text = String.Format("{0}", Demodulator.x);
        }

        private void save_Click(object sender, EventArgs e)
        {
            demodulation_functions.sendComand = true;
            demodulation_functions.F = Convert.ToInt64(Fvalue.Text);
            if (Show.Checked) { demodulation_functions.show = true; } else { demodulation_functions.show = false; }
            this.Close();
        }

        private void ExponentiationForm_Load(object sender, EventArgs e)
        {
            SRvalue.Text = String.Format("{0} МГц", demodulation_functions.SR / 1000000.0);
            Fvalue.Text = String.Format("{0}", demodulation_functions.F / 1000000.0);
            //button1.Text = String.Format("x{0}", Quadrature_AM_detector.x);
            Fvalue.Text = String.Format("{0}", demodulation_functions.F);
            if (demodulation_functions.show) { Show.Checked = true; } else { Show.Checked = false; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Demodulator.x = Demodulator.x*2;
            SRvalue.Text = String.Format("{0} МГц", demodulation_functions.SR * Demodulator.x / 1000000.0);
            label4.Text = String.Format("{0}", Demodulator.x);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Demodulator.x = Demodulator.x / 2;
            SRvalue.Text = String.Format("{0} МГц", demodulation_functions.SR * Demodulator.x / 1000000.0);
            label4.Text = String.Format("{0}", Demodulator.x);
        }
    }
}
