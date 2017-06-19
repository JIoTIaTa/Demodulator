﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cloo;
using System.Numerics;

namespace Exponentiation
{
    [StructLayout(LayoutKind.Sequential)]
    public struct iq
    {
        public short i;
        public short q;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct iqd
    {
        public double i;
        public double q;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct iqf
    {
        public float i;
        public float q;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct sIQData
    {
        [FieldOffset(0)]
        public Byte[] bytes;

        [FieldOffset(0)]
        public short[] shorts;

        [FieldOffset(0)]
        public iq[] iq;
    }

    public class Quadrature_AM_detector
    {
        public double SR = 0.0d;
        public static sIQData _inData, _outData, _bufferData, _expData, _shiftingData;
        public long F = 0;
        public int login;
        public bool sendComand = false;
        private double tempI = 0;
        private double tempQ = 0;
        public bool show;
        long normalizeCoeff = 1;
        public int x = 1; // коеф інтерполяції
        public int Count;
        public int maxFFT = 65536;
        public int degree = 4;
        public bool busy = false;
        public int averagingValue = 5;
        float[] sin_1024 = new float[1024];
        float[] cos_1024 = new float[1024];
        public double realSpeedPosition; // швидкість модуляції
        public double realCentralFrequencyPosition; // центральна частота
        float sin_cos_position;
        public Complex[] avering_buffer = new Complex[65536];

        public void sin_cos_init()
        {
            for (int i = 0; i < 1024; i++)
            {
                sin_1024[i] = (float)Math.Sin(i * Math.PI * 2 / 1024);
                cos_1024[i] = (float)Math.Cos(i * Math.PI * 2 / 1024);
            }
        }

        public void detection(byte[] inData, byte[] outData)
        {            
            sin_cos_position = (float)(realCentralFrequencyPosition * 1024f / SR);
            Count = inData.Length / 4; // величина вхідних масивів
            _inData.bytes = inData;
            _outData.bytes = outData;
            byte[] data = new byte[inData.Length * x];
            byte[] data2 = new byte[inData.Length];
            byte[] shifting_data = new byte[inData.Length];
            _bufferData.bytes = data;
            _expData.bytes = data2;
            _shiftingData.bytes = shifting_data;
            if (degree == 2)
            {
                normalizeCoeff = 100000;
            } //5
            if (degree == 4)
            {
                normalizeCoeff = 1000000000000000;
            } //14
            if (x == 1)
            {
                for (int i = 0; i < Count; i++)
                {
                    tempI = Math.Abs(_inData.iq[i].i + _inData.iq[i].q);
                    tempQ = 0;
                    _bufferData.iq[i].i = (short)tempI;
                    _bufferData.iq[i].q = (short)tempQ;
                }
            }
            if (x == 2)
            {


                for (int i = 0; i < Count; i++)
                {
                    _bufferData.iq[i * 2].i = _inData.iq[i].i;
                    _bufferData.iq[i * 2].q = _inData.iq[i].q;
                    _bufferData.iq[i * 2 + 1].i = (short)((_inData.iq[i].i + _inData.iq[i + 1].i) / 2);
                    _bufferData.iq[i * 2 + 1].q = (short)((_inData.iq[i].q + _inData.iq[i + 1].q) / 2);
                }
                for (int i = 0; i < Count * 2; i++)
                {
                    tempI = Math.Abs(_bufferData.iq[i].i + _bufferData.iq[i].q);
                    tempQ = 0;
                    _bufferData.iq[i].i = (short)tempI;
                    _bufferData.iq[i].q = (short)tempQ;
                }

            }
            if (degree == 2)
            {
                tempI = 0;
                tempQ = 0;
                for (int i = 0; i < Count; i++)
                {
                    tempI = _inData.iq[i].i * _inData.iq[i].i - _inData.iq[i].q * _inData.iq[i].q;
                    tempQ = 2 * _inData.iq[i].i * _inData.iq[i].q;
                    tempI = tempI / 10;
                    tempQ = tempQ / 10;
                    _outData.iq[i].i = (short)tempI;
                    _outData.iq[i].q = (short)tempQ;
                }
            }
            if (degree == 4)
            {
                tempI = 0;
                tempQ = 0;
                for (int i = 0; i < Count; i++)
                {
                    _expData.iq[i].i = (short)((_inData.iq[i].i * _inData.iq[i].i - _inData.iq[i].q * _inData.iq[i].q) / 10);
                    _expData.iq[i].q = (short)((2 * _inData.iq[i].i * _inData.iq[i].q) / 10);
                    _outData.iq[i].i = (short)((_expData.iq[i].i * _expData.iq[i].i - _expData.iq[i].q * _expData.iq[i].q) / 10); ;
                    _outData.iq[i].q = (short)((2 * _expData.iq[i].i * _expData.iq[i].q) / 10);
                    //_outData.iq[i].i = (short)(_inData.iq[i].i * cos_1024[t] + (float)_inData.iq[i].q * sin_1024[t]);
                    //_outData.iq[i].q = (short)(_inData.iq[i].q * cos_1024[t] - (float)_inData.iq[i].i * sin_1024[t]);
                }
            }
        }

        public void shifting()
        {
            if (realCentralFrequencyPosition > F) sin_cos_position = (float)(realCentralFrequencyPosition * 1024f / SR);
            else sin_cos_position = (float)(1024f + (realCentralFrequencyPosition - F) * 1024f / SR);
            
                for (int i = 0; i < Count; i++)
                {
                    int t = (i * (int)sin_cos_position) % 1024;
                    _shiftingData.iq[i].i = (short)(_shiftingData.iq[i].i + (short)(_inData.iq[i].i * cos_1024[t] + (float)_inData.iq[i].q * sin_1024[t]));
                    _shiftingData.iq[i].q = (short)(_shiftingData.iq[i].q + (short)(_inData.iq[i].q * cos_1024[t] - (float)_inData.iq[i].i * sin_1024[t]));
                }         
            
        }
    }
}