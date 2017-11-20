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
        private long SR = 9999999; // частота дискретизації (вхідного сигналу)
        private long F = 5555555; // центральна частота (вхідного сигналу)
        private string info; // строка виведення інформації в вікні СПАРК
        public DemodulatorVisual_Form visual_Form; // форма візуалізації 
        public Constellation constellation_Form;
        Detector phase_detector = new Detector(65536, modulation_type.PSK_4);
        public static bool calculate_parametrs_bool = true;        
        private change inDataLength_change = new change(); // для визначення, чи змінилась довжинавхідних даних
        public int[] demodulate_I_unit;
        public int[] demodulate_Q_unit;
        public byte[] after_phase_detector;
        


        public string Name
        {
            get { return "Демодулятор"; }
        }

        public string Version
        {
            get { return string.Format("Beta v.{0}.{1}.{2}", DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year); }
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
            if (visual_Form == null || visual_Form.IsDisposed)
            {
                visual_Form = new DemodulatorVisual_Form() { dem_functions = dem_functions };
                visual_Form.Show();
            }
            else
            {
                visual_Form.Focus();
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
           
        }

        public void Start(string mesage, byte[] inData)
        {
            inDataLength_change.old_value = inData.Length;
            if (dem_functions.demodulator_busy == false)
            {
                dem_functions.demodulator_busy = true;
                dem_functions.display_FFT = true;
                dem_functions.display_constellation = true;
                dem_functions.display_exponent = true;

            }
            string outMessage = ""; // команда, що буде предана наступному модулю
            _incom += inData.Length;            
                if (!string.IsNullOrEmpty(mesage))
                {
                    var message = mesage;
                    dem_functions.sendComand = true;
                    try
                    {
                        if (message.Contains("%%SAMPLERATE&"))
                        {
                            string headerExecute_stringBuffer = message.Substring(message.LastIndexOf("%%SAMPLERATE&") + 13);
                            if (headerExecute_stringBuffer.Contains("%%")) headerExecute_stringBuffer = headerExecute_stringBuffer.Substring(0, headerExecute_stringBuffer.IndexOf("%%"));
                            SR = Convert.ToUInt32(headerExecute_stringBuffer);
                            dem_functions.SR = SR;

                        //if (message.Contains("%%FPCH&"))
                        //{
                        //    headerExecute_stringBuffer = message.Substring(message.LastIndexOf("%%FPCH&") + 7);
                        //    if (headerExecute_stringBuffer.Contains("%%"))
                        //        headerExecute_stringBuffer = headerExecute_stringBuffer.Substring(0, headerExecute_stringBuffer.IndexOf("%%"));
                        //    F = Convert.ToInt64(headerExecute_stringBuffer);
                        //    dem_functions.F = F;
                        //}

                        //else { F = Convert.ToInt64(dem_functions.SR / 2); dem_functions.F = F; }
                        F = Convert.ToInt64(dem_functions.SR / 2); dem_functions.F = F;

                        }                       
                    }
                    catch {}
                 }
            if (dem_functions.sendComand)
            {
                if (dem_functions.filter_type == Filter_type.simple)
                {
                    outMessage = "%%FPCH&" + ((long)(dem_functions.F)) + "%%SAMPLERATE&" + ((long)(dem_functions.SR));
                }
                else
                {
                    outMessage = "%%FPCH&" + ((long)(dem_functions.F)) + "%%SAMPLERATE&" + ((long)(dem_functions.SR_after_filter));
                }               
                dem_functions.sendComand = false;
            }
            Array.Resize(ref outData, inData.Length);
                try
                {
                    //Весь хід демодуляції тут!
                    ////////////////////////////Коли змінилася величина вхідних даних////////////////////////////
                    if (inDataLength_change.new_value != inData.Length)
                    {
                        inDataLength_change.new_value = inDataLength_change.old_value;
                        dem_functions.demodulator_init(inData.Length);
                        phase_detector.ReInit(inData.Length, modulation_type.PSK_4);
                        if (visual_Form != null) { visual_Form.new_length(inData.Length); }
                }
                    ////////////////////////////Розрахунок центральної частоти////////////////////////////                
                    if (calculate_parametrs_bool)
                    {
                        dem_functions._exponentiation(ref inData);
                        dem_functions.centralFrequency = dem_functions._F_calculating();
                    }
                    ////////////////////////////Знесення////////////////////////////
                    dem_functions._shifting_function(ref inData);
                    ////////////////////////////**************************////////////////////////////
                    if (calculate_parametrs_bool)
                    {
                        dem_functions._detection();
                        dem_functions.speedFrequency = dem_functions._speed_calculating();
                    }
                    ////////////////////////////**************************////////////////////////////

                    dem_functions._filtering_function(ref outData);
                    ////////////////////////////**************************////////////////////////////
                    if (calculate_parametrs_bool)
                    {
                        dem_functions.SymbolsPerSapmle = dem_functions._BitPerSapmle();
                    }
                    ////////////////////////////**************************////////////////////////////
                    outData = dem_functions.IQ_filtered.bytes;
                    ////////////////////////////**************************////////////////////////////
                    _outcom += outData.Length;
                    if (dem_functions.filter_type == Filter_type.simple)
                    {
                        info = string.Format("Вхідний буффер:  {3}\nЧастота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц\nВизначення параметрів: {2}", dem_functions.SR / 1000000.0, dem_functions.F / 1000000.0, Convert.ToString(calculate_parametrs_bool), inData.Length);
                    }
                    else
                    {
                        info = string.Format("Вхідний буффер:  {3}\nЧастота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц\nВизначення параметрів: {2}", dem_functions.SR_after_filter / 1000000.0, dem_functions.F / 1000000.0, Convert.ToString(calculate_parametrs_bool), inData.Length);
                    }
                    //info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц\nФАПЧ status: {2}\n ", dem_functions.SR / 1000000.0, dem_functions.F / 1000000.0, Convert.ToString(calculate_parametrs_bool));
                    DoneWorck(this, outMessage, outData);
                }
                catch (Exception exception)
                {
                    DoneWorck(this, outMessage, null);
                    outMessage = "";
                    info = string.Format("ДЕМ ПРИУНИВ\n{0}\n{1}\n{2}", exception.Source, exception.TargetSite, exception.Message);
            }
            _busy = false;
        
    }

    public void Stop()
    {           
            dem_functions.demodulator_busy = false;
            _busy = false;
            dem_functions.display_FFT = false;
            dem_functions.display_constellation = false;
            dem_functions.display_exponent = false;
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
            if(dem_functions.filter_type == Filter_type.simple)
            {
                return string.Format("%%FPCH&{0}%%SAMPLERATE&{1}", Convert.ToDecimal(dem_functions.F), Convert.ToDecimal(dem_functions.SR));
            }
            else
            {
                return string.Format("%%FPCH&{0}%%SAMPLERATE&{1}", Convert.ToDecimal(dem_functions.F), Convert.ToDecimal(dem_functions.SR_after_filter));
            }           
        }

        public event Sdelal DoneWorck;
    }    
}



