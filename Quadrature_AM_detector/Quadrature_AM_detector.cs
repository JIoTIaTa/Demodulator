using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cloo;

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
        [FieldOffset(0)] public Byte[] bytes;

        [FieldOffset(0)] public short[] shorts;

        [FieldOffset(0)] public iq[] iq;
    }

    public class Quadrature_AM_detector
    {
        public double SR = 0.0d;
        public static sIQData _inData, _outData, _bufferData, _bufferData2;
        public long F = 0;
        public int login;
        public bool sendComand = false;
        private double tempI = 0;
        private double tempQ = 0;
        private double averingBefore = 0;
        private double averingAfter = 0;
        private long normalizeCoeff = 1; // 12
        public bool show;
        public int x = 1;
        public int Count;
        public int maxFFT = 65536;
        public int degree = 2;
        public bool busy = false;


        public void quadrature_AM_detector(byte[] inData, byte[] outData)
        {
            Count = inData.Length/4; // величина вхідних масивів
            _inData.bytes = inData;
            _outData.bytes = outData;
            byte[] data = new byte[inData.Length * x];
            byte[] data2 = new byte[inData.Length];
            _bufferData.bytes = data;
            _bufferData2.bytes = data2;
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
                    _bufferData.iq[i*2].i = _inData.iq[i].i;
                    _bufferData.iq[i*2].q = _inData.iq[i].q;
                    _bufferData.iq[i*2 + 1].i = (short) ((_inData.iq[i].i + _inData.iq[i + 1].i)/2);
                    _bufferData.iq[i*2 + 1].q = (short) ((_inData.iq[i].q + _inData.iq[i + 1].q)/2);
                }
                for (int i = 0; i < Count*2; i++)
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
                    tempI = _inData.iq[i].i*_inData.iq[i].i - _inData.iq[i].q*_inData.iq[i].q;
                    tempQ = 2*_inData.iq[i].i*_inData.iq[i].q;
                    tempI = tempI/10;
                    tempQ = tempQ/10;
                    _outData.iq[i].i = (short) tempI;
                    _outData.iq[i].q = (short) tempQ;
                }
            }
            if (degree == 4)
            {
                tempI = 0;
                tempQ = 0;
                for (int i = 0; i < Count; i++)
                {
                    _bufferData2.iq[i].i = (short)((_inData.iq[i].i * _inData.iq[i].i - _inData.iq[i].q * _inData.iq[i].q) / 10);
                    _bufferData2.iq[i].q = (short)((2 * _inData.iq[i].i * _inData.iq[i].q) / 10);
                    _outData.iq[i].i = (short)((_bufferData2.iq[i].i * _bufferData2.iq[i].i - _bufferData2.iq[i].q * _bufferData2.iq[i].q) / 10); ;
                    _outData.iq[i].q = (short)((2 * _bufferData2.iq[i].i * _bufferData2.iq[i].q) / 10); ;
                }
            }
        }
    }
}