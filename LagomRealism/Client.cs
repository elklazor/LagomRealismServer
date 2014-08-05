using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace LagomRealism
{
    class Client
    {
        public int ID;
       // private Player player;
        public NetConnection Connection;
        public bool ReceivedWorld = false;
        private Vector2 position;
        public long Identifier;
        public bool Connected;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Client(int id,NetConnection connect)
        {
            ID = id;
            Connection = connect;
            Identifier = connect.RemoteUniqueIdentifier;
            Connected = true;
            position = new Vector2(10, 0);
        }

        public override string ToString()
        {
            
            return "ID: " + ID.ToString() + " |:| ReceivedWorld: " + ReceivedWorld.ToString();
        }
    }
}
