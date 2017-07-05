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

namespace demodulation_namespace
{
    public partial class DemodulatorVisual_Form : Form
    {
        public float beginBW = 0.0f; // для порівняння і конфігурації фільтра
        public float BW = 0.0f;
        public Demodulator demodulation_functions;
        private double fNormolize = 1d / 4294967296; // коефициент нормализации сигнала
        private double SR = 0.0d; // частота дискретизации
        private double FFIR = 0.0d;// реальная частота сигнала
        private double bw = 0.0d;//полоса пропускания
        private int N;//порядок ШПФ
        private float ACHAvering;
        private float SignalAvering ;
        Complex[] visual_data/* = new Complex[65536]*/;
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


        public DemodulatorVisual_Form()
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
            if (demodulation_functions.maxFFT == 256) { comboBoxFFT.SelectedIndex = 0; }
            if (demodulation_functions.maxFFT == 512) { comboBoxFFT.SelectedIndex = 1; }
            if (demodulation_functions.maxFFT == 1024) { comboBoxFFT.SelectedIndex = 2; }
            if (demodulation_functions.maxFFT == 2048) { comboBoxFFT.SelectedIndex = 3; }
            if (demodulation_functions.maxFFT == 4096) { comboBoxFFT.SelectedIndex = 4; }
            if (demodulation_functions.maxFFT == 8192) { comboBoxFFT.SelectedIndex = 5; }
            if (demodulation_functions.maxFFT == 16384) { comboBoxFFT.SelectedIndex = 6; }
            if (demodulation_functions.maxFFT == 32768) { comboBoxFFT.SelectedIndex = 7; }
            if (demodulation_functions.maxFFT == 65536) { comboBoxFFT.SelectedIndex = 8; }
            SR = demodulation_functions.SR;    
            toolStripStatusLabel1.Text = "Працюю без помилок";
            numericUpDown3.Value = demodulation_functions.fftAveragingValue;
            if (Quadrature_AM_demodulatorSPARKInterface.calculate_parametrs_bool) { calculate_parametrs.Checked = true; } else { calculate_parametrs.Checked = false; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.SHIFTING ) { FFT_data_display.SelectedIndex = 0; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.EXPONENT) { FFT_data_display.SelectedIndex = 1; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.DETECTED) { FFT_data_display.SelectedIndex = 2; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.FILTERING) { FFT_data_display.SelectedIndex = 3; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.INPUT) { FFT_data_display.SelectedIndex = 4; }
        }             

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (comboBoxFFT.SelectedIndex == 0) { demodulation_functions.maxFFT = 256; }
            if (comboBoxFFT.SelectedIndex == 1) { demodulation_functions.maxFFT = 512; }
            if (comboBoxFFT.SelectedIndex == 2) { demodulation_functions.maxFFT = 1024; }
            if (comboBoxFFT.SelectedIndex == 3) { demodulation_functions.maxFFT = 2048; }
            if (comboBoxFFT.SelectedIndex == 4) { demodulation_functions.maxFFT = 4096; }
            if (comboBoxFFT.SelectedIndex == 5) { demodulation_functions.maxFFT = 8192; }
            if (comboBoxFFT.SelectedIndex == 6) { demodulation_functions.maxFFT = 16384; }
            if (comboBoxFFT.SelectedIndex == 7) { demodulation_functions.maxFFT = 32768; }
            if (comboBoxFFT.SelectedIndex == 8) { demodulation_functions.maxFFT = 65536; }
            MitovScope.Channels[0].Data.Clear();
            toolStripStatusLabel1.Text = string.Format("Порядок ШПФ змінено на {0}", demodulation_functions.maxFFT);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            if (BW != (float)MitovScope.Cursors[0].Position.X) { BW = (float)MitovScope.Cursors[0].Position.X; demodulation_functions.configureFirFilter(BW); toolStripStatusLabel1.Text = string.Format("{0}", (float)MitovScope.Cursors[0].Position.X); }            
             try
            {

                toolStripProgressBar1.Value = averingRepeat * 100 / demodulation_functions.fftAveragingValue;
                toolStripStatusLabel.Text = string.Format("{0} %", toolStripProgressBar1.Value);
                BW_label.Text = "Смуга фільтрації:   " + (BW / 1000.0d) + " кГц";
                toolStripStatusLabel1.Text = string.Format("Working");
            }
            catch
            {
                toolStripStatusLabel1.Text = "ШПФ не проведено :(";
            }

