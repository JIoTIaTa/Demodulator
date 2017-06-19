﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using Cloo;

namespace FirFilterNew
{
    static class Fft
    {        
        private static Complex w(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }
        /// <summary>
        /// Возвращает спектр сигнала
        /// </summary>
        /// <param name="x">Массив значений сигнала. Количество значений должно быть степенью 2</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static Complex[] fft(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = fft(x_even);
                Complex[] X_odd = fft(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + w(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                }
            }
            return X;
        }
        /// <summary>
        /// Возвращает спектр сигнала (для реального сигнала)
        /// </summary>
        /// <param name="x">Массив значений сигнала. Количество значений должно быть степенью 2</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static short[] fft(short[] x)
        {
            short[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new short[2];
                X[0] = (short)(x[0] + x[1]);
                X[1] = (short)(x[0] - x[1]);
            }
            else
            {
                short[] x_even = new short[N / 2];
                short[] x_odd = new short[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                short[] X_even = fft(x_even);
                short[] X_odd = fft(x_odd);
                X = new short[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = (short)(X_even[i] + Math.Cos(-2 * Math.PI * i / N) * X_odd[i]);
                    X[i + N / 2] = (short)(X_even[i] - Math.Cos(-2 * Math.PI * i / N) * X_odd[i]);
                }
            }
            return X;
        }
        /// <summary>
        /// Центровка массива значений полученных в fft (спектральная составляющая при нулевой частоте будет в центре массива)
        /// </summary>
        /// <param name="X">Массив значений полученный в fft</param>
        /// <returns></returns>
        public static Complex[] nfft(Complex[] X)
        {
            int N = X.Length;
            Complex[] X_n = new Complex[N];
            for (int i = 0; i < N / 2; i++)
            {
                X_n[i] = X[N / 2 + i];
                X_n[N / 2 + i] = X[i];
            }
            return X_n;
        }
        /// <summary>
        /// Центровка массива значений полученных в fft (спектральная составляющая при нулевой частоте будет в центре массива)(для реального сигнала)
        /// </summary>
        /// <param name="X">Массив значений полученный в fft</param>
        /// <returns></returns>
        public static short[] nfft(short[] X)
        {
            int N = X.Length;
            short[] X_n = new short[N];
            for (int i = 0; i < N / 2; i++)
            {
                X_n[i] = X[N / 2 + i];
                X_n[N / 2 + i] = X[i];
            }
            return X_n;
        }
        /// <summary>
        /// Возвращает спектр сигнала расчитаный паралельным методом
        /// </summary>
        /// <param name="x">Массив значений сигнала. Количество значений должно быть степенью 2</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static Complex[] fftParallel(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];

                Parallel.For(0, N / 2, i =>
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                });

                Complex[] X_even = fftParallel(x_even);
                Complex[] X_odd = fftParallel(x_odd);
                X = new Complex[N];
                Parallel.For(0, N / 2, i =>
                {
                    X[i] = X_even[i] + w(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                });
            }
            return X;
        }
        /// <summary>
        /// Центровка массива значений полученных в fft (спектральная составляющая при нулевой частоте будет в центре массива). Паралельный метод
        /// </summary>
        /// <param name="X">Массив значений полученный в fft</param>
        /// <returns></returns>
        public static Complex[] nfftParallel(Complex[] X)
        {
            int N = X.Length;
            Complex[] X_n = new Complex[N];
             Parallel.For(0, N / 2, i =>
                {
                X_n[i] = X[N / 2 + i];
                X_n[N / 2 + i] = X[i];
                });
            return X_n;
        }        
    }

    //class GPU_FFT
    //{
    //    ComputeContextPropertyList contextPropetyList;
    //    List<ComputeDevice> Devs = new List<ComputeDevice>();
    //    ComputeContext context; // контекст (для управління об'єктами, такими як командних черг, пам'яті, програми та об'єктів ядра, і для виконання ядра на один або кілька пристроїв)
    //    ComputeCommandQueue command_queue; //команда 
    //    ComputeProgram program; // програма (компіляція і лінковка всіх кодів в файлах .сl)
    //    ComputeKernel kernel; //  робить з функцій з ідендифікаторами __kernel реальний kernel :)
    //    string[] platform_id; // id доступних платформ	
    //    uint ret_num_platforms = 0; //кількість доступних платформ
    //    uint ret = 0; // флажок помилки
    //    string[] cdDevices; // список id доступних пристроїв
    //    uint ciDeviceCount = 0; // кількість доступних пристроїв
    //    const Int32 szGlobalWorkSize = 81920; // загальна кількість work-items, які будуть виконуватись        
    //    const Int16 GroupWorkSize = 128; // ниток в блоці
    //    ComputeBuffer<byte> dev_inData;
    //    string source_str = @"
    //    typedef struct 
    //            {	
	   //             short i;
	   //             short q;
    //            } iq;
    //            typedef struct
    //            {	
	   //             float i;
	   //             float q;
    //            } iqf;
        
    //    ";
    //}
}
