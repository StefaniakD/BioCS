using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioCS
{
    class Program
    {
        //Metryka
        public static int  CompareD(char w, char u)
        {
            if (w == u) return 0;
            else return 1;
        }

        //Podobienstwo
        public static int CompareS(char w, char u)
        {
            if (w == u) return 5;
            else return -5;
        }

        //Funkcja Kary
        public static int Penalty(int x)
        {
            return -(2 + x);
            //return -2;
        }

        static void Main(string[] args)
        {
            
        //Czytanie pliku, zapisanie słow to char[]
        String[] input = System.IO.File.ReadAllLines(@"input.txt");

            char[] Word1 = input[0].ToCharArray();
            char[] Word2 = input[1].ToCharArray();

            int Word1Length = Word1.Length;
            int Word2Length = Word2.Length;

            int penalty;

            int[,] D;
            int[,] A;
            int[,] B;
            int[,] C;
            int[,] S;

            List<char> DList;
            int DLength;

            List<char> SList;
            int SLength;

            char[] SPath;
            char[] Word1S;
            char[] Word2S;

            int iIndicator;
            int jIndicator;

            char[] DPath;
            char[] Word1D;
            char[] Word2D;


            //Wypełnienie tablicy D
            D = new int[Word1Length + 1, Word2Length + 1];
            D[0, 0] = 0;
            for (int i = 1; i <= Word1Length; i++) D[i, 0] = i;
            for (int j = 1; j <= Word2Length; j++) D[0, j] = j;
            for (int i=1; i <= Word1Length; i++)
            {
                for(int j =1; j<= Word2Length; j++)
                {
                    int val = 0;
                    int val1 = D[i - 1, j - 1] + CompareD(Word1[i-1], Word2[j-1]);
                    int val2 = D[i, j - 1] + CompareD('a','b');
                    int val3 = D[i - 1, j] + CompareD('a','b');
                    val = val1;
                    if (val2 < val) val = val2;
                    if (val3 < val) val = val3;
                    D[i, j] = val;                    
                }
            }

            //Wypełnienie tablic A,B,C,S
            A = new int[Word1Length + 1, Word2Length + 1];
            B = new int[Word1Length + 1, Word2Length + 1];
            C = new int[Word1Length + 1, Word2Length + 1];
            S = new int[Word1Length + 1, Word2Length + 1];
            penalty = -2;

            for(int i = 0; i <= Word1Length; i++)
            {
                for (int j = 0; j<= Word2Length; j++)
                {
                    S[i, j] = A[i, j] = B[i, j] = C[i, j] = -100;
                }
            }

            S[0, 0] = 0;
            for (int i = 1; i <= Word1Length; i++)
            {
                S[i, 0] = B[i, 0] = Penalty(i);
            }
            for (int j = 1; j <= Word2Length; j++)
            {
                S[0, j] = A[0, j] = Penalty(j);
            }

            for (int i = 1; i <= Word1Length; i++)
            {
                for (int j = 1; j <= Word2Length; j++)
                {
                    int valA = -100;
                    for(int k = 0; k < j; k++)
                    {
                        int valAB = B[i, k];
                        if (C[i, k] > valAB) valAB = C[i, k];
                        valAB = valAB + Penalty(j-k);
                        if (valAB > valA) valA = valAB;
                    }
                    A[i, j] = valA;

                    int valB = -100;
                    for (int k = 0; k < i; k++)
                    {
                        int valBA = A[k,j];
                        if (C[k,j] > valBA) valBA = C[k,j];
                        valBA = valBA + Penalty(i-k);
                        if (valBA > valB) valB = valBA;
                    }
                    B[i, j] = valB;

                    C[i,j]=S[i-1,j-1]+ CompareS(Word1[i - 1], Word2[j - 1]);

                    int valS = A[i, j];
                    if (B[i, j] > valS) valS = B[i, j];
                    if (C[i, j] > valS) valS = C[i, j];
                    S[i, j] = valS;
                }
            }
            //Wyznaczenie optymalnego dopasowania D (najkrotsza droga)
            DList = new List<char>();
            iIndicator = Word1Length;
            jIndicator = Word2Length;
            DLength = 0;

            while (iIndicator > 0 && jIndicator > 0)
            {
                int val = D[iIndicator, jIndicator];
                if (D[iIndicator-1,jIndicator-1] < val)
                {
                    DList.Add('\\');
                    DLength++;
                    iIndicator--;
                    jIndicator--;
                }
                else
                {
                    if (D[iIndicator - 1, jIndicator] < val)
                    {
                        DList.Add('-');
                        DLength++;
                        iIndicator--;
                    }
                    else
                    {
                        if (D[iIndicator, jIndicator - 1] < val)
                        {
                            DList.Add('|');
                            DLength++;
                            jIndicator--;
                        }
                        else
                        {
                            DList.Add('\\');
                            DLength++;
                            iIndicator--;
                            jIndicator--;
                        }
                    }
                }
            }
            
            //Stworzenie wektora przejść D i słów Word1i2D
            DPath = new char[DLength];
            Word1D = new char[DLength];
            Word2D = new char[DLength];
            iIndicator = Word1Length;
            jIndicator = Word2Length;

            for (int i= 0; i < DLength; i++)
            {
                DPath[DLength-1 - i] = DList[i];

                switch (DList[i])
                {
                    case '\\':
                        Word1D[DLength - 1 - i] = Word1[iIndicator-1];
                        Word2D[DLength - 1 - i] = Word2[jIndicator-1];
                        iIndicator--;
                        jIndicator--;
                        break;

                    case '-':
                        Word1D[DLength - 1 - i] = Word1[iIndicator-1];
                        Word2D[DLength - 1 - i] = '-';
                        iIndicator--;
                        break;

                    case '|':
                        Word1D[DLength - 1 - i] = '-';
                        Word2D[DLength - 1 - i] = Word2[jIndicator-1];
                        jIndicator--;
                        break;
                }
            }

            //Wyznaczenie optymalnego podobienstwa S (najdluzsza droga)
            SList = new List<char>();
            iIndicator = Word1Length;
            jIndicator = Word2Length;
            SLength = 0;

            while (iIndicator > 0 && jIndicator > 0)
            {
                int val = S[iIndicator, jIndicator];
                if (S[iIndicator - 1, jIndicator - 1] > val)
                {
                    SList.Add('\\');
                    SLength++;
                    iIndicator--;
                    jIndicator--;
                }
                else
                {
                    if (S[iIndicator - 1, jIndicator] > val)
                    {
                        SList.Add('-');
                        SLength++;
                        iIndicator--;
                    }
                    else
                    {
                        if (S[iIndicator, jIndicator - 1] > val)
                        {
                            SList.Add('|');
                            SLength++;
                            jIndicator--;
                        }
                        else
                        {
                            SList.Add('\\');
                            SLength++;
                            iIndicator--;
                            jIndicator--;
                        }
                    }
                }
            }

            //Stworzenie wektora przejść S i słów Word1i2S
            SPath = new char[SLength];
            Word1S = new char[SLength];
            Word2S = new char[SLength];
            iIndicator = Word1Length;
            jIndicator = Word2Length;

            for (int i = 0; i < SLength; i++)
            {
                SPath[SLength - 1 - i] = SList[i];

                switch (SList[i])
                {
                    case '\\':
                        Word1S[SLength - 1 - i] = Word1[iIndicator - 1];
                        Word2S[SLength - 1 - i] = Word2[jIndicator - 1];
                        iIndicator--;
                        jIndicator--;
                        break;

                    case '-':
                        Word1S[SLength - 1 - i] = Word1[iIndicator - 1];
                        Word2S[SLength - 1 - i] = '-';
                        iIndicator--;
                        break;

                    case '|':
                        Word1S[SLength - 1 - i] = '-';
                        Word2S[SLength - 1 - i] = Word2[jIndicator - 1];
                        jIndicator--;
                        break;
                }
            }


            //Wypisanie na konsolę
            //Słowa 1
            foreach (char c in Word1)
            {
                System.Console.Write(c);
            }
            System.Console.WriteLine(" "+Word1Length.ToString());
            //Słowa 2
            foreach (char c in Word2)
            {
                System.Console.Write(c);
            }
            System.Console.WriteLine(" "+Word2Length.ToString());
            System.Console.WriteLine();
            //Tablica D
            for (int j = 0; j<= Word2Length; j++)
            {
                for(int i = 0; i<= Word1Length; i++)
                {
                    System.Console.Write(D[i, j].ToString() + " ");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
            //Ścieżka D
            foreach (char c in DPath)
            {
                System.Console.Write(c.ToString() + " ");
            }
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine("Odległość edycyjna: " + D[Word1Length, Word2Length]);
            System.Console.WriteLine();
            //Słowo 1 odległość edycyjna
            foreach (char c in Word1D)
            {
                System.Console.Write(c.ToString() + " ");
            }
            System.Console.WriteLine();
            //Słowo 2 odległość edycyjna
            foreach (char c in Word2D)
            {
                System.Console.Write(c.ToString() + " ");
            }
            System.Console.WriteLine();
            System.Console.WriteLine();
            //A
            for (int j = 0; j <= Word2Length; j++)
            {
                for (int i = 0; i <= Word1Length; i++)
                {
                    System.Console.Write(A[i, j].ToString() + " ");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
            //B
            for (int j = 0; j <= Word2Length; j++)
            {
                for (int i = 0; i <= Word1Length; i++)
                {
                    System.Console.Write(B[i, j].ToString() + " ");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
            //C
            for (int j = 0; j <= Word2Length; j++)
            {
                for (int i = 0; i <= Word1Length; i++)
                {
                    System.Console.Write(C[i, j].ToString() + " ");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
            //S
            for (int j = 0; j <= Word2Length; j++)
            {
                for (int i = 0; i <= Word1Length; i++)
                {
                    System.Console.Write(S[i, j].ToString() + " ");
                }
                System.Console.WriteLine();
            }
            System.Console.WriteLine();
            //Ścieżka S
            foreach (char c in SPath)
            {
                System.Console.Write(c.ToString() + " ");
            }
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine("Podobienstwo wyrazow: " + S[Word1Length, Word2Length]);
            System.Console.WriteLine();
            //Słowo 1 podobienstwo
            foreach (char c in Word1S)
            {
                System.Console.Write(c.ToString() + " ");
            }
            System.Console.WriteLine();
            //Słowo 2 podobienstwo
            foreach (char c in Word2S)
            {
                System.Console.Write(c.ToString() + " ");
            }
            System.Console.WriteLine();
            System.Console.WriteLine();

            System.Console.ReadKey();            
        }
    }
}
