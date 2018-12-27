using System;
using System.Collections.Generic;
using System.Linq;
using BackpackTask;
using TMPro;
using UnityEngine;

namespace SwirAlgorithm
{
    public class Swir : MonoBehaviour
    {
        [SerializeField] AddShop shop;
        [SerializeField] GameObject home;
        [SerializeField] MoveMe me;

        float GetAngle(GameObject shop)
        {
            Vector2 mouseRegCenter = shop.transform.position - home.transform.position;
            float hyp = (float) Math.Sqrt(mouseRegCenter.y * mouseRegCenter.y + mouseRegCenter.x * mouseRegCenter.x);
            if (mouseRegCenter.x > 0)
                return (float) (Math.Asin(mouseRegCenter.y / hyp) / Math.PI * 180);
            return 180 - (float) (Math.Asin(mouseRegCenter.y / hyp) / Math.PI * 180);
        }

        public void Solve()
        {
            List<ItemWithAngle> itemWithAngles = new List<ItemWithAngle>();
            foreach (GameObject shopShop in shop.shops)
                ShopsAdd(itemWithAngles, shopShop);
            itemWithAngles = itemWithAngles.OrderBy(x => x.angle).ToList();
            Queue<Item> resQueue = new Queue<Item>();
            float curWeight = 5;
            float resDistance = 0;

            List<Item> region = new List<Item>();
            for (int i = 0; i < itemWithAngles.Count; i++)
            {
                if (itemWithAngles[i].orderValue <= curWeight)
                {
                    region.Add(itemWithAngles[i]);
                    curWeight -= (float) itemWithAngles[i].orderValue;
                }
                else
                {
                    ResQueueEnqueue(region, resQueue);
                    curWeight = 5;
                    i--;
                }
            }
            if (region.Count > 0) ResQueueEnqueue(region, resQueue);
            me.Move(resQueue);
        }

        void ResQueueEnqueue(List<Item> region, Queue<Item> resQueue)
        {
            foreach (Item item in TravelingSalesman.SolveTravelingSalesman(home, new HashSet<Item>(region)))
                resQueue.Enqueue(item);
            region.Clear();
        }


        void ShopsAdd(List<ItemWithAngle> shops, GameObject shopShop) =>
            shops.Add(new ItemWithAngle(shopShop,
                                        MovingCamera.Round(float.Parse(shopShop.transform.GetChild(0).GetChild(0)
                                                                           .GetComponent<TextMeshProUGUI>().text
                                                                           .Replace('.', ',')), 2),
                                        BackpackSolve.Distance(home, shopShop), GetAngle(shopShop) - 90));

        class ItemWithAngle : Item
        {
            public readonly float angle;

            public ItemWithAngle(GameObject _shop, double orderValue, double _distance, float angle)
                : base(_shop, orderValue, _distance) =>
                this.angle = angle;
        }
    }
}