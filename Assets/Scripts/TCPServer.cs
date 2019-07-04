using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer
{

    private TcpListener tcpListener;

    private Thread tcpListenerThread;

    private TcpClient connectedTcpClient;

    public const int PORT = 7345;

    public string received = "";


    // Update is called once per frame
    void Update()
    {

    }

    public void Server()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    private async void ListenForIncomingRequests()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), PORT);
            
            tcpListener.Start();

            Debug.Log("Server is Listening");

            byte[] bytes = new byte[1024];

            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        int length;

                        while ((length = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);

                            string clientMessage = Encoding.ASCII.GetString(incommingData);

                            Debug.Log("received message from client: " + clientMessage);

                            received = clientMessage;
                        }
                    }
                }
            }

        }
        catch (SocketException e)
        {
            Debug.Log("SocketException " + e.ToString());
        }
    }

    public void ServerSend(string message)
    {
        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {

                // Convert string message to byte array.                 
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(message);
                // Write byte array to socketConnection stream.               
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}
