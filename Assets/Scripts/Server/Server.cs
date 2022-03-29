using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class Server : MonoBehaviour
{
    private List<ClientInServer> clients;
    private List<ClientInServer> disconnectList;

   public int port = 6321;
   private TcpListener server;
   private bool serverStarted;

   private void Start()
   {
       clients = new List<ClientInServer>();
       disconnectList = new List<ClientInServer>();

       try
       {
           server = new TcpListener(IPAddress.Any, port);
           server.Start();

           StartListening();
           serverStarted = true;

           Debug.Log("Server has started on port: " + port.ToString());
       }
       catch(Exception ex)
       {
           Debug.Log("[ERROR]Socket error: " + ex.Message);
       }
   }

   private void Update()
   {
       if (serverStarted == false)
       {
           return;
       }

       foreach (ClientInServer client in clients)
       {
           // If the client is disconnected
           if (IsConnected(client.tcp) == false)
           {
               client.tcp.Close();
               disconnectList.Add(client);
               continue;
           }
           // Check for message from client
           else
           {
               // Get stream
               NetworkStream ns = client.tcp.GetStream();

                // If data is available
               if (ns.DataAvailable)
               {
                   StreamReader sr = new StreamReader(ns, true);
                   string data = sr.ReadLine();

                   if (data != null)
                   {
                       OnIncomingData(client, data);
                   }
               }
           }
       }
   }

    private void OnIncomingData(ClientInServer client, string data)
    {
        Debug.Log(client.clientName + "has sent the following message: " + data);
    }

    private bool IsConnected(TcpClient c)
   {
       try
       {
           if (c != null && c.Client != null & c.Client.Connected)
           {
               if (c.Client.Poll(0, SelectMode.SelectRead))
               {
                   return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
               }
               return true;
           }
           else
           {
               return false;
           }
       }
       catch (Exception ex)
       {
           Debug.Log("[ERROR] Something went wrong when checking IsConnected for client: " + c.Client + "\n"
                    + ex.Message);
           return false;
       }
   }
    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        clients.Add(new ClientInServer(listener.EndAcceptTcpClient(ar)));

        StartListening();

        // Send a message to everyone, to notify that somebody has connected to the server
        Broadcast(clients[clients.Count - 1].clientName + "has connected", clients);
    }

    private void Broadcast(string data, List<ClientInServer> cl)
    {
        foreach (ClientInServer c in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine((data));
                writer.Flush();
            }
            catch (Exception ex)
            {
                Debug.Log("[ERROR] Broadcast have something wrong in client: " + c.clientName + "\nError: " + ex.Message);
            }
        }
    }
}

public class ClientInServer
{
    public TcpClient tcp;
    public string clientName;

    public ClientInServer(TcpClient client)
    {
        this.tcp = client;
        this.clientName = "Guest";
    }
}
