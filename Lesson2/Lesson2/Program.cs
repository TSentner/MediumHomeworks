using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Lesson2
{
    class Program
    {
        static void Main(string[] args)
        {
            var position = new Vector(5, 2);
            Button button1 = new Button(new Rectangle(position, 12, 3, ConsoleColor.Red), "Click Me");
            button1.Draw();

            position = new Vector(5, 7);
            Edit edit1 = new Edit(new Rectangle(position, 100, 3, ConsoleColor.Red), "Tramp pam pamp");
            edit1.Draw();

            position = new Vector(7, 7);
            Label label1 = new Label(new Rectangle(position, 12, 1, ConsoleColor.Blue), "Edit Field");
            label1.Draw();

            position = new Vector(5, 12);
            var checkboxes = new[] {
                new Checkbox(new Rectangle(position, 15, 1, ConsoleColor.Green), "Check1"),
                new Checkbox(new Rectangle(position, 15, 1, ConsoleColor.Green), "Check2"),
                new Checkbox(new Rectangle(position, 15, 1, ConsoleColor.Green), "Check3")
            };

            CheckboxGroup checkboxGroup = new CheckboxGroup(new Rectangle(position, 20, 9, ConsoleColor.Red), "", checkboxes);
            checkboxGroup.Draw();
            checkboxGroup.Click();

            Mouse mouse = new Mouse();
            ConsoleKey key;
            while((key = Console.ReadKey(true).Key) != ConsoleKey.Escape)
            {
                mouse.GetInput(key);
            }

        }
    }

    interface IClickable
    {
        void Click();
        void AddClickListener(Action action);
    }

    public class Mouse : IClickable
    {
        private Action _onClick;
        private Vector _position;
        private byte KeyLayers;
        private char buferChar;

        public Mouse()
        {
            KeyLayers = (byte)ConsoleKey.Enter & (byte)ConsoleKey.Spacebar & (byte)ConsoleKey.Select;
            _position = new Vector(0, 0);
        }

        public void GetInput(ConsoleKey consoleKey)
        {
            if (KeyLayers == (KeyLayers | (1 << ((byte)consoleKey))))
            {
                Click();
            }
            else
            {
                switch (consoleKey)
                {
                    case ConsoleKey.LeftArrow:
                        Move(Vector.Left);
                        break;
                    case ConsoleKey.UpArrow:
                        Move(Vector.Down);
                        break;
                    case ConsoleKey.RightArrow:
                        Move(Vector.Right);
                        break;
                    case ConsoleKey.DownArrow:
                        Move(Vector.Up);
                        break;
                    default:
                        break;
                }
            }
            
        }
        
        public void Move(Vector offset)
        {
            var left = Console.CursorLeft;
            var top = Console.CursorTop;
            _position = new Vector(left, top);
            _position = Painter.CheckLimits(_position + offset);
            Console.SetCursorPosition(_position.X, _position.Y);
        }

        public void Click()
        {
            _onClick?.Invoke();
        }

        public virtual void AddClickListener(Action action)
        {
            _onClick += action;
        }
    }

    public abstract class GUIElement : IClickable
    {
        public enum VerticalAlign
        {
            Left,
            Center,
            Right
        }
        public enum HorizontalAlign
        {
            Top,
            Center,
            Down
        }
        protected Action _onClick { get; private set; }

        protected Rectangle _bounds { get; private set; }
        protected string _text { get; private set; }
        protected string[] _backgroundView { get; private set; }

        protected int _textVerticalOffset { get; private set; }
        protected int _textHorizontalOffset { get; private set; }

        protected int MaxTextLength => Math.Max(0, _bounds.Width - _textVerticalOffset * 2);



        public GUIElement(Rectangle border, string text, Action action = null)
        {
            _bounds = border;

            _textVerticalOffset = 1;
            _textHorizontalOffset = 2;

            SetText(text);
            
            _backgroundView = new string[0];

            _onClick = action;
        }

        protected void SetTextVerticalOffset(int offset) => _textVerticalOffset = Math.Max(0, offset);
        protected void SetTextHorizontalOffset(int offset) => _textHorizontalOffset = Math.Max(0, offset);

        protected void SetView(string[] view) => _backgroundView = view ?? new string[0];

        protected void SetText(string text)
        {
            _text = text ?? string.Empty;

            if (_text.Length > MaxTextLength)
                _text = _text.Substring(0, MaxTextLength);
        }

        protected Vector TextPosition(VerticalAlign verticalAlign = VerticalAlign.Center, HorizontalAlign horizontalAlign = HorizontalAlign.Center)
        {
            int leftOffset;
            int topOffset;

            switch (verticalAlign)
            {
                case VerticalAlign.Left:
                    leftOffset = _textVerticalOffset;
                    break;
                case VerticalAlign.Center:
                    leftOffset = (_bounds.Width - _text.Length + 1) / 2;
                    break;
                case VerticalAlign.Right:
                    leftOffset = _bounds.Width - _text.Length - _textVerticalOffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("verticalAlign");
            }
            switch (horizontalAlign)
            {
                case HorizontalAlign.Top:
                    topOffset = _textHorizontalOffset;
                    break;
                case HorizontalAlign.Center:
                    topOffset = (_bounds.Height - 1) / 2;
                    break;
                case HorizontalAlign.Down:
                    topOffset = _bounds.Height - _textHorizontalOffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("horizontalAlign");
            }


            return Painter.CheckLimits(_bounds.PositionLeftUp + new Vector(leftOffset, topOffset));
        }

        public Rectangle GetBounds() => _bounds;
        
        public void SetLocation(Vector position)
        {
            _bounds = new Rectangle(position, _bounds.Width, _bounds.Height, _bounds.BorderColor);
        }

        public abstract void Draw();

        public virtual void Click()
        {
            _onClick?.Invoke();
        }

        public virtual void AddClickListener(Action action)
        {
            _onClick += action;
        }
    }

    public class Button : GUIElement
    {
        public Button(Rectangle border, string text) : base(border, text)
        {
            SetView(Painter.BracketsRectangleView(border.Width, border.Height));
            SetText(text);
        }

        
        public override void Draw()
        {
            Painter.Paint(_backgroundView, _bounds.PositionLeftUp, _bounds.BorderColor);
            Painter.Paint(_text, TextPosition(), _bounds.BorderColor);
        }
    }

    public class Edit : GUIElement
    {
        public Edit(Rectangle border, string text) : base(border, text)
        {
            SetView(Painter.SolidRectangleView(border.Width, border.Height));
            SetTextVerticalOffset(3);
        }

        public override void Click()
        {
            base.Click();

            SetText(new string(' ', MaxTextLength));
            Draw();

            var cursor = TextPosition(VerticalAlign.Left);
            Console.CursorTop = cursor.Y;
            Console.CursorLeft = cursor.X;
            SetText(Console.ReadLine());
            Draw();
        }

        public override void Draw()
        {
            Painter.Paint(_backgroundView, _bounds.PositionLeftUp, _bounds.BorderColor);
            Painter.Paint(_text, TextPosition(VerticalAlign.Left), _bounds.BorderColor);
        }
    }

    public class Label : GUIElement
    {
        public Label(Rectangle border, string text) : base(border, text)
        {
            SetView(Painter.ClearRectangleView(border.Width, border.Height));
        }

        public override void Draw()
        {
            Painter.Paint(_backgroundView, _bounds.PositionLeftUp, _bounds.BorderColor);
            Painter.Paint(_text, TextPosition(VerticalAlign.Left), _bounds.BorderColor);
        }
    }

    public class CheckboxGroup : GUIElement
    {
        private Checkbox[] _checkboxes;

        public CheckboxGroup(Rectangle border, string text, Checkbox[] checkboxes) : base(border, text)
        {
            SetView(Painter.SolidRectangleView(border.Width, border.Height));
            SetTextVerticalOffset(3);
            for (int i = 0; i < checkboxes.Length; i++)
                if (checkboxes[i] == null)
                    throw new ArgumentNullException("checkboxes " + i);

            _checkboxes = checkboxes;
        }

        private int FindClicked()
        {
            var cursorPosition = new Vector(Console.CursorLeft, Console.CursorTop);
            for (int i = 0; i < _checkboxes.Length; i++)
                if (_checkboxes[i].GetBounds().IsPointInside(cursorPosition))
                    return i;

            return -1;
        }

        public override void Click()
        {
            base.Click();

            var iClicked = FindClicked();

            if (iClicked == -1)
                return;

            for (int i = 0; i < _checkboxes.Length; i++)
            {
                if (_checkboxes[i].Checked && i != iClicked)
                {
                    _checkboxes[i].Uncheck();
                    _checkboxes[i].Draw();
                }
            }

            _checkboxes[iClicked].Click();
        }

        public override void Draw()
        {
            Painter.Paint(_backgroundView, _bounds.PositionLeftUp, _bounds.BorderColor);

            var heights = _checkboxes.Sum(item => item.GetBounds().Height);
            var offset = Math.Max(1, Math.Max(0, _bounds.Height - heights) / Math.Max(2, _checkboxes.Length));
            var yPosition = _bounds.PositionLeftUp.Y + offset;
            
            foreach (var checkbox in _checkboxes)
            {
                checkbox.SetLocation(new Vector(_bounds.PositionLeftUp.X + _textVerticalOffset, yPosition));
                yPosition += offset;
                checkbox.Draw();
            }
            
        }
    }

    public class Checkbox : GUIElement
    {
        public bool Checked { get; private set; }

        public Checkbox(Rectangle border, string text) : base(border, text)
        {
            SetView(Painter.ClearRectangleView(border.Width, border.Height));
        }
        
        public void Uncheck() => Checked = false;

        public override void Click()
        {
            base.Click();

            Checked = true;
            Draw();
        }

        public override void Draw()
        {
            var view = "(" + (Checked ? "*" : " ") + ") " + _text;
            if (view.Length > MaxTextLength)
                view = view.Substring(0, MaxTextLength);

            Painter.Paint(_backgroundView, _bounds.PositionLeftUp, _bounds.BorderColor);
            Painter.Paint(view, TextPosition(VerticalAlign.Left), _bounds.BorderColor);
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

        public bool IsPointInside(Vector point)
        {
            if (point.X < PositionLeftUp.X || point.Y < PositionLeftUp.Y 
                || point.X > PositionRightDown.X || point.Y > PositionRightDown.Y)
            {
                return false;
            }

            return true;
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

    public static class MathExt
    {
        public static int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;
    }


    
}