            Speed_label.Text = "Символьна швидкість: " + demodulation_functions.realSpeedPosition + " Бод";
            T_label.Text = "Період маніпуляції:  " + (1 / demodulation_functions.realSpeedPosition * 1000000.0d) + " мкс";
            F_label.Text = "Центральна частота:  " + (demodulation_functions.realCentralFrequencyPosition / 1000.0d) + " кГц";
            deltaF_label.Text = "Відхилення:  " + ((demodulation_functions.realCentralFrequencyPosition - demodulation_functions.SR / 2) / 1000.0d) + " кГц";           
        }
        public void displayFFT(int length)
        {
            visual_data = new Complex[65536];
            length = length / 4;
            try
            {
                if(demodulation_functions.display == Demodulator.FFT_data_display.SHIFTING)
                {
                    Parallel.For(0, length, k =>
                    {
                        visual_data[k] = new Complex(Demodulator.IQ_shifted.iq[k].i, Demodulator.IQ_shifted.iq[k].q);
                    });
                }
                if (demodulation_functions.display == Demodulator.FFT_data_display.DETECTED)
                {
                    Parallel.For(0, length, k =>
                    {
                        visual_data[k] = new Complex(Demodulator.IQ_detected.iq[k].i, Demodulator.IQ_detected.iq[k].q);
                    });
                }
                if (demodulation_functions.display == Demodulator.FFT_data_display.EXPONENT)
                {
                    Parallel.For(0, length, k =>
                    {
                        visual_data[k] = new Complex(Demodulator.IQ_elevated.iq[k].i, Demodulator.IQ_elevated.iq[k].q);
                    });
                }
                if (demodulation_functions.display == Demodulator.FFT_data_display.FILTERING)
                {
                    Parallel.For(0, length, k =>
                    {
                        visual_data[k] = new Complex(Demodulator.IQ_filtered.iq[k].i, Demodulator.IQ_filtered.iq[k].q);
                    });
                }
                if (demodulation_functions.display == Demodulator.FFT_data_display.INPUT)
                {
                    Parallel.For(0, length, k =>
                    {
                        visual_data[k] = new Complex(Demodulator.IQ_inData.iq[k].i, Demodulator.IQ_inData.iq[k].q);
                    });
                }
                Parallel.For(length, 65536, k =>
                {
                    visual_data[k] = new Complex(0, 0);
                });
                //MessageBox.Show(string.Format("visual_data ->{0}\n_shiftingData -> ({1}, {2})\nlength = {3}", visual_data[1024], demodulation_functions._shiftingData.iq[1024].i, demodulation_functions._shiftingData.iq[1024].i, length));
                //MessageBox.Show(string.Format("visual_data ->{0})", visual_data[4096]));
                visual_data = Fft.fft(visual_data);
                if (demodulation_functions.display != Demodulator.FFT_data_display.DETECTED) { visual_data = Fft.nfft(visual_data); } else { }
                Parallel.For(0, 65536, i =>
                {
                    avering_buffer[i] = (avering_buffer[i] + visual_data[i].Magnitude) /2;
                });
                averingRepeat++;
                if (averingRepeat >= demodulation_functions.fftAveragingValue)
                {
                    //avering_buffer = Fft.nfft(avering_buffer);
                    averingRepeat = 0;
                    Parallel.For(0, visual_data.Length, i =>
                    {
                        xAxes[i] = (float)(i * SR / 65536);
                        outFFTdata[i] = (float)(10 * Math.Log((avering_buffer[i] /*/ demodulation_functions.averagingValue*/) * fNormolize, 10));
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
            demodulation_functions.fftAveragingValue = (int)numericUpDown3.Value;
            toolStripStatusLabel1.Text = string.Format("Усереднення ШПФ змінено на {0}", demodulation_functions.fftAveragingValue);
        }

        private void calculate_parametrs_CheckedChanged(object sender, EventArgs e)
        {
            //if (calculate_parametrs.Checked) { demodulation_functions.calculate_parametrs_bool = true; } else { demodulation_functions.calculate_parametrs_bool = false; }            
            Quadrature_AM_demodulatorSPARKInterface.calculate_parametrs_bool = calculate_parametrs.Checked;
        }

        private void FFT_data_display_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FFT_data_display.SelectedIndex == 0) { demodulation_functions.display = Demodulator.FFT_data_display.SHIFTING; }
            if (FFT_data_display.SelectedIndex == 1) { demodulation_functions.display = Demodulator.FFT_data_display.EXPONENT; }
            if (FFT_data_display.SelectedIndex == 2) { demodulation_functions.display = Demodulator.FFT_data_display.DETECTED; }
            if (FFT_data_display.SelectedIndex == 3) { demodulation_functions.display = Demodulator.FFT_data_display.FILTERING; }
            if (FFT_data_display.SelectedIndex == 4) { demodulation_functions.display = Demodulator.FFT_data_display.INPUT; }
        }
    }
}
