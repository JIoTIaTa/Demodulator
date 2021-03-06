﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Interface;
//using signal_tester;
using System.Windows.Forms;
//using Filter;

namespace Exponentiation
{

    public class Quadrature_AM_detectorSPARKInterface : IDecoder
    {
        private bool _busy;
        private double _incom = 0; // вхідні відліки в байтах
        private double _outcom = 0; // вихідні відліки в байтах
        private double errorsNumber = 0; // кількість зривів
        public Quadrature_AM_detector Quadrature_AM_detector = new Quadrature_AM_detector(); // обьєкт класу, де здійснюється фільтрація
        private byte[] outData = new byte[524288]; // масив відфільтрованих даних
        private Quadrature_AM_detectorForm Quadrature_AM_detectorForm; // форма, що відкривається при подвійному кліку на блок
        private long SR = 9999999; // частота дискретизації (вхідного сигналу)
        private long F = 5555555; // центральна частота (вхідного сигналу)
        private string info; // строка виведення інформації в вікні СПАРК
        public VisualForm visual; // форма візуалізації
        
        

        public string Name
        {
            get { return "Квадратурний детектор"; }
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
            if (Quadrature_AM_detectorForm == null || Quadrature_AM_detectorForm.IsDisposed)
            {
                Quadrature_AM_detectorForm = new Quadrature_AM_detectorForm { Quadrature_AM_detector = Quadrature_AM_detector };
            }
            Quadrature_AM_detectorForm.Show();
            return "Квадратурний детектор";
        }

        public void Info()
        {
            //MessageBox.Show("Даний модуль призначений для фільтрації сигналу \n" +
            //                "у смузі пропускання, яка обирається у попередньому\n" +
            //                "модулі, наприклад модулі ШПФ");
            MessageBox.Show("\nКвадратурний детектор");
        }

        public void Start()
        {

        }

        public void Visual()
        {
            if (visual == null || visual.IsDisposed)
            {
                //visual = new VisualForm(firFilter.filter, firFilter.SR, firFilter.BW, firFilter.F); // імпульсно-частотна характеристика фільтра
                //visual = new VisualForm(firFilter.filter);
                visual = new VisualForm() { Quadrature_AM_detector = Quadrature_AM_detector };
                visual.Show();
            }
            else
            {
                visual.Focus();
            }
        }

        public void Start(string mesage, byte[] inData)
        {
            string outMessage = ""; // команда, що буде предана наступному модулю
            _incom += inData.Length;            
                if (!string.IsNullOrEmpty(mesage))
                {
                    var comanda = mesage;
                    //MessageBox.Show(String.Format("{0}",mesage));
                    try
                    {
                        if (comanda.Contains("%%SAMPLERATE&"))
                        {
                            string strheader = comanda.Substring(comanda.LastIndexOf("%%SAMPLERATE&") + 13);
                            if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
                            SR = Convert.ToUInt32(strheader);
                            Quadrature_AM_detector.SR = SR * Quadrature_AM_detector.x;

                            if (comanda.Contains("%%FPCH&"))
                            {
                                strheader = comanda.Substring(comanda.LastIndexOf("%%FPCH&") + 7);
                                if (strheader.Contains("%%"))
                                    strheader = strheader.Substring(0, strheader.IndexOf("%%"));
                                F = Convert.ToInt64(strheader);
                                Quadrature_AM_detector.F = F;
                            }

                            else
                                F = Convert.ToInt64(Quadrature_AM_detector.SR / 2);
                        }
                        outMessage = "%%FPCH&" + ((long)(Quadrature_AM_detector.F)) + "%%SAMPLERATE&" + ((long)(Quadrature_AM_detector.SR));
                        info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц", Quadrature_AM_detector.SR / 1000000.0, Quadrature_AM_detector.F / 1000000.0);
                    }
                    catch
                    {
                    }
                    
                    //if (comanda.Contains("%%NUM&"))
                    //{
                    //    int a1; // номер смуги для вибору фільтра (з повідомлення від ШПФ)
                    //    long ffir; // центральна чатота сигналу, що потрібно відфільтрувати (з повідомлення від ШПФ)
                    //    string strheader = comanda.Substring(comanda.LastIndexOf("%%NUM&") + 6);
                    //    if (strheader.Contains("%%"))
                    //        strheader = strheader.Substring(0, strheader.IndexOf("%%"));
                    //    a1 = Convert.ToInt32(strheader);

                    //    if (a1 == Exponentiation.login)
                    //    {
                    //        info = string.Format("Модуль піднесення до степеня № {0} зконфігуровано",
                    //                              Exponentiation.login);
                    //    }
                    //}
                    }
            try
            {
                if (Quadrature_AM_detector.sendComand)
                    {
                        outMessage = "%%FPCH&" + ((long)(Quadrature_AM_detector.F)) + "%%SAMPLERATE&" + ((long)(Quadrature_AM_detector.SR));
                        Quadrature_AM_detector.sendComand = false;
                        //DoneWorck(this, outMessage, outData);
                        //MessageBox.Show("переписав частоту");
                        info = string.Format("Частота дискретизації:  {0} МГц\nЦентральна частота:  {1} МГц", Quadrature_AM_detector.SR / 1000000.0, Quadrature_AM_detector.F / 1000000.0);
                    }
                Quadrature_AM_detector.quadrature_AM_detector(inData, outData);
                Array.Resize(ref outData, inData.Length * Quadrature_AM_detector.x); // для інтерполяції
                _outcom += outData.Length;
                DoneWorck(this, outMessage, outData);
                //outMessage = "";
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
            _busy = false;
            _incom = 0;
            _outcom = 0;
    }

        public void SetParam(string param)
        {
            try
            {
            //    if (param.Contains("%%NUM&"))
            //    {
            //        string strheader = param.Substring(param.LastIndexOf("%%NUM&") + 6);
            //        if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
            //        Exponentiation.login = Convert.ToInt32(strheader);
            //    }
                //if (param.Contains("%%FPCH&"))
                //{
                    string strheader = param.Substring(param.LastIndexOf("%%FPCH&") + 7);
                    if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
                    Quadrature_AM_detector.F = Convert.ToInt64(strheader);
                    strheader = param.Substring(param.LastIndexOf("%%SAMPLERATE&") + 13);
                    if (strheader.Contains("%%")) strheader = strheader.Substring(0, strheader.IndexOf("%%"));
                    Quadrature_AM_detector.SR = Convert.ToDouble(strheader);
                //}
            }
            catch
            {
            //    MessageBox.Show("Дані не визначені");
            }
        }

        public string GetParam()
        {
            //return string.Format("%%NUM&{0}", Exponentiation.login);
            return string.Format("%%FPCH&{0}%%SAMPLERATE&{1}", Convert.ToDecimal(Quadrature_AM_detector.F), Convert.ToDecimal(Quadrature_AM_detector.SR));
            //return "";
        }

        public event Sdelal DoneWorck;
    }
}



