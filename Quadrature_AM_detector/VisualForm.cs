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
using System.Threading.Tasks;

//using Filter;

namespace Exponentiation
{
    public partial class VisualForm : Form
    {
        public float beginBW = 0.0f; // для порівняння і конфігурації фільтра
        public float BW = 0.0f;
        public Quadrature_AM_detector Quadrature_AM_detector;
        private double fNormolize = 1d / 4294967296; // коефициент нормализации сигнала
        private double SR = 0.0d; // частота дискретизации
        //private double F = 0.0d; // цетральная частота спектра (половина частоты дискретизации)
        private double FFIR = 0.0d;// реальная частота сигнала
        private double bw = 0.0d;//полоса пропускания
        private int N;//порядок ШПФ
        private float ACHAvering;
        private float SignalAvering ;
        //Complex[] detection = new Complex[65536]; // для детектування  //коли визначення параметрів було тут
        //Complex[] exponentiation = new Complex[65536]; // для піднесення до степені    //коли визначення параметрів було тут
        Complex[] visual_data = new Complex[65536];
        //Complex[] avering_buffer = new Complex[65536];
        double [] avering_buffer = new double[65536];
        public float[] xAxes = new float[65536]; // значення осі Х ШТП
        public float[] outFFTdata = new float[65536]; // значення осі У ШПФ
        /*зона пошуку гармоніки швидкості модуляції*/
        //int minSpeedZoneCoef;
        //int maxSpeedZoneCoef;
        /*зона пошуку центральної частоти*/
        //int minCentralFrequencyZoneCoef;
        //int maxCentralFrequencyZoneCoef;
        /**/
        //float maxValue; // змінна для знаходження пікової гармоніки
        //int speedPosition; // позиція пікової гармоніки швидкості
        //int centralPosition; // позиція пікової гармоніки центральної частоти
        int averingRepeat = 0;


        public VisualForm()
        {
            InitializeComponent();
        }

        private void VisualForm_Load(object sender, EventArgs e)
        {
            LoadVisualForm();
        }

        private void LoadVisualForm()
        {
            MitovScope.Channels[0].Color = Color.Gray;
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
            //Quadrature_AM_detector.FFTdeep = Quadrature_AM_detector.Count; //???????????????????????????????????
            toolStripStatusLabel1.Text = "Працюю без помилок";
            numericUpDown3.Value = Quadrature_AM_detector.averagingValue;
            if (Quadrature_AM_detectorSPARKInterface.calculate_parametrs_bool) { calculate_parametrs.Checked = true; } else { calculate_parametrs.Checked = false; }
            //calculate_parametrs.Checked = Quadrature_AM_detectorSPARKInterface.calculate_parametrs_bool;
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
            toolStripStatusLabel1.Text = string.Format("Порядок ШПФ змінено на {0}", Quadrature_AM_detector.maxFFT);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            if (BW != (float)MitovScope.Cursors[0].Position.X) { BW = (float)MitovScope.Cursors[0].Position.X; Quadrature_AM_detector.configureFirFilter(BW); toolStripStatusLabel1.Text = string.Format("{0}", (float)MitovScope.Cursors[0].Position.X); }            
             try
            {

                toolStripProgressBar1.Value = averingRepeat * 100 / Quadrature_AM_detector.averagingValue;
                toolStripStatusLabel.Text = string.Format("{0} %", toolStripProgressBar1.Value);
                BW_label.Text = "Смуга фільтрації:   " + (BW / 1000.0d) + " кГц";
                toolStripStatusLabel1.Text = string.Format("Working");
            }
            catch
            {
                toolStripStatusLabel1.Text = "ШПФ не проведено :(";
            }

            Speed_label.Text = "Символьна швидкість: " + Quadrature_AM_detector.realSpeedPosition + " Бод";
            T_label.Text = "Період маніпуляції:  " + (1 / Quadrature_AM_detector.realSpeedPosition * 1000000.0d) + " мкс";
            F_label.Text = "Центральна частота:  " + (Quadrature_AM_detector.realCentralFrequencyPosition / 1000.0d) + " кГц";
            deltaF_label.Text = "Відхилення:  " + ((Quadrature_AM_detector.realCentralFrequencyPosition - Quadrature_AM_detector.SR / 2) / 1000.0d) + " кГц";
            //}
            //else { MitovScope.Channels[0].Data.Clear(); }
        }
        //public void F_calculating()
        //{
        //    if (Quadrature_AM_detector.busy)
        //    {
        //        if (N > Quadrature_AM_detector.maxFFT) { N = Quadrature_AM_detector.maxFFT; }
        //        //-----Блок визначення центральної частоти-----               
        //        try
        //        {
        //            Parallel.For(0, N, k =>
        //            {
        //                exponentiation[k] = new Complex(Quadrature_AM_detector._expData.iq[k].i, Quadrature_AM_detector._expData.iq[k].q); // тут поки outData, щоб бачити в наступному модулі цей вихід !!!
        //            });
        //            Parallel.For(N, 65536, k =>
        //            {
        //                exponentiation[k] = new Complex(0, 0);
        //            });

