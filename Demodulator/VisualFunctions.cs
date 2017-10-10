using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Windows.Forms;

namespace demodulation
{
    /// <summary>/// Проба зробити абстрактну фабрику для відображення ШПФ</summary>
    public abstract class DataVisual
    {
        public FFT_data_display data_type { get; protected set; }
        public int data_length { get; protected set; }
        public int FFT_deep { get; protected set; }
        public DataVisual(ref Complex[] visual,  int deep)
        {
            FFT_deep = deep;
        }
    }
    sealed public class inDataVisual : DataVisual
    {
        public inDataVisual(ref Complex[] visual,   int FFT_deep)
            : base(ref visual, FFT_deep)
        {
            for (int k = 0; k < Demodulator.IQ_inData.bytes.Length / 4; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_inData.iq[k].i, Demodulator.IQ_inData.iq[k].q);
            }
            for (int k = Demodulator.IQ_inData.bytes.Length / 4;  k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }
    }
    sealed public class expDataVisual : DataVisual
    {
        public expDataVisual(ref Complex[] visual,   int FFT_deep)
            : base(ref visual,  FFT_deep)
        {
            for (int k = 0; k < Demodulator.IQ_elevated.bytes.Length / 4; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_elevated.iq[k].i, Demodulator.IQ_elevated.iq[k].q);
            }
            for (int k = Demodulator.IQ_elevated.bytes.Length / 4;  k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }

    }
    sealed public class detectDataVisual : DataVisual
    {
        public detectDataVisual(ref Complex[] visual,   int FFT_deep)
            : base(ref visual,  FFT_deep)
        {
            for (int k = 0; k < Demodulator.IQ_detected.bytes.Length / 4; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_detected.iq[k].i, Demodulator.IQ_detected.iq[k].q);
            }
            for (int k = Demodulator.IQ_detected.bytes.Length / 4; k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }

    }
    sealed public class shiftDataVisual : DataVisual
    {
        public shiftDataVisual(ref Complex[] visual,  int FFT_deep)
            : base(ref visual,   FFT_deep)
        {
            for (int k = 0; k < Demodulator.IQ_shifted.bytes.Length / 4; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_shifted.iq[k].i, Demodulator.IQ_shifted.iq[k].q);
            }
            for (int k = Demodulator.IQ_shifted.bytes.Length / 4; k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }

    }
    sealed public class filterDataVisual : DataVisual
    {
        public filterDataVisual(ref Complex[] visual,  int FFT_deep)
            : base(ref visual,  FFT_deep)
        {
            for (int k = 0; k < Demodulator.IQ_filtered.bytes.Length / 4; k++)
            {
                visual[k] = new Complex(Demodulator.IQ_filtered.iq[k].i, Demodulator.IQ_filtered.iq[k].q);
            }
            for (int k = Demodulator.IQ_filtered.bytes.Length / 4; k < FFT_deep; k++)
            {
                visual[k] = new Complex(0, 0);
            }
        }
    }
    /// <summary>/// Фабрика створення комплексного масиву для відображення/// </summary>
    public class VisuaslFactory_FFT
    {
        public DataVisual CreateVisual(ref Complex[] visual, FFT_data_display data_type, int FFT_deep)
        {
            switch (data_type)
            {
                case FFT_data_display.INPUT: return new inDataVisual(ref visual, FFT_deep);
                case FFT_data_display.EXPONENT: return new expDataVisual(ref visual, FFT_deep);
                case FFT_data_display.SHIFTING: return new shiftDataVisual(ref visual,  FFT_deep);
                case FFT_data_display.DETECTED: return new detectDataVisual(ref visual,  FFT_deep);
                case FFT_data_display.FILTERING: return new filterDataVisual(ref visual,  FFT_deep);
            }
            throw new NotImplementedException();
        }
    }


