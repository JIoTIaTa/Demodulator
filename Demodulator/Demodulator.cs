using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;

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
    enum TPassTypeName { LPF, HPF, BPF, NOTCH };
    public enum TWindowType
    {
        NONE, KAISER, SINC, HANNING, HAMMING, BLACKMAN,
        FLATTOP, BLACKMAN_HARRIS, BLACKMAN_NUTTALL, NUTTALL,
        KAISER_BESSEL, TRAPEZOID, GAUSS, SINE, TEST
    };
    public enum FFT_data_display
    {
        SHIFTING, FILTERING, INPUT
    };
    public enum Exponent_data_display
    {
       MODULE, ELEVATE
    };

    public enum modulation_type { PSK_2, PSK_4, PSK_8, QAM_16 }

    public enum Filter_type { simple, poliphase }
    public partial class Demodulator
    {

        [DllImport(@"..\\..\\data\\CUDA_FFT.dll")]
        public static extern int deviceFFT(ref Complex inData, ref Complex outData, int FFT_deep, int device_number);
        [DllImport(@"..\\..\\data\\CUDA_FFT.dll")]
        public static extern int FFT_centering(ref Complex inData, ref Complex outData, int FFT_deep, int device_number);

        /// <summary>Функція ініціалізації буферів, необхідних для роботи модуля, з вказанням їх довжини</summary>
        /// <param name="Length">Довжина масивів які необхідно виділити в байтах</param>
        public void demodulator_init(int Length)
        {
            try
            {
                IQ_lenght = Length / 4;
                if (IQ_lenght > maxFFT) { IQ_lenght = maxFFT; }
                IQ_detected.bytes = new byte[Length];
                IQ_elevated.bytes = new byte[Length];
                IQ_shifted.bytes = new byte[Length];
                IQ_filtered.bytes = new byte[Length];
                Array.Resize(ref tempI_buffer, IQ_lenght);
                Array.Resize(ref tempQ_buffer, IQ_lenght);
                for (int i = 0; i < 16384; i++)
                {
                    sin_16384[i] = (float)Math.Sin(i * Math.PI * 2 / 16384);
                    cos_16384[i] = (float)Math.Cos(i * Math.PI * 2 / 16384);
                }
                warningMessage = "Стан: Працює без збоїв";
            }
            catch (Exception exception)
            {
                warningMessage = string.Format("{0}.{1}: {2}", exception.Source, exception.TargetSite, exception.Message);
            }
        }
        /// <summary>Функція конфігурації параметрів фільтрації</summary>
        private void configFilter()
        {
            try
            {
                Filter_Math FIR = new Filter_Math();
                FilterBandwich = (float)(speedFrequency * 2 / 0.85);
                BW = (float)(FilterBandwich / SR);
                filterCoefficients = new float[filterOrder];
                //_FIR(ref filterCoefficients[0], filterOrder, TPassTypeName.LPF, BW, 0.0f, FIR_WindowType, FIR_beta);
                filterCoefficients = FIR.BasicFIR(filterOrder, TPassTypeName.LPF, BW, 0, FIR_WindowType, FIR_beta, 0.0f);
                warningMessage = "Стан: Працює без збоїв";
            }
            catch { warningMessage = "Стан: Проблеми з налаштуванням фільтра"; }
        }
    }


    /// <summary>
    /// Клас зберігання двох величин
    /// </summary>
    public class change
    {
        private int _old;
        private int _new;

        public int old_value { get { return _old; } set { _old = value; } }
        public int new_value { get { return _new; } set { _new = value; } }
    }

}
