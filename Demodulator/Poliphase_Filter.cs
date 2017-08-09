using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace demodulation
{
    class Poliphase_Filter
    {
        private int IQ_inData_length;
        private int IQ_interpolated_length;
        public int IQ_outData_length;
        private double SampleRate = 0.0d;
        private double Bandwich = 0.0d;
        private int decimation_coef;
        private int interpolation_coef;
        private sIQData IQ_inData, IQ_outData, IQ_remainded, IQ_interpolated;
        private float[] filterCoefficients;
        private TWindowType FIR_WindowType = TWindowType.SINC; // тип вікна фільтра
        private float FIR_beta = 3.2f; // коефіціент БЕТА фільтра
        int filterOrder = 2;

        public Poliphase_Filter()
        {
            IQ_inData.bytes = new byte[IQ_inData_length * 4];
            IQ_interpolated.bytes = new byte[IQ_inData_length * 4];
            IQ_outData.bytes = new byte[IQ_inData_length * 4];
            IQ_remainded.bytes = new byte[filterOrder * 4];
            filterCoefficients = new float[filterOrder];
        }
        public Poliphase_Filter(int inData_Length, double SampleRate, double Bandwich)
        {
            IQ_inData_length = inData_Length / 4;
            this.SampleRate = SampleRate;
            this.Bandwich = Bandwich;
            IQ_inData.bytes = new byte[IQ_inData_length * 4];
            IQ_interpolated.bytes = new byte[IQ_inData_length * 4];
            IQ_outData.bytes = new byte[IQ_inData_length * 4];
            IQ_remainded.bytes = new byte[filterOrder * 4];
            filterCoefficients = new float[filterOrder];
        }
        public void configFilter(byte[] inData, double SampleRate, double Bandwich, int filterOrder, TWindowType WindowType, float Beta_coef)
        {
            Filter_Math FIR = new Filter_Math();
            IQ_inData_length = inData.Length / 4;
            IQ_inData.bytes = inData;
            this.SampleRate = SampleRate;
            this.Bandwich = Bandwich;
            FIR_WindowType = WindowType;
            FIR_beta = Beta_coef;
            this.filterOrder = filterOrder;
            float BW = (float)(Bandwich / SampleRate);
            calc_DI_coef();
            Array.Resize(ref filterCoefficients, this.filterOrder);
            filterCoefficients = FIR.BasicFIR(this.filterOrder, TPassTypeName.LPF, BW, 0, FIR_WindowType, FIR_beta, 0.0f);
        }
        private void calc_DI_coef()
        {
            double calcDI_coef = 0.0d;
            double realDI_coef = Bandwich / SampleRate;
            double decimation_coef_d = SampleRate;
            while (decimation_coef_d > 10) { decimation_coef_d /= 10; }
            decimation_coef = (int)Math.Round(decimation_coef_d);
            decimation_coef *= 2;
            interpolation_coef = decimation_coef - 1;
            do
            {
                calcDI_coef = (interpolation_coef * 1.0d) / (decimation_coef * 1.0d);
                if (interpolation_coef != 0) { interpolation_coef--; } else { decimation_coef *= 2; interpolation_coef = decimation_coef - 1; }
            } while (Math.Abs(realDI_coef - calcDI_coef) >= 0.01);
            IQ_interpolated_length = IQ_inData_length * interpolation_coef;
            IQ_outData_length = IQ_interpolated_length / decimation_coef;
            filterOrder *= interpolation_coef;
        }


        private void interpolation_buffers()
        {
            Array.Resize(ref IQ_interpolated.bytes, IQ_interpolated_length * 4);
            Array.Clear(IQ_interpolated.bytes, 0, IQ_interpolated.bytes.Length);
        }
        private void decimation_buffers()
        {
            Array.Resize(ref IQ_outData.bytes, IQ_outData_length * 4);
            Array.Clear(IQ_outData.bytes, 0, IQ_outData.bytes.Length);
        }


        public byte[] filtering()
        {
            int size = 0;
            interpolation_buffers();
            try
            {
                ///////////////////////////////////// Фаза інтерполяції /////////////////////////////////////
                size = IQ_interpolated_length;
                int itteration = 0;
                int filter_coef_shift = 0;
                int inData_coef_shift = 0;
                for (int j = 0; j < size; j++)
                {
                    iqf _sum;
                    _sum.i = 0;
                    _sum.q = 0;
                    for (int i = 0; i < filterOrder / interpolation_coef; i++)
                    {
                        int a = inData_coef_shift - i;
                        int b = filter_coef_shift + i * interpolation_coef;
                        if (a >= 0)
                        {
                            _sum.i += IQ_inData.iq[a].i * filterCoefficients[b];
                            _sum.q += IQ_inData.iq[a].q * filterCoefficients[b];
                        }
                        else
                        {
                            _sum.i += IQ_remainded.iq[Math.Abs(a + 1)].i * filterCoefficients[b];
                            _sum.q += IQ_remainded.iq[Math.Abs(a + 1)].q * filterCoefficients[b];
                        }
                    }
                    IQ_interpolated.iq[j].i = (short)(_sum.i * interpolation_coef * 1f);
                    IQ_interpolated.iq[j].q = (short)(_sum.q * interpolation_coef * 1f);
                    if (itteration < interpolation_coef) { itteration++; filter_coef_shift -= 1; } else { itteration = 1; filter_coef_shift = interpolation_coef - 1; ; inData_coef_shift += 1; }
                    if (j == 0) { itteration = 1; inData_coef_shift += 1; filter_coef_shift = interpolation_coef - 1; }
                }

                Array.Resize(ref IQ_remainded.bytes, filterOrder);
                for (int j = 0; j < IQ_remainded.bytes.Length / 4; j++)
                {
                    IQ_remainded.iq[j].i = IQ_interpolated.iq[IQ_interpolated_length - j - 1].i;
                    IQ_remainded.iq[j].q = IQ_interpolated.iq[IQ_interpolated_length - j - 1].q;
                }
                ///////////////////////////////////// Фаза децимації /////////////////////////////////////
                decimation_buffers();
                size = IQ_outData_length;
                for (int j = 0; j < size; j++)
                {
                    iqf _sum;
                    _sum.i = 0;
                    _sum.q = 0;
                    for (int i = 0; i < filterOrder; i++)
                    {
                        int a = j * decimation_coef - i;
                        if (a >= 0)
                        {
                            _sum.i += IQ_interpolated.iq[a].i * filterCoefficients[i];
                            _sum.q += IQ_interpolated.iq[a].q * filterCoefficients[i];
                        }
                        else
                        {
                            _sum.i += IQ_remainded.iq[Math.Abs(a + 1)].i * filterCoefficients[i];
                            _sum.q += IQ_remainded.iq[Math.Abs(a + 1)].q * filterCoefficients[i];
                        }
                    }
                    IQ_outData.iq[j].i = (short)(_sum.i);
                    IQ_outData.iq[j].q = (short)(_sum.q);
                }
                Array.Resize(ref IQ_remainded.bytes, (filterOrder / interpolation_coef) - 1);
                for (int j = 0; j < filterOrder; j++)
                {
                    IQ_remainded.iq[j].i = IQ_outData.iq[IQ_outData_length - j - 1].i;
                    IQ_remainded.iq[j].q = IQ_outData.iq[IQ_outData_length - j - 1].q;
                }
            }
            catch (Exception) { }
            return IQ_outData.bytes;
        }
    }
}
