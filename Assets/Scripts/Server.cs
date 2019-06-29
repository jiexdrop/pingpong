using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState
{
    STOP, 

    START,
    GAME,
    SCORE,
}

public class Server : MonoBehaviour
{
    [Header("Objects")]
    public GameObject serverPlayer;
    public GameObject clientPlayer;
    public GameObject ball;

    [Header("Walls")]
    public RectTransform wallTop;
    public RectTransform wallBottom;
    public RectTransform wallLeft;
    public RectTransform wallRight;
    

    private bool goingRight = false;
    private bool goingLeft = false;

    private bool clientGoingRight = false;
    private bool clientGoingLeft = false;

    public float speed = 4f;

    private GameState state = GameState.STOP;
    
    UDPSocket s = new UDPSocket();

    Vector3 pos;
    Vector3 ballPos;
    Vector3 ballInitPos;
    Vector3 ballSpeed;

    void Start()
    {

        s.Server();
        Debug.LogError("SERVER START");
    }

    void Update()
    {
        if (goingRight)
        {
            pos = serverPlayer.transform.position;
            pos.x += Time.deltaTime * speed;
            serverPlayer.transform.position = pos;

            s.ServerSend(Messages.SERVER.ToString() + pos.ToString());
        }

        if (goingLeft)
        {
            pos = serverPlayer.transform.position;
            pos.x -= Time.deltaTime * speed;
            serverPlayer.transform.position = pos;

            s.ServerSend(Messages.SERVER.ToString() + pos.ToString());
        }


        if (s.received.StartsWith(Messages.START.ToString()) || Input.GetKeyDown(KeyCode.Space))
        {
            Debug.LogError("STARTED GAME");
            state = GameState.START;
        }

        switch (state)
        {
            case GameState.START:
                ballPos = ball.transform.position;
                ballInitPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
                ballSpeed = randomBallSpeed();
                state = GameState.GAME;
                break;
            case GameState.GAME:
                RectTransform ballTransform = ball.GetComponent<RectTransform>();
                RectTransform clientTransform = clientPlayer.GetComponent<RectTransform>();
                RectTransform serverTransform = serverPlayer.GetComponent<RectTransform>();
                

                if(rectOverlaps(ballTransform, wallLeft))
                {
                    //Debug.Log("Left");
                    ballSpeed.x *= -1;
                }

                if (rectOverlaps(ballTransform, wallRight))
                {
                    //Debug.Log("Right");
                    ballSpeed.x *= -1;
                }

                if (rectOverlaps(ballTransform, wallTop))
                {
                    //Debug.Log("Top");
                    //ballSpeed.y *= -1;
                    ballPos = ballInitPos;
                    state = GameState.START;
                }

                if (rectOverlaps(ballTransform, wallBottom))
                {
                    //Debug.Log("Bottom");
                    //ballSpeed.y *= -1;
                    ballPos = ballInitPos;
                    state = GameState.START;
                }

                if (rectOverlaps(ballTransform, clientTransform))
                {
                    ballSpeed.y *= -1;
                }

                if (rectOverlaps(ballTransform, serverTransform))
                {
                    ballSpeed.y *= -1;
                }

                ballPos += ballSpeed;
                ball.transform.position = ballPos;

                s.ServerSend(Messages.BALL.ToString() + ballPos.ToString());

                break;
        }

        // Update client position
        if (s.received.StartsWith(Messages.CLIENT.ToString()))
        {
            s.received = s.received.Replace(Messages.CLIENT.ToString(), "");
            pos = Join.StringToVector3(s.received);
            clientPlayer.transform.position = pos;
        }


    }

    public Vector3 randomBallSpeed(float speed = 0.05f)
    {
        int rand = Random.Range(0, 5);
        switch(rand)
        {
            case 0:
                return new Vector3(0.05f, 0.05f);
            case 1:
                return new Vector3(0.05f, -0.05f);
            case 2:
                return new Vector3(-0.05f, 0.05f);
            case 3:
                return new Vector3(-0.05f, -0.05f);
            case 4:
                return new Vector3(0.05f, 0.05f);
            default:
                return new Vector3(0.05f, 0.05f);
        }
    }

    public bool rectOverlaps(RectTransform rectTrans1, RectTransform rectTrans2)
    {
        float widthRect1 = rectTrans1.rect.width;
        float heightRect1 = rectTrans1.rect.height;
        float widthRect2 = rectTrans2.rect.width;
        float heightRect2 = rectTrans2.rect.height;
        Vector3 posRect1 = Camera.main.WorldToScreenPoint(new Vector3(rectTrans1.position.x, rectTrans1.position.y));
        Vector3 posRect2 = Camera.main.WorldToScreenPoint(new Vector3(rectTrans2.position.x, rectTrans2.position.y));

        Rect rect1 = new Rect(posRect1.x - (widthRect1 * 0.5f), Screen.height - posRect1.y - (heightRect1* 0.5f), widthRect1, heightRect1);
        Rect rect2 = new Rect(posRect2.x - (widthRect2 * 0.5f), Screen.height - posRect2.y - (heightRect2* 0.5f), widthRect2, heightRect2);

        return rect1.Overlaps(rect2);
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
