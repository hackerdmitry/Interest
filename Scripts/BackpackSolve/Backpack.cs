using System.Collections.Generic;

namespace BackpackTask
{
    class Backpack
    {
        List<Item> bestItems;

        readonly double maxW;

        double bestPrice;

        public Backpack(double maxW) => this.maxW = maxW;

        // Создание всех наборов перестановок значений
        public void MakeAllSets(List<Item> items)
        {
            if (items.Count > 0)
                CheckSet(items);

            for (int i = 0; i < items.Count; i++)
            {
                List<Item> newSet = new List<Item>(items);
                newSet.RemoveAt(i);
                MakeAllSets(newSet);
            }
        }

        // Проверка, является ли данный набор лучшим решением задачи
        void CheckSet(List<Item> items)
        {
            if (bestItems == null)
            {
                if (CalcWeigth(items) <= maxW)
                {
                    bestItems = items;
                    bestPrice = CalcPrice(items);
                }
            }
            else
            {
                if (CalcWeigth(items) <= maxW && CalcPrice(items) > bestPrice)
                {
                    bestItems = items;
                    bestPrice = CalcPrice(items);
                }
            }
        }

        // Вычисляет общий вес набора предметов
        double CalcWeigth(List<Item> items)
        {
            double sumW = 0;
            foreach (Item i in items) sumW += i.orderValue;
            return sumW;
        }

        // Вычисляет общую стоимость набора предметов
        double CalcPrice(List<Item> items)
        {
            double sumPrice = 0;
            foreach (Item i in items) sumPrice += i.distance;
            return sumPrice;
        }

        // Возвращает решение задачи (набор предметов)
        public List<Item> GetBestSet() => bestItems;
    }
}