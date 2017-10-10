//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;

//namespace demodulation
//{
    
//    /// <summary>/// Проба зробити абстрактну фабрику для відображення сигнального сузір'я</summary>
//    public abstract class constellation_new
//    {
//        public bool GetBit(short val, int num)
//        {
//            if ((num > 15) || (num < 0))
//            {
//                throw new Exception();
//            }
//            return ((val >> num) & 1) > 0;
//        }

//        public byte SetBit(byte val, int num, bool bit)
//        {
//            if ((num > 7) || (num < 0))
//            {
//                throw new Exception();
//            }
//            byte tempVal = 1;
//            tempVal = (byte)(tempVal << num);
//            val = (byte)(val & (~tempVal));
//            if (bit)
//            {
//                val = (byte)(val | (tempVal));
//            }
//            return val;
//        }
//    }
//    sealed public class constellation_inDataVisual_new : constellation_new
//    {
//        public constellation_inDataVisual_new(ref byte[] data, int BytesPerSymbol)
//        {
//            bool value_I;
//            bool value_Q;
//            int count = 0;
//            for (int k = 0; k < Demodulator.IQ_inData.bytes.Length / 4; k++)
//            {
//                for (int j = 0; j < 16 / BytesPerSymbol; j += BytesPerSymbol)
//                {
//                    for (int i = j; i < BytesPerSymbol; i++)
//                    {
//                        value_I = GetBit(Demodulator.IQ_inData.iq[k].i, i);
//                        data[count] = SetBit(data[k], i, value_I);
//                        value_Q = GetBit(Demodulator.IQ_inData.iq[k].q, i);
//                        Q_data[count] = SetBit(Q_data[k], i, value_I);
//                        count++;
//                    }
//                }
//            }
//        }
//    }
//    sealed public class constellation_expDataVisual_new : constellation_new
//    {
//        public constellation_expDataVisual_new(ref byte[] data, int BytesPerSymbol)
//        {
//            bool value_I;
//            bool value_Q;
//            int count = 0;
//            for (int k = 0; k < Demodulator.IQ_elevated.bytes.Length / 4; k++)
//            {
//                for (int j = 0; j < 16 / BytesPerSymbol; j += BytesPerSymbol)
//                {
//                    for (int i = j; i < BytesPerSymbol; i++)
//                    {
//                        value_I = GetBit(Demodulator.IQ_elevated.iq[k].i, i);
//                        data[count] = SetBit(data[k], i, value_I);
//                        value_Q = GetBit(Demodulator.IQ_elevated.iq[k].q, i);
//                        Q_data[count] = SetBit(Q_data[k], i, value_I);
//                        count++;
//                    }
//                }
//            }
//        }
//    }
//    sealed public class constellation_shiftDataVisual_new : constellation_new
//    {
//        public constellation_shiftDataVisual_new(ref byte[] data, int BitesPerSymbol)
//        {
//            bool value_I;
//            bool value_Q;
//            int count = 0;
//            for (int ValueInBuffer = 0; ValueInBuffer < Demodulator.IQ_shifted.bytes.Length / 4; ValueInBuffer++)
//            {
//                for (int SymbolInShort = 0; SymbolInShort < 16; SymbolInShort += BitesPerSymbol)
//                {
//                    for (int BitInSymbol = 0; BitInSymbol < BitesPerSymbol; BitInSymbol++)
//                    {
//                        int BitInShort = SymbolInShort + BitInSymbol;
//                        value_I = GetBit(Demodulator.IQ_shifted.iq[ValueInBuffer].i, BitInShort);
//                        data[count] = SetBit(data[ValueInBuffer], BitInSymbol, value_I);
//                        value_Q = GetBit(Demodulator.IQ_shifted.iq[ValueInBuffer].q, BitInShort);
//                        Q_data[count] = SetBit(Q_data[ValueInBuffer], BitInSymbol, value_Q);
//                    }
//                    MessageBox.Show(string.Format("Demodulator.IQ_shifted.iq[{0}].i = {1}\nI_data[{2}] = {3}", ValueInBuffer, Demodulator.IQ_shifted.iq[ValueInBuffer].i, count, data[count]));
//                    count++;
//                }
//            }
//        }
//    }
//    sealed public class constellation_detectDataVisual_new : constellation_new
//    {
//        public constellation_detectDataVisual_new(ref byte[] data, int BytesPerSymbol)
//        {
//            bool value_I;
//            bool value_Q;
//            int count = 0;
//            for (int k = 0; k < Demodulator.IQ_detected.bytes.Length / 4; k++)
//            {
//                for (int j = 0; j < 16 / BytesPerSymbol; j += BytesPerSymbol)
//                {
//                    for (int i = j; i < BytesPerSymbol; i++)
//                    {
//                        value_I = GetBit(Demodulator.IQ_detected.iq[k].i, i);
//                        data[count] = SetBit(data[k], i, value_I);
//                        value_Q = GetBit(Demodulator.IQ_detected.iq[k].q, i);
//                        Q_data[count] = SetBit(Q_data[k], i, value_I);
//                        count++;
//                    }
//                }
//            }
//        }
//    }
//    sealed public class constellation_filterDataVisual_new : constellation_new
//    {
//        public constellation_filterDataVisual_new(ref byte[] data, int BitesPerSymbol)
//        {
//            bool value = false;
//            int count = 0;
//            for (int ValueInBuffer = 0; ValueInBuffer < Demodulator.IQ_filtered.bytes.Length / 4; ValueInBuffer++)
//            {
//                for (int SymbolInByte = 0; SymbolInByte < 8; SymbolInByte += BitesPerSymbol)
//                {
//                    for (int BitInSymbol = 0; BitInSymbol < BitesPerSymbol; BitInSymbol++)
//                    {
//                        int BitInShort = SymbolInByte + BitInSymbol;
//                        value = GetBit(Demodulator.IQ_filtered.bytes[ValueInBuffer], BitInShort);
//                        data[count] = SetBit(data[ValueInBuffer], BitInSymbol, value);
//                    }
//                    MessageBox.Show(string.Format("Demodulator.IQ_filtered.iq[{0}].i = {1}\nI_data[{2}] = {3}", ValueInBuffer, Demodulator.IQ_filtered.iq[ValueInBuffer].i, count, data[count]));
//                    count++;
//                }
//            }
//        }
//    }
//    /// <summary>Фабрика створення масиву точок для відображеня сузір'я</summary>
//    public class VisuaslFactory_constellation_new
//    {
//        public byte[] data;
//        private int BytesPerSymbol = 0;

