using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioCS
{
    class HirshbergAlignment
    {
        public static char[] Word1Result;
        public static char[] Word2Result;

        public const int Match = 2;
        public const int Mismath = -1;
        public const int Gap = -2;

        public static int Compare(char a, char b)
        {
            return (a == b) ? Match : Mismath;
            //if (a == b) return Match;
            //else return Mismath;
        }

        //Tworzy Macierz S, wykorzystując jedynie dwa rzędy pamięci na raz
        //Zwraca koordynaty i wartość maksymalnej wartości.
        private static void SmithWaterman(char[] u, char[] w, out int max_i, out int max_j, out int max_value)
        {
            int uLength = u.Length;
            int wLength = w.Length;

            int[] h0 = new int[uLength + 1];
            int[] h1 = new int[uLength + 1];

            max_value = 0;
            max_i = 0;
            max_j = 0;

            for(int j = 1; j < wLength + 1; j++)
            {
                for (int i = 1; i < uLength + 1; i++)
                {
                    int max = 0;
                    int a = h0[i - 1] + ((u[i - 1] == w[j - 1]) ? Match : Mismath);
                    if (a > max) max = a;
                    a = h1[i - 1] + Gap;
                    if (a > max) max = a;
                    a = h0[i] + Gap;
                    if (a > max) max = a;

                    h1[i] = max;

                    if (h1[i] > max_value)
                    {
                        max_value = h1[i];
                        max_i = i - 1;
                        max_j = j - 1;
                    }
                }
                h0 = h1;
                h1 = new int[uLength + 1];
            }
        }
        
        private static void LocalToGlobal(char[] u, char[] w, out char[] uLocal, out char[] wLocal, out int maxScore)
        {
            int border0, border1;
            SmithWaterman(u, w, out border0, out border1, out maxScore);
            char[] uu = new char[border0 + 1];
            Array.Copy(u, uu, border0 + 1);
            Array.Reverse(uu);
            char[] ww = new char[border1 + 1];
            Array.Copy(w, ww, border1 + 1);
            Array.Reverse(ww);
            SmithWaterman(uu, ww, out border0, out border1, out maxScore);
            uLocal = new char[border0 + 1];
            Array.Copy(uu, uLocal, border0 + 1);
            Array.Reverse(uLocal);
            wLocal = new char[border1 + 1];
            Array.Copy(ww, wLocal, border1 + 1);
            Array.Reverse(wLocal);
        }

        private static void NeedlemanWunch(char[] u, char[] w, out char[] Word2Result, out char[] Word1Result)
        {
            int i, j;
            int[,] f = new int[u.Length + 1, w.Length + 1];
            for (i = 0; i < u.Length + 1; i++)
                f[i, 0] = Gap * i;
            for (j = 0; j < w.Length + 1; j++)
                f[0, j] = Gap * j;
            for (i = 1; i < u.Length + 1; i++)
                for (j = 1; j < w.Length + 1; j++)
                {
                    int max = f[i - 1, j - 1] + ((u[i - 1] == w[j - 1]) ? Match : Mismath);
                    int a = f[i - 1, j] + Gap;
                    if (a > max) max = a;
                    a = f[i, j - 1] + Gap;
                    if (a > max) max = a;

                    f[i, j] = max;
                }

            StringBuilder alu = new StringBuilder();
            StringBuilder alw = new StringBuilder();

            i = u.Length;
            j = w.Length;

            while (i > 0 || j > 0)
            {
                if (i > 0 && j > 0 && f[i, j] ==
                    (f[i - 1, j - 1] + ((u[i - 1] == w[j - 1]) ? Match : Mismath)))
                {
                    alu.Append(u[i - 1]);
                    alw.Append(w[j - 1]);
                    i--;
                    j--;
                }
                else if (i > 0 && f[i, j] == f[i - 1, j] + Gap)
                {
                    alu.Append(u[i - 1]);
                    alw.Append("-");
                    i--;
                }
                else if (j > 0 && f[i,j] == f[i, j - 1] + Gap)
                {
                    alu.Append("-");
                    alw.Append(w[j - 1]);
                    j--;
                }
            }
            Word1Result = alu.ToString().ToCharArray();
            Array.Reverse(Word1Result);
            Word2Result = alw.ToString().ToCharArray();
            Array.Reverse(Word2Result);
        }

        private static int[] NWScore(char[] u, char[] w)
        {
            int[] score0 = new int[w.Length + 1];
            for (int i = 1; i < w.Length + 1; i++)
                score0[i] = score0[i - 1] + Gap;
            int[] score1 = new int[w.Length + 1];

            for(int i = 1; i < u.Length + 1; i++)
            {
                score1[0] = score0[0] + Gap;
                for (int j = 1; j < w.Length + 1; j++)
                {
                    score1[j] = Math.Max(score0[j - 1] + ((u[i - 1] == w[j - 1]) ? Match : Mismath),
                        Math.Max(score1[j - 1] + Gap, score0[j] + Gap)); ;

                }
                score0 = score1;
                score1 = new int[w.Length + 1];
            }
            return score0;
        }

        private static char[] SubCharArray(char[] x,int start, int end, bool shouldReverse)
        {
            char[] result = new char[end - start];
            Array.Copy(x, start, result, 0, end - start);
            if (shouldReverse)
                Array.Reverse(result);
            return result;
        }

        private static int PartitionY(int[] sl, int[] sr)
        {
            int max_value = int.MinValue;
            int index = -1;
            int[] srr = new int[sr.Length];
            Array.Copy(sr, srr, sr.Length);
            Array.Reverse(srr);
            for (int i = 0; i < sl.Length; i++)
            {
                int value = sl[i] + srr[i];
                if(value > max_value)
                {
                    max_value = value;
                    index = i;
                }
            }
            return index;
        }

        private static void Hirshberg(char[] u, char[] w, out char[] Word1Result, out char[] Word2Result)
        {
            if (u.Length == 0 || w.Length == 0)
            {
                if (u.Length == 0)
                {
                    Word1Result = new char[w.Length];
                    Word2Result = new char[w.Length];
                    Array.Copy(w, Word2Result, w.Length);

                    for (int i = 0; i < w.Length; i++)
                    {
                        Word1Result[i] = '-';
                    }
                }
                else //w.Length == 0
                {
                    Word1Result = new char[u.Length];
                    Word2Result = new char[u.Length];
                    Array.Copy(u, Word1Result, u.Length);

                    for (int i = 0; i < u.Length; i++)
                    {
                        Word2Result[i] = '-';
                    }
                }
            }
            else if (u.Length == 1 || w.Length == 1)
            {
                NeedlemanWunch(u, w, out Word1Result, out Word2Result);
            }
            else
            {
                char[] Word1ResultL, Word2ResultL, Word1ResultR, Word2ResultR;

                int umid = u.Length / 2;
                int[] score_l = NWScore(SubCharArray(u, 0, umid, false),w);
                int[] score_r = NWScore(SubCharArray(u, umid, u.Length, true), SubCharArray(w, 0, w.Length, true));
                int wmid = PartitionY(score_l, score_r);
                Hirshberg(SubCharArray(u, 0, umid, false), SubCharArray(w, 0, wmid, false), out Word1ResultL, out Word2ResultL);
                Hirshberg(SubCharArray(u, umid, u.Length, false), SubCharArray(w, wmid, w.Length, false), out Word1ResultR, out Word2ResultR);

                Word1Result = new char[Word1ResultL.Length + Word1ResultR.Length];
                Array.Copy(Word1ResultL, Word1Result, Word1ResultL.Length);
                Array.Copy(Word1ResultR, 0, Word1Result, Word1ResultL.Length, Word1ResultR.Length);

                Word2Result = new char[Word2ResultL.Length + Word2ResultR.Length];
                Array.Copy(Word2ResultL, Word2Result, Word2ResultL.Length);
                Array.Copy(Word2ResultR, 0, Word2Result, Word2ResultL.Length, Word2ResultR.Length);
            }
        }
        public static void Align(char[] u, char[] w)
        {
            char[] uLocal;
            char[] wLocal;
            int maxScore;
            LocalToGlobal(u, w, out uLocal, out wLocal, out maxScore);

            Hirshberg(uLocal, wLocal, out Word1Result, out Word2Result);

            Console.WriteLine("Podobienstwo sekwencji (Hirshberg): " + maxScore);
            Console.WriteLine("Optymalne dopasowanie (Hirshberg):");
            Console.WriteLine();
            foreach (char c in Word2Result)
                Console.Write(c + " ");
            Console.WriteLine();
            foreach (char c in Word1Result)
                Console.Write(c + " ");
            Console.WriteLine();
        }
    }
}
