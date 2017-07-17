using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace demodulation
{
    public abstract class DataVisual
    {
        public Demodulator.FFT_data_display data_type { get; protected set; }
        public int data_length { get; protected set; }
        public int FFT_deep { get; protected set; }
        public DataVisual(ref Complex[] visual, Demodulator.FFT_data_display type, int length, int deep)
        {
            data_type = type;
            data_length = length;
            FFT_deep = deep;
        }
    }
    sealed public class inDataVisual : DataVisual
    {
        public inDataVisual(ref Complex[] visual, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
            : base(ref visual, data_type, length, FFT_deep)
        {
            for (int k = 0; k < length; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_inData.iq[k].i, Demodulator.IQ_inData.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }
    }
    sealed public class expDataVisual : DataVisual
    {
        public expDataVisual(ref Complex[] visual, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
            : base(ref visual, data_type, length, FFT_deep)
        {
            for (int k = 0; k < length; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_elevated.iq[k].i, Demodulator.IQ_elevated.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }

    }
    sealed public class detectDataVisual : DataVisual
    {
        public detectDataVisual(ref Complex[] visual, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
            : base(ref visual, data_type, length, FFT_deep)
        {
            for (int k = 0; k < length; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_detected.iq[k].i, Demodulator.IQ_detected.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }

    }
    sealed public class shiftDataVisual : DataVisual
    {
        public shiftDataVisual(ref Complex[] visual, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
            : base(ref visual, data_type, length, FFT_deep)
        {
            for (int k = 0; k < length; k++)
            {
                for (int i = 0; i < length; i++)
                    visual[k] = new Complex(Demodulator.IQ_shifted.iq[k].i, Demodulator.IQ_shifted.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }

    }
    sealed public class filterDataVisual : DataVisual
    {
        public filterDataVisual(ref Complex[] visual, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
            : base(ref visual, data_type, length, FFT_deep)
        {
            for (int k = 0; k < length; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_filtered.iq[k].i, Demodulator.IQ_filtered.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }
    }
    public class VisuaslFactory
    {
        public DataVisual CreateVisual(ref Complex[] visual, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
        {
            switch (data_type)
            {
                case Demodulator.FFT_data_display.INPUT: return new inDataVisual(ref visual, data_type, length, FFT_deep);
                case Demodulator.FFT_data_display.EXPONENT: return new expDataVisual(ref visual, data_type, length, FFT_deep);
                case Demodulator.FFT_data_display.SHIFTING: return new shiftDataVisual(ref visual, data_type, length, FFT_deep);
                case Demodulator.FFT_data_display.DETECTED: return new detectDataVisual(ref visual, data_type, length, FFT_deep);
                case Demodulator.FFT_data_display.FILTERING: return new filterDataVisual(ref visual, data_type, length, FFT_deep);
            }
            throw new NotImplementedException();
        }
    }


    /// <summary>Клас, що реалізує функцію заповнення комплексного масиву даними для відображення</summary>
    sealed public class FFT_array_creating
    {
        /// <summary> Функція зоповнення комплексного масиву </summary>
        /// <param name="visual_data">Посилання на комплексний масив, який буде використаний ШПФ</param>
        /// <param name="data_type">Тип даних, над якими буде проводитися ШПФ</param>
        /// <param name="length">Довжина I = Q відліків, над якими буде проводитися ШПФ </param>
        /// 
        public Complex[] create_shifted(Complex[] visual_data, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
        {
            visual_data = new Complex[length * 4];
            for (int k = 0; k < length; k++)
            {                
                visual_data[k] = new Complex(Demodulator.IQ_shifted.iq[k].i, Demodulator.IQ_shifted.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual_data[k] = new Complex(0, 0);
            }
            return visual_data;
        }
        public Complex[] create_elevated(Complex[] visual_data, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
        {
            visual_data = new Complex[length * 4];
            for (int k = 0; k < length; k++)
            {
                visual_data[k] = new Complex(Demodulator.IQ_elevated.iq[k].i, Demodulator.IQ_elevated.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual_data[k] = new Complex(0, 0);
            }
            return visual_data;
        }
        public Complex[] create_detected(Complex[] visual_data, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
        {
            visual_data = new Complex[length * 4];
            for (int k = 0; k < length; k++)
            {
                visual_data[k] = new Complex(Demodulator.IQ_detected.iq[k].i, Demodulator.IQ_detected.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual_data[k] = new Complex(0, 0);
            }
            return visual_data;
        }
        public Complex[] create_input(Complex[] visual_data, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
        {
            visual_data = new Complex[length * 4];
            for (int k = 0; k < length; k++)
            {
                visual_data[k] = new Complex(Demodulator.IQ_inData.iq[k].i, Demodulator.IQ_inData.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual_data[k] = new Complex(0, 0);
            }
            return visual_data;
        }
        public Complex[] create_filtered(Complex[] visual_data, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
        {
            visual_data = new Complex[length * 4];
            for (int k = 0; k < length; k++)
            {
                visual_data[k] = new Complex(Demodulator.IQ_filtered.iq[k].i, Demodulator.IQ_filtered.iq[k].q);
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual_data[k] = new Complex(0, 0);
            }
            return visual_data;
        }
        //private Dictionary<string, Func<double, double, double>> _operations = new Dictionary<string, Func<double, double, double>>
        //{ }
        public void create(ref Complex[] visual_data, Demodulator.FFT_data_display data_type, int length, int FFT_deep)
        {
            switch (data_type)
            {
                case Demodulator.FFT_data_display.SHIFTING:
                    {
                        for (int k = 0; k < length; k++)
                        {
                             visual_data[k] = new Complex(Demodulator.IQ_shifted.iq[k].i, Demodulator.IQ_shifted.iq[k].q);
                        }
                    }
                    break;
                case Demodulator.FFT_data_display.EXPONENT:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            visual_data[k] = new Complex(Demodulator.IQ_elevated.iq[k].i, Demodulator.IQ_elevated.iq[k].q);
                        }
                    }
                    break;
                case Demodulator.FFT_data_display.DETECTED:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            visual_data[k] = new Complex(Demodulator.IQ_detected.iq[k].i, Demodulator.IQ_detected.iq[k].q);
                        }
                    }
                    break;
                case Demodulator.FFT_data_display.FILTERING:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            visual_data[k] = new Complex(Demodulator.IQ_filtered.iq[k].i, Demodulator.IQ_filtered.iq[k].q);
                        }
                    }
                    break;
                case Demodulator.FFT_data_display.INPUT:
                    {
                        for (int k = 0; k < length; k++)
                        {
                            visual_data[k] = new Complex(Demodulator.IQ_inData.iq[k].i, Demodulator.IQ_inData.iq[k].q);
                        }
                    }
                    break;
                default:
                    break;
            }
            for (int k = length; k < FFT_deep; k++)
            {
                visual_data[k] = new Complex(0, 0);
            }
        }
    }
}
