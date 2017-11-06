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
        static int d = 57;
        static Matrix<double> x;
        static Vector<int>    y;
        static Vector<double> w;

        static void Main(string[] args)
        {
            var xy = ToMatrix(true);
            x = xy.Item1;
            y = xy.Item2;
            w = CreateWheights();
        }

        ///<summary>
        ///Transforms list of "mail" data into an x matrix and an y vector
        ///</summary>
        static Tuple<Matrix<double>, Vector<int>> ToMatrix(bool randomized)
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

            Vector<int> y = Vector<int>.Build.Dense(N); //Create y Vector

            for (int i = 0; i < N; i++)
            {
                y[i] = allData[i].y; //Fill y Vector
            }

            return new Tuple<Matrix<double>, Vector<int>>(x, y);
        }

        /// <summary>
        /// Return a vector of weights with the value 1/N.
        /// </summary>
        static Vector<double> CreateWheights() => Vector<double>.Build.Dense(N, 1 / N);

        /// <summary>
        /// Dit is stap (3) hier snap ik dus geen kut van!
        /// </summary>
        static void BaseLearner()
        {
            /*
            p>  = w>+ /  (w>+ + w>-);
            e>  = 2p >   (1 - p>);
            p=< = w=<+ / (w=<+ + w=<-);
            e=< = 2p=<   (1 - p=<);
            e   = (w>+ + w>-)e> + (w=<+ + w=<-)e=< .
            */

            //Calculating w's

            double wGrPl;
            double wGrMi;
            double wSmPl;
            double wSmMi;

            double b = 0.5;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < d; j++)
                {

                }
            }
        }        
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
