using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Interface;
using System.Threading.Tasks;
using System.Xml;

namespace Filter
{
    public class Filter:IDecoder
    {
        

        /*[DllImport(@"..\\..\\data\\CudaMultdll.dll")]
        public static extern int    deviceMult ( int [] _inbufi, int [] _inbufq, int [] _outbufiFilter, int [] _outbufqFilter,
                                                 int[] _dataI, int[] _dataQ, int N, double centralFreq, double fshiftReal, double sampleRate,
			                                     float []_coefficients ,  int filterOrder  );*/

        [DllImport(@"..\\..\\data\\device10xAmlitude.dll")]
        // public static extern int device10xAmlitude (int[] _inbufi, int[] _inbufq, int[] _outbufiFilter, int[] _outbufqFilter, int N);
        //public static extern int Ukazatel (ref byte inData, ref byte outData, int length);
        public static extern int deviceMult(ref byte inData, ref byte outData, int length, /*ref int _dataI, ref int _dataQ,*/
                 double _centralFreq, double fshiftReal, double _sampleRate/*, ref float _coefficients, int _filterOrder*/);
        //public static extern int deviceFiltering(ref byte inData, ref byte outData, int length, ref short _dataI, ref short _dataQ,
        //        double _centralFreq, double fshiftReal, double _sampleRate, ref float _coefficients, int _filterOrder);

        int myFilterCount;//это номер фильтра который модулю надо хранить!
        private double _sampleRate;
        private int _filterOrder;
        private double _cutoffFrequency;//половина полосы пропускания ФНЧ. Полная полоса пропускания отображается только в окне настройки.
        private double _centralFreq;//центральная частота сигнала, который надо отфильтровать
        private FilteringMath.WindowType _winType;
        private float[] _coefficients;
        private int _numFilter;
        //массивы для комплексного умножения - переноса частоты
        private double[] _cosArr;
        private double[] _sinArr;
        private int N;//целое количество элементов в массиве
        private short [] _dataI;
        private short [] _dataQ;

        //для выполнения перемножения

        private double fshiftReal;
        private sIQData IQInData;
        private bool _busy;
        private float _incom;
        private float _outcom;
        private string _logmsg;
        private int[] _inbufi;//один отсчет - два байта
        private int[] _inbufq;//один отсчет - два байта
        private int[] _outbufiFilter;
        private int[] _outbufqFilter;
        private sIQData _outData;//фильтрованый сигнал
        private byte[] outData;
        private bool _firstStart = true;
        private int _numEl;//количество входных элементов (I=Q)
        private double _newSampleRate;//частота дискретизации после фильтрации
        private int _decim=1; //во сколько раз уменьшаем дискретизацию

      /*  // конструктор фильтров
        public Filter()
        {
            var filterCfgPath = Application.StartupPath + "\\Filters_config.xml";
            var filters = new List<int>();

            if (File.Exists(filterCfgPath))
            {
                //считываем инфо про существующие фильтры
                var xDoc = new XmlDocument();
                xDoc.Load(filterCfgPath);
                XmlElement xRoot = xDoc.DocumentElement;
                foreach (XmlNode xnode in xRoot)
                {
                    if (xnode.Attributes.Count > 0)
                    {
                        XmlNode attr = xnode.Attributes.GetNamedItem("number");
                        if (attr != null)
                        {
                         
                           filters.Add(Convert.ToInt32(attr.Value));
                        }
                    }
                }
                //ищем первый свободный номер
                if (filters.Count == 0)
                {
                    myFilterCount = 1;
                }
                else
                {
                    myFilterCount = filters.Max() + 1;
                    for (int i = 1; i < filters.Max(); i++)
                    {
                        if (!filters.Contains(i))
                            myFilterCount = i;
                    }
                }
                
                    MessageBox.Show("Ви додали фільтр № " + myFilterCount + "\n");  
               

                //добавляем инфу про себя в файл - один новый фильтр               
                XmlNode element = xDoc.CreateElement("filter");
                xDoc.DocumentElement.AppendChild(element);
                XmlAttribute attribute = xDoc.CreateAttribute("number");
                attribute.Value = myFilterCount.ToString();
                element.Attributes.Append(attribute);
                xDoc.Save(filterCfgPath);
            }
         }

        // деструктор фильтров
        ~Filter()
        {
            
            var filterCfgPath = Application.StartupPath + "\\Filters_config.xml";
            if (File.Exists(filterCfgPath))
            {
                var xDoc = new XmlDocument();
                xDoc.Load(filterCfgPath);
                var xRoot = xDoc.DocumentElement;
                foreach (XmlNode xnode in xRoot)
                {
                    if (xnode.Attributes.Count > 0)
                    {
                        XmlNode attr = xnode.Attributes.GetNamedItem("number");
                       if (attr != null)
                            if (Convert.ToInt32(attr.Value) == myFilterCount)
                            {
                               MessageBox.Show("Ви видалили фільтр № " + myFilterCount + "\n");
                               xRoot.RemoveChild(xnode);

                            }
                    }
                }
                xDoc.Save(filterCfgPath);
            }
        }
        */
       

