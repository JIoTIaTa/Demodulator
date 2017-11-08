using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Windows.Forms;

namespace demodulation
{
   
    sealed public class VisualData_new
    {
        private sIQData IQ_data;
        public VisualData_new(ref Complex[] visual, int FFT_deep, ref byte[] data)
        {
            IQ_data.bytes = data;
            if (data.Length / 4 <= FFT_deep)
            {
                for (int k = 0; k < IQ_data.bytes.Length / 4; k++)
                {
                    visual[k] = new Complex(IQ_data.iq[k].i, IQ_data.iq[k].q);
                }
                for (int k = IQ_data.bytes.Length / 4; k < FFT_deep; k++)
                {
                    visual[k] = new Complex(0, 0);
                }
            }
            else
            {
                for (int k = 0; k < FFT_deep; k++)
                {
                    visual[k] = new Complex(IQ_data.iq[k].i, IQ_data.iq[k].q);
                }
            }
        }
    }
    /// <summary>/// Фабрика створення комплексного масиву для відображення/// </summary>
    public class VisuaslFactory_FFT
    {
        public Demodulator dem_functions;     
        public void Create(ref Complex[] visual, int FFT_deep)
        {
            switch (dem_functions.display)
            {
                case FFT_data_display.SHIFTING:
                    new VisualData_new(ref visual, FFT_deep, ref dem_functions.IQ_shifted.bytes);
                    break;
                case FFT_data_display.FILTERING:
                    new VisualData_new(ref visual, FFT_deep, ref dem_functions.IQ_filtered.bytes);
                    break;
                case FFT_data_display.INPUT:
                    new VisualData_new(ref visual, FFT_deep, ref dem_functions.IQ_inData.bytes);
                    break;
                default:
                    break;
            }
        }
    }


    /// <summary>/// Проба зробити абстрактну фабрику для відображення сигнального сузір'я</summary>
    sealed public class constellation_DataVisual_new 
    {
        private sIQData IQ_data;
        public constellation_DataVisual_new(ref short[] I_data, ref short[] Q_data, ref byte[] data)
        {
            IQ_data.bytes = data;
            for (int k = 0; k < IQ_data.bytes.Length / 4; k++)
            {
                I_data[k] = IQ_data.iq[k].i;
                Q_data[k] = IQ_data.iq[k].q;
            }
        }
    }

    /// <summary>Фабрика створення масиву точок для відображеня сузір'я</summary>
    public class VisuaslFactory_constellation
    {
        public Demodulator dem_functions;

        public void CreateVisual(ref short[] I_data, ref short[] Q_data, FFT_data_display data_type)
        {
            //switch (data_type)
            //{
            //    case FFT_data_display.SHIFTING:
            //        new constellation_DataVisual_new (ref I_data, ref Q_data, ref dem_functions.IQ_shifted.bytes);
            //        break;
            //    case FFT_data_display.EXPONENT:
            //        new constellation_DataVisual_new(ref I_data, ref Q_data, ref dem_functions.IQ_elevated.bytes);
            //        break;
            //    case FFT_data_display.DETECTED:
            //        new constellation_DataVisual_new(ref I_data, ref Q_data, ref dem_functions.IQ_detected.bytes);
            //        break;
            //    case FFT_data_display.FILTERING:
                    new constellation_DataVisual_new(ref I_data, ref Q_data, ref dem_functions.IQ_filtered.bytes);
                //    break;
                //case FFT_data_display.INPUT:
                //    new constellation_DataVisual_new(ref I_data, ref Q_data, ref dem_functions.IQ_shifted.bytes);
                //    break;
                //default:
                //    break;
            //}
        }
    }


}
