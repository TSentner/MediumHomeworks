using System;
using System.IO;


namespace Lesson4
{
    class Program
    {
        static void Main(string[] args)
        {
            var sharpChecker = new SomeClass(new CheckSharp());
            sharpChecker.Trigger += () => Console.WriteLine("Sharp finded.");

            using (var stream = StramGenerator.GetFromString("a,b \n c,#d"))
                sharpChecker.Process(stream);


            var evenChecker = new SomeClass(new CheckEven());
            evenChecker.Trigger += () => Console.WriteLine("Even finded.");

            using (var streamReader = new StreamReader("test.txt"))
                evenChecker.Process(streamReader.BaseStream);


            Console.ReadKey();
        }
    }

    public static class StramGenerator
    {
        public static Stream GetFromString(string value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }

    public class SomeClass
    {
        private ICheckSymbol _checker;

        public SomeClass(ICheckSymbol checkSymbol)
        {
            _checker = checkSymbol;
        }

        public event Action Trigger;


        public void Process(Stream stream)
        {
            var reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                    if (_checker.Check((char)reader.Read()))
                        Trigger();
        }
    }

    public interface ICheckSymbol
    {
        bool Check(char symbol);
    }

    public class CheckSharp : ICheckSymbol
    {
        public bool Check(char symbol) => symbol == '#';
    }

    public class CheckEven : ICheckSymbol
    {
        private int? _sum = null;
        private bool _hasNumber = false;

        private bool StartHandleNumbers()
        {
            _sum = 0;
            _hasNumber = false;
            return false;
        }

        private bool HandleNumber(int value)
        {
            _sum += value;
            _hasNumber = true;
            return false;
        }

        private bool EndHandleNumbers()
        {
            var sum = _sum;
            _sum = null;
            _hasNumber = false;
            return sum % 2 == 0;
        }


        public bool Check(char symbol)
        {
            if (symbol == '*')
                return StartHandleNumbers();

            if (_sum == null)
                return false;

            if (Char.IsNumber(symbol))
                return HandleNumber((int)symbol);

            if (symbol == ';' && _hasNumber)
                return EndHandleNumbers();

            return false;
        }
    }
}
