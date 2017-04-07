using System;
using System.Runtime.InteropServices;

namespace Interface
{
    /// <summary>
    /// Інтерфейс,який повинні реалізовувати методи декодування 
    /// </summary>

    [StructLayout(LayoutKind.Sequential)]
    public struct iq
    {
        public short i;
        public short q;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct sIQData
    {
        [FieldOffset(0)]
        public Byte[] bytes;

        [FieldOffset(0)]
        public short[] shorts;

        [FieldOffset(0)]
        public iq[] iq;

        public int bytes_Length { get { return bytes.Length; } }
        public int shorts_Length { get { return bytes.Length / 2; } }
        public int iq_Length { get { return bytes.Length / 4; } }
    }

    public interface IDecoder
    {
        //********************************************************************************************
        //**********************-- Цей блок коду забороняється змінювати --***************************
        //********************************************************************************************
        string Name { get; }           //імя декодера, яке буде відображатись в панелі меню
        string Version { get; }        //версія декодера
        string Author  { get; }        //Підрозділ, де розроблено декодер
        int Id { get; set; }           //Унікальний номер декодера,він є в HESH-таблиці.
        
        ModData GetCurInfo { get; }
        bool Busy { get; set; }
        //--------------------------------
        string Init();                  //ініціалізація вхідних параметрів декодера
        void Info();
        void Start();
        void Visual(); //Виклик вікна візуалізації модуля, якщо потрібно
        void Start(string mesage, byte[] inData);      //початок сеанса обробки 
        //void Start(string mesage, short[] inData);      //початок сеанса обробки 
        void Stop();                  //закінчення сеанса обробки
        void SetParam(string param);
        string GetParam();            //видає стрічку з параметрами декодера, для запису їх в конфіг-файл
        //********************************************************************************************
        event Sdelal DoneWorck; // подія, що виникає при завершенні обробки даних
    }

    public delegate void Sdelal(object sender, string msg, byte[] outData);
    //public delegate void Sdelal(object sender, string msg, short[] outData);
    
    public struct ModData
    {
        public double Incoming;       //Лічильниквхідних даних
        public double Outcoming;      //Лічильник вихідних дани
        public double Zriv;           //Лічильник зривів в роботі модуля
        public string Nastr;         //Стрічка в якій передається інформація про настройки модуля та дані
    }
      
 }
