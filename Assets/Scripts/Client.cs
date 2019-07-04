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

    TCPClient c = new TCPClient();

    Vector3 pos;

    Vector3 startBallPos;
    Vector3 endBallPos;
    float sendsPerSecond = 8f;


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

        string treatRecieved = c.received;

        // Update server position
        if (treatRecieved.StartsWith(Messages.SERVER.ToString()))
        {
            treatRecieved = treatRecieved.Replace(Messages.SERVER.ToString(), "");
            pos = Join.StringToVector3(treatRecieved);
            serverPlayer.transform.position = pos;
        }


        if (treatRecieved.StartsWith(Messages.BALL.ToString()))
        {
            treatRecieved = treatRecieved.Replace(Messages.BALL.ToString(), "");

            startBallPos = ball.transform.position;
            endBallPos = Join.StringToVector3(treatRecieved);
        }

        ball.transform.position = Vector3.MoveTowards(ball.transform.position, endBallPos, Vector3.Distance(startBallPos, endBallPos) / (1f / sendsPerSecond) * Time.deltaTime);

        if (c.received.StartsWith(Messages.RESET.ToString()))
        {
            Debug.LogError("RESET BALL POS: " + c.received);
            c.received = c.received.Replace(Messages.RESET.ToString(), "");
            ball.transform.position = Join.StringToVector3(c.received);
            startBallPos = ball.transform.position;
            endBallPos = ball.transform.position;
        }

    }

    internal void StartServerWithIp(string ip)
    {
        c.Client();
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
