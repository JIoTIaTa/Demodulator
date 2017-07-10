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
        //long normalizeCoeff = 1;
        public static int x = 1; // коеф інтерполяції
        public int IQ_lenght; // кількість I/Q відліків
        public int maxFFT = 65536; // максимальний порядок ШПФ        
        public int modulation_multiplicity = 4; //кратність модуляції 
        public bool demodulator_busy = false; // прапорець роботи тракту
        public int fftAveragingValue = 4; // усереднення ШПФ
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
        public float FilterBandwich = 0; // смуга фільтрація
        static int filterOrder = 101; // порядок фільтру
        float[] filterCoefficients; // коефіціенти фільтра
        byte[] remainded = new byte[filterOrder * 4]; // залишок старого масива - початок для нового
        public TWindowType FIR_WindowType = TWindowType.KAISER; // тип вікна фільтра
        public float FIR_beta = 3.2f; // коефіціент БЕТА фільтра
        public int N = 0;   // для сноса  
        int minSpeedZone; //зона пошуку швидкості маніпуляції
        int maxSpeedZone;
        int minCentralFrequencyZone; // зона пошуку центральної чатоти
        int maxCentralFrequencyZone;
        float maxValue; // змінна для знаходження пікової гармоніки
        int speedPosition; // позиція пікової гармоніки швидкості
        int centralPosition; // позиція пікової гармоніки центральної частоти
        public FFT_data_display display = FFT_data_display.SHIFTING;// які дані буде відображати ШПФ
        public int firstDecim = 0; // треба згадать, юзав для фільрації
        float BW = 0; // смуга для фунції генерування коефіціентів


        /// <summary>Функція ініціалізації буферів, необхідних для роботи модуля, з вказанням їх довжини</summary>
        /// <param name="Length">Довжина масивів які необхідно виділити в байтах</param>
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
       
        /// <summary>Функція визначення частоти маніпуляції (символьної швидкості)</summary>
        public void detection()
        {
            //IQ_inData.bytes = shifted;
            //IQ_inData.bytes = inData;
            IQ_detected.bytes = detected;               
            for (int i = 0; i < IQ_lenght; i++)
            {
                tempI = Math.Abs(IQ_shifted.iq[i].i + IQ_shifted.iq[i].q);
                tempQ = 0;
                IQ_detected.iq[i].i = (short)tempI;
                IQ_detected.iq[i].q = (short)tempQ;
            }
        }

        /// <summary>Функція визначення центральної частоти</summary>
        /// <param name="inData">Масив даних сигналу, центральну частоту якого необхідно визначити</param>
        public void exponentiation(ref byte[] inData)
        {           
            IQ_inData.bytes = inData;
            IQ_elevated.bytes = elevated;
            if (modulation_multiplicity == 2)
            {
                tempI = 0;
                tempQ = 0;
                //normalizeCoeff = 100000;
                for (int i = 0; i < IQ_lenght; i++)
                {
                    tempI = IQ_inData.iq[i].i * IQ_inData.iq[i].i - IQ_inData.iq[i].q * IQ_inData.iq[i].q;
                    tempQ = 2 * IQ_inData.iq[i].i * IQ_inData.iq[i].q;
                    tempI = tempI / 10;
                    tempQ = tempQ / 10;
                    IQ_elevated.iq[i].i = (short)tempI;
                    IQ_elevated.iq[i].q = (short)tempQ;
                }
            }
            if (modulation_multiplicity == 4)
            {
                tempI = 0;
                tempQ = 0;
                //normalizeCoeff = 1000000000000000;
                for (int i = 0; i < IQ_lenght; i++)
                {
                    tempI_buffer[i] = (IQ_inData.iq[i].i * IQ_inData.iq[i].i - IQ_inData.iq[i].q * IQ_inData.iq[i].q) / 10;
                    tempQ_buffer[i] = ((2 * IQ_inData.iq[i].i * IQ_inData.iq[i].q) / 10);
                    IQ_elevated.iq[i].i = (short)((tempI_buffer[i] * tempI_buffer[i] - tempQ_buffer[i] * tempQ_buffer[i]) / 10); ;
                    IQ_elevated.iq[i].q = (short)((2 * tempI_buffer[i] * tempQ_buffer[i]) / 10);
                }

            }
            if (modulation_multiplicity == 8)
            {
                tempI = 0;
                tempQ = 0;
                //normalizeCoeff = 1000000000000000;
                for (int i = 0; i < IQ_lenght; i++)
                {
                    tempI_buffer[i] = (IQ_inData.iq[i].i * IQ_inData.iq[i].i - IQ_inData.iq[i].q * IQ_inData.iq[i].q) / 10;
                    tempQ_buffer[i] = ((2 * IQ_inData.iq[i].i * IQ_inData.iq[i].q) / 10);
                }
                for (int i = 0; i < IQ_lenght; i++)
                {
                    tempI_buffer[i] = (tempI_buffer[i] * tempI_buffer[i] - tempQ_buffer[i] * tempQ_buffer[i]) / 1000;
                    tempQ_buffer[i] = ((2 * tempI_buffer[i] * tempQ_buffer[i]) / 1000);
                    IQ_elevated.iq[i].i = (short)((tempI_buffer[i] * tempI_buffer[i] - tempQ_buffer[i] * tempQ_buffer[i]) / 100); ;
                    IQ_elevated.iq[i].q = (short)((2 * tempI_buffer[i] * tempQ_buffer[i]) / 100);
                }

            }
        }
        /// <summary>Функція зносу сигналу</summary>
        /// <param name="inData">Масив даних сигналу, який потрібно знести</param>
        /// <param name="outData">Масив даних знесеного сигналу</param>
        public void shifting_function(ref byte[] inData)
        {
            IQ_shifted.bytes = shifted;
            IQ_inData.bytes = inData;
            //IQ_outData.bytes = outData;
            if (realCentralFrequencyPosition > F) sin_cos_position = (float)((realCentralFrequencyPosition - SR/2) * 1024f / (SR));
            else sin_cos_position = (float)(1024f + (realCentralFrequencyPosition - F) * 1024f / (SR));
            sin_cos_position = sin_cos_position / 2;
            for (int i = 0; i < IQ_lenght; i++)
            {
                int t = (i * (int)sin_cos_position) % 1024;
                IQ_shifted.iq[i].i = (short)(IQ_inData.iq[i].i * cos_1024[t] + IQ_inData.iq[i].q * sin_1024[t] );
                IQ_shifted.iq[i].q = (short)(IQ_inData.iq[i].q * cos_1024[t] - IQ_inData.iq[i].i * sin_1024[t] );
            }
        }       
        /// <summary>Функція визначення центральної частоти</summary>
        public void F_calculating()
        {
                //-----Блок визначення центральної частоти-----               
                try
                {
                    for (int k = 0; k < IQ_lenght; k++)
                    {
                        exponentiationBuffer[k] = new Complex(IQ_elevated.iq[k].i, IQ_elevated.iq[k].q);
                    }
                    for (int k = IQ_lenght; k < 65536; k++)
                    {
                        exponentiationBuffer[k] = new Complex(0, 0);
                    }
                    exponentiationBuffer = Fft.fft(exponentiationBuffer);
                    exponentiationBuffer = Fft.nfft(exponentiationBuffer);
                    minCentralFrequencyZone = (exponentiationBuffer.Length / 10) * 1;
                    maxCentralFrequencyZone = (exponentiationBuffer.Length / 10) * 9;
                    centralPosition = 0;
                    realCentralFrequencyPosition = 0;
                    for (int i = 0; i < exponentiationBuffer.Length; i++)
                    {
                        if (i > minCentralFrequencyZone & i < maxCentralFrequencyZone)
                        {
                            if (maxValue < Math.Log(exponentiationBuffer[i].Magnitude, 10))
                            {
                                maxValue = (float)Math.Log(exponentiationBuffer[i].Magnitude, 10);
                                centralPosition = i;
                            }
                        }
                    }
                    realCentralFrequencyPosition = (SR / 65536) * centralPosition;
                }
                catch
                {                    
                    MessageBox.Show("Trouble with definding central frequency :(");
                }
        }
        /// <summary>Функція визначення частоти маніпуляції (символьної швидкості)</summary>
        public void speed_calculating()
        {
                //-----Блок визначення швидкості маніпуляції-----   
                try
                {
                    for (int k = 0; k < IQ_lenght; k++)
                    {
                        detectionBuffer[k] = new Complex(IQ_detected.iq[k].i, IQ_detected.iq[k].q);
                    }
                    for (int k = IQ_lenght; k < 65536; k++)
                    {
                        detectionBuffer[k] = new Complex(0, 0);
                    }

                    detectionBuffer = Fft.fft(detectionBuffer);
                    minSpeedZone = (detectionBuffer.Length / 10) * 6;
                    maxSpeedZone = (detectionBuffer.Length / 10) * 9;
                    maxValue = 0;
                    speedPosition = 0;
                    realSpeedPosition = 0;
                    for (int i = 0; i < exponentiationBuffer.Length; i++)
                    {
                        if (i > minSpeedZone & i < maxSpeedZone)
                        {
                            if (maxValue < Math.Log(detectionBuffer[i].Magnitude, 10))
                            {
                                maxValue = (float)Math.Log(detectionBuffer[i].Magnitude, 10);
                                speedPosition = i;
                            }
                        }
                    }
                    realSpeedPosition = (SR / 65536) * speedPosition;
                }
                catch
                {
                    MessageBox.Show("Trouble with definding speed :(");
                }      
        }
        /// <summary>Функція конфігурації параметрів фільтрації</summary>
        public void configFilter()
        {
            try
            {
                FilterBandwich = (float)(realSpeedPosition / 0.85);
                BW = (float)(FilterBandwich / SR);
                filterCoefficients = new float[filterOrder];
                _FIR(ref filterCoefficients[0], filterOrder, TPassTypeName.LPF, BW, 0.0f, FIR_WindowType, FIR_beta);
            }
            catch (Exception) { MessageBox.Show("Trouble with filter config :("); }
        }
        /// <summary>Функція фільтрації</summary>
        public void filtering_function(ref byte[] outData)
        {
            try
            {
                IQ_shifted.bytes = shifted;
                IQ_filtered.bytes = filtered;
                IQ_remainded.bytes = remainded;
                int size = IQ_lenght;

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
                            _sum.i += IQ_shifted.iq[a].i * filterCoefficients[i];
                            _sum.q += IQ_shifted.iq[a].q * filterCoefficients[i];
                        }
                        else
                        {
                            _sum.i += IQ_remainded.iq[Math.Abs(a + 1)].i * filterCoefficients[i];
                            _sum.q += IQ_remainded.iq[Math.Abs(a + 1)].q * filterCoefficients[i];                            
                        }
                    }
                    IQ_filtered.iq[j].i = (short)(_sum.i);
                    IQ_filtered.iq[j].q = (short)(_sum.q);
                }
                for (int j = 0; j < filterOrder; j++)
                {
                    IQ_remainded.iq[j].i = IQ_shifted.iq[IQ_lenght - j - 1].i;
                    IQ_remainded.iq[j].q = IQ_shifted.iq[IQ_lenght - j - 1].q;
                }
                Buffer.BlockCopy(IQ_filtered.bytes, 0, outData, 0, filtered.Length);
            }
            catch (Exception) { MessageBox.Show("Trouble with filter :("); }
        }

        // test_zone (delete after debug)
        public void WriteIputMassive(short[] inData_I, short[] inData_Q, short[] outData_I, short[] outData_Q, int lenght)
        {
            StreamWriter file = new StreamWriter("Array.txt");
            for (int i = 0; i < lenght; i++)
            {
                file.WriteLine(i + "in_I->" + inData_I[i]);
                file.WriteLine(i + "in_Q->" + inData_Q[i]);
                file.WriteLine(i + "out_Q->" + outData_I[i]);
                file.WriteLine(i + "out_Q->" + outData_Q[i]);
            }
            MessageBox.Show("Writed");
            file.Close();
        }
    }
}