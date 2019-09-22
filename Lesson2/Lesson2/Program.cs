using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Geometry;


namespace Lesson2
{
    class Program
    {
        static void Main(string[] args)
        {
            Mouse mouse = new Mouse();
            var builder = new BuilderGUIElements(mouse);

            var rectangle = new Rectangle(new Vector(5, 2), 12, 3);
            var button1 = builder.AddSimpleElement(rectangle, "Click Me", BuilderGUIElements.ButtonCreator());
            button1.AddClickListener(() => Console.Beep());

            rectangle = new Rectangle(new Vector(5, 7), 100, 3);
            builder.AddSimpleElement(rectangle, "Tramp pam pamp", BuilderGUIElements.EditCreator());
            rectangle = new Rectangle(new Vector(7, 6), 12, 1, ConsoleColor.Blue);
            builder.AddSimpleElement(rectangle, "Edit Field", BuilderGUIElements.LabelCreator());


            var checkboxRectangle = new Rectangle(new Vector(0, 0), 15, 1, ConsoleColor.Green);
            var checkboxes = new[] {
                new Checkbox(checkboxRectangle, "Check1"),
                new Checkbox(checkboxRectangle, "Check2"),
                new Checkbox(checkboxRectangle, "Check3")
            };

            var checkboxGroup = builder.AddCheckboxGroup(new Vector(5, 12), 20, 9, checkboxes);


            ConsoleKey key;
            while((key = Console.ReadKey(true).Key) != ConsoleKey.Escape)
            {
                mouse.GetInput(key);
            }

        }
    }
    
    public interface IClickable
    {
        void Click();
        void AddClickListener(Action action);
    }

    public class BuilderGUIElements
    {
        private List<GUIElement> _elements;

        public BuilderGUIElements(Mouse mouse)
        {
            _elements = new List<GUIElement>();
            mouse.AddClickListener(Click);
        }

        private void Click()
        {
            var iClicked = FindClicked();

            if (iClicked == -1)
                return;

            _elements[iClicked].Click();
        }

        private void AddElement(GUIElement guiElement)
        {
            _elements.Add(guiElement);
            guiElement.Draw();
        }

        private int FindClicked() =>
            Rectangle.FindByPoint(_elements.Select(item => item.GetBounds()).ToArray(), Vector.Cursor);


        public T AddSimpleElement<T> (Rectangle rectangle, string text, Func<Rectangle, string, T> creator) where T : GUIElement
        {
            var element = creator(rectangle, text);
            AddElement(element);
            return element;
        }
        
        public CheckboxGroup AddCheckboxGroup(Vector position, int width, int height, Checkbox[] checkboxes, ConsoleColor color = ConsoleColor.Red)
        {
            var group = new CheckboxGroup(new Rectangle(position, width, height, color), "", checkboxes);
            AddElement(group);
            return group;
        }

        public static Func<Rectangle, string, Button> ButtonCreator() => 
            (rect, text) => new Button(rect, text);

        public static Func<Rectangle, string, Edit> EditCreator() =>
            (rect, text) => new Edit(rect, text);

        public static Func<Rectangle, string, Label> LabelCreator() =>
            (rect, text) => new Label(rect, text);

        public static Func<Rectangle, string, Checkbox> CheckboxCreator() =>
            (rect, text) => new Checkbox(rect, text);
    }

    public class Mouse : IClickable
    {
        private Action _onClick;
        private Vector _position;
        private ConsoleKey[] EnterKeys;

        public Mouse()
        {
            EnterKeys = new []{ ConsoleKey.Enter, ConsoleKey.Spacebar, ConsoleKey.Select };
            _position = new Vector(0, 0);
        }

        public void GetInput(ConsoleKey consoleKey)
        {
            if (EnterKeys.Contains(consoleKey))
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
            _position = Painter.CheckLimits(Vector.Cursor + offset);
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

}
