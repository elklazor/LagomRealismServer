using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Lidgren.Network.Xna;
using System.Threading;
namespace LagomRealism
{
    class Program
    {
        static GameServer gs;
        static void Main(string[] args)
        {
            gs = new GameServer();
            gs.Start();
        }
    }
    
    
}
