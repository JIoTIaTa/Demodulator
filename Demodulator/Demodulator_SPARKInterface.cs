using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Interface;
//using signal_tester;
using System.Windows.Forms;
using TXT_writter;
//using Filter;

namespace demodulation
{

    public class Quadrature_AM_demodulatorSPARKInterface : IDecoder
    {
       
        private bool _busy;
        private double _incom = 0; // вхідні відліки в байтах
        private double _outcom = 0; // вихідні відліки в байтах
        private double errorsNumber = 0; // кількість зривів
        public Demodulator dem_functions = new Demodulator(); // обьєкт класу
        private byte[] outData = new byte[524288]; // масив відфільтрованих даних
        private Demodulator_DoubleClick_Form doubleClick_Form; // форма, що відкривається при подвійному кліку на блок
        private long SR = 9999999; // частота дискретизації (вхідного сигналу)
        private long F = 5555555; // центральна частота (вхідного сигналу)
        private string info; // строка виведення інформації в вікні СПАРК
        public DemodulatorVisual_Form visual_Form; // форма візуалізації         
        public static bool calculate_parametrs_bool = true;
        private change inDataLength_change = new change(); // для визначення, чи змінилась довжинавхідних даних
        public int[] demodulate_I_unit;
        public int[] demodulate_Q_unit;

        public string Name
        {
            get { return "Демодулятор"; }
        }

        public string Version
        {
            get { return "beta"; }
        }

        public string Author
        {
            get { return "Dimka Kom"; }
        }

        public int Id { get; set; }

        public ModData GetCurInfo
        {
            get
            {
                var m = new ModData
                {
                    Incoming = _incom/1024,
                    Outcoming = _outcom/1024,
                    Zriv = errorsNumber,
                    Nastr = info,
                };
                return m;
            }
        }

        public bool Busy
        {
            get { return _busy; }
            set { _busy = value; }
        }

        public string Init()
        {
            //if (dem_functions.demodulator_busy == true)
            //{
                if (visual_Form == null || visual_Form.IsDisposed)
                {
                    visual_Form = new DemodulatorVisual_Form() { dem_functions = dem_functions };
                    visual_Form.Show();
                }
                else
                {
                    visual_Form.Focus();
                }
            //}
            //else
            //{
            //    MessageBox.Show("Can't do it, bro");
            //}
            return "Демодулятор";
        }

        public void Info()
        {
            MessageBox.Show("\nДемодулятор");
        }

        public void Start()
        {

        }

        public void Visual()
        {            
            if (doubleClick_Form == null || doubleClick_Form.IsDisposed)
            {
                doubleClick_Form = new Demodulator_DoubleClick_Form { demodulation_functions = dem_functions };
            }
            doubleClick_Form.Show();
        }

