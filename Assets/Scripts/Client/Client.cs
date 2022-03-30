using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class Client : MonoBehaviour
{
    private bool socketReady = false;
    private TcpClient socket;
    private NetworkStream  stream;
    private StreamWriter writer;
    private StreamReader reader;

    public void ConnectedToServer()
    {
        if (socketReady)
        {
            return;
        }

        // Default host, port values
        string host = "127.0.0.1";
        int port = Server.port;

        // Overwrite default value, if there is something in the boxes
        // string inputedHost;
        // int inputedPort;
        // inputedHost = GameObject.Find("HostInput").GetComponent<InputField>().text;
        // if (inputedHost != "")
        // {
        //     host = inputedHost;
        // }
        // int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out inputedPort);
        // if (inputedPort != 0)
        // {
        //     port = inputedPort;
        // }

        // create the socket, and connect to the server
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch (Exception ex)
        {
            Debug.Log("[ERROR] Something went wrong when create socket for client\n" + ex.Message);
        }
    }

    private void Update()
    {
        if (socketReady == false)
        {
            return;
        }

        if (stream.DataAvailable == false)
        {
            return;
        }

        string data = reader.ReadLine();
        if (data != null)
        {
            OnIncomingdata(data);
        }
    }

    private void OnIncomingdata(string data)
    {
        Debug.Log("Server respone: " + data);
    }
}
