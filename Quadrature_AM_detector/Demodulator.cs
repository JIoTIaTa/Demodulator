using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace demodulation
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
    public partial class Demodulator
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
        /// <summary>Функція ініціалізації буферів, необхідних для роботи модуля, з вказанням їх довжини</summary>
        /// <param name="Length">Довжина масивів які необхідно виділити в байтах</param>
        public void demodulator_init(int Length)
        {
            try
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
                warningMessage = "Стан: Працює без збоїв";
            } catch { warningMessage = "Стан: Проблеми з виділенням пам'яті під масиви"; }
           
            //MessageBox.Show(String.Format("bufferDetectData = {0}\nbufferExpData = {1}\nshifting_data = {2}\nfiltering_data = {3}\ntempI_buffer = {4}\ntempQ_buffer = {5}\nCount = {6}", bufferDetectData.Length, bufferExpData.Length, shifting_data.Length, filtering_data.Length, tempI_buffer.Length, tempQ_buffer.Length, Count));
        }
        /// <summary>Функція конфігурації параметрів фільтрації</summary>
        public void configFilter()
        {
            try
            {
                FilterBandwich = (float)(speedFrequency * 2 / 0.85);
                BW = (float)(FilterBandwich / SR);
                filterCoefficients = new float[filterOrder];
                _FIR(ref filterCoefficients[0], filterOrder, TPassTypeName.LPF, BW, 0.0f, FIR_WindowType, FIR_beta);
                warningMessage = "Стан: Працює без збоїв";
            }
            catch { warningMessage = "Стан: Проблеми з налаштуванням фільтра"; }
        }       
    }

    //public class MS_syncronize
    //{
    //    private short[] symbols = new short[3];
    //    private float MS_error_prev = 0;
    //    private float normalize_coef = 0.0001f;
    //    float timer = 0f;
    //    public float Error_calc(short[] symbols)
    //    {
    //        try
    //        {
    //            //MessageBox.Show(string.Format("{0}", timer));
    //            this.symbols = symbols;
    //            float MS_error = 0;
    //            MS_error = (1 - normalize_coef) * MS_error_prev + normalize_coef * ((this.symbols[2] - this.symbols[0]) * this.symbols[1]);
    //            MS_error_prev = MS_error;
    //            //Demodulator.warningMessage = "Стан: Працює без збоїв";
    //            //if (this.timer >= 1000 & normalize_coef>= 0.000001f)
    //            //{
    //            //    this.timer = 0;
    //            //    normalize_coef_increment();
    //            //    //MessageBox.Show(string.Format("{0}", normalize_coef));
    //            //}
    //            //if (this.timer <= 1000) { this.timer += 0.1f; } 
    //            //MessageBox.Show(string.Format("({0} - {1}) * {2}", this.symbols[2], this.symbols[0], this.symbols[1]));
    //            return MS_error;                
    //        }
    //        catch
    //        {
    //            Demodulator.warningMessage = "Стан: Проблеми з тактовою синхронізацією";
    //            return 0;
    //        }
    //    }
    //    public void normalize_coef_increment()
    //    {
    //            normalize_coef = normalize_coef / 10;
    //    }
    //}    
}
