using System;
using System.Collections.Generic;
using System.Linq;
using BackpackTask;
using TMPro;
using UnityEngine;

public class BackpackSolve : MonoBehaviour
{
    [SerializeField] AddShop shop;
    [SerializeField] GameObject home;
    [SerializeField] MoveMe me;

    public void Solve()
    {
        GameObject[] arr = new GameObject[shop.shops.Count];
        shop.shops.CopyTo(arr);
        Queue<Item> resQueue = new Queue<Item>();
        GameObject begin = home;
        float curWeight = 5;
        float resDistance = 0;
        Item selectedItem = null;
        while (shop.shops.Count > 0)
        {
            List<Item> shops = GetListShops(begin);

            Backpack backpack = new Backpack(curWeight);
            float maxDistance = (float) shops.Max(x => x.distance);
            foreach (Item item in shops)
                item.distance = maxDistance - item.distance;
            backpack.MakeAllSets(shops);
            List<Item> solve = backpack.GetBestSet();

            float maxW = (float) solve.Max(x => x.orderValue);
            IEnumerable<Item> suitable = solve.Where(x => (float) x.orderValue == maxW);
            double minE = suitable.Max(x => x.distance);
            selectedItem = suitable.First(x => x.distance == minE);
            float distanceToSelectedItem = maxDistance - (float) selectedItem.distance;

            resDistance += distanceToSelectedItem;
            resQueue.Enqueue(selectedItem);
            Debug.Log($"Выбран магаз с координатами ({selectedItem.shop.transform.position.x}, " +
                      $"{selectedItem.shop.transform.position.y}), расстояние до него = {distanceToSelectedItem}" +
                      $" и вес = {selectedItem.orderValue}");

            if (selectedItem.shop == home)
            {
                begin = home;
                curWeight = 5;
                continue;
            }
            curWeight -= (float) selectedItem.orderValue;
            shop.shops.Remove(selectedItem.shop);
            begin = selectedItem.shop;
        }
        resQueue.Enqueue(new Item(home, 0, Distance(home, selectedItem.shop)));
        resDistance += Distance(home, selectedItem.shop);
        Debug.Log($"Общая пройденная дистанция - {resDistance}");

        foreach (GameObject shopShop in arr)
            shop.shops.Add(shopShop);
        me.Move(resQueue);
    }

    List<Item> GetListShops(GameObject begin)
    {
        List<Item> shops = new List<Item>();
        if (begin == home)
            foreach (GameObject shopShop in shop.shops)
                ShopsAdd(shops, shopShop);
        else
        {
            shops.Add(new Item(home, 0, Distance(home, begin)));
            foreach (GameObject shopShop in shop.shops)
                if (begin != shopShop)
                    ShopsAdd(shops, shopShop);
        }
        return shops;
    }

    void ShopsAdd(List<Item> shops, GameObject shopShop) =>
        shops.Add(new Item(shopShop,
                           MovingCamera.Round(float.Parse(shopShop.transform.GetChild(0).GetChild(0)
                                                              .GetComponent<TextMeshProUGUI>().text
                                                              .Replace('.', ',')), 2), 
                           Distance(home, shopShop)));

    public static float Distance(GameObject p1, GameObject p2)
    {
        Vector3 pos1 = p1.transform.position;
        Vector3 pos2 = p2.transform.position;
        return (float) Math.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) +
                                 (pos1.y - pos2.y) * (pos1.y - pos2.y));
    }
}