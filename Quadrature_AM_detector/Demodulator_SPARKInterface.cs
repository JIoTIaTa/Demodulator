using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Interface;
//using signal_tester;
using System.Windows.Forms;
//using Filter;

namespace demodulation
{

    public class Quadrature_AM_demodulatorSPARKInterface : IDecoder
    {
       
        private bool _busy;
        private double _incom = 0; // вхідні відліки в байтах
        private double _outcom = 0; // вихідні відліки в байтах
        private double errorsNumber = 0; // кількість зривів
        public Demodulator demodulation_functions = new Demodulator(); // обьєкт класу
        private byte[] outData = new byte[524288]; // масив відфільтрованих даних
        private Demodulator_DoubleClick_Form doubleClick_Form; // форма, що відкривається при подвійному кліку на блок
        private long SR = 9999999; // частота дискретизації (вхідного сигналу)
        private long F = 5555555; // центральна частота (вхідного сигналу)
        private string info; // строка виведення інформації в вікні СПАРК
        public DemodulatorVisual_Form visual_Form; // форма візуалізації         
        public static bool calculate_parametrs_bool = true;
        private change inDataLength_change = new change(); // для визначення, чи змінилась довжинавхідних даних

        public string Name
        {
            get { return "Демодулятор"; }
        }

        public string Version
        {
            get { return "v.1.0."; }
        }

        public string Author
        {
            get { return "NDI"; }
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
            if (demodulation_functions.demodulator_busy == true)
            {
                if (visual_Form == null || visual_Form.IsDisposed)
                {
                    visual_Form = new DemodulatorVisual_Form() { demodulation_functions = demodulation_functions };
                    visual_Form.Show();
                }
                else
                {
                    visual_Form.Focus();
                }
            }
            else
            {
                MessageBox.Show("Can't do it, bro");
            }
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
                doubleClick_Form = new Demodulator_DoubleClick_Form { demodulation_functions = demodulation_functions };
            }
            doubleClick_Form.Show();
        }

        public void Start(string mesage, byte[] inData)
        {
            inDataLength_change.old_value = inData.Length;
            demodulation_functions.demodulator_busy = true;
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
                            demodulation_functions.SR = SR;

                        if (message.Contains("%%FPCH&"))
                        {
                            headerExecute_stringBuffer = message.Substring(message.LastIndexOf("%%FPCH&") + 7);
                            if (headerExecute_stringBuffer.Contains("%%"))
                                headerExecute_stringBuffer = headerExecute_stringBuffer.Substring(0, headerExecute_stringBuffer.IndexOf("%%"));
                            F = Convert.ToInt64(headerExecute_stringBuffer);
                            demodulation_functions.F = F;
                        }

                        else { F = Convert.ToInt64(demodulation_functions.SR / 2); demodulation_functions.F = F; }
                               
                        }
                        outMessage = "%%FPCH&" + ((long)(demodulation_functions.F)) + "%%SAMPLERATE&" + ((long)(demodulation_functions.SR));
                        info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц", demodulation_functions.SR / 1000000.0, demodulation_functions.F / 1000000.0);
                    }
                    catch
                    {

                    }
                    }
            if (demodulation_functions.sendComand)
            {
                outMessage = "%%FPCH&" + ((long)(demodulation_functions.F)) + "%%SAMPLERATE&" + ((long)(demodulation_functions.SR));
                demodulation_functions.sendComand = false;
                info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц ", demodulation_functions.SR / 1000000.0, demodulation_functions.F / 1000000.0);
            }
            info = string.Format(string.Format("{0}", inData.Length));
            Array.Resize(ref outData, inData.Length);
            try
            {
                if (inDataLength_change.new_value != inData.Length) { inDataLength_change.new_value = inDataLength_change.old_value; demodulation_functions.demodulator_init(inData.Length); }
                if (calculate_parametrs_bool) { demodulation_functions.exponentiation(ref inData); demodulation_functions.F_calculating(); }
                demodulation_functions.shifting_function(ref inData);
                if (calculate_parametrs_bool) { demodulation_functions.detection(); demodulation_functions.speed_calculating(); }
                if (visual_Form != null && visual_Form.Visible) { visual_Form.displayFFT(inData.Length); }
                demodulation_functions.configFilter();
                demodulation_functions.filtering_function(ref outData);
                _outcom += outData.Length;

                //info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц\nФАПЧ status: {2}\n ", demodulation_functions.SR / 1000000.0, demodulation_functions.F / 1000000.0, Convert.ToString(calculate_parametrs_bool));
                
                DoneWorck(this, outMessage, outData);               
            }
            catch
            {
                _incom = 0;
                _outcom = 0;
                DoneWorck(this, outMessage, null);
                outMessage = "";
            }
            _busy = false;
        
    }

    public void Stop()
    {           
            demodulation_functions.demodulator_busy = false;
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
                    demodulation_functions.F = Convert.ToInt64(strheader);
                    strheader = param.Substring(param.LastIndexOf("%%SAMPLERATE&") + 13);
                    if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
                    demodulation_functions.SR = Convert.ToDouble(strheader);               
            }
            catch
            {
            }
        }

        public string GetParam()
        {
            return string.Format("%%FPCH&{0}%%SAMPLERATE&{1}", Convert.ToDecimal(demodulation_functions.F), Convert.ToDecimal(demodulation_functions.SR));           
        }

        public event Sdelal DoneWorck;
    }    
}



