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
}
