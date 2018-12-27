using UnityEngine;

namespace BackpackTask
{
    public class Item
    {
        public GameObject shop { get; }

        public double orderValue { get; }

        public double distance { get; set; }

        public Item(GameObject _shop, double _orderValue, double _distance)
        {
            shop = _shop;
            orderValue = _orderValue;
            distance = _distance;
        }
    }
}