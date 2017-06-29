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
using FirFilterNew;

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
        public static double SR = 0.0d; // частота дискретизації
        public static sIQData _inData, _outData, _detectedData, _expData, _shiftingData, _data;
        public static sIQData _testBuffer;
        public long F = 0; // центральная частота
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
        public int averagingValue = 1; // усреднение ШПФ
        public static int outDataLeght = 65536;
        public static int inDataLeght = 65536;
        //byte[] expData = new byte[outDataLeght];
        byte[] bufferDetectData = new byte[inDataLeght * x];
        byte[] bufferExpData = new byte[inDataLeght];
        byte[] shifting_data = new byte[inDataLeght];
        double[] tempI_buffer = new double[inDataLeght];
        double[] tempQ_buffer = new double[inDataLeght];
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
        public int FFTdeep = 65536; // глибина FFT
        Complex[] detectionBuffer = new Complex[65536]; // для детектування  
        Complex[] exponentiationBuffer = new Complex[65536]; // для піднесення до степені    
        /*зона пошуку гармоніки швидкості модуляції*/
        int minSpeedZoneCoef;
        int maxSpeedZoneCoef;
        /*зона пошуку центральної частоти*/
        int minCentralFrequencyZoneCoef;
        int maxCentralFrequencyZoneCoef;
        /**/
        float maxValue; // змінна для знаходження пікової гармоніки
        int speedPosition; // позиція пікової гармоніки швидкості
        int centralPosition; // позиція пікової гармоніки центральної частоти

        /// <summary>
        /// Функція ініціалізації масивів косинісів та сінусів, необхідних для зносу сигналу на необхідну частоту
        /// </summary>
        public void sin_cos_init()
        {
            for (int i = 0; i < 1024; i++)
            {
                sin_1024[i] = (float)Math.Sin(i * Math.PI * 2 / 1024);
                cos_1024[i] = (float)Math.Cos(i * Math.PI * 2 / 1024);
            }
        }
        /// <summary>
        /// Функція визначення частоти маніпуляції (символьної швидкості)
        /// </summary>
        /// <param name="inData">Масив даних сигналу, швидкість маніпуляції якого необхідно визначити</param>
        /// /// <param name="outData">Масив даних, найбільша гармоніка по амплітуді якої відповідає частоті маніпуляції</param>
        public void detection(ref byte[] inData)
        {            
            Count = inData.Length / 4; // величина вхідних масивів
            //_inData.bytes = shifting_data;
            _inData.bytes = shifting_data;
            _detectedData.bytes = bufferDetectData;
            if (x == 1)
            {
                Parallel.For(0, Count, i =>
                {
                    tempI = Math.Abs(_inData.iq[i].i + _inData.iq[i].q);
                    tempQ = 0;
                    _detectedData.iq[i].i = (short)tempI;
                    _detectedData.iq[i].q = (short)tempQ;
                });
            }
            if (x == 2)
            {
                Parallel.For(0, Count, i =>
                {
                    _detectedData.iq[i * 2].i = _inData.iq[i].i;
                    _detectedData.iq[i * 2].q = _inData.iq[i].q;
                    _detectedData.iq[i * 2 + 1].i = (short)((_inData.iq[i].i + _inData.iq[i + 1].i) / 2);
                    _detectedData.iq[i * 2 + 1].q = (short)((_inData.iq[i].q + _inData.iq[i + 1].q) / 2);
                });
                Parallel.For(0, Count * 2, i =>
                {
                    tempI = Math.Abs(_detectedData.iq[i].i + _detectedData.iq[i].q);
                    tempQ = 0;
                    _detectedData.iq[i].i = (short)tempI;
                    _detectedData.iq[i].q = (short)tempQ;
                });
            }
        }
        /// <summary>
        /// Функція визначення центральної частоти
        /// </summary>
        /// <param name="inData">Масив даних сигналу, центральну частоту якого необхідно визначити</param>
        /// /// <param name="outData">Масив даних, найбільша гармоніка по амплітуді якої відповідає центральній частоті</param>
        public void exponentiation(ref byte[] inData)
        {
            Count = inData.Length / 4; // величина вхідних масивів
            _inData.bytes = inData;
            _expData.bytes = bufferExpData;
            if (degree == 2)
            {
                normalizeCoeff = 100000;
            } //5
            if (degree == 4)
            {
                normalizeCoeff = 1000000000000000;
            } //14            
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
                    tempI_buffer[i] = (_inData.iq[i].i * _inData.iq[i].i - _inData.iq[i].q * _inData.iq[i].q) / 10;
                    tempQ_buffer[i] = ((2 * _inData.iq[i].i * _inData.iq[i].q) / 10);
                    _expData.iq[i].i = (short)((tempI_buffer[i] * tempI_buffer[i] - tempQ_buffer[i] * tempQ_buffer[i]) / 10); ;
                    _expData.iq[i].q = (short)((2 * tempI_buffer[i] * tempQ_buffer[i]) / 10);
                });
            }
        }

        public void shifting(ref byte[] inData, byte[] outData)
        {            
            _shiftingData.bytes = shifting_data;
            _inData.bytes = inData;
            _outData.bytes = outData;
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

        public void F_calculating()
        {
            if (busy)
            {
                if (FFTdeep > maxFFT) { FFTdeep = maxFFT; }
                //-----Блок визначення центральної частоти-----               
                try
                {
                    Parallel.For(0, FFTdeep, k =>
                    {
                        exponentiationBuffer[k] = new Complex(_expData.iq[k].i, _expData.iq[k].q);
                    });
                    Parallel.For(FFTdeep, 65536, k =>
                    {
                        exponentiationBuffer[k] = new Complex(0, 0);
                    });
                    exponentiationBuffer = Fft.fft(exponentiationBuffer);
                    exponentiationBuffer = Fft.nfft(exponentiationBuffer);
                    minCentralFrequencyZoneCoef = (exponentiationBuffer.Length / 10) * 1;
                    maxCentralFrequencyZoneCoef = (exponentiationBuffer.Length / 10) * 9;
                    centralPosition = 0;
                    realCentralFrequencyPosition = 0;
                    Parallel.For(0, exponentiationBuffer.Length, i =>
                    {
                        if (i > minCentralFrequencyZoneCoef & i < maxCentralFrequencyZoneCoef)
                        {
                            if (maxValue < Math.Log(exponentiationBuffer[i].Magnitude, 10))
                            {
                                maxValue = (float)Math.Log(exponentiationBuffer[i].Magnitude, 10);
                                centralPosition = i;
                            }
                        }
                    });
                    realCentralFrequencyPosition = (SR / 65536) * centralPosition;
                }
                catch
                {                    
                    MessageBox.Show("Trouble with definding central frequency :(");
                }
            }
        }
        public void speed_calculating()
        {
            if (busy)
            {
                //-----Блок визначення швидкості маніпуляції-----   
                try
                {
                    Parallel.For(0, FFTdeep, k =>
                    {
                        detectionBuffer[k] = new Complex(_detectedData.iq[k].i, _detectedData.iq[k].q);
                    });

                    Parallel.For(FFTdeep, 65536, k =>
                    {
                        detectionBuffer[k] = new Complex(0, 0);
                    });

                    detectionBuffer = Fft.fft(detectionBuffer);
                    minSpeedZoneCoef = (detectionBuffer.Length / 10) * 6;
                    maxSpeedZoneCoef = (detectionBuffer.Length / 10) * 9;
                    maxValue = 0;
                    speedPosition = 0;
                    realSpeedPosition = 0;
                    Parallel.For(0, detectionBuffer.Length, i =>
                    {
                        if (i > minSpeedZoneCoef & i < maxSpeedZoneCoef)
                        {
                            if (maxValue < Math.Log(detectionBuffer[i].Magnitude, 10))
                            {
                                maxValue = (float)Math.Log(detectionBuffer[i].Magnitude, 10);
                                speedPosition = i;
                            }
                        }
                    });
                    realSpeedPosition = (SR / 65536) * speedPosition;
                }
                catch
                {
                    MessageBox.Show("Trouble with definding speed :(");
                }
            }            
        }
    }
}