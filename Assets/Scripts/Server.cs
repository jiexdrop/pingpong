using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


    public float speed = 4f;

    public float waitTime = 0f;

    private GameState state = GameState.STOP;

    TCPServer s = new TCPServer();

    Vector3 pos;
    Vector3 ballPos;
    Vector3 ballInitPos;
    Vector3 ballSpeed;

    private IEnumerator sendBallCoroutine;

    void Start()
    {
        s.Server();
        Debug.LogError("SERVER START");
        sendBallCoroutine = SendBallPosition();
    }

    void Update()
    {
        if (goingRight)
        {
            pos = serverPlayer.transform.position;
            pos.x += Time.deltaTime * speed;
            serverPlayer.transform.position = pos;

            MovementMessage mv = new MovementMessage();
            mv.type = MessageType.SERVER;
            mv.x = pos.x;
            mv.y = pos.y;
            mv.frequency = Time.time;

            s.ServerSend(mv);
        }

        if (goingLeft)
        {
            pos = serverPlayer.transform.position;
            pos.x -= Time.deltaTime * speed;
            serverPlayer.transform.position = pos;

            MovementMessage mv = new MovementMessage();
            mv.type = MessageType.SERVER;
            mv.x = pos.x;
            mv.y = pos.y;
            mv.frequency = Time.time;

            s.ServerSend(mv);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.LogError("START GAME");

            state = GameState.START;
        }

        // Messages
        switch (s.received.type)
        {
            case MessageType.NONE:
                break;
            case MessageType.START:

                Debug.LogError("START GAME");

                state = GameState.START;

                break;
            case MessageType.CLIENT:
                MovementMessage mm = (MovementMessage)s.received;
                clientPlayer.transform.position = new Vector3(mm.x, mm.y);
                break;

            default:
                Debug.Log("Unhandled type " + s.received.type.ToString());
                break;
        }
        s.received.OnRead();  // Always set a message to read on recieved


        // State
        switch (state)
        {
            case GameState.STOP:

                break;
            case GameState.START:
                ballPos = ball.transform.position;

                ballInitPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
                ballSpeed = randomBallSpeed();
                waitTime += Time.deltaTime;

                if (waitTime > 3)
                {
                    waitTime = 0;
                    StopCoroutine(sendBallCoroutine); 
                    StartCoroutine(sendBallCoroutine);
                    state = GameState.GAME;
                }
                break;
            case GameState.GAME:
                RectTransform ballTransform = ball.GetComponent<RectTransform>();
                RectTransform clientTransform = clientPlayer.GetComponent<RectTransform>();
                RectTransform serverTransform = serverPlayer.GetComponent<RectTransform>();


                if (rectOverlaps(ballTransform, wallLeft))
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
                    Debug.LogError("SEND RESET");
                    s.ServerSend(new ResetMessage());
                    state = GameState.START;

                }

                if (rectOverlaps(ballTransform, wallBottom))
                {
                    //Debug.Log("Bottom");
                    //ballSpeed.y *= -1;
                    ballPos = ballInitPos;
                    Debug.LogError("SEND RESET");
                    s.ServerSend(new ResetMessage());
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


                break;
        }

    }

    public IEnumerator SendBallPosition()
    {
        while (true)
        {
            float frequency = 1.0f / 6.0f; // Sends 6 times a second

            yield return new WaitForSeconds(frequency); 

            Debug.LogError("Sending ball position !!!");
            MovementMessage ballMovement = new MovementMessage();
            ballMovement.type = MessageType.BALL;
            ballMovement.x = ballPos.x;
            ballMovement.y = ballPos.y;
            ballMovement.frequency = frequency;

            s.ServerSend(ballMovement);
        }
    }

    public Vector3 randomBallSpeed(float speed = 0.05f)
    {
        int rand = Random.Range(0, 5);
        switch (rand)
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

        Rect rect1 = new Rect(posRect1.x - (widthRect1 * 0.5f), Screen.height - posRect1.y - (heightRect1 * 0.5f), widthRect1, heightRect1);
        Rect rect2 = new Rect(posRect2.x - (widthRect2 * 0.5f), Screen.height - posRect2.y - (heightRect2 * 0.5f), widthRect2, heightRect2);

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
