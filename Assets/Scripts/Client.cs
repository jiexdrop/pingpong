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
    float sendsPerSecond = 4f;

    float timeStartedLerping;
    float frequency = 1;
    float lastTimeToReachGoal = 1;

    private GameState state = GameState.STOP;


    void Update()
    {

        if (goingRight)
        {
            pos = clientPlayer.transform.position;
            pos.x += Time.deltaTime * speed;
            clientPlayer.transform.position = pos;

            MovementMessage mv = new MovementMessage();
            mv.type = MessageType.CLIENT;
            mv.x = pos.x;
            mv.y = pos.y;
            mv.frequency = Time.time;

            c.ClientSend(mv);
        }

        if (goingLeft)
        {
            pos = clientPlayer.transform.position;
            pos.x -= Time.deltaTime * speed;
            clientPlayer.transform.position = pos;

            MovementMessage mv = new MovementMessage();
            mv.type = MessageType.CLIENT;
            mv.x = pos.x;
            mv.y = pos.y;
            mv.frequency = Time.time;

            c.ClientSend(mv);
        }

        // Messages
        switch (c.received.type)
        {
            case MessageType.NONE:
                break;
            case MessageType.SERVER:
                {
                    MovementMessage mm = (MovementMessage)c.received;
                    serverPlayer.transform.position = new Vector3(mm.x, mm.y);
                }
                break;
            case MessageType.BALL:
                {
                    MovementMessage mm = (MovementMessage)c.received;
                    startBallPos = ball.transform.position;
                    endBallPos = new Vector3(mm.x, mm.y);
                    frequency = mm.frequency;
                    timeStartedLerping = Time.time;
                }
                break;
            case MessageType.RESET:
                {
                    Debug.LogError("RESET BALL POS");
                }
                break;
            default:
                Debug.Log("Unhandled message type " + c.received.type);
                break;
        }
        c.received.OnRead(); // Always set a message to read on recieved

        // State
        switch (state)
        {
            case GameState.STOP:
                if (c.Connected())
                {
                    state = GameState.START;
                }
                break;
            case GameState.START:
                //Start
                c.ClientSend(new StartMessage());

                Debug.LogError("CLIENT SENT START");

                state = GameState.GAME;
                break;
            case GameState.GAME:

                break;
        }


        // Lerp
        float lerpPercentage = (Time.time - timeStartedLerping) / frequency;
        //Debug.Log(string.Format("lerpPercent[{0}] = (time[{1}] - tS[{2}]) / tTRG[{3}]", lerpPercentage, Time.time, timeStartedLerping, frequency));
        ball.transform.position = Vector3.Lerp(startBallPos, endBallPos, lerpPercentage);

    }

    internal void StartServerWithIp(string ip)
    {
        c.Client(ip);
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
