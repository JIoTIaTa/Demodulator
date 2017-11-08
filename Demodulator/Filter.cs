using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace demodulation
{
    public sealed class Filter
    {
        private TWindowType FIR_WindowType = TWindowType.SINC; // тип вікна фільтра
        Filter_Math FIR;
        public sIQData IQ_inData, IQ_outData, IQ_remainded;
        private int IQ_inData_length;
        private double SampleRate = 0.0d;
        private double Bandwich = 0.0d;
        private float FIR_beta = 3.2f; // коефіціент БЕТА фільтра
        private float[] filterCoefficients;
        int filterOrder = 101;
        public string warningMessage = "Стан: Працює без збоїв";
        public Filter()
        {
            FIR = new Filter_Math();
            IQ_outData.bytes = new byte[IQ_inData_length * 4];
            filterCoefficients = new float[filterOrder];
            IQ_remainded.bytes = new byte[filterOrder * 4];
        }
        public void configFilter(int inData_length, double SampleRate, double Bandwich, TWindowType WindowType, float Beta_coef)
        {
            try
            {
                Array.Resize(ref IQ_outData.bytes, inData_length);
                Array.Resize(ref filterCoefficients, this.filterOrder);
                Array.Resize(ref IQ_remainded.bytes, this.filterOrder * 4);
                IQ_inData_length = inData_length / 4;
                this.SampleRate = SampleRate;
                this.Bandwich = Bandwich;
                FIR_WindowType = WindowType;
                FIR_beta = Beta_coef;
                float BW = (float)(Bandwich / SampleRate);
                filterCoefficients = FIR.BasicFIR(this.filterOrder, TPassTypeName.LPF, BW, 0, FIR_WindowType, FIR_beta, 0.0f);
            }
            catch (Exception exception)
            {
                warningMessage = string.Format("{0}.{1}: {2}", exception.Source, exception.TargetSite, exception.Message);
            }
        }
        /// <summary>Функція фільтрації</summary>
        public  byte[] filtering(byte[] inData)
        {           
            try
            {
                IQ_inData.bytes = inData;
                int size = IQ_inData_length;
                for (int j = 0; j < size; j++)
                {
                    iqf _sum;
                    _sum.i = 0;
                    _sum.q = 0;
                    for (int i = 0; i < filterOrder; i++)
                    {
                        int a = j - i;
                        if (a >= 0)
                        {
                            _sum.i += IQ_inData.iq[a].i * filterCoefficients[i];
                            _sum.q += IQ_inData.iq[a].q * filterCoefficients[i];
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
                for (int j = 0; j < filterOrder; j++)
                {
                    IQ_remainded.iq[j].i = IQ_inData.iq[IQ_inData_length - j - 1].i;
                    IQ_remainded.iq[j].q = IQ_inData.iq[IQ_inData_length - j - 1].q;
                }
            }
            catch (Exception exception)
            {
                warningMessage = string.Format("{0}.{1}: {2}", exception.Source, exception.TargetSite, exception.Message);
            }
            return IQ_outData.bytes;
        }
    }
}
