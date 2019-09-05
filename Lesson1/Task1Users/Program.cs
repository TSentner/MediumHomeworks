using System;
using System.Collections.Generic;
using System.Linq;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Bag
    {
        private List<Item> _items;
        private int _maxWeidth;

        public Bag(List<Item> items, int maxWeidth)
        {
            _items = items;
            _maxWeidth = maxWeidth;
        }

        public void AddItem(string name, int count)
        {
            int currentWeidth = _items.Sum(item => item.Count);
            Item targetItem = _items.FirstOrDefault(item => item.Name == name);

            if (targetItem == null)
                throw new InvalidOperationException();

            if (currentWeidth + count > _maxWeidth)
                throw new InvalidOperationException();
             
            targetItem.AddCount(count);
        }
    }

    class Item
    {
        public int Count { get; private set; }
        public string Name { get; private set; }

        public void AddCount(int count) => Count += count;
    }
}
