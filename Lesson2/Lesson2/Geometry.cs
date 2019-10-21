using System;
using System.Collections.Generic;

namespace Geometry
{
    public static class MathExt
    {
        public static int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;
    }

    public struct Rectangle
    {
        public Vector PositionLeftUp { get; private set; }
        public Vector PositionRightDown { get; private set; }
        public ConsoleColor BorderColor { get; private set; }
        public int Width => PositionRightDown.X - PositionLeftUp.X;
        public int Height => PositionRightDown.Y - PositionLeftUp.Y;

        public Rectangle(Vector positionLeftUp, int width, int height, ConsoleColor borderColor = ConsoleColor.Red)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException("width");
            if (height < 0)
                throw new ArgumentOutOfRangeException("height");

            PositionLeftUp = Painter.CheckLimits(positionLeftUp);
            PositionRightDown = positionLeftUp + new Vector(width, height);
            PositionRightDown = Painter.CheckLimits(PositionRightDown);
            BorderColor = borderColor;
        }

        public bool IsPointInside(Vector point)
        {
            if (point.X < PositionLeftUp.X || point.Y < PositionLeftUp.Y
                || point.X > PositionRightDown.X - 1 || point.Y > PositionRightDown.Y - 1)
            {
                return false;
            }

            return true;
        }

        public static int FindByPoint(Rectangle[] rectangles, Vector point)
        {
            for (int i = 0; i < rectangles.Length; i++)
                if (rectangles[i].IsPointInside(point))
                    return i;

            return -1;
        }
    }

    public struct Vector
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector Up => new Vector(0, 1);
        public static Vector Down => new Vector(0, -1);
        public static Vector Left => new Vector(-1, 0);
        public static Vector Right => new Vector(1, 0);
        public static Vector Cursor => new Vector(Console.CursorLeft, Console.CursorTop);


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Vector comparable)
                return X == comparable.X && Y == comparable.Y;

            return false;
        }
        public static Vector operator +(Vector left, Vector right) =>
            new Vector(left.X + right.X, left.Y + right.Y);
        public static Vector operator -(Vector left, Vector right) =>
            new Vector(left.X - right.X, left.Y - right.Y);
    }

    public static class Painter
    {
        public static void Paint(string[] lines, Vector position, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            for (int i = 0; i < lines.Length; i++)
                Write(lines[i], new Vector(position.X, position.Y + i));

            Console.ForegroundColor = oldColor;
        }

        public static void Paint(string text, Vector position, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Write(text, position);

            Console.ForegroundColor = oldColor;
        }

        public static void Write(string text, Vector position)
        {
            Console.CursorTop = position.Y;
            Console.CursorLeft = position.X;
            Console.Write(text);
        }

        public static string[] ClearRectangleView(int width, int height) =>
            BuildRectangleView(width, height, " ", " ", " ", " ", " ", " ", " ", " ");

        public static string[] SolidRectangleView(int width, int height) =>
            BuildRectangleView(width, height, "╔", "═", "╗", "║", "╝", "═", "╚", "║");

        public static string[] BracketsRectangleView(int width, int height) =>
            BuildRectangleView(width, height, "╔", " ", "╗", "║", "╝", " ", "╚", "║");


        public static string[] BuildRectangleView(int width, int height,
            string leftUp, string up, string upRight, string right,
            string rightDown, string down, string downLeft, string left)
        {
            if (width * height == 0)
                return new string[0];

            var lines = new List<string>();

            string upLine = leftUp;

            string space = "";
            for (int i = 0; i < width - 2; i++)
            {
                space += " ";
                upLine += up;
            }

            if (width > 1)
                upLine += upRight;

            lines.Add(upLine);


            for (int i = 0; i < height - 2; i++)
                lines.Add(left + space + right);

            if (height > 1)
            {
                string downLine = string.Empty;
                downLine += downLeft;

                for (int i = 0; i < width - 2; i++)
                    downLine += down;

                if (width > 1)
                    downLine += rightDown;

                lines.Add(downLine);
            }

            return lines.ToArray();
        }

        public static Vector CheckLimits(Vector vector)
        {
            int x = MathExt.Clamp(vector.X, 0, Console.BufferWidth - 1);
            int y = MathExt.Clamp(vector.Y, 0, Console.BufferHeight - 1);

            return new Vector(x, y);
        }
    }
}
