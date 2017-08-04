using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TXT_writter
{
    /// <summary>Клас запису даних в файл</summary>
    public sealed class Writter
    {
        private string path = "Array Writer\\";
        private short[] _sdata1;
        private short[] _sdata2;
        private short[] _sdata3;
        private short[] _sdata4;
        private short[] _sdata5;
        private short[] _sdata6;
        private float[] _fdata1;
        private float[] _fdata2;
        private float[] _fdata3;
        private float[] _fdata4;
        private float[] _fdata5;
        private float[] _fdata6;
        public Writter(short[] data1, string name1, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _sdata1 = new short[data1.Length];
            _sdata1 = data1;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}\n", name1, i, _sdata1[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(float[] data1, string name1, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _fdata1 = new float[data1.Length];
            _fdata1 = data1;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}\n", name1, i, _fdata1[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(short[] data1, short[] data2, string name1, string name2, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _sdata1 = new short[data1.Length];
            _sdata2 = new short[data2.Length];
            _sdata1 = data1;
            _sdata2 = data2;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}\n", name1, i, _sdata1[i], name2, i, _sdata2[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(float[] data1, float[] data2, string name1, string name2, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _fdata1 = new float[data1.Length];
            _fdata2 = new float[data2.Length];
            _fdata1 = data1;
            _fdata2 = data2;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}\n", name1, i, _fdata1[i], name2, i, _fdata2[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(short[] data1, short[] data2, short[] data3, string name1, string name2, string name3, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _sdata1 = new short[data1.Length];
            _sdata2 = new short[data2.Length];
            _sdata3 = new short[data3.Length];
            _sdata1 = data1;
            _sdata2 = data2;
            _sdata3 = data3;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}  \t{6}.{7} -> {8}\n", name1, i, _sdata1[i], name2, i, _sdata2[i], name3, i, _sdata3[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(float[] data1, float[] data2, float[] data3, string name1, string name2, string name3, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _fdata1 = new float[data1.Length];
            _fdata2 = new float[data2.Length];
            _fdata3 = new float[data3.Length];
            _fdata1 = data1;
            _fdata2 = data2;
            _fdata3 = data3;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}  \t{6}.{7} -> {8}\n", name1, i, _fdata1[i], name2, i, _fdata2[i], name3, i, _fdata3[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(short[] data1, short[] data2, short[] data3, short[] data4, string name1, string name2, string name3, string name4, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _sdata1 = new short[data1.Length];
            _sdata2 = new short[data2.Length];
            _sdata3 = new short[data3.Length];
            _sdata4 = new short[data4.Length];
            _sdata1 = data1;
            _sdata2 = data2;
            _sdata3 = data3;
            _sdata4 = data4;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}  \t{6}.{7} -> {8}  \t{9}.{10} -> {11}\n", name1, i, _sdata1[i], name2, i, _sdata2[i], name3, i, _sdata3[i], name4, i, _sdata4[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(float[] data1, float[] data2, float[] data3, float[] data4, string name1, string name2, string name3, string name4, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _fdata1 = new float[data1.Length];
            _fdata2 = new float[data2.Length];
            _fdata3 = new float[data3.Length];
            _fdata4 = new float[data4.Length];
            _fdata1 = data1;
            _fdata2 = data2;
            _fdata3 = data3;
            _fdata4 = data4;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}  \t{6}.{7} -> {8}  \t{9}.{10} -> {11}\n", name1, i, _fdata1[i], name2, i, _fdata2[i], name3, i, _fdata3[i], name4, i, _fdata4[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(short[] data1, short[] data2, short[] data3, short[] data4, short[] data5, string name1, string name2, string name3, string name4, string name5, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _sdata1 = new short[data1.Length];
            _sdata2 = new short[data2.Length];
            _sdata3 = new short[data3.Length];
            _sdata4 = new short[data4.Length];
            _sdata5 = new short[data5.Length];
            _sdata1 = data1;
            _sdata2 = data2;
            _sdata3 = data3;
            _sdata4 = data4;
            _sdata5 = data5;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}  \t{6}.{7} -> {8}  \t{9}.{10} -> {11}  \t{12}.{13} -> {14}\n", name1, i, _sdata1[i], name2, i, _sdata2[i], name3, i, _sdata3[i], name4, i, _sdata4[i], name5, i, _sdata5[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(float[] data1, float[] data2, float[] data3, float[] data4, float[] data5, string name1, string name2, string name3, string name4, string name5, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _fdata1 = new float[data1.Length];
            _fdata2 = new float[data2.Length];
            _fdata3 = new float[data3.Length];
            _fdata4 = new float[data4.Length];
            _fdata5 = new float[data5.Length];
            _fdata1 = data1;
            _fdata2 = data2;
            _fdata3 = data3;
            _fdata4 = data4;
            _fdata5 = data5;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}  \t{6}.{7} -> {8}  \t{9}.{10} -> {11}  \t{12}.{13} -> {14}\n", name1, i, _fdata1[i], name2, i, _fdata2[i], name3, i, _fdata3[i], name4, i, _fdata4[i], name5, i, _fdata5[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(short[] data1, short[] data2, short[] data3, short[] data4, short[] data5, short[] data6, string name1, string name2, string name3, string name4, string name5, string name6, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _sdata1 = new short[data1.Length];
            _sdata2 = new short[data2.Length];
            _sdata3 = new short[data3.Length];
            _sdata4 = new short[data4.Length];
            _sdata5 = new short[data5.Length];
            _sdata6 = new short[data6.Length];
            _sdata1 = data1;
            _sdata2 = data2;
            _sdata3 = data3;
            _sdata4 = data4;
            _sdata5 = data5;
            _sdata6 = data6;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}  \t{6}.{7} -> {8}  \t{9}.{10} -> {11}  \t{12}.{13} -> {14  \t{15}.{16} -> {17}\n", name1, i, _sdata1[i], name2, i, _sdata2[i], name3, i, _sdata3[i], name4, i, _sdata4[i], name5, i, _sdata5[i], name6, i, _sdata6[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
        public Writter(float[] data1, float[] data2, float[] data3, float[] data4, float[] data5, float[] data6, string name1, string name2, string name3, string name4, string name5, string name6, string fileName)
        {
            Directory.CreateDirectory("Array Writer");
            path += fileName + ".txt";
            _fdata1 = new float[data1.Length];
            _fdata2 = new float[data2.Length];
            _fdata3 = new float[data3.Length];
            _fdata4 = new float[data4.Length];
            _fdata5 = new float[data5.Length];
            _fdata6 = new float[data6.Length];
            _fdata1 = data1;
            _fdata2 = data2;
            _fdata3 = data3;
            _fdata4 = data4;
            _fdata5 = data5;
            _fdata6 = data6;
            StreamWriter file = new StreamWriter(path);
            for (int i = 0; i < data1.Length; i++)
            {
                file.WriteLine(string.Format("{0}.{1} -> {2}  \t{3}.{4} -> {5}  \t{6}.{7} -> {8}  \t{9}.{10} -> {11}  \t{12}.{13} -> {14  \t{15}.{16} -> {17}\n", name1, i, _fdata1[i], name2, i, _fdata2[i], name3, i, _fdata3[i], name4, i, _fdata4[i], name5, i, _fdata5[i], name6, i, _fdata6[i]));
            }
            MessageBox.Show(string.Format("Wrote here: {0}", path));
            file.Close();
        }
    }
}
