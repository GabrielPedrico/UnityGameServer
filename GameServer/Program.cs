using System;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "Downtown Madness Server [Made by pedripedrio]";
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            //Server.Start(50, 26950); // Maximo de players e Port (Original)
            Server.Start(50, 25565); // Maximo de players e Port (Hamachi)
        }

        private static void MainThread()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Main thread iniciada. Rodando a {Constants.TICKS_PER_SEC} ticks por segundo.");
            Console.ForegroundColor = ConsoleColor.White;
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
