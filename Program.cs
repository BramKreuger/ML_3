using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.Threading;

namespace ML_3
{
    class Program
    {
        static int N;
        static int M = 1; //Dit moet anders
        static int d = 57;
        static Matrix<double> x;
        static Vector<double>    y;
        static Vector<double> w;

        static void Main(string[] args)
        {
            var xy = ToMatrix(true);
            x = xy.Item1;
            y = xy.Item2;
            w = CreateWheights();
        }

        #region Voor het initaliseren van de matrices en gewichten.


        ///<summary>
        ///Transforms list of "mail" data into an x matrix and an y vector
        ///</summary>
        static Tuple<Matrix<double>, Vector<double>> ToMatrix(bool randomized)
        {
            SpamClassificatie main = new SpamClassificatie();

            List<Mail> allData = new List<Mail>();
            main.LoadFile("spambase.txt", allData); //\bin\Debug
            N = allData.Count;

            if (randomized)
                allData.Shuffle();

            Matrix<double> x = Matrix<double>.Build.Dense(N, d); //Create x Matrix

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < d; j++)
                {
                    x[i, j] = allData[i].x[j]; //Fill x Matrix
                }
            }

            Vector<double> y = Vector<double>.Build.Dense(N); //Create y Vector

            for (int i = 0; i < N; i++)
            {
                y[i] = allData[i].y; //Fill y Vector
            }

            return new Tuple<Matrix<double>, Vector<double>>(x, y);
        }

        /// <summary>
        /// Return a vector of weights with the value 1/N.
        /// </summary>
        static Vector<double> CreateWheights() => Vector<double>.Build.Dense(N, 1 / N);


        #endregion

        #region Voor het berekenen van de uiteindelijke G(x)


        /// <summary>
        /// Geef als input welk model hij meekrijgt en welk datapunt hij moet classificeren..?
        /// </summary>
        static int BaseLearner(int m, int x)
        {
            return 1; //Hij moet per datapunt goed of fout, dus 1 of -1 returnen.
        }        

        /// <summary>
        /// Berekend de error functie zoals beschreven in slide 19 van college 12b.
        /// </summary>
        /// <returns></returns>
        static double Error(int m)
        {
            double errM = 0;
            for (int i = 0; i < N; i++) //Itereer door de datapunten heen
            {
                errM += (w[i] * I(Convert.ToInt32(y[i]), BaseLearner(m, i))) / w[i];
            }
            return errM;
        }

        /// <summary>
        /// Bereken de Alpha waarde zoals beschreven in slide 19 van college 12b.
        /// </summary>
        static double Alpha(int m)
        {
            return (1 / 2) * Math.Log((1 - Error(m)) / Error(m));
        }

        /// <summary>
        /// Itereer over alle gewichten heen en update ze en normaliseer ze daarna.
        /// </summary>
        static void UpdateWeights(int m)
        {
            double weightSum = 0;
            for (int i = 0; i < N; i++) //Update gewichten
            {
                w[i] = w[i] * Math.Exp(-y[i] * BaseLearner(m, i) * Alpha(m));
                weightSum += w[i];
            }

            for (int i = 0; i < N; i++) //Normaliseer de gewichten door ze te delen door het totale gewicht.
            {
                w[i] = w[i] / weightSum;
            }
        }

        /// <summary>
        /// De "I" functie uit de error formule, vergelijkt of 2 int's gelijk zijn zoja: +1, anders -1.
        /// </summary>
        static int I(int a, int b)
        {
            if (a == b) return 1;
            else return -1;
        }

        //En dan nog een eindfunctie die G(X) berekend...

        #endregion
    }

    //Deze onderstaande dingen zijn voor het random shuffelen van de mail lijst
    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

    static class MyExtensions
    {
        ///<summary> Shuffle a list in random order. </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
