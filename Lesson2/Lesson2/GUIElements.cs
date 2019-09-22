using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace Lesson2
{
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

        public virtual void Click()
        {
            _onClick?.Invoke();
        }

        public virtual void AddClickListener(Action action)
        {
            _onClick += action;
        }

        public abstract void Draw();

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



        private int FindClicked() =>
            Rectangle.FindByPoint(_checkboxes.Select(item => item.GetBounds()).ToArray(), Vector.Cursor);

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

}