        //            exponentiation = Fft.fft(exponentiation);
        //            exponentiation = Fft.nfft(exponentiation);

        //            minCentralFrequencyZoneCoef = (exponentiation.Length / 10) * 1;
        //            maxCentralFrequencyZoneCoef = (exponentiation.Length / 10) * 9;
        //            centralPosition = 0;
        //            Quadrature_AM_detector.realCentralFrequencyPosition = 0;
        //            Parallel.For(0, exponentiation.Length, i =>
        //            {
        //                if (i > minCentralFrequencyZoneCoef & i < maxCentralFrequencyZoneCoef)
        //                {
        //                    if (maxValue < Math.Log(exponentiation[i].Magnitude, 10))
        //                    {
        //                        maxValue = (float)Math.Log(exponentiation[i].Magnitude, 10);
        //                        centralPosition = i;
        //                    }
        //                }
        //            });
        //            Quadrature_AM_detector.realCentralFrequencyPosition = (SR / 65536) * centralPosition;
        //        }
        //        catch
        //        {
        //            toolStripStatusLabel1.Text = "Trouble with definding central frequency :(";
        //        }
        //    }
        //    else { MitovScope.Channels[0].Data.Clear(); }
        //}
        //public void speed_calculating()
        //{
        //    if (Quadrature_AM_detector.busy)
        //    {
        //        //-----Блок визначення швидкості маніпуляції-----   
        //        try
        //        {
        //            Parallel.For(0, N, k =>
        //            {
        //                detection[k] = new Complex(Quadrature_AM_detector._detectedData.iq[k].i, Quadrature_AM_detector._detectedData.iq[k].q);
        //            });

        //            Parallel.For(N, 65536, k =>
        //            {
        //                detection[k] = new Complex(0, 0);
        //            });

        //            detection = Fft.fft(detection);
        //            minSpeedZoneCoef = (detection.Length / 10) * 6;
        //            maxSpeedZoneCoef = (detection.Length / 10) * 9;
        //            maxValue = 0;
        //            speedPosition = 0;
        //            Quadrature_AM_detector.realSpeedPosition = 0;
        //            Parallel.For(0, detection.Length, i =>
        //            {
        //                if (i > minSpeedZoneCoef & i < maxSpeedZoneCoef)
        //                {
        //                    if (maxValue < Math.Log(detection[i].Magnitude, 10))
        //                    {
        //                        maxValue = (float)Math.Log(detection[i].Magnitude, 10);
        //                        speedPosition = i;
        //                    }
        //                }
        //            });
        //            Quadrature_AM_detector.realSpeedPosition = (SR / 65536) * speedPosition;
        //        }
        //        catch
        //        {
        //            toolStripStatusLabel1.Text = "Trouble with definding speed :(";
        //        }
        //    }
        //    else { MitovScope.Channels[0].Data.Clear(); }
        //}
        public void displayFFT(int length)
        {
            try
            {
                Parallel.For(0, length, k =>
            {
                visual_data[k] = new Complex(Quadrature_AM_detector._shiftingData.iq[k].i, Quadrature_AM_detector._shiftingData.iq[k].q);
            });
                Parallel.For(length, 65536, k =>
                {
                    visual_data[k] = new Complex(0, 0);
                });
                visual_data = Fft.fft(visual_data);
                visual_data = Fft.nfft(visual_data);
                Parallel.For(0, 65536, i =>
                {
                    avering_buffer[i] = (avering_buffer[i] + visual_data[i].Magnitude) /2;
                });
                averingRepeat++;
                if (averingRepeat >= Quadrature_AM_detector.averagingValue)
                {
                    //avering_buffer = Fft.nfft(avering_buffer);
                    averingRepeat = 0;
                    Parallel.For(0, visual_data.Length, i =>
                    {
                        xAxes[i] = (float)(i * SR / 65536);
                        outFFTdata[i] = (float)(10 * Math.Log((avering_buffer[i] /*/ Quadrature_AM_detector.averagingValue*/) * fNormolize, 10));
                    });
                    MitovScope.Channels[0].Data.SetXYData(xAxes, outFFTdata);
                    Array.Clear(avering_buffer, 0, 65536);
                }
            }
            catch
            {
                toolStripStatusLabel1.Text = "Trouble with averingFFT :(";
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Quadrature_AM_detector.averagingValue = (int)numericUpDown3.Value;
            toolStripStatusLabel1.Text = string.Format("Усереднення ШПФ змінено на {0}", Quadrature_AM_detector.averagingValue);
        }

        private void calculate_parametrs_CheckedChanged(object sender, EventArgs e)
        {
            //if (calculate_parametrs.Checked) { Quadrature_AM_detector.calculate_parametrs_bool = true; } else { Quadrature_AM_detector.calculate_parametrs_bool = false; }            
            Quadrature_AM_detectorSPARKInterface.calculate_parametrs_bool = calculate_parametrs.Checked;
        }
    }
}
