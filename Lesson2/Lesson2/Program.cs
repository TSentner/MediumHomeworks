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
            var position = new Vector(5, 2);
            Button Button2 = new Button(new Rectangle(position, 11, 3, ConsoleColor.Red), "Knock");

            Button2.Draw();


            Console.ReadLine();
        }
    }
    
    public abstract class GUIElement
    {
        protected int _textOffset { get; private set; }

        protected string _text { get; private set; }
        protected Rectangle _bounds { get; private set; }
        protected string[] _view { get; private set; }

        public GUIElement(Rectangle border, string text)
        {
            _bounds = border;

            _textOffset = 0;

            SetText(text);
            
            _view = new string[0];
        }

        protected virtual void SetTextOffset(int offset) => _textOffset = offset;

        protected void SetView(string[] view) => _view = view ?? new string[0];

        protected void SetText(string text)
        {
            _text = text ?? string.Empty;

            int cut = Math.Max(0, _bounds.Width - _textOffset);
            if (_text.Length > cut)
                _text = _text.Substring(0, cut);
        }

        protected Vector TextPosition()
        {
            int leftOffset = (_bounds.Width - _text.Length + 1) / 2;
            int topOffset = (_bounds.Height - 1) / 2;
            return _bounds.PositionLeftUp + new Vector(leftOffset, topOffset);
        }

        public abstract void Draw();

        public abstract void Click();
    }

    public class Button : GUIElement
    {
        public Button(Rectangle border, string text) : base(border, text)
        {
            SetView(Painter.BracketsRectangleView(border.Width, border.Height));

            SetTextOffset(2);
            Console.WriteLine(_text);
            SetText(text);
            Console.WriteLine(_text);

        }

        public override void Click()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            Painter.Paint(_view, _bounds.PositionLeftUp, ConsoleColor.Red);
            
            Painter.Paint(_text, TextPosition(), ConsoleColor.Red);
        }
    }

    public class Edit : GUIElement
    {
        public Edit(Rectangle border, string text) : base(border, text)
        {
            SetView(Painter.SolidRectangleView(border.Width, border.Height));
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
        public Label(Rectangle border, string text) : base(border, text)
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
        public CheckboxGroup(Rectangle border, string text) : base(border, text)
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
        public Checkbox(Rectangle border, string text) : base(border, text)
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

            PositionLeftUp = Painter.CheckLimits(positionLeftUp);
            PositionRightDown = positionLeftUp + new Vector(width, height);
            PositionRightDown = Painter.CheckLimits(PositionRightDown);
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

        public static Vector Up => new Vector(0, 1);

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


            string downLine = string.Empty;
            downLine += downLeft;

            for (int i = 0; i < width - 2; i++)
                downLine += down;

            if (width > 1)
                downLine += rightDown;

            lines.Add(downLine);


            return lines.ToArray();
        }

        public static Vector CheckLimits(Vector vector)
        {
            int x = MathExt.Clamp(vector.X, 0, Console.BufferWidth - 1);
            int y = MathExt.Clamp(vector.Y, 0, Console.BufferHeight - 1);

            return new Vector(x, y);
        }
    }

    public static class MathExt
    {
        public static int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;
    }
}
