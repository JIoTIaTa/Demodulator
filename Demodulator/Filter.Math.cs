using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demodulation
{
    class Filter_Math
    {
        private float[] coefficients;
        double Besself(double x)
        {
            double Sum = 0.0f, XtoIpower;
            int i, j, Factorial;
            for (i = 1; i < 10; i++)
            {
                XtoIpower = Math.Pow(x / 2, i);
                Factorial = 1;
                for (j = 1; j <= i; j++) Factorial *= j;
                Sum += Math.Pow(XtoIpower / Factorial, 2);
            }
            return (1 + Sum);
        }

        double Sinc(double x)
        {
            if (x > -1.0E-5f && x < 1.0E-5f) return ((float)1.0);
            return ((float)Math.Sin(x) / x);
        }

        public float[] BasicFIR(int NumTaps, TPassTypeName PassType, double OmegaC, double BW, TWindowType WindowType, double beta, double alpha = 0.0f)
        {
            coefficients = new float[NumTaps];
            int j;
            double Arg, OmegaLow, OmegaHigh;

            switch (PassType)
            {
                case TPassTypeName.LPF:
                    for (j = 0; j < NumTaps; j++)
                    {
                        Arg = j - (NumTaps - 1) / 2;
                        coefficients[j] = (float)(OmegaC * Sinc(OmegaC * Arg * Math.PI));
                    }
                    break;

                case TPassTypeName.HPF:
                    if (NumTaps % 2 == 1) // Odd tap counts
                    {
                        for (j = 0; j < NumTaps; j++)
                        {
                            Arg = j - (NumTaps - 1) / 2;
                            coefficients[j] = (float)(Sinc(Arg * Math.PI) - OmegaC * Sinc(OmegaC * Arg * Math.PI));
                        }
                    }

                    else  // Even tap counts
                    {
                        for (j = 0; j < NumTaps; j++)
                        {
                            Arg = j - (NumTaps - 1) / 2;
                            if (Arg == 0) coefficients[j] = 0;
                            else coefficients[j] = (float)(Math.Cos(OmegaC * Arg * Math.PI) / Math.PI / Arg + Math.Cos(Arg * Math.PI));
                        }
                    }
                    break;

                case TPassTypeName.BPF:
                    OmegaLow = OmegaC - BW / 2;
                    OmegaHigh = OmegaC + BW / 2;
                    for (j = 0; j < NumTaps; j++)
                    {
                        Arg = j - (NumTaps - 1) / 2;
                        if (Arg == 0) coefficients[j] = 0;
                        else coefficients[j] = (float)((Math.Cos(OmegaLow * Arg * Math.PI) - Math.Cos(OmegaHigh * Arg * Math.PI)) / Math.PI / Arg);
                    }
                    break;

                case TPassTypeName.NOTCH:  // If NumTaps is even for Notch filters, the response at Pi is attenuated.
                    OmegaLow = OmegaC - BW / 2;
                    OmegaHigh = OmegaC + BW / 2;
                    for (j = 0; j < NumTaps; j++)
                    {
                        Arg = (float)j - (float)(NumTaps - 1) / 2;
                        coefficients[j] = (float)(Sinc(Arg * Math.PI) - OmegaHigh * Sinc(OmegaHigh * Arg * Math.PI) - OmegaLow * Sinc(OmegaLow * Arg * Math.PI));
                    }
                    break;
            }
            WindowData(NumTaps, WindowType, alpha, beta, false);
            return coefficients;
        }

        void WindowData(int N, TWindowType WindowType, double Alpha, double Beta, bool UnityGain)
        {
            if (WindowType == TWindowType.NONE) return;

            int j, M, TopWidth;
            double dM;
            double[] WinCoeff;

            if (WindowType == TWindowType.KAISER || WindowType == TWindowType.FLATTOP) Alpha = 0;

            if (Alpha < 0) Alpha = 0;
            if (Alpha > 1) Alpha = 1;

            if (Beta < 0) Beta = 0;
            if (Beta > 10) Beta = 10;

            WinCoeff = new double[N + 2];
            //if(WinCoeff == NULL)
            // {
            //  //ShowMessage("Failed to allocate memory in FFTFunctions::WindowFFTData() ");
            //  return;
            // }
            TopWidth = (int)(Alpha * N);
            if (TopWidth % 2 != 0) TopWidth++;
            if (TopWidth > N) TopWidth = N;
            M = N - TopWidth;
            dM = M + 1;


            if (WindowType == TWindowType.KAISER)
            {
                double Arg;
                for (j = 0; j < M; j++)
                {
                    Arg = Beta * Math.Sqrt(1 - Math.Pow(((2 * j + 2 - dM) / dM), 2));
                    WinCoeff[j] = Besself(Arg) / Besself(Beta);
                }
            }

            else if (WindowType == TWindowType.SINC)  // Lanczos
            {
                for (j = 0; j < M; j++) WinCoeff[j] = Sinc((float)(2 * j + 1 - M) / dM * Math.PI);
                for (j = 0; j < M; j++) WinCoeff[j] = Math.Pow(WinCoeff[j], Beta);
            }

            else if (WindowType == TWindowType.SINE)  // Hanning if Beta = 2
            {
                for (j = 0; j < M / 2; j++) WinCoeff[j] = Math.Sin((float)(j + 1) * Math.PI / dM);
                for (j = 0; j < M / 2; j++) WinCoeff[j] = Math.Pow(WinCoeff[j], Beta);
            }

            else if (WindowType == TWindowType.HANNING)
            {
                for (j = 0; j < M / 2; j++) WinCoeff[j] = 0.5 - 0.5 * Math.Cos((float)(j + 1) * Math.PI * 2 / dM);
            }

            else if (WindowType == TWindowType.HAMMING)
            {
                for (j = 0; j < M / 2; j++)
                    WinCoeff[j] = 0.54 - 0.46 * Math.Cos((float)(j + 1) * Math.PI * 2 / dM);
            }

            else if (WindowType == TWindowType.BLACKMAN)
            {
                for (j = 0; j < M / 2; j++)
                {
                    WinCoeff[j] = 0.42
                    - 0.50 * Math.Cos((float)(j + 1) * Math.PI * 2 / dM)
                    + 0.08 * Math.Cos((float)(j + 1) * Math.PI * 2 * 2 / dM);
                }
            }

            else if (WindowType == TWindowType.FLATTOP)
            {
                for (j = 0; j <= M / 2; j++)
                {
                    WinCoeff[j] = 1
                    - 1.93293488969227 * Math.Cos((float)(j + 1) * Math.PI * 2 / dM)
                    + 1.28349769674027 * Math.Cos((float)(j + 1) * Math.PI * 2 * 2 / dM)
                    - 0.38130801681619 * Math.Cos((float)(j + 1) * Math.PI * 2 * 3 / dM)
                    + 0.02929730258511 * Math.Cos((float)(j + 1) * Math.PI * 2 * 4 / dM);
                }
            }


            else if (WindowType == TWindowType.BLACKMAN_HARRIS)
            {
                for (j = 0; j < M / 2; j++)
                {
                    WinCoeff[j] = 0.35875
                    - 0.48829 * Math.Cos((float)(j + 1) * Math.PI * 2 / dM)
                    + 0.14128 * Math.Cos((float)(j + 1) * Math.PI * 2 * 2 / dM)
                    - 0.01168 * Math.Cos((float)(j + 1) * Math.PI * 2 * 3 / dM);
                }
            }

            else if (WindowType == TWindowType.BLACKMAN_NUTTALL)
            {
                for (j = 0; j < M / 2; j++)
                {
                    WinCoeff[j] = 0.3535819
                    - 0.4891775 * Math.Cos((float)(j + 1) * Math.PI * 2 / dM)
                    + 0.1365995 * Math.Cos((float)(j + 1) * Math.PI * 2 * 2 / dM)
                    - 0.0106411 * Math.Cos((float)(j + 1) * Math.PI * 2 * 3 / dM);
                }
            }

            else if (WindowType == TWindowType.NUTTALL)
            {
                for (j = 0; j < M / 2; j++)
                {
                    WinCoeff[j] = 0.355768
                    - 0.487396 * Math.Cos((float)(j + 1) * Math.PI * 2 / dM)
                    + 0.144232 * Math.Cos((float)(j + 1) * Math.PI * 2 * 2 / dM)
                    - 0.012604 * Math.Cos((float)(j + 1) * Math.PI * 2 * 3 / dM);
                }
            }

            else if (WindowType == TWindowType.KAISER_BESSEL)
            {
                for (j = 0; j <= M / 2; j++)
                {
                    WinCoeff[j] = 0.402
                    - 0.498 * Math.Cos(Math.PI * 2 * (float)(j + 1) / dM)
                    + 0.098 * Math.Cos(2 * Math.PI * 2 * (float)(j + 1) / dM)
                    + 0.001 * Math.Cos(3 * Math.PI * 2 * (float)(j + 1) / dM);
                }
            }

            else if (WindowType == TWindowType.TRAPEZOID) // Rectangle for Alpha = 1  Triangle for Alpha = 0
            {
                int K = M / 2;
                if (M % 2 == 1) K++;
                for (j = 0; j < K; j++) WinCoeff[j] = (float)(j + 1) / (float)K;
            }

            else if (WindowType == TWindowType.GAUSS)
            {
                for (j = 0; j < M / 2; j++)
                {
                    WinCoeff[j] = ((float)(j + 1) - dM / 2) / (dM / 2) * 2.7183;
                    WinCoeff[j] *= WinCoeff[j];
                    WinCoeff[j] = Math.Exp(-WinCoeff[j]);
                }
            }

            else // Error.
            {
                //ShowMessage("Incorrect window type in WindowFFTData");
                //delete[] WinCoeff;
                return;
            }

            // Fold the coefficients over.
            for (j = 0; j < M / 2; j++) WinCoeff[N - j - 1] = WinCoeff[j];

            // This is the flat top if Alpha > 0. Cannot be applied to a Kaiser or Flat Top.
            if (WindowType != TWindowType.KAISER && WindowType != TWindowType.FLATTOP)
            {
                for (j = M / 2; j < N - M / 2; j++) WinCoeff[j] = 1;
            }

            // This will set the gain of the window to 1. Only the Flattop window has unity gain by design. 
            if (UnityGain)
            {
                double Sum = 0;
                for (j = 0; j < N; j++) Sum += WinCoeff[j];
                Sum /= (float)N;
                if (Sum != 0) for (j = 0; j < N; j++) WinCoeff[j] /= Sum;
            }

            // Apply the window to the data.
            for (j = 0; j < N; j++) coefficients[j] *= (float)WinCoeff[j];
            // delete[] WinCoeff;
        }
    }
}
