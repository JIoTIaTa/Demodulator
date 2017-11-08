using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace demodulation
{
    public partial class Demodulator
    {
        public double SR = 0.0d; // частота дискретизації
        public double SR_after_filter; // частота дискретизації після фільтрації
        public sIQData IQ_inData, IQ_outData, IQ_detected, IQ_elevated, IQ_shifted, IQ_remainded, IQ_filtered;
        public long F = 0; // центральная частота
        public bool sendComand = false;
        private double tempI = 0; // змінна для тимчасових даних
        private double tempQ = 0; // змінна для тимчасових даних
        public bool show;
        public static int x = 1; // коеф інтерполяції
        public int IQ_lenght; // кількість I/Q відліків
        public int maxFFT = 65536; // максимальний порядок ШПФ        
        public int modulation_multiplicity = 4; //кратність модуляції 
        public bool demodulator_busy = false; // прапорець роботи тракту
        public int fftAveragingValue = 4; // усереднення ШПФ
        double[] tempI_buffer = new double[128]; // для зберігання I відліків подвійної точності 
        double[] tempQ_buffer = new double[128]; // для зберігання Q відліків подвійної точності 
        float[] sin_16384 = new float[16384]; // масив синусів для зносу
        float[] cos_16384 = new float[16384]; // масив косинусів для зносу        
        public double speedFrequency = 0.0d; // швидкість модуляції
        public double centralFrequency = 0.0d; // центральна частота
        public float sin_cos_position = 0; // позиція син/кос для вибору з таблиці
        public float FilterBandwich = 0; // смуга фільтрація
        static int filterOrder = 101; // порядок фільтру
        float[] filterCoefficients; // коефіціенти фільтра
        byte[] remainded = new byte[filterOrder * 4]; // залишок старого масива - початок для нового
        public TWindowType FIR_WindowType = TWindowType.SINC; // тип вікна фільтра
        public float FIR_beta = 3.2f; // коефіціент БЕТА фільтра
        public int N = 0;   // для сноса  
        float maxValue; // змінна для знаходження пікової гармоніки 
        public FFT_data_display display = FFT_data_display.FILTERING;// які дані буде відображати ШПФ
        public int firstDecim = 0; // треба згадать, юзав для фільрації
        float BW = 0; // смуга для фунції генерування коефіціентів
        public float SymbolsPerSapmle = 0;
        public float speed_error_I = 0;
        public float speed_error_Q = 0;
        public float speed_error = 0;
        public bool write = false;
        public string warningMessage = "Стан: Працює без збоїв";
        public float MS_correct = 0.0f;
        Poliphase_Filter Poliphase_filter = new Poliphase_Filter();
        Filter simple_filter = new Filter();
        public Filter_type filter_type = Filter_type.simple;
        public float central_freq_correct = 0.0f;
        public int centralPosition = 0; // позиція точки ШПФ центральної гармоніки
        public bool display_constellation = false; // відображення фазового сузір'я
        public bool display_FFT = false; // відображення ШПФ
        public short display_Tick = 10;
        public bool display_exponent = false; // відображення ШПФ
        public Exponent_data_display exp_display = Exponent_data_display.ELEVATE;
    }
}
