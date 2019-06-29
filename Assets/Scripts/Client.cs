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

    public float speed = 4f;

    UDPSocket c = new UDPSocket();

    Vector3 pos;

    void Update()
    {

        if (goingRight)
        {
            pos = clientPlayer.transform.position;
            pos.x += Time.deltaTime * speed;
            clientPlayer.transform.position = pos;

            c.ClientSend(Messages.CLIENT.ToString() + pos.ToString());
        }

        if (goingLeft)
        {
            pos = clientPlayer.transform.position;
            pos.x -= Time.deltaTime * speed;
            clientPlayer.transform.position = pos;

            c.ClientSend(Messages.CLIENT.ToString() + pos.ToString());
        }


        // Update server position
        if (c.received.StartsWith(Messages.SERVER.ToString()))
        {
            c.received = c.received.Replace(Messages.SERVER.ToString(), "");
            pos = Join.StringToVector3(c.received);
            serverPlayer.transform.position = pos;
        }
        if (c.received.StartsWith(Messages.BALL.ToString())){
            c.received = c.received.Replace(Messages.BALL.ToString(), "");
            Debug.LogError(c.received);
            ball.transform.position = Join.StringToVector3(c.received);
        }

    }

    internal void StartServerWithIp(string ip)
    {
        c.Client(ip);
        //Start
        c.ClientSend(Messages.START.ToString());

        Debug.LogError("CLIENT START WITH IP: " + ip);

    }

    public void RightPressed()
    {
        goingRight = true;
    }

    public void RightReleased()
    {
        goingRight = false;
    }

    public void LeftPressed()
    {
        goingLeft = true;
    }

    public void LeftReleased()
    {
        goingLeft = false;
    }
}
