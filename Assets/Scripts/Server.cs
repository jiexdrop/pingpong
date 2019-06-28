using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public GameObject serverPlayer;
    public GameObject clientPlayer;
    public GameObject ball;

    private bool goingRight = false;
    private bool goingLeft = false;

    private bool clientGoingRight = false;
    private bool clientGoingLeft = false;

    public float speed = 4f;

    UDPSocket s = new UDPSocket();

    void Start()
    {

        s.Server();
        Debug.LogError("SERVER START");
    }

    void Update()
    {
        if (goingRight)
        {
            Vector3 pos = serverPlayer.transform.position;
            pos.x += Time.deltaTime * speed;
            serverPlayer.transform.position = pos;
        }

        if (goingLeft)
        {
            Vector3 pos = serverPlayer.transform.position;
            pos.x -= Time.deltaTime * speed;
            serverPlayer.transform.position = pos;
        }

        switch (s.received)
        {
            case "CLIENT_RIGHT_PRESSED":
                clientGoingRight = true;
                break;
            case "CLIENT_RIGHT_RELEASED":
                clientGoingRight = false;
                break;
            case "CLIENT_LEFT_PRESSED":
                clientGoingLeft = true;
                break;
            case "CLIENT_LEFT_RELEASED":
                clientGoingLeft = false;
                break;
            default:
                break;
        }

        if (clientGoingRight)
        {
            Vector3 pos = clientPlayer.transform.position;
            pos.x += Time.deltaTime * speed;
            clientPlayer.transform.position = pos;
        }

        if (clientGoingLeft)
        {
            Vector3 pos = clientPlayer.transform.position;
            pos.x -= Time.deltaTime * speed;
            clientPlayer.transform.position = pos;
        }


    }

    public void RightPressed()
    {
        goingRight = true;
        s.ServerSend(Connection.SERVER.ToString() + "_" + Directions.RIGHT_PRESSED.ToString());
    }

    public void RightReleased()
    {
        goingRight = false;
        s.ServerSend(Connection.SERVER.ToString() + "_" + Directions.RIGHT_RELEASED.ToString());
    }

    public void LeftPressed()
    {
        goingLeft = true;
        s.ServerSend(Connection.SERVER.ToString() + "_" + Directions.LEFT_PRESSED.ToString());
    }

    public void LeftReleased()
    {
        goingLeft = false;
        s.ServerSend(Connection.SERVER.ToString() + "_" + Directions.LEFT_RELEASED.ToString());
    }
}
