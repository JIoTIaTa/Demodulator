using Mitov.SignalLab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace demodulation
{
    public partial class Exponent : Form
    {
        public Demodulator dem_functions;
        Complex[] visual_data = new Complex[65536];
        double[] avering_buffer = new double[65536];
        int averingRepeat = 0;
        private double fNormolize = 1d / 4294967296; // коефициент нормализации сигнала

        [DllImport(@"..\\..\\data\\CUDA_FFT.dll")]
        public static extern int deviceFFT(ref Complex inData, ref Complex outData, int FFT_deep, int device_number);
        [DllImport(@"..\\..\\data\\CUDA_FFT.dll")]
        public static extern int FFT_centering(ref Complex inData, ref Complex outData, int FFT_deep, int device_number);
        public Exponent()
        {
            InitializeComponent();
            this.Top = 518;
            this.Left = 440;
        }

        private void timer_exponent_Tick(object sender, EventArgs e)
        {
            try
            {
                timer_exponent.Interval = dem_functions.display_Tick;
                switch (dem_functions.exp_display)
                {
                    case Exponent_data_display.MODULE:
                        if (dem_functions.IQ_detected.bytes.Length / 4 <= dem_functions.maxFFT)
                        {
                            for (int k = 0; k < dem_functions.IQ_detected.bytes.Length / 4; k++)
                            {
                                visual_data[k] = new Complex(dem_functions.IQ_detected.iq[k].i, dem_functions.IQ_detected.iq[k].q);
                            }
                            for (int k = dem_functions.IQ_detected.bytes.Length / 4; k < dem_functions.maxFFT; k++)
                            {
                                visual_data[k] = new Complex(0, 0);
                            }
                        }
                        else
                        {
                            for (int k = 0; k < dem_functions.maxFFT; k++)
                            {
                                visual_data[k] = new Complex(dem_functions.IQ_detected.iq[k].i, dem_functions.IQ_detected.iq[k].q);
                            }
                        }
                        break;
                    case Exponent_data_display.ELEVATE:
                        if (dem_functions.IQ_elevated.bytes.Length / 4 <= dem_functions.maxFFT)
                        {
                            for (int k = 0; k < dem_functions.IQ_elevated.bytes.Length / 4; k++)
                            {
                                visual_data[k] = new Complex(dem_functions.IQ_elevated.iq[k].i, dem_functions.IQ_elevated.iq[k].q);
                            }
                            for (int k = dem_functions.IQ_elevated.bytes.Length / 4; k < dem_functions.maxFFT; k++)
                            {
                                visual_data[k] = new Complex(0, 0);
                            }
                        }
                        else
                        {
                            for (int k = 0; k < dem_functions.maxFFT; k++)
                            {
                                visual_data[k] = new Complex(dem_functions.IQ_elevated.iq[k].i, dem_functions.IQ_elevated.iq[k].q);
                            }
                        }
                        break;
                    default:
                        break;
                }
                int Error = 999;
                Error = deviceFFT(ref visual_data[0], ref visual_data[0], dem_functions.maxFFT, 0);
                Error = FFT_centering(ref visual_data[0], ref visual_data[0], dem_functions.maxFFT, 0);

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
                        MitovScope.XAxis.AdditionalAxes[0].Axis.Max.Tick.Value = dem_functions.SR;
                        genericReal_exponent.SendData(out_FFT_Data);
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

        private void checkBox_Exponent_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.display_exponent = checkBox_Exponent.Checked;
        }

        private void Exponent_Load(object sender, EventArgs e)
        {
            checkBox_Exponent.Checked = dem_functions.display_exponent;
            switch (dem_functions.modulation_multiplicity)
            {
                case 1:
                    radioButton_moduleX.Checked = true;
                    dem_functions.exp_display = Exponent_data_display.MODULE;
                    break;
                case 2:
                    radioButton_X2.Checked = true;
                    dem_functions.exp_display = Exponent_data_display.ELEVATE;
                    break;
                case 4:
                    radioButton_X4.Checked = true;
                    dem_functions.exp_display = Exponent_data_display.ELEVATE;
                    break;
                case 8:
                    radioButton_X4.Checked = true;
                    dem_functions.exp_display = Exponent_data_display.ELEVATE;
                    break;
                default:
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            checkBox_Exponent.Checked = dem_functions.display_exponent;
            timer_exponent.Enabled = dem_functions.display_exponent;
        }

        private void radioButton_moduleX_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.exp_display = Exponent_data_display.MODULE;
            dem_functions.modulation_multiplicity = 1;
        }

        private void radioButton_X2_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.exp_display = Exponent_data_display.ELEVATE;
            dem_functions.modulation_multiplicity = 2;
        }

        private void radioButton_X4_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.exp_display = Exponent_data_display.ELEVATE;
            dem_functions.modulation_multiplicity = 4;
        }

        private void radioButton_x8_CheckedChanged(object sender, EventArgs e)
        {
            dem_functions.exp_display = Exponent_data_display.ELEVATE;
            dem_functions.modulation_multiplicity = 8;
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
    }
}
