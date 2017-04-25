using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using signal_tester;

namespace Exponentiation
{
    public partial class Quadrature_AM_detectorForm : Form
    {
        public Quadrature_AM_detector Quadrature_AM_detector = new Quadrature_AM_detector();

        public Quadrature_AM_detectorForm()
        {
            InitializeComponent();
        }

        private void FirFilterForm_Shown(object sender, EventArgs e)
        {
            SRvalue.Text = String.Format("{0} МГц", Quadrature_AM_detector.SR/ 1000000.0);
            Fvalue.Text = String.Format("{0}", Quadrature_AM_detector.F / 1000000.0);
            //button1.Text = String.Format("x{0}", Quadrature_AM_detector.x);
            Fvalue.Text = String.Format("{0}", Quadrature_AM_detector.F);
            if (Quadrature_AM_detector.show) { Show.Checked = true; } else { Show.Checked = false; }
            exponentiationLevel.Value = Quadrature_AM_detector.degree;
            label4.Text = String.Format("{0}", Quadrature_AM_detector.x);
        }

        private void save_Click(object sender, EventArgs e)
        {
            Quadrature_AM_detector.sin_cos_init();
            Quadrature_AM_detector.sendComand = true;
            Quadrature_AM_detector.F = Convert.ToInt64(Fvalue.Text);
            if (Show.Checked) { Quadrature_AM_detector.show = true; } else { Quadrature_AM_detector.show = false; }
            Quadrature_AM_detector.degree = (int)exponentiationLevel.Value;
            this.Close();
        }

        private void ExponentiationForm_Load(object sender, EventArgs e)
        {
            SRvalue.Text = String.Format("{0} МГц", Quadrature_AM_detector.SR / 1000000.0);
            Fvalue.Text = String.Format("{0}", Quadrature_AM_detector.F / 1000000.0);
            //button1.Text = String.Format("x{0}", Quadrature_AM_detector.x);
            Fvalue.Text = String.Format("{0}", Quadrature_AM_detector.F);
            if (Quadrature_AM_detector.show) { Show.Checked = true; } else { Show.Checked = false; }
            exponentiationLevel.Value = Quadrature_AM_detector.degree;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Quadrature_AM_detector.x = Quadrature_AM_detector.x*2;
            SRvalue.Text = String.Format("{0} МГц", Quadrature_AM_detector.SR * Quadrature_AM_detector.x / 1000000.0);
            label4.Text = String.Format("{0}",Quadrature_AM_detector.x);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Quadrature_AM_detector.x = Quadrature_AM_detector.x / 2;
            SRvalue.Text = String.Format("{0} МГц", Quadrature_AM_detector.SR * Quadrature_AM_detector.x / 1000000.0);
            label4.Text = String.Format("{0}", Quadrature_AM_detector.x);
        }
    }
}
