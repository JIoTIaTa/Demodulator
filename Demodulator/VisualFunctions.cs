using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Windows.Forms;

namespace demodulation
{
    public abstract class DataVisual
    {
        public FFT_data_display data_type { get; protected set; }
        public int data_length { get; protected set; }
        public int FFT_deep { get; protected set; }
        public DataVisual(ref Complex[] visual, FFT_data_display type, int deep)
        {
            data_type = type;
            FFT_deep = deep;
        }
    }
    sealed public class inDataVisual : DataVisual
    {
        public inDataVisual(ref Complex[] visual, FFT_data_display data_type,  int FFT_deep)
            : base(ref visual, data_type,  FFT_deep)
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
        public expDataVisual(ref Complex[] visual, FFT_data_display data_type,  int FFT_deep)
            : base(ref visual, data_type, FFT_deep)
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
        public detectDataVisual(ref Complex[] visual, FFT_data_display data_type,  int FFT_deep)
            : base(ref visual, data_type,  FFT_deep)
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
        public shiftDataVisual(ref Complex[] visual, FFT_data_display data_type,  int FFT_deep)
            : base(ref visual, data_type,  FFT_deep)
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
        public filterDataVisual(ref Complex[] visual, FFT_data_display data_type,  int FFT_deep)
            : base(ref visual, data_type,  FFT_deep)
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
    public class VisuaslFactory
    {
        public DataVisual CreateVisual(ref Complex[] visual, FFT_data_display data_type, int FFT_deep)
        {
            switch (data_type)
            {
                case FFT_data_display.INPUT: return new inDataVisual(ref visual, data_type, FFT_deep);
                case FFT_data_display.EXPONENT: return new expDataVisual(ref visual, data_type, FFT_deep);
                case FFT_data_display.SHIFTING: return new shiftDataVisual(ref visual, data_type, FFT_deep);
                case FFT_data_display.DETECTED: return new detectDataVisual(ref visual, data_type, FFT_deep);
                case FFT_data_display.FILTERING: return new filterDataVisual(ref visual, data_type, FFT_deep);
            }
            throw new NotImplementedException();
        }
    }
}
