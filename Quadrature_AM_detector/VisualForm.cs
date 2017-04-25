using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;  
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using FirFilterNew;

//using Filter;

namespace Exponentiation
{
    public partial class VisualForm : Form
    {
        public Quadrature_AM_detector Quadrature_AM_detector;
        private double fNormolize = 1f; // коефициент нормализации сигнала
        private double SR = 0.0d; // частота дискретизации
        private double F = 0.0d; // цетральная частота спектра (половина частоты дискретизации)
        private double FFIR = 0.0d;// реальная частота сигнала
        private double bw = 0.0d;//полоса пропускания
        private int N;//порядок ШПФ
        private float ACHAvering;
        private float SignalAvering ;
        Complex[] detection = new Complex[65536]; // для детектування
        Complex[] exponentiation = new Complex[65536]; // для детектування
        public float[] xAxes = new float[65536];
        public float[] outFFTdata = new float[65536];
        int minSpeedZoneCoef;
        int maxSpeedZoneCoef;
        int minCentralFrequencyZoneCoef;
        int maxCentralFrequencyZoneCoef;
        float maxValue;
        int speedPosition;
        int centralPosition;
        double realSpeedPosition;
        double realCentralFrequencyPosition;
        


        public VisualForm()
        {
            InitializeComponent();
        }

        private void LoadVisualForm()
        {
            if (Quadrature_AM_detector.maxFFT == 256) { comboBoxFFT.SelectedIndex = 0; }
            if (Quadrature_AM_detector.maxFFT == 512) { comboBoxFFT.SelectedIndex = 1; }
            if (Quadrature_AM_detector.maxFFT == 1024) { comboBoxFFT.SelectedIndex = 2; }
            if (Quadrature_AM_detector.maxFFT == 2048) { comboBoxFFT.SelectedIndex = 3; }
            if (Quadrature_AM_detector.maxFFT == 4096) { comboBoxFFT.SelectedIndex = 4; }
            if (Quadrature_AM_detector.maxFFT == 8192) { comboBoxFFT.SelectedIndex = 5; }
            if (Quadrature_AM_detector.maxFFT == 16384) { comboBoxFFT.SelectedIndex = 6; }
            if (Quadrature_AM_detector.maxFFT == 32768) { comboBoxFFT.SelectedIndex = 7; }
            if (Quadrature_AM_detector.maxFFT == 65536) { comboBoxFFT.SelectedIndex = 8; }
            SR = Quadrature_AM_detector.SR;
            F = Quadrature_AM_detector.F;            
            N = Quadrature_AM_detector.Count;
            if (N > Quadrature_AM_detector.maxFFT) { N = Quadrature_AM_detector.maxFFT; }
            for (int k = 0; k < N; k++)
            {
                detection[k] = new Complex(Quadrature_AM_detector._bufferData.iq[k].i, Quadrature_AM_detector._bufferData.iq[k].q);
                exponentiation[k] = new Complex(Quadrature_AM_detector._outData.iq[k].i, Quadrature_AM_detector._outData.iq[k].q);
            }
            for (int k = N; k < 65536; k++)
            {
                detection[k] = new Complex(0, 0);
                exponentiation[k] = new Complex(0, 0);
            }
            try
            {
                detection = Fft.fft(detection);
                exponentiation = Fft.fft(exponentiation);
                exponentiation = Fft.nfft(exponentiation);
            }
            catch
            {
                MessageBox.Show("ШПФ не проведено :(");
            }            
            minSpeedZoneCoef = (detection.Length / 10) * 6;
            maxSpeedZoneCoef = (detection.Length / 10) * 9;
            minCentralFrequencyZoneCoef = (exponentiation.Length / 10) * 4;
            maxCentralFrequencyZoneCoef = (exponentiation.Length / 10) * 6;
            maxValue = 0;
            speedPosition = 0;
            centralPosition = 0;
            realSpeedPosition = 0;
            realCentralFrequencyPosition = 0;

            for (int i = 0; i < detection.Length; i++)
            {
                if (i > minSpeedZoneCoef & i < maxSpeedZoneCoef)
                {
                    if (maxValue < Math.Log(detection[i].Magnitude, 10))
                    {
                        maxValue = (float) Math.Log(detection[i].Magnitude, 10);
                        speedPosition = i;
                    }
                }
            }

            for (int i = 0; i < exponentiation.Length; i++)
            {
            if (i > minCentralFrequencyZoneCoef & i < maxCentralFrequencyZoneCoef)
                    {
                        if (maxValue < Math.Log(exponentiation[i].Magnitude, 10))
                        {
                            maxValue = (float) Math.Log(exponentiation[i].Magnitude, 10);
                            centralPosition = i;
                        }
                    }
                xAxes[i] = (float)(i * SR / 65536);
                outFFTdata[i] = (float)Math.Log(exponentiation[i].Magnitude/ Quadrature_AM_detector.averagingValue, 10);
            }
            MitovScope.Channels[0].Data.SetXYData(xAxes,outFFTdata);

            realSpeedPosition = (SR/65536)*speedPosition;
                realCentralFrequencyPosition = (SR/65536)*centralPosition;
            //------------------------------------------------------------------------------------------------------  
           
            Speed_label.Text = "Частота маніауляції: " + realSpeedPosition + " Гц";
            T_label.Text = "Період маніпуляції:  " + (1 / realSpeedPosition * 1000000.0d) + " мкс";
            F_label.Text = "Центральна частота:  " + (realCentralFrequencyPosition / 1000.0d) + " кГц";
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 10000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.refreshButton, "Перебудувати графік");
        }

    

