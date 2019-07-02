using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class UDPSocket
{
    public const int PORT = 7345;
    UdpClient server;
    UdpClient client;

    IPEndPoint epServer;
    IPEndPoint epClient;

    public Queue<string> received = new Queue<string>();


    public void Server()
    {
        server = new UdpClient(PORT);

        Thread thread = new Thread(new ThreadStart(ServerRecieve));
        thread.Start();
    }

    public void Client(string address)
    {
        client = new UdpClient();
        epServer = new IPEndPoint(IPAddress.Parse(address), PORT);
        client.Connect(epServer);

        Thread thread = new Thread(new ThreadStart(ClientRecieve));
        thread.Start();
    }

    public void ClientSend(string text)
    {

        byte[] textByteArray = Encoding.ASCII.GetBytes(text);
        client.Send(textByteArray, textByteArray.Length);

    }

    public void ServerSend(string text)
    {
        if (epClient != null)
        {
            byte[] textByteArray = Encoding.ASCII.GetBytes(text);
            server.Send(textByteArray, textByteArray.Length, epClient);
        }
        else
        {
            Debug.LogError("NO CLIENT TO SEND TO");
        }
    }

    public void ServerRecieve()
    {
        while (true)
        {
            var remoteEP = new IPEndPoint(IPAddress.Any, PORT);
            var data = server.Receive(ref remoteEP); //Listen on port
            //Debug.LogError("-SERVER--RCV from " + remoteEP.ToString());
            received.Enqueue(Encoding.ASCII.GetString(data));
            epClient = remoteEP;
        }
    }

    public void ClientRecieve()
    {
        while (true)
        {
            var data = client.Receive(ref epServer); //Listen on port
            //Debug.LogError("-CLIENT--RCV from " + remoteEP.ToString());
            received.Enqueue(Encoding.ASCII.GetString(data));
        }
    }
}
