using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace demodulation
{
    class Gardner_detector
    {
        Calculate_Modulation_Speed_Error ms_I_error;
        Calculate_Modulation_Speed_Error ms_Q_error;
        PhaseShift_finder phase_finder;
        private sIQData IQ_signal;
        private int SymbolsPerPeriod = 0;
        private int begin_phase_I = 0;
        private int begin_phase_Q = 0;
        private int take_symbol_I = 0;
        private int take_symbol_Q = 0;
        private double real_pos_I = 0.0d;
        private double real_pos_Q = 0.0d;
        private float speed_error_I = 0.0f;
        private float speed_error_Q = 0.0f;
        private float error = 0.0f;
        private float SymbolsPerSapmle;
        private int SPS_int ;
        private int[] take_symbols_I;
        private int[] take_symbols_Q;
        private int IQ_length;

        public Gardner_detector(byte[] inData, float SymbolsPerSapmle)
        {
            IQ_signal.bytes = inData;
            IQ_length = inData.Length / 4;
            ms_I_error = new Calculate_Modulation_Speed_Error();
            ms_Q_error = new Calculate_Modulation_Speed_Error();
            this.SymbolsPerSapmle = SymbolsPerSapmle;
            SymbolsPerPeriod = (int)SymbolsPerSapmle * 2;
            SPS_int = (int)Math.Ceiling(this.SymbolsPerSapmle);
            take_symbols_I = new int[(int)(IQ_length / SymbolsPerSapmle)];
            take_symbols_Q = new int[(int)(IQ_length / SymbolsPerSapmle)];
        }
        public void BeginPhaseCalc()
        {
            try
            {
                int bytesPerPample = (int)(Math.Ceiling(SymbolsPerSapmle) * 4);
                sIQData IQ_firstSample;
                byte[] firstSample = new byte[bytesPerPample];
                IQ_firstSample.bytes = firstSample;
                for (int i = 0; i < IQ_firstSample.bytes.Length; i++)
                {
                    IQ_firstSample.bytes[i] = IQ_signal.bytes[i];
                }
                phase_finder = new PhaseShift_finder(IQ_firstSample.bytes);
                begin_phase_I = phase_finder.I_shift();
                begin_phase_Q = phase_finder.Q_shift();
                real_pos_I = begin_phase_I;
                real_pos_Q = begin_phase_Q;
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Gardner_detector.BeginPhaseCalc");
                throw;
            }
        }

        public int[] take_I()
        {
            try
            {
                int Tick = 0;
                do
                {
                    int start_Symbol_I = 0;
                    int midle_Symbol_I = 0;
                    int end_Symbol_I = 0;
                    double end_Symbol_I_WithPreverError = 0.0d;
                    short[] temp_I = new short[3];
                    start_Symbol_I = (int)Math.Round(real_pos_I);
                    midle_Symbol_I = (int)(Math.Round(real_pos_I + (SymbolsPerSapmle / 2)));
                    end_Symbol_I_WithPreverError = real_pos_I + SymbolsPerSapmle;
                    end_Symbol_I = (int)(Math.Round(end_Symbol_I_WithPreverError));
                    temp_I[0] = IQ_signal.iq[start_Symbol_I].i;
                    temp_I[1] = IQ_signal.iq[midle_Symbol_I].i;
                    temp_I[2] = IQ_signal.iq[end_Symbol_I].i;
                    speed_error_I = ms_I_error.Error_calc(temp_I);
                    if (begin_phase_I != 0) { speed_error_I = (float)(speed_error_I - (end_Symbol_I - end_Symbol_I_WithPreverError)); real_pos_I = end_Symbol_I_WithPreverError - speed_error_I; }
                    else { real_pos_I = end_Symbol_I_WithPreverError; }
                    take_symbol_I = (int)Math.Round(real_pos_I);
                    begin_phase_I = take_symbol_I;
                    take_symbols_I[Tick] = take_symbol_I;
                    Tick++;
                } while (Tick < take_symbols_I.Length);
                return take_symbols_I;
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Gardner_detector.take_I");
                throw;
            }
        }
        public int[] take_Q()
        {
            try
            {
                int Tick = 0;
                do
                {
                    int start_Symbol_Q = 0;
                    int midle_Symbol_Q = 0;
                    int end_Symbol_Q = 0;
                    double end_Symbol_Q_WithPreverError = 0.0d;
                    short[] temp_Q = new short[3];
                    start_Symbol_Q = (int)Math.Round(real_pos_Q);
                    midle_Symbol_Q = (int)(Math.Round(real_pos_Q + (SymbolsPerSapmle / 2)));
                    end_Symbol_Q_WithPreverError = real_pos_Q + SymbolsPerSapmle;
                    end_Symbol_Q = (int)(Math.Round(end_Symbol_Q_WithPreverError));
                    temp_Q[0] = IQ_signal.iq[start_Symbol_Q].q;
                    temp_Q[1] = IQ_signal.iq[midle_Symbol_Q].q;
                    temp_Q[2] = IQ_signal.iq[end_Symbol_Q].q;
                    speed_error_Q = ms_Q_error.Error_calc(temp_Q);
                    error = speed_error_I + speed_error_Q;
                    if (begin_phase_Q != 0) { speed_error_Q = (float)(speed_error_Q - (end_Symbol_Q - end_Symbol_Q_WithPreverError)); real_pos_Q = end_Symbol_Q_WithPreverError - speed_error_Q; }
                    else { real_pos_Q = end_Symbol_Q_WithPreverError; }
                    take_symbol_Q = (int)Math.Round(real_pos_Q);
                    begin_phase_Q = take_symbol_Q;
                    take_symbols_Q[Tick] = take_symbol_Q;
                    Tick++;
                } while (Tick < take_symbols_Q.Length);
                return take_symbols_Q;

            }
            catch (Exception)
            {
                MessageBox.Show("Error: Gardner_detector.take_Q");
                throw;
            }
        }
    }
    sealed class Calculate_Modulation_Speed_Error
    {
        private short[] symbols = new short[3];
        private float MS_error_prev = 1;
        public float normalize_coef = 0.0f;
        public float Error_calc(short[] symbols)
        {
            try
            {
                float normalize_coef = 1f;
                this.symbols = symbols;
                float MS_error = 0;
                int temp = ((this.symbols[2] - this.symbols[0]) * this.symbols[1]);
                while ((int)(temp * normalize_coef) != 0)
                {
                    normalize_coef = normalize_coef / 10;
                }
                this.normalize_coef = normalize_coef;
                MS_error = normalize_coef * temp;
                MS_error_prev = MS_error;
                return MS_error;
            }
            catch
            {
                MessageBox.Show("Error: Gardner_detector.Calculate_Modulation_Speed_Error");
                return 0;
            }
        }
    }
    sealed class PhaseShift_finder
    {
        private sIQData IQ_first_symbol;
        public PhaseShift_finder(byte[] inData)
        {
            IQ_first_symbol.bytes = inData;
        }
        public int I_shift()
        {
            try
            {
                int begin_phase = 0;
                float max_value = 0;
                for (int i = 0; i < IQ_first_symbol.bytes.Length / 4; i++)
                {
                    if (max_value < IQ_first_symbol.iq[i].i)
                    {
                        max_value = IQ_first_symbol.iq[i].i;
                        begin_phase = i;
                    }
                }
                return begin_phase;
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Gardner_detector.PhaseShift_finder.I_shift");
                throw;
            }
        }
        public int Q_shift()
        {
            try
            {
                int begin_phase = 0;
                float max_value = 0;
                for (int i = 0; i < IQ_first_symbol.bytes.Length / 4; i++)
                {
                    if (max_value < IQ_first_symbol.iq[i].q)
                    {
                        max_value = IQ_first_symbol.iq[i].q;
                        begin_phase = i;
                    }
                }
                return begin_phase;
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Gardner_detector.PhaseShift_finder.Q_shift");
                throw;
            }
        }
    }
}
