using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;
using FFT;
using System.Threading.Tasks;

//using Filter;

namespace demodulation
{

    public partial class DemodulatorVisual_Form : Form
    {
        public float beginBW = 0.0f; // для порівняння і конфігурації фільтра
        public float BW = 0.0f;
        public Demodulator dem_functions;
        VisuaslFactory visualData;
        private double fNormolize = 1d / 4294967296; // коефициент нормализации сигнала
        private double SR = 0.0d; // частота дискретизации
        private double FFIR = 0.0d;// реальная частота сигнала
        private double bw = 0.0d;//полоса пропускания
        private int N;//порядок ШПФ
        private float ACHAvering;
        private float SignalAvering;
        Complex[] visual_data = new Complex[65536];
        double[] avering_buffer = new double[65536];
        public float[] xAxes = new float[65536]; // значення осі Х ШТП
        public float[] outFFTdata = new float[65536]; // значення осі У ШПФ
        int averingRepeat = 0;
        private int inData_length;

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
            if (dem_functions.maxFFT == 256) { comboBoxFFT.SelectedIndex = 0; }
            if (dem_functions.maxFFT == 512) { comboBoxFFT.SelectedIndex = 1; }
            if (dem_functions.maxFFT == 1024) { comboBoxFFT.SelectedIndex = 2; }
            if (dem_functions.maxFFT == 2048) { comboBoxFFT.SelectedIndex = 3; }
            if (dem_functions.maxFFT == 4096) { comboBoxFFT.SelectedIndex = 4; }
            if (dem_functions.maxFFT == 8192) { comboBoxFFT.SelectedIndex = 5; }
            if (dem_functions.maxFFT == 16384) { comboBoxFFT.SelectedIndex = 6; }
            if (dem_functions.maxFFT == 32768) { comboBoxFFT.SelectedIndex = 7; }
            if (dem_functions.maxFFT == 65536) { comboBoxFFT.SelectedIndex = 8; }
            SR = dem_functions.SR;
            toolStripStatusLabel1.Text = "Стан: Працюю без помилок";
            numericUpDown_fftAveragingValue.Value = dem_functions.fftAveragingValue;
            exponentiationLevel.Value = dem_functions.modulation_multiplicity;
            exponentiationLevel.Increment = exponentiationLevel.Value;
            if (Quadrature_AM_demodulatorSPARKInterface.calculate_parametrs_bool) { calculate_parametrs.Checked = true; } else { calculate_parametrs.Checked = false; }
            if (dem_functions.display == demodulation.FFT_data_display.INPUT) { FFT_data_display.SelectedIndex = 0; }
            if (dem_functions.display == demodulation.FFT_data_display.EXPONENT) { FFT_data_display.SelectedIndex = 1; }
            if (dem_functions.display == demodulation.FFT_data_display.DETECTED) { FFT_data_display.SelectedIndex = 2; }
            if (dem_functions.display == demodulation.FFT_data_display.SHIFTING) { FFT_data_display.SelectedIndex = 3; }
            if (dem_functions.display == demodulation.FFT_data_display.FILTERING) { FFT_data_display.SelectedIndex = 4; }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (comboBoxFFT.SelectedIndex == 0) { if (inData_length > 256) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 256; } }
            if (comboBoxFFT.SelectedIndex == 1) { if (inData_length > 512) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 512; } }
            if (comboBoxFFT.SelectedIndex == 2) { if (inData_length > 1024) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 1024; } }
            if (comboBoxFFT.SelectedIndex == 3) { if (inData_length > 2048) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 2048; } }
            if (comboBoxFFT.SelectedIndex == 4) { if (inData_length > 4096) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 4096; } }
            if (comboBoxFFT.SelectedIndex == 5) { if (inData_length > 8192) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 8192; } }
            if (comboBoxFFT.SelectedIndex == 6) { if (inData_length > 16384) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 16384; } }
            if (comboBoxFFT.SelectedIndex == 7) { if (inData_length > 32768) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 32768; } }
            if (comboBoxFFT.SelectedIndex == 8) { if (inData_length > 65536) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 65536; } }
            MitovScope.Channels[0].Data.Clear();
            toolStripStatusLabel1.Text = string.Format("Стан: Порядок ШПФ змінено на {0}", dem_functions.maxFFT);
            ReSize_visual_buffers(dem_functions.maxFFT);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (dem_functions.display == demodulation.FFT_data_display.EXPONENT) { exponentiationLevel.Enabled = true; } else { exponentiationLevel.Enabled = false; }
                toolStripProgressBar1.Value = averingRepeat * 100 / dem_functions.fftAveragingValue;
                toolStripStatusLabel.Text = string.Format("{0} %", toolStripProgressBar1.Value);
                toolStripStatusLabel1.Text = Demodulator.warningMessage;                
                writter_checkBox.Checked = dem_functions.write;
                
            }
            catch
            {
                toolStripStatusLabel1.Text = "Стан: ШПФ не проведено :(";
            }

            Speed_label.Text = string.Format("Символьна швидкість:  {0:0.00} кГц", dem_functions.speedFrequency / 1000.0d);
            T_label.Text = string.Format("Період маніпуляції:  {0:0.00} мкс", (1 / dem_functions.speedFrequency * 1000000.0d));
            F_label.Text = string.Format("Центральна частота:  {0:0.00} кГц", (dem_functions.centralFrequency / 1000.0d));
            deltaF_label.Text = string.Format("Відхилення від центру:  {0:0.00} кГц",((dem_functions.centralFrequency - dem_functions.SR / 2) / 1000.0d));
            BPS_label.Text = string.Format("Відліків на символ:  {0:0.00}", dem_functions.SymbolsPerSapmle);
            deltaMS_I_label.Text = string.Format("Відхилення швидкості I:  {0}", dem_functions.speed_error_I);
            deltaMS_Q_label.Text = string.Format("Відхилення швидкості Q:  {0}", dem_functions.speed_error_Q);
            deltaMS_label.Text = string.Format("Відхилення швидкості :  {0}", dem_functions.speed_error);
            SR_label.Text = string.Format("Частота дискретизації:  {0:0.00} кГц", dem_functions.SR / 1000.0d);
            filterBW_textBox.Text = string.Format("{0:0.00}", dem_functions.FilterBandwich );
        }
       

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            dem_functions.fftAveragingValue = (int)numericUpDown_fftAveragingValue.Value;
            toolStripStatusLabel1.Text = string.Format("Стан: Усереднення ШПФ змінено на {0}", dem_functions.fftAveragingValue);
        }

        private void calculate_parametrs_CheckedChanged(object sender, EventArgs e)
        {
            Quadrature_AM_demodulatorSPARKInterface.calculate_parametrs_bool = calculate_parametrs.Checked;
        }

        private void FFT_data_display_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FFT_data_display.SelectedIndex == 0) { dem_functions.display = demodulation.FFT_data_display.INPUT; }
            if (FFT_data_display.SelectedIndex == 1) { dem_functions.display = demodulation.FFT_data_display.EXPONENT; }
            if (FFT_data_display.SelectedIndex == 2) { dem_functions.display = demodulation.FFT_data_display.DETECTED; }
            if (FFT_data_display.SelectedIndex == 3) { dem_functions.display = demodulation.FFT_data_display.SHIFTING; }
            if (FFT_data_display.SelectedIndex == 4) { dem_functions.display = demodulation.FFT_data_display.FILTERING; }
        }

        private void exponentiationLevel_ValueChanged(object sender, EventArgs e)
        {
            exponentiationLevel.Increment = exponentiationLevel.Value;
            dem_functions.modulation_multiplicity = (int)exponentiationLevel.Value;
        }


        private void ReSize_visual_buffers(int length)
        {
            Array.Resize(ref avering_buffer, length);
            Array.Resize(ref visual_data, length);
            Array.Resize(ref xAxes, length);
            Array.Clear(avering_buffer, 0, length);
        }
        public void displayFFT()
        {            
            try
            {
                switch (dem_functions.display)
                {
                    case demodulation.FFT_data_display.SHIFTING: inData_length = Demodulator.IQ_shifted.bytes.Length;
                        break;
                    case demodulation.FFT_data_display.EXPONENT: inData_length = Demodulator.IQ_elevated.bytes.Length;
                        break;
                    case demodulation.FFT_data_display.DETECTED: inData_length = Demodulator.IQ_detected.bytes.Length;
                        break;
                    case demodulation.FFT_data_display.FILTERING: inData_length = Demodulator.IQ_filtered.bytes.Length;
                        break;
                    case demodulation.FFT_data_display.INPUT: inData_length = Demodulator.IQ_inData.bytes.Length;
                        break;
                    default:
                        break;
                }
                visualData = new VisuaslFactory();
                visualData.CreateVisual(ref visual_data, dem_functions.display, dem_functions.maxFFT);
                visual_data = Fft.fft(visual_data);
                if (dem_functions.display != demodulation.FFT_data_display.DETECTED) { visual_data = Fft.nfft(visual_data); } else { }
                for (int i = 0; i < dem_functions.maxFFT; i++)
                {
                    avering_buffer[i] = (avering_buffer[i] + visual_data[i].Magnitude);
                }
                Array.Clear(visual_data, 0, dem_functions.maxFFT);
                averingRepeat++;
                if (averingRepeat >= dem_functions.fftAveragingValue)
                {
                    averingRepeat = 0;
                    for (int i = 0; i < visual_data.Length; i++)
                    {
                        xAxes[i] = (float)(i * SR / dem_functions.maxFFT);
                        outFFTdata[i] = (float)(10 * Math.Log((avering_buffer[i] / dem_functions.fftAveragingValue) * fNormolize, 10));
                    }
                    MitovScope.Channels[0].Data.SetXYData(xAxes, outFFTdata);
                    Array.Clear(avering_buffer, 0, dem_functions.maxFFT);
                    Array.Clear(outFFTdata, 0, dem_functions.maxFFT);
                }
            }
            catch (Exception)
            {
                toolStripStatusLabel1.Text = "Стан: Trouble with averingFFT :(";
            }
        }

        private void writter_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.write = writter_checkBox.Checked;
        }

        private void filterBW_textBox_TextChanged(object sender, EventArgs e)
        {
            dem_functions.FilterBandwich = (float)Convert.ToDouble(filterBW_textBox.Text);
        }

        private void MScorrect_textBox_TextChanged(object sender, EventArgs e)
        {
            dem_functions.MS_correct = (float)Convert.ToDouble(MScorrect_textBox.Text);
        }
    }    
}