        public void Start(string mesage, byte[] inData)
        {
            inDataLength_change.old_value = inData.Length;
            dem_functions.demodulator_busy = true;
            string outMessage = ""; // команда, що буде предана наступному модулю
            _incom += inData.Length;            
                if (!string.IsNullOrEmpty(mesage))
                {
                    var message = mesage;
                    try
                    {
                        if (message.Contains("%%SAMPLERATE&"))
                        {
                            string headerExecute_stringBuffer = message.Substring(message.LastIndexOf("%%SAMPLERATE&") + 13);
                            if (headerExecute_stringBuffer.Contains("%%")) headerExecute_stringBuffer = headerExecute_stringBuffer.Substring(0, headerExecute_stringBuffer.IndexOf("%%"));
                            SR = Convert.ToUInt32(headerExecute_stringBuffer);
                            dem_functions.SR = SR;

                        if (message.Contains("%%FPCH&"))
                        {
                            headerExecute_stringBuffer = message.Substring(message.LastIndexOf("%%FPCH&") + 7);
                            if (headerExecute_stringBuffer.Contains("%%"))
                                headerExecute_stringBuffer = headerExecute_stringBuffer.Substring(0, headerExecute_stringBuffer.IndexOf("%%"));
                            F = Convert.ToInt64(headerExecute_stringBuffer);
                            dem_functions.F = F;
                        }

                        else { F = Convert.ToInt64(dem_functions.SR / 2); dem_functions.F = F; }
                               
                        }
                        outMessage = "%%FPCH&" + ((long)(dem_functions.F)) + "%%SAMPLERATE&" + ((long)(dem_functions.SR));
                        info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц", dem_functions.SR / 1000000.0, dem_functions.F / 1000000.0);
                    }
                    catch
                    {

                    }
                    }
            if (dem_functions.sendComand)
            {
                outMessage = "%%FPCH&" + ((long)(dem_functions.F)) + "%%SAMPLERATE&" + ((long)(dem_functions.SR));
                dem_functions.sendComand = false;
                info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц ", dem_functions.SR / 1000000.0, dem_functions.F / 1000000.0);
            }
            //info = string.Format(string.Format("{0}", inData.Length));
            Array.Resize(ref outData, inData.Length);
            try
            {
                if (inDataLength_change.new_value != inData.Length)
                {
                    inDataLength_change.new_value = inDataLength_change.old_value;
                    dem_functions.demodulator_init(inData.Length);
                }
                if (calculate_parametrs_bool)
                {
                    dem_functions._exponentiation(ref inData);
                    dem_functions.centralFrequency = dem_functions._F_calculating();
                }
                dem_functions._shifting_function(ref inData);
                if (calculate_parametrs_bool)
                {
                    dem_functions._detection();
                    dem_functions.speedFrequency = dem_functions._speed_calculating() + dem_functions.MS_correct;
                    dem_functions.SymbolsPerSapmle = dem_functions._BitPerSapmle();
                }
                dem_functions._filtering_function(ref outData);
                if (visual_Form != null && visual_Form.Visible) { visual_Form.displayFFT(); }
                Gardner_detector ms_sync = new Gardner_detector(Demodulator.IQ_shifted.bytes, dem_functions.SymbolsPerSapmle);
                ms_sync.BeginPhaseCalc();
                demodulate_I_unit = new int[(int)((inData.Length / 4) / dem_functions.SymbolsPerSapmle)];
                demodulate_Q_unit = new int[(int)((inData.Length / 4) / dem_functions.SymbolsPerSapmle)];               
                demodulate_I_unit = ms_sync.take_I();
                demodulate_Q_unit = ms_sync.take_Q();
                //if (dem_functions.write)
                //{
                //    short[] temp_short_I = new short[demodulate_I_unit.Length];
                //    short[] temp_short_Q = new short[demodulate_Q_unit.Length];
                //    for (int i = 0; i < demodulate_I_unit.Length; i++)
                //    {
                //        temp_short_I[i] = (short)demodulate_I_unit[i];
                //        temp_short_Q[i] = (short)demodulate_Q_unit[i];
                //    }
                //    Writter Write = new Writter(temp_short_I, temp_short_Q, "I", "Q", "sympols_for_demod");
                //    dem_functions.write = false;                    
                //}
                    _outcom += outData.Length;
                info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц\nФАПЧ status: {2}\n ", dem_functions.SR / 1000000.0, dem_functions.F / 1000000.0, Convert.ToString(calculate_parametrs_bool));
                DoneWorck(this, outMessage, outData);               
            }
            catch
            {
                _incom = 0;
                _outcom = 0;
                DoneWorck(this, outMessage, null);
                outMessage = "";
                info = "Помилка роботи модуля";
            }
            _busy = false;
        
    }

    public void Stop()
    {           
            dem_functions.demodulator_busy = false;
            _busy = false;
            _incom = 0;
            _outcom = 0;
    }

        public void SetParam(string param)
        {
            try
            {
                    string strheader = param.Substring(param.LastIndexOf("%%FPCH&") + 7);
                    if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
                    dem_functions.F = Convert.ToInt64(strheader);
                    strheader = param.Substring(param.LastIndexOf("%%SAMPLERATE&") + 13);
                    if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
                    dem_functions.SR = Convert.ToDouble(strheader);               
            }
            catch
            {
            }
        }

        public string GetParam()
        {
            return string.Format("%%FPCH&{0}%%SAMPLERATE&{1}", Convert.ToDecimal(dem_functions.F), Convert.ToDecimal(dem_functions.SR));           
        }

        public event Sdelal DoneWorck;
    }    
}



