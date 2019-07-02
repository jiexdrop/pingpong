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

    Vector3 startBallPos;
    Vector3 endBallPos;
    float sendsPerSecond = 4f;

    public float t = 0;

    float lastSendTime;

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

        if (c.received.Count > 0)
        {
            string treatRecieved = c.received.Dequeue();

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

                lastSendTime = 0;
                startBallPos = ball.transform.position;
                endBallPos = Join.StringToVector3(treatRecieved);
            }
            Debug.Log("Last send time: " + lastSendTime);
            lastSendTime += Time.deltaTime;
            t += Time.deltaTime / (1f / sendsPerSecond);
            ball.transform.position = Vector3.Lerp(startBallPos, endBallPos, t);

            //if (c.received.StartsWith(Messages.RESET_BALL.ToString()))
            //{
            //    Debug.LogError("RESET BALL POS: " + c.received);
            //    c.received = c.received.Replace(Messages.RESET_BALL.ToString(), "");
            //    ball.transform.position = Join.StringToVector3(c.received);
            //}
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
