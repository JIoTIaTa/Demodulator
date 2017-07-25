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
using FFT;
using TXT_writter;

namespace demodulation
{
   

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

    public partial class Demodulator
    {        
        public  double SR = 0.0d; // частота дискретизації
        public static sIQData IQ_inData, IQ_outData, IQ_detected, IQ_elevated, IQ_shifted, IQ_remainded, IQ_filtered;
        public MS_syncronize speed_syncronize_I = new MS_syncronize();
        public MS_syncronize speed_syncronize_Q = new MS_syncronize();
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
        public double speedFrequency = 0.0d; // швидкість модуляції
        public double centralFrequency = 0.0d; // центральна частота
        float sin_cos_position = 0; // позиція син/кос для вибору з таблиці
        public float FilterBandwich = 0; // смуга фільтрація
        static int filterOrder = 101; // порядок фільтру
        float[] filterCoefficients; // коефіціенти фільтра
        byte[] remainded = new byte[filterOrder * 4]; // залишок старого масива - початок для нового
        public TWindowType FIR_WindowType = TWindowType.SINC; // тип вікна фільтра
        public float FIR_beta = 3.2f; // коефіціент БЕТА фільтра
        public int N = 0;   // для сноса  
        float maxValue; // змінна для знаходження пікової гармоніки 
        public FFT_data_display display = FFT_data_display.FILTERING;// які дані буде відображати ШПФ
        public int firstDecim = 0; // треба згадать, юзав для фільрації
        float BW = 0; // смуга для фунції генерування коефіціентів
        public float BitPerSapmle = 0;
        public float speed_error_I = 0;
        public float speed_error_Q = 0;
        public float speed_error = 0;
        public bool write = false;
        public static string warningMessage = "Стан: Працює без збоїв";



        /// <summary>Функція визначення частоти маніпуляції (символьної швидкості)</summary>
        public sIQData _detection()
        {
            try
            {
                IQ_detected.bytes = detected;
                for (int i = 0; i < IQ_lenght; i++)
                {
                    tempI = Math.Abs(IQ_shifted.iq[i].i + IQ_shifted.iq[i].q);
                    tempQ = 0;
                    IQ_detected.iq[i].i = (short)tempI;
                    IQ_detected.iq[i].q = (short)tempQ;
                }
                warningMessage = "Стан: Працює без збоїв";                
            }
            catch 
            {
                warningMessage = "Стан: Проблеми з детектуванням";
            }
            return IQ_detected;
        }

        /// <summary>Функція визначення центральної частоти</summary>
        /// <param name="inData">Масив даних сигналу, центральну частоту якого необхідно визначити</param>
        public sIQData _exponentiation(ref byte[] inData)
        {
            try
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
                warningMessage = "Стан: Працює без збоїв";                
            }
            catch 
            {
                warningMessage = "Стан: Проблеми з піднесенням до степеня";                
            }
            return IQ_elevated;
        }
        
        /// <summary>Функція зносу сигналу</summary>
        /// <param name="inData">Масив даних сигналу, який потрібно знести</param>
        /// <param name="outData">Масив даних знесеного сигналу</param>
        public sIQData _shifting_function(ref byte[] inData)
        {
            try
            {
                IQ_shifted.bytes = shifted;
                IQ_inData.bytes = inData;
                if (centralFrequency > F) sin_cos_position = (float)((centralFrequency - SR / 2) * 1024f / (SR));
                else sin_cos_position = (float)(1024f + (centralFrequency - F) * 1024f / (SR));
                sin_cos_position = sin_cos_position / 2;
                for (int i = 0; i < IQ_lenght; i++)
                {
                    int t = (i * (int)sin_cos_position) % 1024;
                    IQ_shifted.iq[i].i = (short)(IQ_inData.iq[i].i * cos_1024[t] + IQ_inData.iq[i].q * sin_1024[t]);
                    IQ_shifted.iq[i].q = (short)(IQ_inData.iq[i].q * cos_1024[t] - IQ_inData.iq[i].i * sin_1024[t]);
                }
                warningMessage = "Стан: Працює без збоїв";
            }
            catch { warningMessage = "Стан: Проблеми зі зносом"; }
            return IQ_shifted;
        }       
        
        /// <summary>Функція визначення центральної частоти</summary>
        public double _F_calculating()
        {
            double realcentralPos = 0.0d; // повертає функція 
            try
            {
                int centralPosition = 0; // позиція пікової гармоніки центральної частоти                
                int minCentralFrequencyZone = 0; // зона пошуку центральної чатоти
                int maxCentralFrequencyZone = 0; // зона пошуку центральної чатоти
                Complex[] exponentiationBuffer = new Complex[65536]; // для піднесення до степені  
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
                realcentralPos = 0;
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
                realcentralPos = (SR / 65536) * centralPosition;
                warningMessage = "Стан: Працює без збоїв";
            }
            catch
            {
                warningMessage = "Стан: Проблеми з визначенням центральної частоти";
            }
            return realcentralPos;
        }
        
