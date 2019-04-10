using System;
using System.Collections.Generic;

namespace ASD
{

    public class CodesCounting : MarshalByRefObject
    {

        public int CountCodes(string text, string[] codes, out int[][] solutions)
        {
            int[] tab_ile = new int[text.Length + 1];
            Ile_podlist(text, ref codes, ref tab_ile); //referencje żeby było szybciej
            int[][][] solution_tab = new int[text.Length + 1][][];
            for (int i = 0; i <= text.Length; i++)
            {
                int ile_podlist = 0;
                solution_tab[i] = new int[tab_ile[i]][];
                for (int j = 0; j < codes.Length; j++)
                {

                    int k;
                    if (((k = i - codes[j].Length) >= 0) && (text.Substring(k, i - k) == codes[j]))
                    {
                        if (k == 0)
                        {
                            solution_tab[i][ile_podlist] = new int[1];
                            solution_tab[i][ile_podlist][0] = j;
                            ile_podlist++;
                        }
                        else
                        {
                            int zxc = 0;
                            foreach (var a in solution_tab[k])
                            {
                                solution_tab[i][ile_podlist] = new int[solution_tab[k][zxc].Length + 1];
                                int b;
                                for (b = 0; b < a.Length; b++)
                                {
                                    solution_tab[i][ile_podlist][b] = a[b];
                                }
                                solution_tab[i][ile_podlist][b] = j;
                                zxc++;
                                ile_podlist++;
                            }
                        }
                    }
                }
            }

            solutions = (int[][])solution_tab[text.Length].Clone();
            return tab_ile[tab_ile.Length - 1];
        }

        void Ile_podlist(string text, ref string[] codes, ref int[] tab_ile)
        {
            for (int i = 0; i <= text.Length; i++)
            {
                for (int j = 0; j < codes.Length; j++)
                {
                    int k;
                    if ((k = i - codes[j].Length) >= 0)
                    {
                        if (text.Substring(k, i - k) == codes[j])
                        {
                            if (k == 0)
                            {
                                tab_ile[i]++;
                            }
                            else
                            {
                                tab_ile[i] += tab_ile[k];
                            }
                        }
                    }
                }
            }
        }

    }
}
