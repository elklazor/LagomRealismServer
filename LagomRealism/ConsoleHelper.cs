using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    class ConsoleHelper
    {
        /// <summary>
        /// Heyo
        /// </summary>
        private GameServer GC;
        public volatile bool End = false;
        public volatile bool Stop = false;
        public ConsoleHelper(GameServer gc)
        {
            GC = gc;
        }
        public void ListenForInput()
        {
            start:
            string s = Console.ReadLine();
            switch (s)
            {
                case "list":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    foreach (Client c in GC.clients)
                    {
                        Console.WriteLine(c.ToString());
                    }
                    if (GC.clients.Count == 0)
                        Console.WriteLine("No clients connected");

                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "stop":
                    Console.WriteLine("Stop requested by user");
                    End = true;
                    break;
                case "ent":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    foreach (WorldEntity e in GC.world.entities)
                    {
                        
                        Console.WriteLine(e.ToString());
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
            goto start;
        }
    }
}
