using Lidgren.Network;
using Lidgren.Network.Xna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LagomRealism
{
    class GameServer
    {
        public List<Client> clients = new List<Client>();
        private ConsoleHelper ch;
        public World world = new World();
        private Timer timer;
       
        public void Start()
        {
            //Now im testing for realzz
           //OK
            Console.ForegroundColor = ConsoleColor.Green;
            ch = new ConsoleHelper(this);
            Thread t = new Thread(ch.ListenForInput);
            t.Start();
            NetPeerConfiguration config = new NetPeerConfiguration("lagom");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;
            NetServer server = new NetServer(config);
            server.Start();

            int clientNum = 0;

            double nextSendUpdates = NetTime.Now;
            double nextLowUpdate = NetTime.Now;
           
             world.Load("Config");
            //world.Load(1600, 400);
            Console.WriteLine("Server up and running on 14242");
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape || !ch.End)
            {
                if (ch.End)
                    goto CloseServer;
                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            break;
                        case NetIncomingMessageType.ConnectionLatencyUpdated:
                            break;
                        case NetIncomingMessageType.Data:
                            MessageType mt = (MessageType)msg.ReadInt32();
                            
                            switch (mt)
	                            {
                                    case MessageType.WorldSeed:
                                        break;
                                    case MessageType.EntityUpdate:

                                        int c = msg.ReadInt32(); //ID
                                        WorldEntity locEntity = world.entities.First(cl => cl.ID == c);
                                        locEntity.State = msg.ReadInt32();
                                        break;
                                    case MessageType.ClientPosition:

                                        try
                                        {
                                            int b = msg.ReadInt32();
                                            Client cli = clients.First(i => i.ID == b);
                                            cli.Position = msg.ReadVector2();
                                            cli.AnimState = msg.ReadInt32();
                                            cli.Flip = msg.ReadBoolean();
                                            cli.WeaponPosition = msg.ReadVector2();
                                            cli.WeaponRotation = msg.ReadFloat();
                                            cli.Idle = msg.ReadBoolean();
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine("Error, id not exist yes");
                                        }
                                        break;
                                    case MessageType.ClientDisconnecting:
                                        int id = msg.ReadInt32();
                                        clients.First(h => h.ID == id).Connected = false;
                                        Console.WriteLine("Client disconnected (Closed)");
                                        break;

                                    default:
                                    
                                        break;
	                            }

                            break;
                        case NetIncomingMessageType.DiscoveryRequest:
                            //New connection
                            server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                            NetOutgoingMessage netm = server.CreateMessage();
                            
                            break;
                        case NetIncomingMessageType.DiscoveryResponse:
                            break;
                        case NetIncomingMessageType.Error:
                            break;
                        case NetIncomingMessageType.NatIntroductionSuccess:
                            break;
                        case NetIncomingMessageType.Receipt:
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            { 
                                //New player connected
                                clients.Add(new Client(clientNum++, msg.SenderConnection));
                                Console.WriteLine(clients.First(c => c.Identifier == msg.SenderConnection.RemoteUniqueIdentifier) + " connected! \n Sending world seed");
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                Console.WriteLine("Client disconnected (Timed out)");
                                try
                                {
                                    //clients.Remove(clients.First(c => c.Identifier == msg.SenderConnection.RemoteUniqueIdentifier));
                                    clients.First(c => c.Identifier == msg.SenderConnection.RemoteUniqueIdentifier).Connected = false;
                                }
                                catch (Exception)
                                { Console.WriteLine("ERROR: Player already disconnected"); }
                            }
                            break;
                        case NetIncomingMessageType.UnconnectedData:
                            break;
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                            Console.WriteLine(msg.ReadString());
                            break;
                    }

                    //Send updates
                    double now = NetTime.Now;
                    if (now > nextSendUpdates)
                    {
                        List<WorldEntity> itemsNeedUpdate = world.entities.Where(localEntity => localEntity.NeedUpdate == true).ToList() ;
                        foreach (Client client in clients.ToList())
                        {
                            if (!client.ReceivedWorld)
                            {
                                Console.WriteLine("Sending seed and ID to:{0}",client.ID);
                                NetOutgoingMessage message = server.CreateMessage();
                                message.Write((int)MessageType.WorldSeed);
                                message.Write(client.ID);
                                message.Write(world.WorldConfigToString());
                                server.SendMessage(message, client.Connection, NetDeliveryMethod.ReliableOrdered);

                                foreach (WorldEntity entity in world.entities)
                                {
                                    NetOutgoingMessage m = server.CreateMessage();
                                    m.Write((int)MessageType.EntityUpdate);
                                    m.Write(true);
                                    m.Write(entity.ID);
                                    m.Write(entity.Type);
                                    m.Write(entity.X);
                                    m.Write(entity.Y);
                                    server.SendMessage(m, client.Connection, NetDeliveryMethod.ReliableOrdered);
                                }
                                client.ReceivedWorld = true;
                                continue;
                            }
                            
                            foreach (Client client2 in clients.ToList())
                            {
                               if(client.Connection != client2.Connection)
                               {
                                    NetOutgoingMessage om = server.CreateMessage();
                                    om.Write((int)MessageType.ClientPosition);
                                    om.Write(client2.ID);
                                    om.Write(client2.Position);
                                    om.Write(client2.Connected);
                                    om.Write(client2.AnimState);
                                    om.Write(!client2.Flip);
                                    om.Write(client2.WeaponPosition);
                                    om.Write(client2.WeaponRotation);
                                    om.Write(client2.Idle);
                                    server.SendMessage(om, client.Connection, NetDeliveryMethod.UnreliableSequenced);
                                    if (!client2.Connected)
                                    {
                                        clients.Remove(client2);
                                    }
                               }

                               foreach (var entity in itemsNeedUpdate)
                               {
                                   
                                   NetOutgoingMessage entityMessage = server.CreateMessage();
                                   entityMessage.Write((int)MessageType.EntityUpdate);
                                   entityMessage.Write(false);
                                   entityMessage.Write(entity.ID);
                                   entityMessage.Write(entity.State);
                                   server.SendMessage(entityMessage, client2.Connection, NetDeliveryMethod.Unreliable);
                                   entity.NeedUpdate = false;
                               }
                            }

                        }
                        nextSendUpdates += (1.0 / 30.0);
                    }
                    
                }
            }
            CloseServer:
            foreach (Client c in clients)
            {
                NetOutgoingMessage mess = server.CreateMessage();
                mess.Write((int)MessageType.ServerClosing);
                server.SendMessage(mess, c.Connection, NetDeliveryMethod.ReliableUnordered);
            }
        
        }
    }
}
