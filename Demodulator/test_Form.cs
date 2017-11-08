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
    public partial class test_Form : Form
    {
        public test_Form()
        {
            InitializeComponent();
        }
        private sIQData IQ_data;
        
       
        int averingRepeat = 0;
        private double fNormolize = 1d / 4294967296; // коефициент нормализации сигнала

        [DllImport(@"..\\..\\data\\CUDA_FFT.dll")]
        public static extern int deviceFFT(ref Complex inData, ref Complex outData, int FFT_deep, int device_number);
        [DllImport(@"..\\..\\data\\CUDA_FFT.dll")]
        public static extern int FFT_centering(ref Complex inData, ref Complex outData, int FFT_deep, int device_number);

        public void display(byte[] data)
        {
            Complex[] visual_data = new Complex[65536];
            double[] avering_buffer = new double[65536];
            IQ_data.bytes = data;
            try
            {
                IQ_data.bytes = data;
                if (data.Length / 4 <= 65536)
                {
                    for (int k = 0; k < IQ_data.bytes.Length / 4; k++)
                    {
                        visual_data[k] = new Complex(IQ_data.iq[k].i, IQ_data.iq[k].q);
                    }
                    for (int k = IQ_data.bytes.Length / 4; k < 65536; k++)
                    {
                        visual_data[k] = new Complex(0, 0); 
                    }
                }
                else
                {
                    for (int k = 0; k < 65536; k++)
                    {
                        visual_data[k] = new Complex(IQ_data.iq[k].i, IQ_data.iq[k].q);
                    }
                }
                int Error = 999;
                Error = deviceFFT(ref visual_data[0], ref visual_data[0], 65536, 0);
                Error = FFT_centering(ref visual_data[0], ref visual_data[0], 65536, 0);
                for (int i = 0; i < 65536; i++)
                {
                    avering_buffer[i] = (avering_buffer[i] + visual_data[i].Magnitude);
                }
                Array.Clear(visual_data, 0, 65536);
                averingRepeat++;
                if (averingRepeat >=4)
                {
                    RealBuffer out_FFT_Data = new RealBuffer(65536);
                    averingRepeat = 0;
                    for (int i = 0; i <65536; i++)
                    {
                        out_FFT_Data[i] = (float)(10 * Math.Log((avering_buffer[i] / 4) * fNormolize, 10));
                    }
                    try
                    {
                        MitovScope.XAxis.AdditionalAxes[0].Axis.Min.Tick.Value = 0;
                        MitovScope.XAxis.AdditionalAxes[0].Axis.Max.Tick.Value = 65000000;
                        genericReal_FFT.SendData(out_FFT_Data);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    Array.Clear(avering_buffer, 0, 65536);
                    //Array.Clear(outFFTdata, 0, dem_functions.maxFFT);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("{0}.{1}: {2}", exception.Source, exception.TargetSite, exception.Message));
                throw;
            }
        }
    }
}
