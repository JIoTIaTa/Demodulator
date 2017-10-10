using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace demodulation
{
    public class Detector
    {
        const double Pi = 3.14159d; // 180
        const double Pi_by_2 = 1.570795d; // 90
        const double Pi_by_4 = 0.7853975d; // 45
        private sIQData IQ_inData;
        public byte[] detection_data;
        private modulation_type phase_type;
        private int IQ_length;
        byte alphabet;

        public Detector(int inData_lenght, modulation_type modulation_type)
        {
            phase_type = modulation_type;
            IQ_length = inData_lenght / 4;
            IQ_inData.bytes = new byte[inData_lenght];
            detection_data = new byte[IQ_length];
        }
        public void ReInit(int new_inData_length, modulation_type modulation_type)
        {
            IQ_length = new_inData_length / 4;
            Array.Resize(ref IQ_inData.bytes, new_inData_length);
            Array.Resize(ref detection_data, IQ_length);
        }
        public byte[] detection(byte[] inData)
        {
            IQ_inData.bytes = inData;
            double instantaneous_phase = 0.0d;
            for (int i = 0; i < IQ_length; i++)
            {
                double I = IQ_inData.iq[i].i;
                double Q = IQ_inData.iq[i].q;

                //1 | 2 | 7 | 8 октанти
                if (IQ_inData.iq[i].i >= 0)
                {
                    //1|2 октанти
                    if (IQ_inData.iq[i].q > 0)
                    {
                        //1 октант
                        if (Math.Abs(IQ_inData.iq[i].i) > Math.Abs(IQ_inData.iq[i].q))
                        {
                            instantaneous_phase = arctg__1_8_oct(IQ_inData.iq[i]);
                        }
                        //2 октант
                        else
                        {
                            instantaneous_phase = arctg__2_3_oct(IQ_inData.iq[i]);
                        }
                    }
                    //7|8 октанти
                    else
                    {
                        //8 октант
                        if (Math.Abs(IQ_inData.iq[i].i) > Math.Abs(IQ_inData.iq[i].q))
                        {
                            instantaneous_phase = arctg__1_8_oct(IQ_inData.iq[i]);
                            if (instantaneous_phase < 0) { instantaneous_phase += 2 * Pi; }
                        }
                        //7 октант
                        else
                        {
                            instantaneous_phase = arctg__6_7_oct(IQ_inData.iq[i]);
                            if (instantaneous_phase < 0) { instantaneous_phase += 2 * Pi; }
                        }
                    }
                }
                //3|4|5|6 октанти
                else
                {
                    //4|3 октанти
                    if (IQ_inData.iq[i].q > 0)
                    {
                        //3 октант
                        if (Math.Abs(IQ_inData.iq[i].i) > Math.Abs(IQ_inData.iq[i].q))
                        {
                            instantaneous_phase = arctg__2_3_oct(IQ_inData.iq[i]);
                            //if (detection_data[i] < 0) { detection_data[i] += Pi; }
                        }
                        //4 октант
                        else
                        {
                            instantaneous_phase = arctg__4_5_oct(IQ_inData.iq[i]);
                            //if (detection_data[i] < 0) { detection_data[i] += Pi; }
                        }
                    }
                    //5|6 октанти
                    else
                    {
                        //5 октант
                        if (Math.Abs(IQ_inData.iq[i].i) > Math.Abs(IQ_inData.iq[i].q))
                        {
                            instantaneous_phase = arctg__4_5_oct(IQ_inData.iq[i]);
                            if (instantaneous_phase < 0) { instantaneous_phase += 2 * Pi; }
                        }
                        //6 октант
                        else
                        {
                            instantaneous_phase = arctg__6_7_oct(IQ_inData.iq[i]);
                            if (instantaneous_phase < 0) { instantaneous_phase += 2 * Pi; }
                        }
                    }
                }
                //if (detection_data[i] + Pi_by_8 > Pi) { detection_data[i] = detection_data[i] - Pi + Pi_by_8; } else { detection_data[i] += Pi_by_8; } // зміщую на 45 градусів, щоб вірно відобразити сузір'я
                switch (phase_type)
                {
                    case modulation_type.PSK_2:
                        if (instantaneous_phase >= Pi) { alphabet = 0; } // 0 - 180
                        else { alphabet = 1; } // 180 - 360
                        break;
                    case modulation_type.PSK_4:
                        if (instantaneous_phase >= 0 & instantaneous_phase < Pi_by_2) { alphabet = 0; } // 0 - 90
                        if (instantaneous_phase >= Pi_by_2 & instantaneous_phase < Pi) { alphabet = 1; } // 90 - 180
                        if (instantaneous_phase >= Pi & instantaneous_phase < Pi + Pi_by_2) { alphabet = 2; } // 180 - 270 
                        if (instantaneous_phase >= Pi + Pi_by_2 & instantaneous_phase < 2 * Pi) { alphabet = 3; } // 270 - 360
                        break;
                    case modulation_type.PSK_8:
                        if (instantaneous_phase >= 0 & instantaneous_phase < Pi_by_4) { alphabet = 0; } // 0 - 45
                        if (instantaneous_phase >= Pi_by_4 & instantaneous_phase < Pi_by_2) { alphabet = 1; } // 45 - 90
                        if (instantaneous_phase >= Pi_by_2 & instantaneous_phase < Pi_by_2 + Pi_by_4) { alphabet = 2; } // 90 - 135
                        if (instantaneous_phase >= Pi_by_2 + Pi_by_4 & instantaneous_phase < Pi) { alphabet = 3; } // 135 - 180
                        if (instantaneous_phase >= Pi & instantaneous_phase < Pi + Pi_by_4) { alphabet = 4; } // 180 - 225
                        if (instantaneous_phase >= Pi + Pi_by_4 & instantaneous_phase < Pi + Pi_by_2) { alphabet = 5; } // 225 - 270
                        if (instantaneous_phase >= Pi + Pi_by_2 & instantaneous_phase < Pi + Pi_by_2 + Pi_by_4) { alphabet = 6; } // 270 - 315
                        if (instantaneous_phase >= Pi + Pi_by_2 + Pi_by_4 & instantaneous_phase < 2 * Pi) { alphabet = 7; } // 315 - 360
                        break;
                    case modulation_type.QAM_16:
                        break;
                    default:
                        break;
                }
                detection_data[i] = alphabet;
            }
            return detection_data;
        }
        /// <summary>Визначення арктангенсу для 1 і 8 остантів</summary>
        private double arctg__1_8_oct(iq sample)
        {
            double arct = (double)((double)(sample.i * sample.q) / ((double)(sample.i * sample.i) + mult_with_const(sample.q)));
            return arct;
        }
        /// <summary>Визначення арктангенсу для 2 і 3 остантів</summary>
        private double arctg__2_3_oct(iq sample)
        {
            //////////////////////***********************/////////////////////////////
            //double a1 = sample.q * sample.q;
            //double a2 = sample.i * sample.q;
            //double a3 = mult_with_const(sample.i);
            //double arct = Pi_by_2 - a2 / (a1 + a3);

            //////////////////////***********************/////////////////////////////
            double arct = Pi_by_2 - (double)((double)(sample.i * sample.q) / ((double)(sample.q * sample.q) + mult_with_const(sample.i)));
            return arct;
        }
        /// <summary>Визначення арктангенсу для 4 і 5 остантів</summary>
        private double arctg__4_5_oct(iq sample)
        {
            double arct = Pi + (double)((double)(sample.i * sample.q) / ((double)(sample.i * sample.i) + mult_with_const(sample.q)));
            return arct;
        }
        /// <summary>Визначення арктангенсу для 6 і 7 остантів</summary>
        private double arctg__6_7_oct(iq sample)
        {
            double arct = -Pi_by_2 - (double)((double)(sample.i * sample.q) / ((double)(sample.q * sample.q) + mult_with_const(sample.i)));
            return arct;
        }
        /// <summary>Піднесення в квадрат та помноження на константу 0.28125 побітовим зсувом</summary>
        private short mult_with_const(short value)
        {
            short new_value;
            int elevated = value * value; // value^2
            int value1 = elevated >> 2; // value^2 / 4
            int value2 = elevated >> 5; // value^2 / 32
            new_value = (short)(value1 + value2); // value^2 * 0.28125
            return new_value;
        }
    }
}
