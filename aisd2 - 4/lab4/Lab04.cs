using System;
using System.Collections.Generic;

namespace ASD
{
    public enum TaxAction
    {
        Empty,
        TakeMoney,
        TakeCarrots
    }

    public class TaxCollectorManager : MarshalByRefObject
    {
        public int CollectMaxTax(int[] dist, int[] money, int[] carrots, int maxCarrots, int startingCarrots, out TaxAction[] collectingPlan)
        {
            collectingPlan = null;
            int liczba_miast = money.Length;
            (int, int)?[,] tablica = new(int, int)?[liczba_miast, maxCarrots + 1];

            ///////////////Pierwsze przypisanie
            //bierze pieniadze
            tablica[0, startingCarrots] = (money[0], -1);

            //bierze marchewki
            int marchewki;
            if (startingCarrots + carrots[0] > maxCarrots)
            {
                marchewki = maxCarrots;
            }
            else
            {
                marchewki = startingCarrots + carrots[0];
            }
            tablica[0, marchewki] = (0, -1);


            ///////////////Petla przypisan
            for (int x = 1; x < liczba_miast; x++)
            {
                for (int i = 0; i < maxCarrots + 1; i++)
                {
                    if (tablica[x - 1, i] != null)
                    {
                        int carrots_prev = i;
                        int money_prev = tablica[x - 1, i].Value.Item1;
                        if (carrots_prev >= dist[x])
                        {
                            //bierze pieniadze i wstawianie do tablicy
                            if (tablica[x, carrots_prev - dist[x]] == null)
                            {
                                tablica[x, carrots_prev - dist[x]] = (money_prev + money[x], carrots_prev);
                            }
                            else
                            {
                                if (tablica[x, carrots_prev - dist[x]].Value.Item1 < money_prev + money[x])
                                {
                                    tablica[x, carrots_prev - dist[x]] = (money_prev + money[x], carrots_prev);
                                }
                            }

                            //bierze marchewki obliczenia
                            if (carrots_prev - dist[x] + carrots[x] > maxCarrots)
                            {
                                marchewki = maxCarrots;
                            }
                            else
                            {
                                marchewki = carrots_prev - dist[x] + carrots[x];
                            }

                            //wstawianie do tablicy
                            if (tablica[x, marchewki] == null)
                            {
                                tablica[x, marchewki] = (money_prev, carrots_prev);
                            }
                            else
                            {
                                if (tablica[x, marchewki].Value.Item1 < money_prev)
                                {
                                    tablica[x, marchewki] = (money_prev, carrots_prev);
                                }
                            }
                        }
                    }
                }
            }

            //Znajdywanie max ilosc pieniedzy, prev_index to index przedostatniego elementu, ktory trafi do collecting plan
            int max = int.MinValue;
            int prev_index = -1;
            for (int i = 0; i < maxCarrots + 1; i++)
            {
                if (tablica[liczba_miast - 1, i] != null)
                {
                    if (tablica[liczba_miast - 1, i].Value.Item1 > max && i >= startingCarrots)
                    {
                        max = tablica[liczba_miast - 1, i].Value.Item1;
                        prev_index = tablica[liczba_miast - 1, i].Value.Item2;
                    }
                }
            }

            //Gdy osiolek nie moze dojsc w zadnym wypadku
            if (max == int.MinValue)
            {
                collectingPlan = null;
                return -1;
            }

            //Przypisanie collecting plan
            collectingPlan = new TaxAction[liczba_miast];
            int money_before = max;
            for (int i = liczba_miast - 1; i > 0; i--)
            {
                if (tablica[i - 1, prev_index].Value.Item1 < money_before)
                {
                    collectingPlan[i] = TaxAction.TakeMoney;
                }
                else
                {
                    collectingPlan[i] = TaxAction.TakeCarrots;
                }
                money_before = tablica[i - 1, prev_index].Value.Item1;
                prev_index = tablica[i - 1, prev_index].Value.Item2;
            }
            if (money_before != 0)
            {
                collectingPlan[0] = TaxAction.TakeMoney;
            }
            else
            {
                collectingPlan[0] = TaxAction.TakeCarrots;
            }

            return max;
        }
    }
}
