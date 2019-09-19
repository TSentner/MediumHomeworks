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
            Vector position = new Vector(10, 0);
            Button Button1 = new Button(new Rectangle(position, 10, 1, ConsoleColor.Red));
            position = new Vector(5, 2);
            Button Button2 = new Button(new Rectangle(position, 10, 1, ConsoleColor.Red));
            Button1.Draw();
            Button2.Draw();


            Console.ReadLine();
        }
    }

    public class GUICreator
    {

    }

    public abstract class GUIElement
    {
        protected Rectangle _bounds { get; private set; }
        protected string[] _view { get; private set; }

        public GUIElement(Rectangle border)
        {
            _bounds = border;
            _view = new string[0];
        }

        protected void SetView(string[] view) => _view = view ?? new string[0];

        public abstract void Draw();

        public abstract void Click();
    }

    public class Button : GUIElement
    {

        public Button(Rectangle border) : base(border)
        {
            SetView(Painter.SolidRectangleView(border.Width, border.Height));
        }

        public override void Click()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            Painter.Paint(_view, _bounds.PositionLeftUp, ConsoleColor.Red);
        }
    }

    public class Edit : GUIElement
    {
        public Edit(Rectangle border) : base(border)
        {
        }

        public override void Click()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }

    public class Label : GUIElement
    {
        public Label(Rectangle border) : base(border)
        {
        }

        public override void Click()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }

    public class CheckboxGroup : GUIElement
    {
        public CheckboxGroup(Rectangle border) : base(border)
        {
        }

        public override void Click()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }

    public class Checkbox : GUIElement
    {
        public Checkbox(Rectangle border) : base(border)
        {
        }

        public override void Click()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }

    public struct Rectangle
    {
        public Vector PositionLeftUp { get; private set; }
        public Vector PositionRightDown { get; private set; }
        public ConsoleColor BorderColor { get; private set; }
        public int Width => PositionRightDown.X - PositionLeftUp.X;
        public int Height => PositionRightDown.Y - PositionLeftUp.Y;

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
            {
                Console.CursorTop = position.Y + i;
                Console.CursorLeft = position.X;
                Console.Write(lines[i]);
            }

            
            Console.ForegroundColor = oldColor;
        }

        public static string[] SolidRectangleView(int width, int height) =>
            BuildRectangleView(width, height, "╔", "═", "╗", "║", "╝", "═", "╚", "║");

        public static string[] BracketsRectangleView(int width, int height) =>
            BuildRectangleView(width, height, "╔", " ", "╗", "║", "╝", " ", "╚", "║");


        public static string[] BuildRectangleView(int width, int height, 
            string leftUp, string up, string upRight, string right, 
            string rightDown, string down, string downLeft, string left)
        {
            var lines = new List<string>();

            string upLine = leftUp;
            string space = "";
            for (int i = 0; i < width; i++)
            {
                space += " ";
                upLine += up;
            }

            upLine += upRight;

            lines.Add(upLine);

            for (int i = 0; i < height; i++)
                lines.Add(left + space + right);

            string downLine = string.Empty;
            downLine += downLeft;
            for (int i = 0; i < width; i++)
                downLine += down;

            downLine += rightDown + "\n";
            lines.Add(downLine);


            return lines.ToArray();
        }
    }
}
