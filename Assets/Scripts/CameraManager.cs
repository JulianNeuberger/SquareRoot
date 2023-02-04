using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float slowScrollSpeed = 2;
    public float fastScrollSpeed = 7;
    public float slowScrollDistance = 30;
    public float fastScrollDistance = 15;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mousePosX = Input.mousePosition.x;
        float mousePosY = Input.mousePosition.y;

        if (mousePosX < slowScrollDistance)
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

        if (mousePosX >= Screen.width - slowScrollDistance)
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

        if (mousePosY < slowScrollDistance)
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

        if (mousePosY >= Screen.height - slowScrollDistance)
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