        public string Name
        {
            get { return "Фільтр CUDA"; }
        }

        public string Version
        {
            get { return "1.0"; }
        }

        public string Author
        {
            get { return "NDV NDI"; }
        }

        public int Id
        {
            get;
            set;
        }

        public ModData GetCurInfo
        {
            get
            {
                var m = new ModData
                            {
                                Incoming = (uint) (_incom/1024),
                                Outcoming = (uint) (_outcom/1024),
                                Zriv = 0,
                                Nastr = "Фільтрація потоку даних на відеокарті"
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
            
            _winType = FilteringMath.WindowType.Blackman;// тип фікна фільтру
            var dlgWnd = new SettingWnd(_filterOrder, _cutoffFrequency, _centralFreq, _numFilter);
            dlgWnd.ShowDialog();
            if (dlgWnd.DialogResult == DialogResult.OK)
            {
                _filterOrder = dlgWnd.GetFilterOrder();
                _cutoffFrequency = dlgWnd.GetCutoffFrequency();
                _centralFreq = dlgWnd.GetCentralFreq();
                _decim = dlgWnd.GetDecimSampleRate();
                _numFilter = dlgWnd.GetNumberFilter();
                MessageBox.Show(String.Format("CentrFreq={0};SamRate={1}; cutoffreq={2},filteroder{3}; decim={4} ", 
                _centralFreq,_sampleRate, _cutoffFrequency, _filterOrder, _decim));
            }

            return "Фільтр CUDA";
        }

        public void Info()
        {
            MessageBox.Show("\n Номер фільтра: " + _numFilter + "\n Версія: " + Version + "\n Розробник: " + Author + "\n Призначення: Фільтрація потоку даних.\n Настройка: ще не написана.\n Вхідні дані: потік даних. \n Вихідні дані: відфільтрований потік даних.");
        }

        public void Start()
        {
        }

        public void Visual()
        {
            var a = new VisualForm(_coefficients);
            a.Show();
        }


        
        public void Start(string mesage, byte[] inData)
        {
            _busy = true;

            //if (!string.IsNullOrEmpty(mesage))
            //{
            //    var comanda = mesage;
            //    //пришедшая SampleRate используется для рассчета параметров фильтра 
            //    //дальше передается новая частота дискретизации
            //    if (comanda.Contains("%%SAMPLERATE&"))
            //    {
            //        string strheader = comanda.Substring(comanda.LastIndexOf("%%SAMPLERATE&") + 13);
            //        if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
            //        _sampleRate = Convert.ToDouble(strheader);
            //        _newSampleRate = _sampleRate / _decim;
            //        _coefficients = FilteringMath.MakeLowPassKernel(_sampleRate, _filterOrder, _cutoffFrequency,
            //            FilteringMath.WindowType.Blackman /*_winType*/);
                    
            //        CalcArrForFreqShift(_sampleRate, _centralFreq);
            //       /* MessageBox.Show(String.Format("CentrFreq={0};SamRate={1}; cutoffreq={2},filteroder{3}; decim={4} ",
            //       _centralFreq, _sampleRate, _cutoffFrequency, _filterOrder, _decim));*/
            //       // _logmsg = String.Format("%%SAMPLERATE&{0}",_newSampleRate);
            //        _logmsg = mesage + "%%SAMPLERATE&" + _newSampleRate.ToString();
            //    }


            //   /* if (comanda.Contains("%%NUM&"))
            //    {
            //        int numFilter;
            //        float bw;
            //        long ffir;
            //        int b1 = 0;
            //        float b2 = 1f;
            //        float temp;
            //        string strheader = comanda.Substring(comanda.LastIndexOf("%%NUM&") + 6);
            //        if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
            //        numFilter = Convert.ToInt32(strheader);
            //        if (numFilter == _numFilter)
            //        {
            //            strheader = comanda.Substring(comanda.LastIndexOf("%%BW&") + 5);
            //            if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
            //            _cutoffFrequency = Convert.ToDouble(strheader)/2;
            //            strheader = comanda.Substring(comanda.LastIndexOf("%%FFIR&") + 7);
            //            if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
            //            _centralFreq = Convert.ToInt64(strheader);
                       

            //        }
            //    }*/

            //    //с первой посылкой должна приходить команда с заголовком, которая должна передаваться дальше
            //    if (comanda.Contains("%%IQHEADER&"))
            //    {
            //        //пересылаем ее без изменений
            //        string strheader = comanda.Substring(comanda.LastIndexOf("%%IQHEADER&") + 11);
            //        if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));                
            //        _logmsg = _logmsg + "%%IQHEADER&" + strheader;
            //    }
            //}            
            //=========================================
            if (inData != null)
            {
                
               // IQInData.bytes = inData;
               // outData = new byte[inData.Length];
               // //_outcom = _incom += inData.Length * 2;   -   было до прореживания
               // _incom = _incom + inData.Length;
               // _outcom = _outcom + (int)(inData.Length / _decim);
               // //далее считаем, что один отсчет - 2 байта
               // if (inData.Length % 2 != 0)//проверяем четность количества отсчетов
               //     MessageBox.Show("Кількість відліків I і Q у вхідному масиві даних різна", "Помилка блоку фільтрації");
               // _numEl = inData.Length / 4; //входных элементов в каждом канале                                                         ????? проверить может, убрать

               // //если первый раз - создаем буффер с данными
               // if (_firstStart)
               // {
               //     //довжина - кількість I=Q відліків, бо тепер у нас новий фільтр
               //     _inbufi = new int[_numEl];
               //     _inbufq = new int[_numEl];                   

                    
               //     _firstStart = false;
                    
               // }
                               
               // _outData.bytes = new byte[IQInData.bytes_Length / _decim];

               // //разбираем вхідний масив, копіюємо його
               // for (int j = 0; j < _numEl; j++)
               // {
               //     _inbufi[j] = IQInData.iq[j].i;
               //     _inbufq[j] = IQInData.iq[j].q;
               // }

               // // перший раз заповняємо нулями буфер, що необхідний для правильної фільтрації
                _dataI = new short[_filterOrder];
                _dataQ = new short[_filterOrder];
                for (int j = 0; j < _filterOrder; j++)
                {
                    _dataI[j] = 0;
                    _dataQ[j] = 0;
                }
               // // викликаємо функцію, що буде генерувати синфазну та квадратурну складові, множити їх з вхідними I та Q відліками
               // // та проводити фільтрацію (але без децимації)
               //_outbufiFilter = new int[_numEl];
               //_outbufqFilter = new int[_numEl];

                //deviceMult(ref inData[0], ref outData[0], inData.Length, ref _dataI[0], ref _dataQ[0],
                //    _centralFreq, fshiftReal, _sampleRate, ref _coefficients[0], _filterOrder);


               // //device10xAmlitude (_inbufi, _inbufq, _outbufiFilter, _outbufqFilter, _numEl);


                // надалі заповняємо буфер останніми елементами буферу вхідних відліків 
                

               // //Decimation();


                
                //DoneWorck(this, _logmsg, _outData.bytes);
                outData = new byte[inData.Length];
                //Ukazatel(ref inData[0], ref outData[0], inData.Length);]
                _centralFreq = 250000;
                fshiftReal = 200000;
                _sampleRate = 500000;
                _filterOrder = 41;
                _cutoffFrequency = 100000;
                _winType = FilteringMath.WindowType.Blackman;// тип фікна фільтру
                _coefficients = FilteringMath.MakeLowPassKernel(_sampleRate, _filterOrder, _cutoffFrequency,
                        _winType);
                // MessageBox.Show(String.Format("_coefficients[0]={0};_coefficients[10]={1}; _coefficients[20]={2}",
                //_coefficients[0], _coefficients[10], _coefficients[20]));
                deviceMult(ref inData[0], ref outData[0], inData.Length, _centralFreq, fshiftReal, _sampleRate);
                //deviceFiltering(ref inData[0], ref outData[0], inData.Length, ref _dataI[0], ref _dataQ[0],
                //  _centralFreq, fshiftReal, _sampleRate, ref _coefficients[0], _filterOrder);
                //for (int j = 0; j < _filterOrder; j++)
                //{
                //    _dataI[j] = IQInData.iq[_numEl - j - 1].i;
                //    _dataQ[j] = IQInData.iq[_numEl - j - 1].q;
                //}
                DoneWorck(this, mesage, outData);
                _incom += inData.Length;
                _outcom +=outData.Length;
            }
            else
            {
                //DoneWorck(this, _logmsg, null);
            }
            _logmsg = "";
            //=================================
            _busy = false;
        }

        public void Stop()
        {
            
        }

        public void SetParam(string param)
        {
            try
            {
                _filterOrder =
                    int.Parse(param.Substring(param.IndexOf("<FilterOrder>") + 13,
                        param.IndexOf("<CutoffFrequency>") - param.IndexOf("<FilterOrder>") - 13));
                _cutoffFrequency = int.Parse(param.Substring(param.IndexOf("<CutoffFrequency>") + 17, param.IndexOf("<CentralFrequency>") - param.IndexOf("<CutoffFrequency>") - 17));
                _centralFreq = int.Parse(param.Substring(param.IndexOf("<CentralFrequency>") + 18, param.Length - param.IndexOf("<CentralFrequency>") - 18));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + " Структура файлу конфігурації не відповідає параметрам модулю.", "Помилка завантаження параметрів блоку фільтрації");
            }            
        }

        public string GetParam()
        {
            string str = "<FilterOrder>" + _filterOrder + "<CutoffFrequency>" + _cutoffFrequency + "<CentralFrequency>" + _centralFreq;
            return str;
        }

        public event Sdelal DoneWorck;

        ////рассчет массива косинусов и синусов для переноса частоты
        //private void CalcArrForFreqShift(double sampleRate, double centralFreq)
        //{
        //    double fshift = Math.Abs(sampleRate / 2 - centralFreq);
        //    MessageBox.Show(String.Format("Perenos-> SamRate={0}; centralFreq={1} ", sampleRate, centralFreq));
        //    // зная центральную частоту и частоту дискретизации, вычисляем частоту, на которую надо сдвинуть

        //    double ndouble = sampleRate / fshift; //дробное количество элементов в массиве               
        //    N = (int)(sampleRate / fshift);
        //    int k = 1; //сколько периодов синусоиды будем генерировать для увеличения точности

        //    if (N != ndouble)
        //    {
        //        while (N * k < 300)
        //        {
        //            k = k * 10;
        //        }
        //        while (N * k < 1000)
        //        {
        //            k = k * 2;
        //        }
        //    }
            
           
        //    MessageBox.Show("Дробное количество элементов " + ndouble + " целое количество элементов " + N + " неточность переноса частоты " + deltaFshift);            

            
        //}
         //рассчет массива косинусов и синусов для переноса частоты
        private void CalcArrForFreqShift(double sampleRate, double centralFreq)
        {
            double fshift = Math.Abs(sampleRate / 2 - centralFreq);// зная центральную частоту и частоту дискретизации, вычисляем частоту, на которую надо сдвинуть
            
            double ndouble = sampleRate / fshift;//дробное количество элементов в массиве               
            N = (int)(sampleRate / fshift);
            int k = 1;//сколько периодов синусоиды будем генерировать для увеличения точности

            if (N != ndouble)
            {
                while (N * k < 300)
                {
                    k = k * 10;
                }
                while (N * k < 1000)
                {
                    k = k * 2;
                }
            }

            N = (int)(ndouble * k);
            double fshiftReal = sampleRate * k / N;     //реально получившаяся частота сдвига сигнала
            double deltaFshift = fshift - fshiftReal;  //ошибка переноса частоты

            MessageBox.Show("Дробное количество элементов " + ndouble + " целое количество элементов " + N + " неточность переноса частоты " + deltaFshift);            

            _cosArr = new double[N];
            _sinArr = new double[N];

            for (int n = 0; n < N; n++)
            {
                _cosArr[n] =  Math.Cos(2 * Math.PI * fshiftReal * n / sampleRate);
                if (centralFreq > sampleRate/2)
                {   //понижаем частоту
                    _sinArr[n] = -Math.Sin(2 * Math.PI * fshiftReal * n / sampleRate);
                }
                else
                {   //повышаем частоту
                    _sinArr[n] = Math.Sin(2 * Math.PI * fshiftReal * n / sampleRate);
                }
            }
        }

        //перенос частоты комплексным умножением
        //выполняется над массивом входных данных, разделенных на I и Q составляющие, перед фильтрацией
        
        private void Decimation()
        {
            int outi = 0;
            for (int j = 0; j < _numEl; j = j + _decim)
            {
                _outData.iq[outi].i = (short)_outbufiFilter[j];
                _outData.iq[outi].q = (short)_outbufqFilter[j];
            outi++;
           // write(_outbufiFilter, _outbufqFilter);
            }
             

                
             
                
            
        }
       /* public void write(int[] _outbufiFilter, int[] _outbufqFilter)
        {
            StreamWriter file = new StreamWriter("Після фільтрації (пряме пеермноження) .txt");
            for (int i = 0; i < _numEl ; i++)
            {
                file.WriteLine(i + ".I->" + _outbufiFilter[i]);
                file.WriteLine(i + ".Q->" + _outbufqFilter[i]);
            }
            MessageBox.Show("Масиви після фільтрації записано");
            file.Close();
        }*/
        //фильтрация
       /* private void Filtr()
        {
            int outi = 0;
            
            for (int i = 0; i < _decim*_outData.iq_Length; i = i + 2)
            {
                _isum = 0;
                _qsum = 0;
                for (int j = 0; j < _filterOrder/2 ; j = j + 2)
                {
                    _isum = _isum + _ibuf[i + j] * _coefficients[j];
                    _qsum = _qsum + _qbuf[i + j] * _coefficients[j];
                }

                _isum = _isum + _ibuf[i + _filterOrder / 2] * _coefficients[_filterOrder / 2];
                _qsum = _qsum + _qbuf[i + _filterOrder / 2] * _coefficients[_filterOrder / 2];

                for (int j = _filterOrder / 2; j < _filterOrder; j =    j + 2)
                {
                    _isum = _isum + _ibuf[i + j] * _coefficients[j];
                    _qsum = _qsum + _qbuf[i + j] * _coefficients[j];
                }

                _outData.iq[outi].i = (short)_isum;
                _outData.iq[outi].q = (short)_qsum;
                outi++;
            }
        }*/
    }
}
