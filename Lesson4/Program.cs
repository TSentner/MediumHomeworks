using System;
using System.IO;
using System.Reflection;

namespace Lesson4
{
    class Program
    {
        static void Main(string[] args)
        {
            var consoleWritter = new ConsoleWritter();

            DateTimeProvider.Set(new DateTime(2019, 10, 5));

            var evenDayOfWeekWriter = new EvenDayOfWeekWriter(consoleWritter);
            new SomeClassA(evenDayOfWeekWriter).Do("evenDayOfWeekWriter");

            var fileWritter = new FileWritter();
            new SomeClassA(fileWritter).Do("fileWritter");

            var fileConsoleWritter = new FileWritter(consoleWritter);
            new SomeClassA(fileConsoleWritter).Do("fileConsoleWritter");

            var fileConsoleEvenWriter = new EvenDayOfWeekWriter(fileConsoleWritter);
            new SomeClassA(fileConsoleEvenWriter).Do("fileConsoleEvenWriter");

        }
    }

    public class SomeClassA
    {
        private ILogWritter _logWritter;

        public SomeClassA(ILogWritter logWritter) => _logWritter = logWritter;

        public void Do(string message)
        {
            _logWritter?.Write(message);
        }
    }

    public interface ILogWritter
    {
        void Write(string message);
    }

    public class ConsoleWritter : ILogWritter
    {
        public void Write(string message) => Console.WriteLine(message);
    }

    public class FileWritter : ILogWritter
    {
        private readonly string _logFileName = "log.txt";

        private ILogWritter _writter;

        public FileWritter(ILogWritter writter = null) => _writter = writter;

        public void Write(string message)
        {
            File.AppendAllText(_logFileName, message + "\n");
            _writter?.Write(message);
        }
    }

    public class EvenDayOfWeekWriter : ILogWritter
    {
        private ILogWritter _writter;

        public EvenDayOfWeekWriter(ILogWritter consoleWritter) => _writter = consoleWritter;

        public void Write(string message)
        {
            if ((int)DateTimeProvider.Get().DayOfWeek % 2 == 0)
                _writter.Write(message);
        }
    }

    public static class DateTimeProvider
    {
        private static DateTime? _dateTime = null;
        public static DateTime Get() => _dateTime ?? DateTime.Now;
        public static void Set(DateTime? dateTime) => _dateTime = dateTime;
    }
}