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

    public class GUICreator
    {

    }

    public abstract class GUIElement
    {
        protected Rectangle _bounds;

        public GUIElement(Rectangle border)
        {
            _bounds = border;
        }

        

        public abstract void Draw();
        public abstract void Click();
    }

    public class Button : GUIElement
    {
        public override void Draw()
        {
            _bounds.Draw();
        }
    }

    public class Edit : GUIElement
    {

    }

    public class Label : GUIElement
    {

    }

    public class CheckboxGroup : GUIElement
    {

    }

    public class Checkbox : GUIElement
    {

    }

    public class Rectangle
    {
        public Vector PositionLeftUp { get; private set; }
        public Vector PositionRightDown { get; private set; }
        public ConsoleColor BorderColor { get; private set; }

        public Rectangle(Vector positionLeftUp, int width, int height, ConsoleColor borderColor)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException("width");
            if (height < 0)
                throw new ArgumentOutOfRangeException("height");

            PositionLeftUp = positionLeftUp;
            PositionRightDown = positionLeftUp + new Vector(width, height);
            BorderColor = borderColor;
        }

        public void Draw()
        {
            string border = "╔";
            string space = "";
            string temp = "";
            for (int i = 0; i < Width; i++)
            {
                space += " ";
                border += "═";
            }

            for (int j = 0; j < PositionLeftUp.X; j++)
                temp += " ";

            border += "╗" + "\n";

            for (int i = 0; i < Height; i++)
                border += temp + "║" + space + "║" + "\n";

            border += temp + "╚";
            for (int i = 0; i < Width; i++)
                border += "═";

            border += "╝" + "\n";

            Console.ForegroundColor = BorderColor;
            Console.CursorTop = PositionLeftUp.Y;
            Console.CursorLeft = PositionLeftUp.X;
            Console.Write(border);
            Console.ResetColor();
        }

        public int Width => PositionRightDown.X - PositionLeftUp.X;
        public int Height => PositionRightDown.Y - PositionLeftUp.Y;

    }
    public class Vector
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Vector comparable = obj as Vector;
            if (comparable == null)
                return false;

            return X == comparable.X && Y == comparable.Y;
        }
        public static Vector operator +(Vector left, Vector right) => new Vector(left.X + right.X, left.Y + right.Y);
        public static Vector operator -(Vector left, Vector right) => new Vector(left.X - right.X, left.Y - right.Y);

    }
}
