using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lesson3
{
    class Program
    {
        static void Main(string[] args)
        {
            var loading = new Loading();

            var request = new Request();

            bool isSendEnd = false;
            var taskSend = request.Send("https://habr.com", 
                html => {
                    isSendEnd = true;
                    Console.WriteLine(html);
                }, 
                error => {
                    isSendEnd = true;
                    Console.WriteLine(error);
                });

            while (taskSend.IsCompleted == false)
            {
                if (isSendEnd == false)
                {
                    loading.WriteNextFrame();
                    Thread.Sleep(200);
                }
            }

            Console.ReadKey();
        }
    }

    public class Request
    {
        private readonly HttpClient client = new HttpClient();

        public async Task Send(string url, Action<string> successCallback, Action<string> errorCallback = null, Action<string> doneCallback = null)
        {
            string error = null;
            try
            {
                var response = await client.GetStringAsync(url);
                successCallback?.Invoke(response);
            }
            catch (Exception exception)
            {
                error = exception.Message;
                errorCallback?.Invoke(error);
            }
            finally
            {
                doneCallback?.Invoke(error);
            }
        }
    }

    internal class Loading
    {
        private int _currentAnimationFrame;
        private char[] _spinnerAnimationFrames;

        public Loading()
        {
            _spinnerAnimationFrames = new[] { '|', '/', '-', '\\' };
        }



        public void WriteNextFrame()
        {
            // Store the current position of the cursor
            var originalX = Console.CursorLeft;
            var originalY = Console.CursorTop;

            // Write the next frame (character) in the spinner animation
            Console.Write(_spinnerAnimationFrames[_currentAnimationFrame]);

            // Keep looping around all the animation frames
            _currentAnimationFrame = ++_currentAnimationFrame % _spinnerAnimationFrames.Length;

            // Restore cursor to original position
            Console.SetCursorPosition(originalX, originalY);
        }
    }
}
