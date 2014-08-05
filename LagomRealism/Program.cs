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

        static void Main(string[] args)
        {
            List<Client> clients = new List<Client>();
            ConsoleHelper ch = new ConsoleHelper(clients);
            Thread t = new Thread(ch.ListenForInput);
            t.Start();
            NetPeerConfiguration config = new NetPeerConfiguration("lagom");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;
            int y = 400;
            int x = 1600;
            List<GameEntity> entities = new List<GameEntity>();
            NetServer server = new NetServer(config);
            server.Start();

            int clientNum = 0;

            double nextSendUpdates = NetTime.Now;
            double nextLowUpdate = NetTime.Now;
            int imageWidth = 800;
            int imageHeight = 400;
            World world = new World();
            world.Load(imageWidth,imageHeight);
            Console.WriteLine("Server up and running on 14242");
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape || !ch.End)
            {
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

                                        int c = msg.ReadInt32();
                                        entities.First(i => i.ID == c).State = (EntityState)msg.ReadInt32();

                                        break;
                                    case MessageType.ClientPosition:

                                        int b = msg.ReadInt32();
                                        clients.First(i => i.ID == b).Position = msg.ReadVector2();
                                        
                                        break;
                                    case MessageType.ClientDisconnecting:
                                        clients.First(h => h.ID == msg.ReadInt32()).Connected = false;
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
                        
                        foreach (Client client in clients.ToList())
                        {
                            if (!client.ReceivedWorld)
                            {
                                Console.WriteLine("Sending seed and ID to:{0}",client.ID);
                                NetOutgoingMessage message = server.CreateMessage();
                                message.Write((int)MessageType.WorldSeed);
                                message.Write(client.ID);
                                message.Write(world.Seed);
                                message.Write(x);
                                message.Write(y);
                                server.SendMessage(message, client.Connection, NetDeliveryMethod.ReliableOrdered);
                                client.ReceivedWorld = true;
                                continue;
                            }
                            
                            foreach (Client client2 in clients.ToList())
                            {
                                NetOutgoingMessage om = server.CreateMessage();
                                om.Write((int)MessageType.ClientPosition);
                                om.Write(client2.ID);
                                om.Write(client2.Position);
                                om.Write(client2.Connected);
                                server.SendMessage(om, client.Connection, NetDeliveryMethod.UnreliableSequenced);
                                if (!client2.Connected)
                                {
                                    clients.Remove(client2);
                                }
                            }
                        }
                        nextSendUpdates += (1.0 / 30.0);
                    }
                    
                }
            }

            foreach (Client c in clients)
            {
                NetOutgoingMessage mess = server.CreateMessage();
                mess.Write((int)MessageType.ServerClosing);
                server.SendMessage(mess, c.Connection, NetDeliveryMethod.ReliableUnordered);

            }
        }
    }
}
