using System;
using System.Text;

namespace BioCS
{
    class HirshbergAlignment
    {
        //Zmienne przechowujące wynik dopasowania
        public static char[] Word1Result;
        public static char[] Word2Result;

        //Wartości dopasowania/niedopasowania
        public const int Match = Program.MATCH;
        public const int Mismatch = Program.MISMATCH;
        public const int Gap = Program.PENALTY;
        //public const int Match = 2;
        //public const int Mismatch = -1;
        //public const int Gap = -2;

        //Porównanie dwóch znaków
        public static int Compare(char a, char b)
        {
            return (a == b) ? Match : Mismatch;
            //if (a == b) return Match;
            //else return Mismatch;
        }

        //Tworzy Macierz S, wykorzystując jedynie dwa rzędy pamięci na raz
        //Zwraca wartość maksymalna.
        private static int MaxScore(char[] u, char[] w)
        {
            int uLength = u.Length;
            int wLength = w.Length;

            int[] h0 = new int[uLength + 1];
            int[] h1 = new int[uLength + 1];
            

            for(int j = 1; j < wLength + 1; j++)
            {
                for (int i = 1; i < uLength + 1; i++)
                {
                    int max = 0;
                    int a = h0[i - 1] + ((u[i - 1] == w[j - 1]) ? Match : Mismatch);
                    if (a > max) max = a;
                    a = h1[i - 1] + Gap;
                    if (a > max) max = a;
                    a = h0[i] + Gap;
                    if (a > max) max = a;

                    h1[i] = max;
                    
                }
                h0 = h1;
                h1 = new int[uLength + 1];
            }
            return h0[uLength];
        }

        //Oblicza dopasowania dla najprostszych przypadków algoytmu Hirschberga
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
                    int max = f[i - 1, j - 1] + ((u[i - 1] == w[j - 1]) ? Match : Mismatch);
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
                    (f[i - 1, j - 1] + ((u[i - 1] == w[j - 1]) ? Match : Mismatch)))
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

        //Zwraca ostatni wiersz tabeli Needlman-Wunsch'a
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
                    score1[j] = Math.Max(score0[j - 1] + ((u[i - 1] == w[j - 1]) ? Match : Mismatch),
                        Math.Max(score1[j - 1] + Gap, score0[j] + Gap)); ;

                }
                score0 = score1;
                score1 = new int[w.Length + 1];
            }
            return score0;
        }

        //Oblicza podwektor na podstawie wektora
        private static char[] SubCharArray(char[] x,int start, int end, bool shouldReverse)
        {
            char[] result = new char[end - start];
            Array.Copy(x, start, result, 0, end - start);
            if (shouldReverse)
                Array.Reverse(result);
            return result;
        }

        //Oblicza 
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
            Hirshberg(u, w, out Word1Result, out Word2Result);

            Console.WriteLine("Podobienstwo sekwencji (Hirshberg): " + MaxScore(u, w));
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
