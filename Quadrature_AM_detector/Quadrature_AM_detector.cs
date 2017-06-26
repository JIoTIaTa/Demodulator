using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cloo;
using System.Numerics;

namespace Exponentiation
{
    [StructLayout(LayoutKind.Sequential)]
    public struct iq
    {
        public short i;
        public short q;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct iqd
    {
        public double i;
        public double q;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct iqf
    {
        public float i;
        public float q;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct sIQData
    {
        [FieldOffset(0)]
        public Byte[] bytes;

        [FieldOffset(0)]
        public short[] shorts;

        [FieldOffset(0)]
        public iq[] iq;
    }

    public class Quadrature_AM_detector
    {
        enum TPassTypeName { LPF, HPF, BPF, NOTCH };
        public enum TWindowType
        {
            NONE, KAISER, SINC, HANNING, HAMMING, BLACKMAN,
            FLATTOP, BLACKMAN_HARRIS, BLACKMAN_NUTTALL, NUTTALL,
            KAISER_BESSEL, TRAPEZOID, GAUSS, SINE, TEST
        };

        [DllImport("..\\..\\data\\FIR.dll", EntryPoint = "BasicFIR", CallingConvention = CallingConvention.StdCall)]
        static extern void _FIR(ref float FIRCoeff, int numTaps, TPassTypeName PassType, float OmegaC, float BW, TWindowType WindowTyte, float WinBeta);
        public double SR = 0.0d; // частота дискретизації
        public static sIQData _inData, _outData, _bufferData, _expData, _shiftingData, _data;
        public static sIQData _testBuffer;        public long F = 0; // центральная частота
        public int login;
        public bool sendComand = false;
        private double tempI = 0;
        private double tempQ = 0;
        public bool show;
        long normalizeCoeff = 1;
        public static int x = 1; // коеф інтерполяції
        public int Count;
        public int maxFFT = 65536; // максимальний порядок ШПФ
        public int degree = 4; // кратність модуляції
        public bool busy = false;
        public int averagingValue = 2;
        public static int outDataLeght = 65536;
        public static int inDataLeght = 65536;
        byte[] expData = new byte[outDataLeght];
        byte[] bufferData = new byte[inDataLeght * x];
        byte[] bufferExpData = new byte[inDataLeght];
        byte[] shifting_data = new byte[inDataLeght];
        float[] sin_1024 = new float[1024];
        float[] cos_1024 = new float[1024];
        public double realSpeedPosition = 0; // швидкість модуляції
        public double realCentralFrequencyPosition = 0; // центральна частота
        float sin_cos_position = 0; // позиція син/кос для вибору з таблиці
        float FilterBandwich = 0; // смуга фільтрація
        int decimationCoef = 1; // коефіцінт децимації
        static int filterOrder = 50; // порядок фільтру
        float[] filterCoefficients; // коефіціенти фільтра
        byte[] data = new byte[filterOrder * 4]; // залишок старого масива - початок для нового
        public TWindowType WindowType = TWindowType.KAISER; // тип вікна фільтра
        public float beta = 3.2f; // коефіціент БЕТА фільтра
        public int N = 0;   // для сноса       


        public void sin_cos_init()
        {
            for (int i = 0; i < 1024; i++)
            {
                sin_1024[i] = (float)Math.Sin(i * Math.PI * 2 / 1024);
                cos_1024[i] = (float)Math.Cos(i * Math.PI * 2 / 1024);
            }
        }

        public void detection(byte[] inData, byte[] outData)
        {
            outDataLeght = outData.Length;
            sin_cos_position = (float)(realCentralFrequencyPosition * 1024f / SR);
            Count = inData.Length / 4; // величина вхідних масивів
            _inData.bytes = inData;
            //_outData.bytes = outData;
            
            _outData.bytes = outData;
            _bufferData.bytes = bufferData;
            _expData.bytes = bufferExpData;
            _shiftingData.bytes = shifting_data;
            _testBuffer.bytes = expData;
            if (degree == 2)
            {
                normalizeCoeff = 100000;
            } //5
            if (degree == 4)
            {
                normalizeCoeff = 1000000000000000;
            } //14
            if (x == 1)
            {
                Parallel.For(0, Count, i =>
                {
                    tempI = Math.Abs(_inData.iq[i].i + _inData.iq[i].q);
                    tempQ = 0;
                    _bufferData.iq[i].i = (short)tempI;
                    _bufferData.iq[i].q = (short)tempQ;
                });
            }
            if (x == 2)
            {


                Parallel.For(0, Count, i =>
                {
                    _bufferData.iq[i * 2].i = _inData.iq[i].i;
                    _bufferData.iq[i * 2].q = _inData.iq[i].q;
                    _bufferData.iq[i * 2 + 1].i = (short)((_inData.iq[i].i + _inData.iq[i + 1].i) / 2);
                    _bufferData.iq[i * 2 + 1].q = (short)((_inData.iq[i].q + _inData.iq[i + 1].q) / 2);
                });
                Parallel.For(0, Count * 2, i =>
                {
                    tempI = Math.Abs(_bufferData.iq[i].i + _bufferData.iq[i].q);
                    tempQ = 0;
                    _bufferData.iq[i].i = (short)tempI;
                    _bufferData.iq[i].q = (short)tempQ;
                });

            }
            if (degree == 2)
            {
                tempI = 0;
                tempQ = 0;
                Parallel.For(0, Count, i =>
                {
                    tempI = _inData.iq[i].i * _inData.iq[i].i - _inData.iq[i].q * _inData.iq[i].q;
                    tempQ = 2 * _inData.iq[i].i * _inData.iq[i].q;
                    tempI = tempI / 10;
                    tempQ = tempQ / 10;
                    _expData.iq[i].i = (short)tempI;
                    _expData.iq[i].q = (short)tempQ;
                });
            }
            if (degree == 4)
            {
                tempI = 0;
                tempQ = 0;
                Parallel.For(0, Count, i =>
                {
                    _expData.iq[i].i = (short)((_inData.iq[i].i * _inData.iq[i].i - _inData.iq[i].q * _inData.iq[i].q) / 10); // _expData зробить double!!!!!!!!
                    _expData.iq[i].q = (short)((2 * _inData.iq[i].i * _inData.iq[i].q) / 10);
                    _outData.iq[i].i = (short)((_expData.iq[i].i * _expData.iq[i].i - _expData.iq[i].q * _expData.iq[i].q) / 10); ;
                    _outData.iq[i].q = (short)((2 * _expData.iq[i].i * _expData.iq[i].q) / 10);
                    //_outData.iq[i].i = (short)(_inData.iq[i].i * cos_1024[t] + (float)_inData.iq[i].q * sin_1024[t]);
                    //_outData.iq[i].q = (short)(_inData.iq[i].q * cos_1024[t] - (float)_inData.iq[i].i * sin_1024[t]);
                });
            }
        }

        public void shifting()
        {
            if (realCentralFrequencyPosition > F) sin_cos_position = (float)((realCentralFrequencyPosition - SR/2) * 1024f / (SR));
            else sin_cos_position = (float)(1024f + (realCentralFrequencyPosition - F) * 1024f / (SR));

            Parallel.For(0, Count, i =>
            {
                int t = (i * (int)sin_cos_position) % 1024;
                _shiftingData.iq[i].i = (short)(_shiftingData.iq[i].i + (short)(_inData.iq[i].i * cos_1024[t] + _inData.iq[i].q * sin_1024[t]));
                _shiftingData.iq[i].q = (short)(_shiftingData.iq[i].q + (short)(_inData.iq[i].q * cos_1024[t] - _inData.iq[i].i * sin_1024[t]));
            });                     
        }
        public void configureFirFilter(float bw = 1f)
        {
            //MessageBox.Show("Filter reconfiger");        
            FilterBandwich = bw;
            float _decim = 1f / FilterBandwich;
            if (_decim < 1) _decim = 1;
            decimationCoef = Convert.ToInt32(_decim);
            filterCoefficients = new float[filterOrder];
            //data = new byte[filterOrder * 4];
            _FIR(ref filterCoefficients[0], filterOrder, TPassTypeName.LPF, FilterBandwich, 0.0f, WindowType, beta);
        }
        public void cpuParralelFiltering(ref byte[] inData, ref byte[] outData, ref byte[] data, float[] sin, float[] cos,
                                 float[] coefficients, int length, int filter0rder, int firstDecim, int decim)
        {
            int Count = length / 4; // величина вхідних масивів
            int size = (int)Math.Ceiling((decimal)(Count - firstDecim) / decim);
            iqf[] _tempData = new iqf[Count];
            _inData.bytes = inData;
            _outData.bytes = outData;
            _data.bytes = data;

            Parallel.For(0, size, j =>
            {
                iqf _sum;
                _sum.i = 0;
                _sum.q = 0;
                for (int i = 0; i < filter0rder; i++)
                {
                    int a = firstDecim + j * decim - i;
                    if (a >= 0)
                    {
                        _sum.i += (float)_tempData[a].i * coefficients[i];
                        _sum.q += (float)_tempData[a].q * coefficients[i];
                    }
                    else
                    {
                        _sum.i = _sum.i + (float)_data.iq[Math.Abs(a + 1)].i * coefficients[i];
                        _sum.q = _sum.q + (float)_data.iq[Math.Abs(a + 1)].q * coefficients[i];
                    }
                }
                _outData.iq[j].i = (short)_sum.i;
                _outData.iq[j].q = (short)_sum.q;
            });

            Parallel.For(0, filter0rder, j =>
            {
                _data.iq[j].i = (short)_tempData[Count - j - 1].i;
                _data.iq[j].q = (short)_tempData[Count - j - 1].q;
            });
        }
    }
}