using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson3
{
    class Program
    {
        static void Main(string[] args)
        {
            SomeGlobalClass SGC = new SomeGlobalClass();

            BigData d = null;
            try
            {
                d = new BigData();
            }
            finally
            {
                d?.Dispose();
            }

            using (BigData d2 = new BigData())
            {

            }

            Console.WriteLine("Memory used before collection:       {0:N0}",
                        GC.GetTotalMemory(false));

            GC.Collect();

            Console.WriteLine("Memory used after full collection:   {0:N0}",
                              GC.GetTotalMemory(true));

            SGC.DoSomething();

            while (true)
            {

            }
        }
    }

    class SomeGlobalClass
    {
        public static SomeGlobalClass Instance;
        public event Action OnSomething;

        public SomeGlobalClass()
        {
            Instance = this;
        }

        public void DoSomething()
        {
            if (OnSomething != null)
                OnSomething();
        }
    }

    class BigData : IDisposable
    {
        private bool _disposedValue = false; // Для определения избыточных вызовов

        public int[] Data { get; private set; }

        public BigData()
        {
            Data = new int[100000];
            SomeGlobalClass.Instance.OnSomething += EventHandler;
        }

        public void EventHandler()
        {
            if (_disposedValue)
                throw new ObjectDisposedException("Use called on disposed BigData");

        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                Data = null;
            }

            if (SomeGlobalClass.Instance != null)
                SomeGlobalClass.Instance.OnSomething -= EventHandler;

            _disposedValue = true;
        }

        ~BigData()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
