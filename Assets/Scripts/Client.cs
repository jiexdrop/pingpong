using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    public GameObject serverPlayer;
    public GameObject clientPlayer;
    public GameObject ball;

    private bool goingRight = false;
    private bool goingLeft = false;

    private bool serverGoingRight = false;
    private bool serverGoingLeft = false;

    public float speed = 4f;

    UDPSocket c = new UDPSocket();


    void Update()
    {
        if (goingRight)
        {
            Vector3 pos = clientPlayer.transform.position;
            pos.x += Time.deltaTime * speed;
            clientPlayer.transform.position = pos;
        }

        if (goingLeft)
        {
            Vector3 pos = clientPlayer.transform.position;
            pos.x -= Time.deltaTime * speed;
            clientPlayer.transform.position = pos;
        }


        switch (c.received)
        {
            case "SERVER_RIGHT_PRESSED":
                serverGoingRight = true;
                break;
            case "SERVER_RIGHT_RELEASED":
                serverGoingRight = false;
                break;
            case "SERVER_LEFT_PRESSED":
                serverGoingLeft = true;
                break;
            case "SERVER_LEFT_RELEASED":
                serverGoingLeft = false;
                break;
            default:
                break;
        }

        if (serverGoingRight)
        {
            Vector3 pos = serverPlayer.transform.position;
            pos.x += Time.deltaTime * speed;
            serverPlayer.transform.position = pos;
        }

        if (serverGoingLeft)
        {
            Vector3 pos = serverPlayer.transform.position;
            pos.x -= Time.deltaTime * speed;
            serverPlayer.transform.position = pos;
        }

    }

    internal void StartServerWithIp(string ip)
    {
        c.Client(ip);
        //Start
        c.ClientSend("");

        Debug.LogError("CLIENT START WITH IP: " + ip);

    }

    public void RightPressed()
    {
        goingRight = true;
        c.ClientSend(Connection.CLIENT.ToString() + "_" + Directions.RIGHT_PRESSED.ToString());
    }

    public void RightReleased()
    {
        goingRight = false;
        c.ClientSend(Connection.CLIENT.ToString() + "_" + Directions.RIGHT_RELEASED.ToString());
    }

    public void LeftPressed()
    {
        goingLeft = true;
        c.ClientSend(Connection.CLIENT.ToString() + "_" + Directions.LEFT_PRESSED.ToString());
    }

    public void LeftReleased()
    {
        goingLeft = false;
        c.ClientSend(Connection.CLIENT.ToString() + "_" + Directions.LEFT_RELEASED.ToString());
    }
}
