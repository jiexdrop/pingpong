using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient
{
    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    public const int PORT = 7345;

    public string received = "";

    public void Client()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("localhost", PORT);

            byte[] bytes = new byte[1024];

            while (true)
            {
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;

                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);

                        string serverMessage = Encoding.ASCII.GetString(incommingData);

                        Debug.Log("received message from server: " + serverMessage);

                        received = serverMessage;
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }


    public void ClientSend(string message)
    {
        if (socketConnection == null)
        {
            return;
        }

        try
        {
            NetworkStream stream = socketConnection.GetStream();

            if (stream.CanWrite)
            {
                // String converted
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);

                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                //Debug.Log("Client sent message");
            }

        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}
