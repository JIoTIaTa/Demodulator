﻿using System;
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
        private FFT_array_creating array_for_FFT;
        private double fNormolize = 1d / 4294967296; // коефициент нормализации сигнала
        private double SR = 0.0d; // частота дискретизации
        private double FFIR = 0.0d;// реальная частота сигнала
        private double bw = 0.0d;//полоса пропускания
        private int N;//порядок ШПФ
        private float ACHAvering;
        private float SignalAvering;
        Complex[] visual_data = new Complex[65536];
        //Complex[] visual_data;
        double[] avering_buffer = new double[65536];
        public float[] xAxes = new float[65536]; // значення осі Х ШТП
        public float[] outFFTdata = new float[65536]; // значення осі У ШПФ
        int averingRepeat = 0;
        private int inData_length;

        private void ReSize_visual_buffers(int length)
        {
            //MessageBox.Show(string.Format("{0}", demodulation_functions.maxFFT));
            Array.Resize(ref avering_buffer, length);
            Array.Resize(ref visual_data, length);
            Array.Resize(ref xAxes, length);
            Array.Clear(avering_buffer, 0, length);
        }

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
            toolStripStatusLabel1.Text = "Стан: Працюю без помилок";
            numericUpDown_fftAveragingValue.Value = demodulation_functions.fftAveragingValue;
            exponentiationLevel.Value = demodulation_functions.modulation_multiplicity;
            exponentiationLevel.Increment = exponentiationLevel.Value;
            if (Quadrature_AM_demodulatorSPARKInterface.calculate_parametrs_bool) { calculate_parametrs.Checked = true; } else { calculate_parametrs.Checked = false; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.INPUT) { FFT_data_display.SelectedIndex = 0; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.EXPONENT) { FFT_data_display.SelectedIndex = 1; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.DETECTED) { FFT_data_display.SelectedIndex = 2; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.SHIFTING) { FFT_data_display.SelectedIndex = 3; }
            if (demodulation_functions.display == Demodulator.FFT_data_display.FILTERING) { FFT_data_display.SelectedIndex = 4; }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (comboBoxFFT.SelectedIndex == 0) { if (inData_length > 256) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 256; } }
            if (comboBoxFFT.SelectedIndex == 1) { if (inData_length > 512) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 512; } }
            if (comboBoxFFT.SelectedIndex == 2) { if (inData_length > 1024) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 1024; } }
            if (comboBoxFFT.SelectedIndex == 3) { if (inData_length > 2048) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 2048; } }
            if (comboBoxFFT.SelectedIndex == 4) { if (inData_length > 4096) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 4096; } }
            if (comboBoxFFT.SelectedIndex == 5) { if (inData_length > 8192) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 8192; } }
            if (comboBoxFFT.SelectedIndex == 6) { if (inData_length > 16384) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 16384; } }
            if (comboBoxFFT.SelectedIndex == 7) { if (inData_length > 32768) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 32768; } }
            if (comboBoxFFT.SelectedIndex == 8) { if (inData_length > 65536) { demodulation_functions.maxFFT = inData_length; } else { demodulation_functions.maxFFT = 65536; } }
            MitovScope.Channels[0].Data.Clear();
            toolStripStatusLabel1.Text = string.Format("Стан: Порядок ШПФ змінено на {0}", demodulation_functions.maxFFT);
            ReSize_visual_buffers(demodulation_functions.maxFFT);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (demodulation_functions.display == Demodulator.FFT_data_display.EXPONENT) { exponentiationLevel.Enabled = true; } else { exponentiationLevel.Enabled = false; }
                toolStripProgressBar1.Value = averingRepeat * 100 / demodulation_functions.fftAveragingValue;
                toolStripStatusLabel.Text = string.Format("{0} %", toolStripProgressBar1.Value);
                BW_label.Text = "Смуга фільтрації:   " + (BW / 1000.0d) + " кГц";
                toolStripStatusLabel1.Text = string.Format("Стан: Працює без збоїв");
            }
            catch
            {
                toolStripStatusLabel1.Text = "Стан: ШПФ не проведено :(";
            }

            Speed_label.Text = "Символьна швидкість: " + demodulation_functions.realSpeedPosition + " Бод";
            T_label.Text = "Період маніпуляції:  " + (1 / demodulation_functions.realSpeedPosition * 1000000.0d) + " мкс";
            F_label.Text = "Центральна частота:  " + (demodulation_functions.realCentralFrequencyPosition / 1000.0d) + " кГц";
            deltaF_label.Text = "Відхилення від центру:  " + ((demodulation_functions.realCentralFrequencyPosition - demodulation_functions.SR / 2) / 1000.0d) + " кГц";
            BW_label.Text = "Смуга фільтрації:  " + demodulation_functions.FilterBandwich + " кГц";
        }
        public void displayFFT(int length)
        {
            inData_length = length;
            length = length / 4;
            try
            {
                //visual_data = new Complex[demodulation_functions.maxFFT];
                array_for_FFT = new FFT_array_creating();
                array_for_FFT.create(ref visual_data, demodulation_functions.display, length, demodulation_functions.maxFFT);
                visual_data = Fft.fft(visual_data);
                if (demodulation_functions.display != Demodulator.FFT_data_display.DETECTED) { visual_data = Fft.nfft(visual_data); } else { }
                for (int i = 0; i < demodulation_functions.maxFFT; i++)
                {
                    avering_buffer[i] = (avering_buffer[i] + visual_data[i].Magnitude);
                }
                averingRepeat++;
                if (averingRepeat >= demodulation_functions.fftAveragingValue)
                {
                    averingRepeat = 0;
                    for (int i = 0; i < visual_data.Length; i++)
                    {
                        xAxes[i] = (float)(i * SR / demodulation_functions.maxFFT);
                        outFFTdata[i] = (float)(10 * Math.Log((avering_buffer[i] / demodulation_functions.fftAveragingValue) * fNormolize, 10));
                    }
                    MitovScope.Channels[0].Data.SetXYData(xAxes, outFFTdata);
                    Array.Clear(avering_buffer, 0, demodulation_functions.maxFFT);
                    Array.Clear(outFFTdata, 0, demodulation_functions.maxFFT);
                }
            }
            catch (Exception)
            {
                toolStripStatusLabel1.Text = "Стан: Trouble with averingFFT :(";
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            demodulation_functions.fftAveragingValue = (int)numericUpDown_fftAveragingValue.Value;
            toolStripStatusLabel1.Text = string.Format("Стан: Усереднення ШПФ змінено на {0}", demodulation_functions.fftAveragingValue);
        }

        private void calculate_parametrs_CheckedChanged(object sender, EventArgs e)
        {
            Quadrature_AM_demodulatorSPARKInterface.calculate_parametrs_bool = calculate_parametrs.Checked;
        }

        private void FFT_data_display_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FFT_data_display.SelectedIndex == 0) { demodulation_functions.display = Demodulator.FFT_data_display.INPUT; }
            if (FFT_data_display.SelectedIndex == 1) { demodulation_functions.display = Demodulator.FFT_data_display.EXPONENT; }
            if (FFT_data_display.SelectedIndex == 2) { demodulation_functions.display = Demodulator.FFT_data_display.DETECTED; }
            if (FFT_data_display.SelectedIndex == 3) { demodulation_functions.display = Demodulator.FFT_data_display.SHIFTING; }
            if (FFT_data_display.SelectedIndex == 4) { demodulation_functions.display = Demodulator.FFT_data_display.FILTERING; }
        }

        private void exponentiationLevel_ValueChanged(object sender, EventArgs e)
        {
            exponentiationLevel.Increment = exponentiationLevel.Value;
            demodulation_functions.modulation_multiplicity = (int)exponentiationLevel.Value;
        }
    }

    /// <summary>Клас, що реалізує функцію заповнення комплексного масиву даними для відображення</summary>
    public class FFT_array_creating
    {
        /// <summary> Функція зоповнення комплексного масиву </summary>
        /// <param name="visual_data">Посилання на комплексний масив, який буде використаний ШПФ</param>
        /// <param name="data_type">Тип даних, над якими буде проводитися ШПФ</param>
        /// <param name="length">Довжина I = Q відліків, над якими буде проводитися ШПФ </param>
        public void create (ref Complex[] visual_data, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
        {
            switch (data_type)
            {
                case Demodulator.FFT_data_display.SHIFTING:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            for (int i = 0; i < length; i++)
                                visual_data[k] = new Complex(Demodulator.IQ_shifted.iq[k].i, Demodulator.IQ_shifted.iq[k].q);
                        }
                    }
                    break;
                case Demodulator.FFT_data_display.EXPONENT:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            visual_data[k] = new Complex(Demodulator.IQ_elevated.iq[k].i, Demodulator.IQ_elevated.iq[k].q);
                        }
                    }
                    break;
                case Demodulator.FFT_data_display.DETECTED:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            visual_data[k] = new Complex(Demodulator.IQ_detected.iq[k].i, Demodulator.IQ_detected.iq[k].q);
                        }
                    }
                    break;
                case Demodulator.FFT_data_display.FILTERING:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            visual_data[k] = new Complex(Demodulator.IQ_filtered.iq[k].i, Demodulator.IQ_filtered.iq[k].q);
                        }
                    }
                    break;
                case Demodulator.FFT_data_display.INPUT:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            visual_data[k] = new Complex(Demodulator.IQ_inData.iq[k].i, Demodulator.IQ_inData.iq[k].q);
                        }
                    }
                    break;
                default:
                    break;
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual_data[k] = new Complex(0, 0);
            }
        }
    }
}