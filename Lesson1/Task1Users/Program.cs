using System;
using System.Collections.Generic;

namespace Task
{
    class Program
    {
        public static void Main(string[] args)
        {
            var v = new Vector(5, 5);
            Vectors vectors = new Vectors(
                new Vector[3] {
                    v,
                    new Vector(10, 10),
                    new Vector(15, 15)
                }
            );

            while (vectors.AnyAlive())
            {
                vectors.CheckAlive();
                vectors.AddRandom();
                vectors.WriteAlive();
            }
        }

        class Vectors
        {
            private Vector[] _vectors;
            private bool[] _isAlive;
            private Random random = new Random();

            public Vectors(Vector[] vectors)
            {
                _vectors = vectors;
                _isAlive = new bool[vectors.Length];
                for (int i = 0; i < _isAlive.Length; i++)
                    _isAlive[i] = true;
            }

            public void CheckAlive()
            {
                for (int i = 0; i < _vectors.Length; i++)
                {
                    if (_isAlive[i] == false)
                        continue;

                    for (int j = 0; j < _vectors.Length; j++)
                    {
                        if (i == j)
                            continue;

                        if (_vectors[i].Equals(_vectors[j]))
                        {
                            _isAlive[i] = _isAlive[j] = false;
                            break;
                        }
                    }
                }
            }

            public void AddRandom()
            {
                for (int i = 0; i < _vectors.Length; i++)
                {
                    _vectors[i] += new Vector(random.Next(-1, 1), random.Next(-1, 1));
                    _vectors[i].CheckLimits();
                }
            }

            public void WriteAlive()
            {
                for (int i = 0; i < _isAlive.Length; i++)
                {
                    if (_isAlive[i])
                    {
                        Console.SetCursorPosition(_vectors[i].X, _vectors[i].Y);
                        Console.Write(i + 1);
                    }
                }
            }

            public bool AnyAlive()
            {
                for (int i = 0; i < _isAlive.Length; i++)
                    if (_isAlive[i])
                        return true;

                return false;
            }
        }

        class Vector
        {
            public int X { get; private set; }
            public int Y { get; private set; }

            public Vector(int x, int y)
            {
                X = x;
                Y = y;
            }

            public void CheckLimits()
            {
                X = Clamp(X, 0, Console.BufferWidth - 1);
                Y = Clamp(Y, 0, Console.BufferHeight - 1);
            }
            private int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                Vector comparable = obj as Vector;
                if (comparable == null)
                    return false;

                return X == comparable.X && Y == comparable.Y;
            }
            public static Vector operator +(Vector left, Vector right) => new Vector (left.X + right.X, left.Y + right.Y);

        }
    }
}