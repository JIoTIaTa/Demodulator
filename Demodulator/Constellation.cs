using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace demodulation
{
    public partial class Constellation : Form
    {
        public sIQData IQ_signal;
        static int size = 65536;
        public Constellation()
        {
            InitializeComponent();
        }
        //public void BeginDisplay(byte[] inData)
        //{
        //    try
        //    {

        //        if (size != inData.Length / 4)
        //        {
        //            size = inData.Length / 4;
        //            Array.Resize(ref IQ_signal.bytes, size * 4);
        //            Array.Resize(ref I_data, size);
        //            Array.Resize(ref Q_data, size);
        //        }
        //        //Array.Clear(IQ_signal.bytes, 0, IQ_signal.bytes.Length - 1);
        //        Array.Copy(inData, IQ_signal.bytes, inData.Length);
        //        //IQ_signal.bytes = inData;
        //        for (int i = 0; i < inData.Length / 4; i++)
        //        {
        //            I_data[i] = IQ_signal.iq[i].i;
        //            Q_data[i] = IQ_signal.iq[i].q;
        //        }
        //        Mitov_constellation.Channels[0].Data.Clear();
        //        Mitov_constellation.Channels[0].Data.SetXYData(I_data, Q_data);
        //    }
        //    catch
        //    {
        //    }
        //}
        public void BeginDisplay(ref short[] I_data, ref short[] Q_data)
        {
            try
            {        
                Mitov_constellation.Channels[0].Data.Clear();
                Mitov_constellation.Channels[0].Data.SetXYData(I_data, Q_data);
            }
            catch
            {
            }
        }

        private void checkBox_constellation_CheckedChanged(object sender, EventArgs e)
        {
            Quadrature_AM_demodulatorSPARKInterface.display_constellation = checkBox_constellation.Checked;
        }
    }
}
