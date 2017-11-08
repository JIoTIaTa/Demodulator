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
using System.Runtime.InteropServices;
using Mitov.PlotLab;
using Mitov.SignalLab;
using System.Threading;

//using Filter;

namespace demodulation
{

    public partial class DemodulatorVisual_Form : Form
    {
        public float beginBW = 0.0f; // для порівняння і конфігурації фільтра
        public float BW = 0.0f;
        public Demodulator dem_functions;
        VisuaslFactory_FFT visualBuffers;
        private double fNormolize = 1d / 4294967296; // коефициент нормализации сигнала
        private double SR = 0.0d; // частота дискретизации
        Complex[] visual_data = new Complex[65536];
        double[] avering_buffer = new double[65536];
        public float[] outFFTdata = new float[65536]; // значення осі У ШПФ
        int averingRepeat = 0;
        private int inData_length;
        private Constellation Constellation_VisualForm;
        private Exponent Exponent_VisualForm;
                

        [DllImport(@"..\\..\\data\\CUDA_FFT.dll")]
        public static extern int deviceFFT(ref Complex inData, ref Complex outData,  int FFT_deep, int device_number);
        [DllImport(@"..\\..\\data\\CUDA_FFT.dll")]
        public static extern int FFT_centering(ref Complex inData, ref Complex outData, int FFT_deep, int device_number);
        public DemodulatorVisual_Form()
        {
            InitializeComponent();
            Top = 0;
            Left = 0;
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void VisualForm_Load(object sender, EventArgs e)
        {

            LoadVisualForm();
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void LoadVisualForm()
        {
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
            if (Quadrature_AM_demodulatorSPARKInterface.calculate_parametrs_bool) { calculate_parametrs.Checked = true; } else { calculate_parametrs.Checked = false; }
            if (dem_functions.display == demodulation.FFT_data_display.INPUT) { FFT_data_display.SelectedIndex = 0; }
            if (dem_functions.display == demodulation.FFT_data_display.SHIFTING) { FFT_data_display.SelectedIndex = 1; }
            if (dem_functions.display == demodulation.FFT_data_display.FILTERING) { FFT_data_display.SelectedIndex = 2; }

            if (dem_functions.filter_type == Filter_type.simple ) { radioButton_simpleFilter.Checked = true; ; }
            if (dem_functions.filter_type == Filter_type.poliphase ) { radioButton_poliphaseFilter.Checked = true; ; }

            typeWindow.SelectedIndex = (int)dem_functions.FIR_WindowType;
            visualBuffers = new VisuaslFactory_FFT() { dem_functions = dem_functions };
            timer_FFT.Enabled = dem_functions.display_FFT;
            checkBox_FFT.Checked = dem_functions.display_FFT;
            //Виклик форми відображення сигнального сузір'я//
            if (Constellation_VisualForm == null || Constellation_VisualForm.IsDisposed)
            {
                Constellation_VisualForm = new Constellation() { dem_functions = dem_functions };
                Constellation_VisualForm.Show();
            }
            //Виклик форми відображення піднесення до ступеня//
            if (Exponent_VisualForm== null || Exponent_VisualForm.IsDisposed)
            {
                Exponent_VisualForm = new Exponent() { dem_functions = dem_functions };
                Exponent_VisualForm.Show();
            }
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                toolStripStatusLabel1.Text = dem_functions.warningMessage;
                if (dem_functions.warningMessage != "Стан: Працює без збоїв") { BackColor = Color.Red; } else { BackColor = Color.White; } 
                Speed_label.Text = string.Format("Символьна швидкість:  {0:0.00} кГц", dem_functions.speedFrequency / 1000.0d);
                T_label.Text = string.Format("Період маніпуляції:  {0:0.00} мкс", (1 / dem_functions.speedFrequency * 1000000.0d));
                F_label.Text = string.Format("Центральна частота:  {0:0.00} кГц", (dem_functions.centralFrequency / 1000.0d));
                deltaF_label.Text = string.Format("Відхилення:  {0:0.00} кГц", ((dem_functions.centralFrequency - dem_functions.F) / 1000.0d));
                BPS_label.Text = string.Format("Відліків на символ:  {0:0.00}", dem_functions.SymbolsPerSapmle);
                inputSR_label.Text = string.Format("Частота дискретизації на вході:  {0:0.00} кГц", dem_functions.SR / 1000.0d);
                afterFilterSR_label.Text = string.Format("Частота дискретизації після децимації:  {0:0.00} кГц", dem_functions.SR_after_filter / 1000.0d);
                numericUpDown_BW.Value = Convert.ToDecimal(dem_functions.FilterBandwich);
                checkBox_FFT.Checked = dem_functions.display_FFT;
                timer_FFT.Enabled = dem_functions.display_FFT;
            }
            catch (Exception exception)
            {
                dem_functions.warningMessage = string.Format("{0}.{1}: {2}", exception.Source, exception.TargetSite, exception.Message);
            }

        }

        ////////////////////////////******************************************************************************////////////////////////////
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            dem_functions.fftAveragingValue = (int)numericUpDown_fftAveragingValue.Value;
            toolStripStatusLabel1.Text = string.Format("Стан: Усереднення ШПФ змінено на {0}", dem_functions.fftAveragingValue);
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void calculate_parametrs_CheckedChanged(object sender, EventArgs e)
        {
            Quadrature_AM_demodulatorSPARKInterface.calculate_parametrs_bool = calculate_parametrs.Checked;
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void FFT_data_display_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FFT_data_display.SelectedIndex == 0) { dem_functions.display = demodulation.FFT_data_display.INPUT; }
            if (FFT_data_display.SelectedIndex == 1) { dem_functions.display = demodulation.FFT_data_display.SHIFTING; }
            if (FFT_data_display.SelectedIndex == 2) { dem_functions.display = demodulation.FFT_data_display.FILTERING; }
        }      
        ////////////////////////////******************************************************************************////////////////////////////
        public void new_length(int length)
        {
            inData_length = length;
            if (inData_length <= 65536) { dem_functions.maxFFT = inData_length; } else { dem_functions.maxFFT = 65536; }
            ReSize_visual_buffers(dem_functions.maxFFT);
            switch (dem_functions.maxFFT)
            {
                case 256:
                    comboBoxFFT.SelectedIndex = 0;
                    break;
                case 512:
                    comboBoxFFT.SelectedIndex = 1;
                    break;
                case 1024:
                    comboBoxFFT.SelectedIndex = 2;
                    break;
                case 2048:
                    comboBoxFFT.SelectedIndex = 3;
                    break;
                case 4096:
                    comboBoxFFT.SelectedIndex = 4;
                    break;
                case 8192:
                    comboBoxFFT.SelectedIndex = 5;
                    break;
                case 16384:
                    comboBoxFFT.SelectedIndex = 6;
                    break;
                case 32768:
                    comboBoxFFT.SelectedIndex = 7;
                    break;
                case 65536:
                    comboBoxFFT.SelectedIndex = 8;
                    break;
                default:
                    comboBoxFFT.Text = string.Format("{0}", inData_length);
                    break;
            }
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void ReSize_visual_buffers(int length)
        {
            Array.Resize(ref avering_buffer, length);
            Array.Resize(ref visual_data, length);
            Array.Clear(avering_buffer, 0, length);
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void filterBW_textBox_TextChanged(object sender, EventArgs e)
        {
            dem_functions.FilterBandwich = Convert.ToSingle(numericUpDown_BW.Value);
        }

        ////////////////////////////******************************************************************************////////////////////////////
        private void radioButton_simpleFilter_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.filter_type = Filter_type.simple;
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void radioButton_poliphaseFilter_CheckedChanged(object sender, EventArgs e)
        {          
            dem_functions.filter_type = Filter_type.poliphase;
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void numericUpDown_central_freq_corerct_ValueChanged(object sender, EventArgs e)
        {
            dem_functions.central_freq_correct = Convert.ToSingle(numericUpDown_central_freq_corerct.Value * 1000);
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void typeWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            dem_functions.FIR_WindowType = (TWindowType)typeWindow.SelectedIndex;
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void numericUpDown_MScorrect_ValueChanged(object sender, EventArgs e)
        {
            dem_functions.MS_correct = (float)Convert.ToDouble(numericUpDown_MScorrect.Value);
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void comboBoxFFT_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxFFT.SelectedIndex)
            {
                case 0:
                    dem_functions.maxFFT = 256;                    
                    break;
                case 1:
                    dem_functions.maxFFT = 512; 
                    break;
                case 2:
                    dem_functions.maxFFT = 1024; 
                    break;
                case 3:
                   dem_functions.maxFFT = 2048; 
                    break;
                case 4:
                    dem_functions.maxFFT = 4096; 
                    break;
                case 5:
                   dem_functions.maxFFT = 8192; 
                    break;
                case 6:
                    dem_functions.maxFFT = 16384; 
                    break;
                case 7:
                    dem_functions.maxFFT = 32768; 
                    break;
                case 8:
                    dem_functions.maxFFT = 65536; 
                    break;
                default:
                    break;
            }
            MitovScope.Channels[0].Data.Clear();
            toolStripStatusLabel1.Text = string.Format("Стан: Порядок ШПФ змінено на {0}", dem_functions.maxFFT);
            ReSize_visual_buffers(dem_functions.maxFFT);
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void checkBox_FFT_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.display_FFT = checkBox_FFT.Checked;
            timer_FFT.Enabled = checkBox_FFT.Checked;
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            if (Constellation_VisualForm == null || Constellation_VisualForm.IsDisposed)
            {
                Constellation_VisualForm = new Constellation() { dem_functions = dem_functions };
                Constellation_VisualForm.Show();
            }
            else
            {
                Constellation_VisualForm.Close();
            }
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void DemodulatorVisual_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Constellation_VisualForm != null)
            {
                Constellation_VisualForm.Close();
            }
            if (Exponent_VisualForm != null)
            {
                Exponent_VisualForm.Close();
            }
        }
        ////////////////////////////******************************************************************************////////////////////////////
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                if (dem_functions.display == demodulation.FFT_data_display.FILTERING) { SR = dem_functions.SR_after_filter; } else { SR = dem_functions.SR; }
                visualBuffers.Create(ref visual_data, dem_functions.maxFFT);
                //*********************************************************************************//
                int Error = 999;
                Error = deviceFFT(ref visual_data[0], ref visual_data[0], dem_functions.maxFFT, 0);
                Error = FFT_centering(ref visual_data[0], ref visual_data[0], dem_functions.maxFFT, 0);
                //*********************************************************************************//


                for (int i = 0; i < dem_functions.maxFFT; i++)
                {
                    avering_buffer[i] = (avering_buffer[i] + visual_data[i].Magnitude);
                }
                Array.Clear(visual_data, 0, dem_functions.maxFFT);
                averingRepeat++;
                if (averingRepeat >= dem_functions.fftAveragingValue)
                {
                    RealBuffer out_FFT_Data = new RealBuffer(dem_functions.maxFFT);
                    averingRepeat = 0;
                    for (int i = 0; i < dem_functions.maxFFT; i++)
                    {
                        //xAxes[i] = (float)(i * SR / dem_functions.maxFFT);
                        //outFFTdata[i] = (float)(10 * Math.Log((avering_buffer[i] / dem_functions.fftAveragingValue) * fNormolize, 10));
                        out_FFT_Data[i] = (float)(10 * Math.Log((avering_buffer[i] / dem_functions.fftAveragingValue) * fNormolize, 10));
                    }
                    try
                    {
                        //MitovScope.Channels[0].Data.SetXYData(xAxes, outFFTdata);
                        MitovScope.XAxis.AdditionalAxes[0].Axis.Min.Tick.Value = 0;
                        MitovScope.XAxis.AdditionalAxes[0].Axis.Max.Tick.Value = SR;
                        genericReal_FFT.SendData(out_FFT_Data);
                    }
                    catch { }
                    Array.Clear(avering_buffer, 0, dem_functions.maxFFT);
                }
            }
            catch (Exception exception)
            {                
                dem_functions.warningMessage = string.Format("{0}.{1}: {2}", exception.Source, exception.TargetSite, exception.Message);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dem_functions.display_Tick = (short)numericUpDown1.Value;
            timer_FFT.Interval = dem_functions.display_Tick;
        }

        private void MitovScope_MouseMove(object sender, MouseEventArgs e)
        {
            double x, y, freq;
            MitovScope.Cursors[0].Color = Color.Red;
            MitovScope.XAxis.GetValueAt(e.X, e.Y, out x);
            try
            {
                y = MitovScope.Channels[0].GetYPointsAtValue(x)[0];
            }
            catch (Exception)
            {
                MitovScope.Cursors[0].Visible = false;
                MitovScope.Text = string.Empty;
                return;
            }
            MitovScope.XAxis.AdditionalAxes[0].Axis.GetValueAt(e.X, e.Y, out freq);
            if (freq == 0)
            {
                MitovScope.Cursors[0].Visible = false;
                MitovScope.Text = string.Empty;
                return;
            }

            label_freq_dBm.Text = string.Format("{0} kHz, {1} dBm", Math.Round(freq / 1000, 3), Math.Round(y, 0));
            if (MitovScope.Cursors[0].Visible == false)
                MitovScope.Cursors[0].Visible = true;
            MitovScope.Cursors[0].Position.X = x;            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Exponent_VisualForm == null || Exponent_VisualForm.IsDisposed)
            {
                Exponent_VisualForm = new Exponent() { dem_functions = dem_functions };
                Exponent_VisualForm.Show();
            }
            else
            {
                Exponent_VisualForm.Close();
            }
        }

        private void DemodulatorVisual_Form_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Exponent_VisualForm.WindowState = FormWindowState.Minimized;
                Constellation_VisualForm.WindowState = FormWindowState.Minimized;
            }
            if (WindowState == FormWindowState.Normal)
            {
                Exponent_VisualForm.WindowState = FormWindowState.Normal;
                Constellation_VisualForm.WindowState = FormWindowState.Normal;
            }
        }
    }    
}
