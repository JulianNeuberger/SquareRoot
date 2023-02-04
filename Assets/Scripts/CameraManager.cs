using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float slowScrollSpeed = 2;
    public float fastScrollSpeed = 7;
    public float slowScrollDistance = 30;
    public float fastScrollDistance = 15;

    public float gateLeft = -5;
    public float gateRight = 5;
    public float gateTop = 5;
    public float gateBottom = -7;

    // Update is called once per frame
    void LateUpdate()
    {
        float mousePosX = Input.mousePosition.x;
        float mousePosY = Input.mousePosition.y;

        if (mousePosX < slowScrollDistance && transform.position.x > gateLeft)
        {
            if(mousePosX < fastScrollDistance)
            {
                transform.Translate(Vector3.left * fastScrollSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.left * slowScrollSpeed * Time.deltaTime);
            }
        }

        if (mousePosX >= Screen.width - slowScrollDistance && transform.position.x < gateRight)
        {
            if (mousePosX >= Screen.width - fastScrollDistance)
            {
                transform.Translate(Vector3.right * fastScrollSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.right * slowScrollSpeed * Time.deltaTime);
            }
        }

        if (mousePosY < slowScrollDistance && transform.position.y > gateBottom)
        {
            if (mousePosY < fastScrollDistance)
            {
                transform.Translate(Vector3.down * fastScrollSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.down * slowScrollSpeed * Time.deltaTime);
            }
        }

        if (mousePosY >= Screen.height - slowScrollDistance && transform.position.y < gateTop)
        {
            if (mousePosY >= Screen.height - fastScrollDistance)
            {
                transform.Translate(Vector3.up * fastScrollSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.up * slowScrollSpeed * Time.deltaTime);
            }
        }
    }
}
