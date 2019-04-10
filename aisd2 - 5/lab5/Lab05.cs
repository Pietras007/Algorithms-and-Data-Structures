using System;
using System.Collections.Generic;
using ASD.Graphs;
using System.Linq;

namespace ASD
{
    public class RoutePlanner : MarshalByRefObject
    {

        // mozna dodawac metody pomocnicze

        /// <summary>
        /// Znajduje dowolny cykl skierowany w grafie lub stwierdza, że go nie ma.
        /// </summary>
        /// <param name="g">Graf wejściowy</param>
        /// <returns>Kolejne wierzchołki z cyklu lub null, jeśli cyklu nie ma.</returns>
        public int[] FindCycle(Graph g)
        {
            Stack<int> Stos = new Stack<int>();
            int[] odwiedzone = new int[g.VerticesCount];
            int pocz_cyklu = -1;

            Predicate<int> preVisit = delegate (int n)
            {
                odwiedzone[n] = 1;
                Stos.Push(n);

                return true;
            };

            Predicate<int> postVisit = delegate (int n)
            {
                Stos.Pop();
                odwiedzone[n] = 2;
                return true;
            };
            Predicate<Edge> edgeVisit = delegate (Edge e)
            {
                if (odwiedzone[e.To] == 1)
                {
                    pocz_cyklu = e.To;
                    return false;
                }
                return true;
            };

            g.GeneralSearchAll<EdgesStack>(preVisit, postVisit, edgeVisit, out int cc);

            List<int> Cycle_list = new List<int>();

            if (Stos.Count == 0)
            {
                return null;
            }

            Cycle_list.Add(pocz_cyklu);
            int k = Stos.Pop();
            while (k != pocz_cyklu)
            {
                Cycle_list.Add(k);
                k = Stos.Pop();
            }
            Cycle_list.Reverse();

            return Cycle_list.ToArray();
        }

