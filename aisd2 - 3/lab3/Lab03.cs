using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public class Lab03 : MarshalByRefObject
    {
        // Część 1
        //  Sprawdzenie czy podany ciąg stopni jest grafowy
        //  0.5 pkt
        public bool IsGraphic(int[] sequence)
        {
            //Wstepne przepisania
            int seq_length = sequence.Length;
            int[] seq = (int[])sequence.Clone();
            int index_number = 0;

            //Pierwsze sortowanie
            Array.Sort(seq, Comparer<int>.Create((a, b) => b.CompareTo(a)));

            //Pierwsze waruneki stopu
            if (seq[0] > seq_length)
            {
                return false;
            }

            //USUNIETE ZGODNIE Z EMAILEM!!!!!
            /*
            if (seq[0] == 2 && seq[seq_length - 1] == 2)
            {
                return true;
            }
            */

            //Lemat o usciskach dloni
            if (seq.Sum() % 2 != 0)
            {
                return false;
            }

            while (true)
            {
                //Warunki stopu
                if (seq[seq_length - 1] < 0)
                {
                    return false;
                }

                //Czy kazdy zero
                bool czy_kazdy_zero = true;
                foreach (var a in seq)
                {
                    if (a != 0)
                    {
                        czy_kazdy_zero = false;
                        break;
                    }
                }

                if (czy_kazdy_zero == true)
                {
                    return true;
                }

                //odejmowanie
                int seq_index_number = seq[index_number];
                int zmniejszone_poczatek_index;
                int zmniejszone_ile = 0;
                seq[index_number] = 0;

                //Czy seq_index_number nie jest wiekszy od pozostalych wierzcholkow
                if(seq_index_number > seq_length - index_number - 1)
                {
                    return false;
                }

                //sprawdzenie ile elementow jest rownych
                for (int i = index_number + 1; i < seq_index_number + index_number + 1; i++)
                {
                    if (seq_length > seq_index_number + index_number + 1)
                    {
                        if (seq[seq_index_number + index_number + 1] == seq[i])
                        {
                            zmniejszone_ile++;
                        }
                    }
                    seq[i]--;
                }
                zmniejszone_poczatek_index = seq_index_number + index_number - zmniejszone_ile + 1;

                int niezmniejszone_ile = 0;
                int niezmniejszone_koniec_index = -2;
                for(int i = seq_index_number + index_number + 1; i <seq_length; i++)
                {
                    if(seq[seq_index_number + index_number + 1] == seq[i])
                    {
                        niezmniejszone_ile++;
                        niezmniejszone_koniec_index = i;
                    }
                    else
                    {
                        break;
                    }
                }

                //Tyle bedzie dodawanych lub odejmowanych
                int ile_zmieniamy;
                if (zmniejszone_ile < niezmniejszone_ile)
                {
                    ile_zmieniamy = zmniejszone_ile;
                }
                else
                {
                    ile_zmieniamy = niezmniejszone_ile;
                }

                //sortowanie tylko czesci elementow poprzezdodanie +-1
                int k = niezmniejszone_koniec_index;
                for (int i=zmniejszone_poczatek_index; i<zmniejszone_poczatek_index + ile_zmieniamy;i++)
                {
                    int pom = seq[i];
                    seq[i] = seq[k];
                    seq[k] = pom;
                    k--;
                }

                index_number++;
            }
        }

        //Część 2
        // Konstruowanie grafu na podstawie podanego ciągu grafowego
        // 1.5 pkt
        public Graph ConstructGraph(int[] sequence)
        {

            //Jezeli nie mozna
            if (IsGraphic(sequence) == false)
                return null;

            //Poczatkowe przypisania
            Graph graf = new AdjacencyListsGraph<SimpleAdjacencyList>(false, sequence.Length);
            int seq_length = sequence.Length;
            int[] seq = (int[])sequence.Clone();
            int[] numery = new int[seq_length];

            //Tablica numerow wierzcholkow
            for (int i = 0; i < seq_length; i++)
            {
                numery[i] = i;
            }

            //Pierwsze sortowanie
            Array.Sort(seq, numery, Comparer<int>.Create((a, b) => b.CompareTo(a)));

            int ktora_krawedz = 0;
            int index_number = 0;
            while (true)
            {
                //Warunek stopu
                bool czy_kazdy_zero = true;
                foreach (var a in seq)
                {
                    if (a != 0)
                    {
                        czy_kazdy_zero = false;
                    }
                }

                if (czy_kazdy_zero == true)
                {
                    break;
                }

                //odejmowanie
                int seq_index_number = seq[index_number];
                int zmniejszone_poczatek_index;
                int zmniejszone_ile = 0;
                seq[index_number] = 0;

                //sprawdzenie ile elementow jest rownych
                for (int i = index_number + 1; i < seq_index_number + index_number + 1; i++)
                {
                    if (seq_length > seq_index_number + index_number + 1)
                    {
                        if (seq[seq_index_number + index_number + 1] == seq[i])
                        {
                            zmniejszone_ile++;
                        }
                    }
                    seq[i]--;
                    graf.AddEdge(new Edge(numery[index_number], numery[i]));
                    ktora_krawedz++;
                }
                zmniejszone_poczatek_index = seq_index_number + index_number - zmniejszone_ile + 1;

                int niezmniejszone_ile = 0;
                int niezmniejszone_koniec_index = -2;
                for (int i = seq_index_number + index_number + 1; i < seq_length; i++)
                {
                    if (seq[seq_index_number + index_number + 1] == seq[i])
                    {
                        niezmniejszone_ile++;
                        niezmniejszone_koniec_index = i;
                    }
                    else
                    {
                        break;
                    }
                }

                //Tyle bedzie dodawanych lub odejmowanych
                int ile_zmieniamy;
                if (zmniejszone_ile < niezmniejszone_ile)
                {
                    ile_zmieniamy = zmniejszone_ile;
                }
                else
                {
                    ile_zmieniamy = niezmniejszone_ile;
                }

                //sortowanie tylko czesci elementow poprzez dodanie +-1 (ostatecznie jednak poprzez zamiane)
                int k = niezmniejszone_koniec_index;
                for (int i = zmniejszone_poczatek_index; i < zmniejszone_poczatek_index + ile_zmieniamy; i++)
                {
                    int pom = seq[i];
                    seq[i] = seq[k];
                    seq[k] = pom;

                    int pom2 = numery[i];
                    numery[i] = numery[k];
                    numery[k] = pom2;

                    k--;
                }

                index_number++;
            }

            return graf;
        }

        //Część 3
        // Wyznaczanie minimalnego drzewa (bądź lasu) rozpinającego algorytmem Kruskala
        // 2 pkt
        public Graph MinimumSpanningTree(Graph graph, out double min_weight)
        {
            if (graph.Directed == true)
            {
                throw new ArgumentException();
            }

            EdgesMinPriorityQueue edges = new EdgesMinPriorityQueue();
            Predicate<Edge> wyjscie = delegate (Edge a)
            {
                if (a.From < a.To)
                {
                    edges.Put(a);
                }
                return true;
            };

            graph.GeneralSearchAll<EdgesStack>(null, null, wyjscie, out int cc);

            UnionFind union = new UnionFind(graph.VerticesCount);
            Graph graf = new AdjacencyListsGraph<SimpleAdjacencyList>(false, graph.VerticesCount);
            min_weight = 0;
            int ile_wstawione_krawedzi = 0;
            int ile_wierzcholkow = graph.VerticesCount;
            while(edges.Empty == false)
            {
                Edge e = edges.Get();
                if (union.Find(e.From) != union.Find(e.To))
                {
                    union.Union(e.From, e.To);
                    graf.AddEdge(e);
                    min_weight += e.Weight;
                    ile_wstawione_krawedzi++;
                }

                if (ile_wstawione_krawedzi == ile_wierzcholkow - 1)
                {
                    break;
                }
            }
            return graf;
        }
    }
}