    private void VisualForm_Load(object sender, EventArgs e)
        {
            LoadVisualForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            LoadVisualForm();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (comboBoxFFT.SelectedIndex == 0) { Quadrature_AM_detector.maxFFT = 256; }
            if (comboBoxFFT.SelectedIndex == 1) { Quadrature_AM_detector.maxFFT = 512; }
            if (comboBoxFFT.SelectedIndex == 2) { Quadrature_AM_detector.maxFFT = 1024; }
            if (comboBoxFFT.SelectedIndex == 3) { Quadrature_AM_detector.maxFFT = 2048; }
            if (comboBoxFFT.SelectedIndex == 4) { Quadrature_AM_detector.maxFFT = 4096; }
            if (comboBoxFFT.SelectedIndex == 5) { Quadrature_AM_detector.maxFFT = 8192; }
            if (comboBoxFFT.SelectedIndex == 6) { Quadrature_AM_detector.maxFFT = 16384; }
            if (comboBoxFFT.SelectedIndex == 7) { Quadrature_AM_detector.maxFFT = 32768; }
            if (comboBoxFFT.SelectedIndex == 8) { Quadrature_AM_detector.maxFFT = 65536; }

            MitovScope.Channels[0].Data.Clear();
            N = Quadrature_AM_detector.Count;
            if (N > Quadrature_AM_detector.maxFFT) { N = Quadrature_AM_detector.maxFFT; }
            for (int k = 0; k < N; k++)
            {
                detection[k] = new Complex(Quadrature_AM_detector._bufferData.iq[k].i, Quadrature_AM_detector._bufferData.iq[k].q);
                exponentiation[k] = new Complex(Quadrature_AM_detector._outData.iq[k].i, Quadrature_AM_detector._outData.iq[k].q);
            }
            for (int k = N; k < 65536; k++)
            {
                detection[k] = new Complex(0, 0);
                exponentiation[k] = new Complex(0, 0);
            }
            try
            {
                detection = Fft.fft(detection);
                exponentiation = Fft.fft(exponentiation);
                exponentiation = Fft.nfft(exponentiation);
            }
            catch
            {
                MessageBox.Show("ШПФ не проведено :(");
            }
            int minSpeedZoneCoef = (detection.Length / 10) * 6;
            int maxSpeedZoneCoef = (detection.Length / 10) * 9;
            int minCentralFrequencyZoneCoef = (exponentiation.Length / 10) * 4;
            int maxCentralFrequencyZoneCoef = (exponentiation.Length / 10) * 6;
            float maxValue = 0;
            int speedPosition = 0;
            int centralPosition = 0;

            double realSpeedPosition = 0;
            double realCentralFrequencyPosition = 0;

            for (int i = 0; i < detection.Length; i++)
            {
                if (i > minSpeedZoneCoef & i < maxSpeedZoneCoef)
                {
                    if (maxValue < Math.Log(detection[i].Magnitude, 10))
                    {
                        maxValue = (float)Math.Log(detection[i].Magnitude, 10);
                        speedPosition = i;
                    }
                }
            }

            for (int i = 0; i < exponentiation.Length; i++)
            {
                if (i > minCentralFrequencyZoneCoef & i < maxCentralFrequencyZoneCoef)
                {
                    if (maxValue < Math.Log(exponentiation[i].Magnitude, 10))
                    {
                        maxValue = (float)Math.Log(exponentiation[i].Magnitude, 10);
                        centralPosition = i;
                    }
                }
                xAxes[i] = (float)(i * SR / 65536);
                outFFTdata[i] = (float)Math.Log(exponentiation[i].Magnitude/ Quadrature_AM_detector.averagingValue, 10);
            }
            MitovScope.Channels[0].Data.SetXYData(xAxes, outFFTdata);

            realSpeedPosition = (SR / 65536) * speedPosition;
            realCentralFrequencyPosition = (SR / 65536) * centralPosition;
            //------------------------------------------------------------------------------------------------------  

            Speed_label.Text = "Частота маніауляції: " + realSpeedPosition + " Гц";
            T_label.Text = "Період маніпуляції:  " + (1 / realSpeedPosition * 1000000.0d) + " мкс";
            F_label.Text = "Центральна частота:  " + (realCentralFrequencyPosition / 1000.0d) + " кГц";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("1");
            if (Quadrature_AM_detector.busy)
            {
                //MitovScope.Channels[0].Data.Clear();
                if (N > Quadrature_AM_detector.maxFFT) { N = Quadrature_AM_detector.maxFFT; }
                for (int k = 0; k < N; k++)
                {
                    detection[k] = new Complex(Quadrature_AM_detector._bufferData.iq[k].i, Quadrature_AM_detector._bufferData.iq[k].q);
                    exponentiation[k] = new Complex(Quadrature_AM_detector._outData.iq[k].i, Quadrature_AM_detector._outData.iq[k].q);
                }
                for (int k = N; k < 65536; k++)
                {
                    detection[k] = new Complex(0, 0);
                    exponentiation[k] = new Complex(0, 0);
                }
                try
                {
                    detection = Fft.fft(detection);
                    exponentiation = Fft.fft(exponentiation);
                    exponentiation = Fft.nfft(exponentiation);
                }
                catch
                {
                    MessageBox.Show("ШПФ не проведено :(");
                }
                minSpeedZoneCoef = (detection.Length / 10) * 6;
                maxSpeedZoneCoef = (detection.Length / 10) * 9;
                minCentralFrequencyZoneCoef = (exponentiation.Length / 10) * 4;
                maxCentralFrequencyZoneCoef = (exponentiation.Length / 10) * 6;
                maxValue = 0;
                speedPosition = 0;
                centralPosition = 0;
                realSpeedPosition = 0;
                realCentralFrequencyPosition = 0;

                for (int i = 0; i < detection.Length; i++)
                {
                    if (i > minSpeedZoneCoef & i < maxSpeedZoneCoef)
                    {
                        if (maxValue < Math.Log(detection[i].Magnitude, 10))
                        {
                            maxValue = (float)Math.Log(detection[i].Magnitude, 10);
                            speedPosition = i;
                        }
                    }
                }

                for (int i = 0; i < exponentiation.Length; i++)
                {
                    if (i > minCentralFrequencyZoneCoef & i < maxCentralFrequencyZoneCoef)
                    {
                        if (maxValue < Math.Log(exponentiation[i].Magnitude, 10))
                        {
                            maxValue = (float)Math.Log(exponentiation[i].Magnitude, 10);
                            centralPosition = i;
                        }
                    }
                    xAxes[i] = (float)(i * SR / 65536);
                    outFFTdata[i] = (float)Math.Log(exponentiation[i].Magnitude/ Quadrature_AM_detector.averagingValue, 10);
                }
                MitovScope.Channels[0].Data.SetXYData(xAxes, outFFTdata);

                realSpeedPosition = (SR / 65536) * speedPosition;
                realCentralFrequencyPosition = (SR / 65536) * centralPosition;
                //------------------------------------------------------------------------------------------------------  

                Speed_label.Text = "Частота маніауляції: " + realSpeedPosition + " Гц";
                T_label.Text = "Період маніпуляції:  " + (1 / realSpeedPosition * 1000000.0d) + " мкс";
                F_label.Text = "Центральна частота:  " + (realCentralFrequencyPosition / 1000.0d) + " кГц";
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Quadrature_AM_detector.averagingValue = (int)numericUpDown3.Value;
        }
    }
}