//        public VisuaslFactory_constellation_new(FFT_data_display data_type, int modulation_multiplicity)
//        {
//            BytesPerSymbol = modulation_multiplicity;
//            switch (data_type)
//            {
//                case FFT_data_display.SHIFTING:
//                    data = new byte[(Demodulator.IQ_shifted.bytes.Length) * (8 / BytesPerSymbol)];
//                    new constellation_shiftDataVisual_new(ref data, modulation_multiplicity);
//                    break;
//                case FFT_data_display.EXPONENT:
//                    data = new byte[(Demodulator.IQ_elevated.bytes.Length) * (8 / BytesPerSymbol)];
//                    new constellation_expDataVisual_new(ref data, modulation_multiplicity);
//                    break;
//                case FFT_data_display.DETECTED:
//                    data = new byte[(Demodulator.IQ_detected.bytes.Length) * (8 / BytesPerSymbol)];
//                    new constellation_detectDataVisual_new(ref data, modulation_multiplicity);
//                    break;
//                case FFT_data_display.FILTERING:
//                    data = new byte[(Demodulator.IQ_filtered.bytes.Length) * (8 / BytesPerSymbol)];
//                    new constellation_filterDataVisual_new(ref data, modulation_multiplicity);
//                    break;
//                case FFT_data_display.INPUT:
//                    data = new byte[(Demodulator.IQ_inData.bytes.Length) * (8 / BytesPerSymbol)];
//                    new constellation_inDataVisual_new(ref data, modulation_multiplicity);
//                    break;
//                default:
//                    break;
//            }
//        }
//    }
//}
