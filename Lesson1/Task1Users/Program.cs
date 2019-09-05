using System;

namespace Task1Users
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Player 
    {
        public string Name { get; private set; }
        public int Age { get; private set; }

        public Movement Movement { get; private set; } = new Movement();
        public Weapon Weapon { get; private set; } = new Weapon();

        public Player(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public void Attack()
        {
            //attack
        }
    }

    class Weapon
    {
        public int Damage { get; private set; }
        public float Cooldown { get; private set; }
        public bool IsReloading()
        {
            throw new NotImplementedException();
        }
    }

    class Movement
    {
        public float DirectionX { get; private set; }
        public float DirectionY { get; private set; }
        public float Speed { get; private set; }

        public void SetDirection(float x, float y)
        {
            DirectionX = x;
            DirectionY = y;
        }
        public void SetSpeed(float speed) => Speed = speed;

        public void Move()
        {
            //Do move
        }
    }
}
