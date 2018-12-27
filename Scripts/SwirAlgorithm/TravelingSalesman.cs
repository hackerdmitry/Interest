using System.Collections.Generic;
using System.Linq;
using BackpackTask;
using UnityEngine;

namespace SwirAlgorithm
{
    class TravelingSalesman
    {
        static double[,] ArrayDistance;

        public static List<Item> SolveTravelingSalesman(GameObject home, HashSet<Item> shops)
        {
            List<Item> resList = new List<Item>();
            
            Item firstItem = shops.First(s => BackpackSolve.Distance(home, s.shop) ==
                                              shops.Min(x => BackpackSolve.Distance(home, x.shop)));

            shops.Remove(firstItem);
            resList.Add(firstItem);

            while (shops.Count > 0)
            {
                Item nextItem = null;
                float minDistance = float.MaxValue;
                foreach (Item shop in shops)
                {
                    float dist = BackpackSolve.Distance(firstItem.shop, shop.shop);
                    if (minDistance > dist)
                    {
                        nextItem = shop;
                        minDistance = dist;
                    }
                }
                
                shops.Remove(nextItem);
                resList.Add(nextItem);
                firstItem = nextItem;
            }
            resList.Add(new Item(home, 0, BackpackSolve.Distance(resList.Last().shop, home)));
            return resList;
        }
    }
}