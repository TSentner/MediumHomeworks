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
            var products = new List<Product> {
                new Product(1, "beer keg", 10, 6),
                new Product(2, "A", 4, 4),
                new Product(3, "B", 1, 3),
                new Product(4, "C", 2, 2),
                new Product(5, "D", 112, 9)
            };

            var query = products.Where(product => product.Cost < 100 && product.Count > 5);

            Console.WriteLine(query.Count());

            Console.ReadKey();
        }
    }

    

    public class Product
    {
        public readonly int Id;
        public string Name { get; private set; }
        public float Cost { get; private set; }
        public int Count { get; private set; }

        public Product(int id, string name, float cost, int count)
        {
            Id = id;
            Name = name;
            SetCost(cost);
            SetCount(count);
        }

        private void CheckPositive(float value, string error)
        {
            if (value < 0f)
                throw new ArgumentOutOfRangeException(error);
        }

        private void SetCost(float cost)
        {
            CheckPositive(cost, "cost");
            Cost = cost;
        }

        private void SetCount(int count)
        {
            CheckPositive(count, "count");
            Count = count;
        }

    }
}
