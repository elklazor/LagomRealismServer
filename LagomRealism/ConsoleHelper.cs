using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagomRealism
{
    class ConsoleHelper
    {
        List<Client> Clients;
        public volatile bool End = false;
        public volatile bool Stop = false;
        public ConsoleHelper(List<Client> clients)
        {
            Clients = clients;
        }
        public void ListenForInput()
        {
            start:
            string s = Console.ReadLine();
            switch (s)
            {
                case "list":
                    foreach (Client c in Clients)
                    {
                        Console.WriteLine(c.ToString());
                    }
                    if (Clients.Count == 0)
                        Console.WriteLine("No clients connected");
                    break;
                case "stop":
                    Console.WriteLine("Stop requested by user");
                    End = true;
                    break;
                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
            goto start;
        }
    }
}