    /// <summary>/// Проба зробити абстрактну фабрику для відображення сигнального сузір'я</summary>
    sealed public class constellation_inDataVisual
    {
        public constellation_inDataVisual(ref short[] I_data, ref short[] Q_data)
        {
            for (int k = 0; k < Demodulator.IQ_inData.bytes.Length / 4; k++)
            {
                I_data[k] = Demodulator.IQ_inData.iq[k].i;
                Q_data[k] = Demodulator.IQ_inData.iq[k].q;
            }
        }
    }
    sealed public class constellation_expDataVisual
    {
        public constellation_expDataVisual(ref short[] I_data, ref short[] Q_data)
        {
            for (int k = 0; k < Demodulator.IQ_elevated.bytes.Length / 4; k++)
            {
                I_data[k] = Demodulator.IQ_elevated.iq[k].i;
                Q_data[k] = Demodulator.IQ_elevated.iq[k].q;
            }
        }
    }
    sealed public class constellation_shiftDataVisual
    {
        public constellation_shiftDataVisual(ref short[] I_data, ref short[] Q_data)
        {
            for (int k = 0; k < Demodulator.IQ_shifted.bytes.Length / 4; k++)
            {
                I_data[k] = Demodulator.IQ_shifted.iq[k].i;
                Q_data[k] = Demodulator.IQ_shifted.iq[k].q;
            }
        }
    }
    sealed public class constellation_detectDataVisual
    {
        public constellation_detectDataVisual(ref short[] I_data, ref short[] Q_data)
        {
            for (int k = 0; k < Demodulator.IQ_detected.bytes.Length / 4; k++)
            {
                I_data[k] = Demodulator.IQ_detected.iq[k].i;
                Q_data[k] = Demodulator.IQ_detected.iq[k].q;
            }
        }
    }
    sealed public class constellation_filterDataVisual
    {
        public constellation_filterDataVisual(ref short[] I_data, ref short[] Q_data)
        {
            for (int k = 0; k < Demodulator.IQ_filtered.bytes.Length / 4; k++)
            {
                I_data[k] = Demodulator.IQ_filtered.iq[k].i;
                Q_data[k] = Demodulator.IQ_filtered.iq[k].q;
            }
        }
    }

    /// <summary>Фабрика створення масиву точок для відображеня сузір'я</summary>
    public class VisuaslFactory_constellation
    {
        public short[] I_data;
        public short[] Q_data;

        public VisuaslFactory_constellation(FFT_data_display data_type)
        {
            switch (data_type)
            {
                case FFT_data_display.SHIFTING:
                    I_data = new short[Demodulator.IQ_shifted.bytes.Length / 4];
                    Q_data = new short[Demodulator.IQ_shifted.bytes.Length / 4];
                    new constellation_shiftDataVisual(ref I_data, ref Q_data);
                    break;
                case FFT_data_display.EXPONENT:
                    I_data = new short[Demodulator.IQ_elevated.bytes.Length / 4];
                    Q_data = new short[Demodulator.IQ_elevated.bytes.Length / 4];
                    new constellation_expDataVisual(ref I_data, ref Q_data);
                    break;
                case FFT_data_display.DETECTED:
                    I_data = new short[Demodulator.IQ_detected.bytes.Length / 4];
                    Q_data = new short[Demodulator.IQ_detected.bytes.Length / 4];
                    new constellation_detectDataVisual(ref I_data, ref Q_data);
                    break;
                case FFT_data_display.FILTERING:
                    I_data = new short[Demodulator.IQ_filtered.bytes.Length / 4];
                    Q_data = new short[Demodulator.IQ_filtered.bytes.Length / 4];
                    new constellation_filterDataVisual(ref I_data, ref Q_data);
                    break;
                case FFT_data_display.INPUT:
                    I_data = new short[Demodulator.IQ_inData.bytes.Length / 4];
                    Q_data = new short[Demodulator.IQ_inData.bytes.Length / 4];
                    new constellation_inDataVisual(ref I_data, ref Q_data);
                    break;
                default:
                    break;
            }
        }
    }


}