        /// <summary>Функція визначення частоти маніпуляції (символьної швидкості)</summary>
        public double _speed_calculating()
        {
            double realSpeedPos = 0.0d;
            try
            {
                int speedPosition = 0; // позиція пікової гармоніки швидкості       
                int minSpeedZone = 0; //зона пошуку швидкості маніпуляції
                int maxSpeedZone = 0; //зона пошуку швидкості маніпуляції
                Complex[] detectionBuffer = new Complex[65536]; // для детектування  
                for (int k = 0; k < IQ_lenght; k++)
                    {
                        detectionBuffer[k] = new Complex(IQ_detected.iq[k].i, IQ_detected.iq[k].q);
                    }
                    for (int k = IQ_lenght; k < 65536; k++)
                    {
                        detectionBuffer[k] = new Complex(0, 0);
                    }

                    detectionBuffer = Fft.fft(detectionBuffer);
                    minSpeedZone = (detectionBuffer.Length / 10) * 2;
                    maxSpeedZone = (detectionBuffer.Length / 10) * 3;
                    maxValue = 0;
                    speedPosition = 0;
                    speedFrequency = 0;
                    for (int i = 0; i < detectionBuffer.Length; i++)
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
                    realSpeedPos = (SR / 65536) * speedPosition;
                    warningMessage = "Стан: Працює без збоїв";
            }
            catch { warningMessage = "Стан: Проблеми з визначенням швидкості"; }
            return realSpeedPos;     
        }
        
        /// <summary>Функція фільтрації</summary>
        public void _filtering_function(ref byte[] outData)
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
                int BPS_int = (int)Math.Round(BitPerSapmle);
                ////////////////////////////////////////////////////////////////////////
                for (int i = 2; i < IQ_filtered.bytes.Length / 4; i+= BPS_int)
                {
                    //MessageBox.Show(string.Format("{0}", i));
                    short[] temp_I = { IQ_filtered.iq[i].i, IQ_filtered.iq[i + (BPS_int ) - 1].i, IQ_filtered.iq[i + (BPS_int * 2) - 1].i };
                    speed_error_I = speed_syncronize_I.Error_calc(temp_I);
                }
                for (int i = 0; i < IQ_filtered.bytes.Length / 4; i += BPS_int)
                {
                    //MessageBox.Show(string.Format("{0}", i));
                    short[] temp_Q = { IQ_filtered.iq[i].q, IQ_filtered.iq[i + (BPS_int) - 1].q, IQ_filtered.iq[i + (BPS_int * 2) - 1].q };
                    speed_error_Q = speed_syncronize_Q.Error_calc(temp_Q);
                }
                speed_error = speed_error_I + speed_error_Q;
                if (write)
                {
                    short[] temp_short_I = new short[IQ_shifted.bytes.Length / 4];
                    short[] temp_short_Q = new short[IQ_shifted.bytes.Length / 4];
                    for (int i = 0; i < IQ_shifted.bytes.Length / 4; i++)
                    {
                        temp_short_I[i] = IQ_shifted.iq[i].i;
                        temp_short_Q[i] = IQ_shifted.iq[i].q;
                    }
                    Writter Write = new Writter(temp_short_I, temp_short_Q, "I", "Q", "after_shifting");
                    //Writter Write1 = new Writter(filterCoefficients,"coef", "filter_coef");
                    write = false;
                }                
                ////////////////////////////////////////////////////////////////////////
                for (int j = 0; j < filterOrder; j++)
                {
                    IQ_remainded.iq[j].i = IQ_shifted.iq[IQ_lenght - j - 1].i;
                    IQ_remainded.iq[j].q = IQ_shifted.iq[IQ_lenght - j - 1].q;
                }
                Buffer.BlockCopy(IQ_filtered.bytes, 0, outData, 0, filtered.Length);
                warningMessage = "Стан: Працює без збоїв";
            }
            catch { warningMessage = "Стан: Проблеми з фільтрацією"; }
        }

        /// <summary>Функція визначення кількості  бітів на символ</summary>
        public float _BitPerSapmle()
        {
            float BPS = 0.0f;
            try
            {
                BPS = (float)(SR / speedFrequency);
                warningMessage = "Стан: Працює без збоїв";
            }
            catch { warningMessage = "Стан: Проблеми з визначенням бітів на такт"; }
            return BPS;
        }
        //public float _speed_error()
        //{
        //    IQ_filtered.bytes = filtered;
        //    float MS_error = 0;
        //    for (int i = 0; i < filtered.Length - 2; i++)
        //    {                
        //        short[] temp = { IQ_filtered.iq[i].i, IQ_filtered.iq[i + 1].i,IQ_filtered.iq[i + 2].i};
        //        MessageBox.Show(string.Format("{0} {1} {2}", temp[0], temp[1], temp[2]));
        //        MS_error = speed_syncronize.Error_calc(temp);
        //        return MS_error;
        //    }
        //    return MS_error;
        //}

    }
}