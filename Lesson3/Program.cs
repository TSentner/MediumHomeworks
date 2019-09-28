using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson3
{
    class Program
    {
        static void Main(string[] args)
        {
            var goods = new Goods(new [] {
                new Good("beer keg", 10, 5),
                new Good("A", 4, 4),
                new Good("B", 1, 3),
                new Good("C", 2, 2),
                new Good("D", 12, 1)
            });

            Console.WriteLine("\n" + goods.View() + "\n\n");

            goods.Sort(good => good.Name);
            Console.WriteLine("Simple sort by Name: \n" + goods.View() + "\n\n");

            goods.Sort(SortingGoods.By.Level);
            Console.WriteLine("Sort by Level: \n" + goods.View() + "\n\n");


            Console.ReadKey();
        }
    }

    
    public class SortingGoods
    {
        public enum By
        {
            Name,
            Cost,
            Level
        }

        private Dictionary<By, Func<Good, IComparable>> _sorts = new Dictionary<By, Func<Good, IComparable>> {
            { By.Name, good => good.Name },
            { By.Cost, good => good.Cost },
            { By.Level, good => good.Level },
        };

        public IEnumerable<Good> Sort(IEnumerable<Good> goods, By sortBy) => goods.OrderBy(_sorts[sortBy]);
    }
       

    public class Goods
    {
        private IEnumerable<Good> _goods;
        private SortingGoods _goodSorts = new SortingGoods();

        public Goods(IEnumerable<Good> goods)
        {
            _goods = goods;
        }
        
        //Generic Simple Sort
        public void Sort<T>(Func<Good, T> sort) => _goods = _goods.OrderBy(sort);

        public void Sort(SortingGoods.By sortBy) => _goods = _goodSorts.Sort(_goods, sortBy);

        public string View() =>
            string.Concat(_goods.Select((good, i) => $"{i} {good.Name} {good.Cost} {good.Level} \n"));
    }

    public class Good
    {
        public string Name { get; private set; }
        public float Cost { get; private set; }
        public int Level { get; private set; }

        public Good(string name, float cost, int level)
        {
            Name = name;
            SetCost(cost);
            SetLevel(level);
        }

        private void SetCost(float cost)
        {
            if (cost < 0f)
                throw new ArgumentOutOfRangeException("cost");

            Cost = cost;
        }

        private void SetLevel(int level)
        {
            if (level < 0f)
                throw new ArgumentOutOfRangeException("level");

            Level = level;
        }
    }
}
