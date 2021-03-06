﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CozyKxlol.Server
{
    public partial class Program
    {

        public static Random _RandomMaker = new Random();
        public static Random RandomMaker
        {
            get
            {
                return _RandomMaker;
            }
        }

        static void Main(string[] args)
        {
            OnAgarServerProgerss();
            OnHappyServerProgerss();

            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage msg;
                while ((msg = AgarServer.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            AgarServer.SendDiscoveryResponse(null, msg.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
                                Console.WriteLine(AgarServer.Connections.Count);
                                AgarConnMgr.Add(msg.SenderConnection);
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnect!");
                                uint removeId = AgarConnMgr.Get(msg.SenderConnection);
                                PlayerBallMgr.Remove(removeId);
                                AgarConnMgr.Remove(msg.SenderConnection);
                                Console.WriteLine(AgarServer.Connections.Count);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            int id = msg.ReadInt32();
                            if (!ProcessPacket(AgarServer, id, msg))
                            {
                                DispatchPacket(AgarServer, id, msg);
                            }
                            break;
                    }
                }
                while ((msg = HappyServer.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            HappyServer.SendDiscoveryResponse(null, msg.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
                                Console.WriteLine(HappyServer.Connections.Count);
                                HappyConnMgr.Add(msg.SenderConnection);
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnect!");
                                uint removeId = HappyConnMgr.Get(msg.SenderConnection);
                                HappyPlayerMgr.Remove(removeId);
                                HappyConnMgr.Remove(msg.SenderConnection);
                                Console.WriteLine(HappyServer.Connections.Count);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            int id = msg.ReadInt32();
                            if (!ProcessPacket(HappyServer, id, msg))
                            {
                                DispatchPacket(HappyServer, id, msg);
                            }
                            break;
                    }
                }
                Thread.Sleep(1);
            }
            AgarServer.Shutdown("app exiting");
        }
    }
}
