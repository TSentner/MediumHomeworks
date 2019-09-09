using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson2
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }

    abstract class Character
    {
        private int _health;
        public int Health
        {
            get => _health;
            private set
            {
                if (value <= 0 && _health > 0)
                    Console.WriteLine("Я умер");
                _health = Math.Max(0, value);
            }
        }

        public Character(int health)
        {
            Health = health;
        }

        protected abstract int CalculateDamage(int damage);

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException("damage");

            if (Health == 0)
                return;

            Health -= CalculateDamage(damage);
        }
    }

    class Wombat: Character
    {
        private int _armor;
        public int Armor { get => _armor; private set => _armor = Math.Max(0, value); }

        public Wombat(int health, int armor) : base(health)
        {
            Armor = armor;
        }

        protected override int CalculateDamage(int damage) => damage < Armor ? 0 : damage - Armor;
    }

    class Human : Character
    {
        private int _agility;
        public int Agility { get => _agility; private set => _agility = Math.Max(0, value); }

        public Human(int health, int agility) : base(health)
        {
            Agility = agility;
        }

        protected override int CalculateDamage(int damage) => (int)Math.Ceiling((float)damage / Agility);
    }
}
