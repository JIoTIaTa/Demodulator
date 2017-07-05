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

namespace demodulation_namespace
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

    /// <summary>
    /// Клас зберігання двох величин
    /// </summary>
    public class change
    {
        private int _old;
        private int _new;
        
        public int old_value {get { return _old; } set { _old = value; } }
        public int new_value { get { return _new; } set { _new = value; } }
    }

    public class Demodulator
    {
        enum TPassTypeName { LPF, HPF, BPF, NOTCH };
        public enum TWindowType
        {
            NONE, KAISER, SINC, HANNING, HAMMING, BLACKMAN,
            FLATTOP, BLACKMAN_HARRIS, BLACKMAN_NUTTALL, NUTTALL,
            KAISER_BESSEL, TRAPEZOID, GAUSS, SINE, TEST
        };

        public enum FFT_data_display
        {
            SHIFTING, EXPONENT, DETECTED, FILTERING, INPUT
        };
        [DllImport("..\\..\\data\\FIR.dll", EntryPoint = "BasicFIR", CallingConvention = CallingConvention.StdCall)]
        static extern void _FIR(ref float FIRCoeff, int numTaps, TPassTypeName PassType, float OmegaC, float BW, TWindowType WindowTyte, float WinBeta);
        public  double SR = 0.0d; // частота дискретизації
        public static sIQData IQ_inData, IQ_outData, IQ_detected, IQ_elevated, IQ_shifted, IQ_remainded, IQ_filtered;
        public long F = 0; // центральная частота
        public bool sendComand = false;
        private double tempI = 0; // змінна для тимчасових даних
        private double tempQ = 0; // змінна для тимчасових даних
        public bool show; 
        long normalizeCoeff = 1;
        public static int x = 1; // коеф інтерполяції
        public int IQ_lenght; // кількість I/Q відліків
        public int maxFFT = 65536; // максимальний порядок ШПФ        
        public int modulation_multiplicity = 4; //кратність модуляції 
        public bool demodulator_busy = false; // прапорець роботи тракту
        public int fftAveragingValue = 1; // усереднення ШПФ
        byte[] detected = new byte[512]; // для визначення швикості
        byte[] elevated = new byte[512]; // піднесений в степінь, для визначення центральної частоти
        byte[] shifted = new byte[512]; // знесений сигнал
        byte[] filtered = new byte[512]; // відфільтрований сигнал
        double[] tempI_buffer = new double[128]; // для зберігання I відліків подвійної точності 
        double[] tempQ_buffer = new double[128]; // для зберігання Q відліків подвійної точності 
        float[] sin_1024 = new float[1024]; // масив синусів для зносу
        float[] cos_1024 = new float[1024]; // масив косинусів для зносу
        Complex[] detectionBuffer = new Complex[65536]; // для детектування  
        Complex[] exponentiationBuffer = new Complex[65536]; // для піднесення до степені  
        public double realSpeedPosition = 0; // швидкість модуляції
        public double realCentralFrequencyPosition = 0; // центральна частота
        float sin_cos_position = 0; // позиція син/кос для вибору з таблиці
        float FilterBandwich = 0; // смуга фільтрація
        int decimationCoef = 1; // коефіцінт децимації
        static int filterOrder = 50; // порядок фільтру
        float[] filterCoefficients; // коефіціенти фільтра
        byte[] remainded = new byte[filterOrder * 4]; // залишок старого масива - початок для нового
        public TWindowType FIR_WindowType = TWindowType.KAISER; // тип вікна фільтра
        public float FIR_beta = 3.2f; // коефіціент БЕТА фільтра
        public int N = 0;   // для сноса  
        int minSpeedZone;
        int maxSpeedZone;
        int minCentralFrequencyZone;
        int maxCentralFrequencyZone;
        float maxValue; // змінна для знаходження пікової гармоніки
        int speedPosition; // позиція пікової гармоніки швидкості
        int centralPosition; // позиція пікової гармоніки центральної частоти
        public FFT_data_display display = FFT_data_display.SHIFTING;
        

        /// <summary>
        /// Функція ініціалізації буферів, необхідних для роботи модуля, з вказанням їх довжини
        /// </summary>
        /// /// <param name="Length">Довжина масивів які необхідно виділити в байтах</param>
        public void demodulator_init(int Length)
        {
            IQ_lenght = Length / 4;
            if (IQ_lenght > maxFFT) { IQ_lenght = maxFFT; }
            Array.Resize(ref detected, Length);
            Array.Resize(ref elevated, Length);
            Array.Resize(ref shifted, Length);
            Array.Resize(ref filtered, Length);
            Array.Resize(ref tempI_buffer, IQ_lenght);
            Array.Resize(ref tempQ_buffer, IQ_lenght);
            for (int i = 0; i < 1024; i++)
            {
                sin_1024[i] = (float)Math.Sin(i * Math.PI * 2 / 1024);
                cos_1024[i] = (float)Math.Cos(i * Math.PI * 2 / 1024);
            }
            //MessageBox.Show(String.Format("bufferDetectData = {0}\nbufferExpData = {1}\nshifting_data = {2}\nfiltering_data = {3}\ntempI_buffer = {4}\ntempQ_buffer = {5}\nCount = {6}", bufferDetectData.Length, bufferExpData.Length, shifting_data.Length, filtering_data.Length, tempI_buffer.Length, tempQ_buffer.Length, Count));
        }
       
        /// <summary>
        /// Функція визначення частоти маніпуляції (символьної швидкості)
        /// </summary>
        /// <param name="inData">Масив даних сигналу, швидкість маніпуляції якого необхідно визначити</param>
        /// /// <param name="outData">Масив даних, найбільша гармоніка по амплітуді якої відповідає частоті маніпуляції</param>
        public void detection(ref byte[] inData)
        {            
            //_inData.bytes = shifting_data;
            IQ_inData.bytes = inData;
            IQ_detected.bytes = detected;   
            Parallel.For(0, IQ_lenght, i =>
            {
                tempI = Math.Abs(IQ_inData.iq[i].i + IQ_inData.iq[i].q);
                tempQ = 0;
                IQ_detected.iq[i].i = (short)tempI;
                IQ_detected.iq[i].q = (short)tempQ;
            });
        }

        /// <summary>
        /// Функція визначення центральної частоти
        /// </summary>
        /// <param name="inData">Масив даних сигналу, центральну частоту якого необхідно визначити</param>
        /// /// <param name="outData">Масив даних, найбільша гармоніка по амплітуді якої відповідає центральній частоті</param>
        public void exponentiation(ref byte[] inData)
        {           
            IQ_inData.bytes = inData;
            IQ_elevated.bytes = elevated;
            if (modulation_multiplicity == 2)
            {
                tempI = 0;
                tempQ = 0;
                normalizeCoeff = 100000;
                Parallel.For(0, IQ_lenght, i =>
                {
                    tempI = IQ_inData.iq[i].i * IQ_inData.iq[i].i - IQ_inData.iq[i].q * IQ_inData.iq[i].q;
                    tempQ = 2 * IQ_inData.iq[i].i * IQ_inData.iq[i].q;
                    tempI = tempI / 10;
                    tempQ = tempQ / 10;
                    IQ_elevated.iq[i].i = (short)tempI;
                    IQ_elevated.iq[i].q = (short)tempQ;
                });
            }
            if (modulation_multiplicity == 4)
            {
                tempI = 0;
                tempQ = 0;
                normalizeCoeff = 1000000000000000;
                Parallel.For(0, IQ_lenght, i =>
                {
                    tempI_buffer[i] = (IQ_inData.iq[i].i * IQ_inData.iq[i].i - IQ_inData.iq[i].q * IQ_inData.iq[i].q) / 10;
                    tempQ_buffer[i] = ((2 * IQ_inData.iq[i].i * IQ_inData.iq[i].q) / 10);
                    IQ_elevated.iq[i].i = (short)((tempI_buffer[i] * tempI_buffer[i] - tempQ_buffer[i] * tempQ_buffer[i]) / 10); ;
                    IQ_elevated.iq[i].q = (short)((2 * tempI_buffer[i] * tempQ_buffer[i]) / 10);
                });
            }
        }
        /// <summary>
        /// Функція зносу сигналу
        /// </summary>
        /// <param name="inData">Масив даних сигналу, який потрібно знести</param>
        /// /// <param name="outData">Масив даних знесеного сигналу</param>
        public void shifting_function(ref byte[] inData, ref byte[] outData)
        {
            IQ_shifted.bytes = shifted;
            IQ_inData.bytes = inData;
            IQ_outData.bytes = outData;
            if (realCentralFrequencyPosition > F) sin_cos_position = (float)((realCentralFrequencyPosition - SR/2) * 1024f / (SR));
            else sin_cos_position = (float)(1024f + (realCentralFrequencyPosition - F) * 1024f / (SR));

            Parallel.For(0, IQ_lenght, i =>
            {
                int t = (i * (int)sin_cos_position) % 1024;
                //IQ_shifted.iq[i].i = (short)((IQ_shifted.iq[i].i + (short)(IQ_inData.iq[i].i * cos_1024[t] + IQ_inData.iq[i].q * sin_1024[t])) / 1);
                //IQ_shifted.iq[i].q = (short)((IQ_shifted.iq[i].q + (short)(IQ_inData.iq[i].q * cos_1024[t] - IQ_inData.iq[i].i * sin_1024[t])) / 1);
                IQ_shifted.iq[i].i = (short)(IQ_shifted.iq[i].i + (short)(IQ_inData.iq[i].i * cos_1024[t] + IQ_inData.iq[i].q * sin_1024[t]));
                IQ_shifted.iq[i].q = (short)(IQ_shifted.iq[i].q + (short)(IQ_inData.iq[i].q * cos_1024[t] - IQ_inData.iq[i].i * sin_1024[t]));
            });
             Buffer.BlockCopy(IQ_shifted.bytes, 0, outData, 0,inData.Length);
        }
        public void configureFirFilter(float bw = 1f)
        {
            FilterBandwich = bw;
            float _decim = 1f / FilterBandwich;
            if (_decim < 1) _decim = 1;
            decimationCoef = Convert.ToInt32(_decim);
            filterCoefficients = new float[filterOrder];
            //data = new byte[filterOrder * 4];
            _FIR(ref filterCoefficients[0], filterOrder, TPassTypeName.LPF, FilterBandwich, 0.0f, FIR_WindowType, FIR_beta);
        }
        public void cpuParralelFiltering(ref byte[] inData, ref byte[] outData, ref byte[] data, float[] sin, float[] cos,
                                 float[] coefficients, int length, int filter0rder, int firstDecim, int decim)
        {
            int Count = length / 4; // величина вхідних масивів
            int size = (int)Math.Ceiling((decimal)(Count - firstDecim) / decim);
            iqf[] _tempData = new iqf[Count];
            IQ_inData.bytes = inData;
            IQ_outData.bytes = outData;
            IQ_remainded.bytes = data;

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
                        _sum.i = _sum.i + (float)IQ_remainded.iq[Math.Abs(a + 1)].i * coefficients[i];
                        _sum.q = _sum.q + (float)IQ_remainded.iq[Math.Abs(a + 1)].q * coefficients[i];
                    }
                }
                IQ_outData.iq[j].i = (short)_sum.i;
                IQ_outData.iq[j].q = (short)_sum.q;
            });

            Parallel.For(0, filter0rder, j =>
            {
                IQ_remainded.iq[j].i = (short)_tempData[Count - j - 1].i;
                IQ_remainded.iq[j].q = (short)_tempData[Count - j - 1].q;
            });
        }
        /// <summary>
        /// Функція визначення центральної частоти
        /// </summary>
        public void F_calculating()
        {
                //-----Блок визначення центральної частоти-----               
                try
                {
                    Parallel.For(0, IQ_lenght, k =>
                    {
                        exponentiationBuffer[k] = new Complex(IQ_elevated.iq[k].i, IQ_elevated.iq[k].q);
                    });
                    Parallel.For(IQ_lenght, 65536, k =>
                    {
                        exponentiationBuffer[k] = new Complex(0, 0);
                    });
                    exponentiationBuffer = Fft.fft(exponentiationBuffer);
                    exponentiationBuffer = Fft.nfft(exponentiationBuffer);
                    minCentralFrequencyZone = (exponentiationBuffer.Length / 10) * 1;
                    maxCentralFrequencyZone = (exponentiationBuffer.Length / 10) * 9;
                    centralPosition = 0;
                    realCentralFrequencyPosition = 0;
                    Parallel.For(0, exponentiationBuffer.Length, i =>
                    {
                        if (i > minCentralFrequencyZone & i < maxCentralFrequencyZone)
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
        /// <summary>
        /// Функція визначення частоти маніпуляції (символьної швидкості)
        /// </summary>
        public void speed_calculating()
        {
                //-----Блок визначення швидкості маніпуляції-----   
                try
                {
                    Parallel.For(0, IQ_lenght, k =>
                    {
                        detectionBuffer[k] = new Complex(IQ_detected.iq[k].i, IQ_detected.iq[k].q);
                    });

                    Parallel.For(IQ_lenght, 65536, k =>
                    {
                        detectionBuffer[k] = new Complex(0, 0);
                    });

                    detectionBuffer = Fft.fft(detectionBuffer);
                    minSpeedZone = (detectionBuffer.Length / 10) * 6;
                    maxSpeedZone = (detectionBuffer.Length / 10) * 9;
                    maxValue = 0;
                    speedPosition = 0;
                    realSpeedPosition = 0;
                    Parallel.For(0, detectionBuffer.Length, i =>
                    {
                        if (i > minSpeedZone & i < maxSpeedZone)
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