        /// <summary>
        /// Rozwiązanie wariantu 1.
        /// </summary>
        /// <param name="g">Graf połączeń, które trzeba zrealizować</param>
        /// <returns>Lista tras autobusów lub null, jeśli zadanie nie ma rozwiązania</returns>
        public int[][] FindShortRoutes(Graph g)
        {
            for (int i = 0; i < g.VerticesCount; i++)
            {
                var a = g.InDegree(i);
                var b = g.OutDegree(i);
                if (a != b)
                {
                    return null;
                }
            }

            if (FindCycle(g) == null)
            {
                return null;
            }

            Graph graf = g.Clone();
            List<int[]> result = new List<int[]>();
            int ktory_el_listy = 0;

            int[] cykl;
            for (; (cykl = FindCycle(graf)) != null;)
            {
                result.Add(new int[cykl.Length]);

                for (int i = 0; i < cykl.Length; i++)
                {
                    result[ktory_el_listy][i] = cykl[i];
                }

                ktory_el_listy++;

                //Dodac do listy Edge
                List<Edge> krawedzie = new List<Edge>();
                for (int i = 0; i < cykl.Length - 1; i++)
                {
                    krawedzie.Add(new Edge(cykl[i], cykl[i + 1]));
                }
                krawedzie.Add(new Edge(cykl[cykl.Length - 1], cykl[0]));

                //Usunac z grafu
                foreach (var e in krawedzie)
                {
                    graf.DelEdge(e);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Rozwiązanie wariantu 2.
        /// </summary>
        /// <param name="g">Graf połączeń, które trzeba zrealizować</param>
        /// <returns>Lista tras autobusów lub null, jeśli zadanie nie ma rozwiązania</returns>
        /// </summary>
        public int[][] FindLongRoutes(Graph g)
        {

            for (int i = 0; i < g.VerticesCount; i++)
            {
                if (g.InDegree(i) != g.OutDegree(i))
                {
                    return null;
                }
            }

            if (FindCycle(g) == null)
            {
                return null;
            }

            Graph graf = g.Clone();
            List<int[]> result = new List<int[]>();
            int ktory_el_listy = 0;
            int[] tab_ile_razy = new int[g.VerticesCount];

            int[] cykl;
            for (; (cykl = FindCycle(graf)) != null;)
            {
                result.Add(new int[cykl.Length]);

                for (int i = 0; i < cykl.Length; i++)
                {
                    result[ktory_el_listy][i] = cykl[i];
                    tab_ile_razy[cykl[i]]++;
                }

                ktory_el_listy++;

                //Dodac do listy Edge
                List<Edge> krawedzie = new List<Edge>();
                for (int i = 0; i < cykl.Length - 1; i++)
                {
                    krawedzie.Add(new Edge(cykl[i], cykl[i + 1]));
                }
                krawedzie.Add(new Edge(cykl[cykl.Length - 1], cykl[0]));

                //Usunac z grafu
                foreach (var e in krawedzie)
                {
                    graf.DelEdge(e);
                }
            }

            int ile_razy_wykonalem = 0;
            int[][] wynik = result.ToArray();
            int count_of_tables = wynik.Length;
            for (int nr = 0; nr < g.VerticesCount; nr++)
            {
                if (tab_ile_razy[nr] > 1)
                {
                    ile_razy_wykonalem = 0;
                    for (int i = 0; i < wynik.Length; i++)
                    {

                        if (wynik[i] != null)
                        {
                            if (wynik[i].Contains(nr))
                            {
                                for (int j = i + 1; j < wynik.Length; j++)
                                {
                                    if (wynik[j] != null)
                                    {
                                        if (wynik[j].Contains(nr))
                                        {
                                            Combine(ref wynik, j, i, nr);
                                            wynik[j] = null;
                                            count_of_tables--;
                                            ile_razy_wykonalem++;
                                            if (ile_razy_wykonalem - 1 == tab_ile_razy[nr])
                                            {
                                                j = wynik.Length;
                                                i = wynik.Length;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            int[][] koniec_wynik = new int[count_of_tables][];
            int index = 0;
            for (int i = 0; i < wynik.Length; i++)
            {
                if (wynik[i] != null)
                {
                    koniec_wynik[index] = wynik[i];
                    index++;
                }
            }
            return koniec_wynik;
        }

        public bool Combine(ref int[][] tab, int a, int b, int welding_number)
        {
            int[] cycle = new int[tab[a].Length + tab[b].Length];
            int index_cycle = 0;
            int pom = 0;
            //przepisuje z b do welding_number wlacznie
            for (; tab[b][pom] != welding_number;)
            {
                cycle[index_cycle] = tab[b][pom];
                index_cycle++;
                pom++;
            }
            cycle[index_cycle] = tab[b][pom];
            index_cycle++;
            pom++;

            //przepisuje z a za welding number
            bool after_welding = false;
            for (int k = 0; k < tab[a].Length; k++)
            {
                if (after_welding == true)
                {
                    cycle[index_cycle] = tab[a][k];
                    index_cycle++;
                }
                if (tab[a][k] == welding_number)
                {
                    after_welding = true;
                }
            }

            //przepisuje z a od 0 do welding_number wlacznie
            int index_b = 0;
            for (; tab[a][index_b] != welding_number;)
            {
                cycle[index_cycle] = tab[a][index_b];
                index_cycle++;
                index_b++;
            }
            cycle[index_cycle] = tab[a][index_b];
            index_cycle++;

            //przepisuje z b poza welding_number
            after_welding = false;
            for (int k = 0; k < tab[b].Length; k++)
            {
                if (after_welding == true)
                {
                    cycle[index_cycle] = tab[b][k];
                    index_cycle++;
                }
                if (tab[b][k] == welding_number)
                {
                    after_welding = true;
                }
            }


            tab[a] = null;
            tab[b] = cycle;
            return true;
        }
    }
}