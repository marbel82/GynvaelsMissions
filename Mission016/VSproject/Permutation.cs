using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCommon
{
    public class Permutation
    {
        public int Numbers { get; private set; }
        public int[] Perm { get { return perm; } }

        int[,] cols;
        int[] poss;
        int[] perm;


        public Permutation(int numbers)
        {
            Numbers = numbers;
            cols = new int[numbers, numbers];
            poss = new int[numbers];
            perm = null;

            // Inicjalizacja wybieraczy
            poss[0] = 0;
            for (int i = 0; i < numbers; i++)
            {
                cols[0, i] = i;
            }

        }

        public bool Next()
        {
            int cc;     // Current Column
            int lbelcc; // Liczba Elementów w Current Column
            int wybi;   // Wybrany Index

            if (perm != null)
            {
                cc = Numbers - 2;
                while (true)
                {
                    lbelcc = Numbers - cc;

                    // Wybranie kolejnej liczby z kolumny
                    poss[cc]++;
                    if (poss[cc] < lbelcc)
                        break;

                    // Cofamy kolumnę
                    cc--;
                    if (cc < 0)
                        return false;
                }
            }
            else
            {
                // Pierwsze wywołanie
                perm = new int[Numbers];
                cc = 0;
                lbelcc = Numbers;
            }

            wybi = poss[cc];
            perm[cc] = cols[cc, wybi];

            while (true)
            {
                cc++;
                if (cc == Numbers)
                    return true;

                lbelcc--;

                // Kopiowanie do kolejnej kolumny, pomijamy wybrany
                // cc jest już na kolejnej kolumnie
                int rp = 0;
                for (int r = 0; r < lbelcc; r++)
                {
                    if (rp == wybi)
                        rp++;
                    cols[cc, r] = cols[cc - 1, rp++];
                }
                wybi = poss[cc] = 0;
                perm[cc] = cols[cc, 0];    
            }    
        }
    }
}
