using System;
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
    public float gateBottom = -15;

    public float dragSpeed = 0.5f;
    private Vector3 dragOrigin;

    public bool cameraDragging = true;


    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        float left = Screen.width * 0.2f;
        float right = Screen.width - (Screen.width * 0.2f);

        if (mousePosition.x < left)
        {
            cameraDragging = true;
        }
        else if (mousePosition.x > right)
        {
            cameraDragging = true;
        }

        if (cameraDragging)
        {

            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(1)) return;

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            float xPos = Math.Min(0.2f, pos.x);
            if(pos.x < 0)
            {
                xPos = Math.Max(-0.2f, pos.x);
            }
            float yPos = Math.Min(0.2f, pos.y);
            if (pos.y < 0)
            {
                yPos = Math.Max(-0.2f, pos.y);
            }
            Vector3 move = new Vector3(xPos * dragSpeed, yPos * dragSpeed, 0);

            if (move.x > 0f)
            {
                if (this.transform.position.x < gateRight)
                {
                    if (move.y > 0f)
                    {
                        if (this.transform.position.y < gateTop)
                        {
                            transform.Translate(move, Space.World);
                        }
                    }
                    else
                    {
                        if (this.transform.position.y > gateBottom)
                        {
                            transform.Translate(move, Space.World);
                        }
                    }
                }
            }
            else
            {
                if (this.transform.position.x > gateLeft)
                {
                    if (move.y > 0f)
                    {
                        if (this.transform.position.y < gateTop)
                        {
                            transform.Translate(move, Space.World);
                        }
                    }
                    else
                    {
                        if (this.transform.position.y > gateBottom)
                        {
                            transform.Translate(move, Space.World);
                        }
                    }
                }
            }
        }

        //float mousePosX = Input.mousePosition.x;
        //float mousePosY = Input.mousePosition.y;

        //if (mousePosX < slowScrollDistance && transform.position.x > gateLeft)
        //{
        //    if(mousePosX < fastScrollDistance)
        //    {
        //        transform.Translate(Vector3.left * fastScrollSpeed * Time.deltaTime);
        //    }
        //    else
        //    {
        //        transform.Translate(Vector3.left * slowScrollSpeed * Time.deltaTime);
        //    }
        //}

        //if (mousePosX >= Screen.width - slowScrollDistance && transform.position.x < gateRight)
        //{
        //    if (mousePosX >= Screen.width - fastScrollDistance)
        //    {
        //        transform.Translate(Vector3.right * fastScrollSpeed * Time.deltaTime);
        //    }
        //    else
        //    {
        //        transform.Translate(Vector3.right * slowScrollSpeed * Time.deltaTime);
        //    }
        //}

        //if (mousePosY < slowScrollDistance && transform.position.y > gateBottom)
        //{
        //    if (mousePosY < fastScrollDistance)
        //    {
        //        transform.Translate(Vector3.down * fastScrollSpeed * Time.deltaTime);
        //    }
        //    else
        //    {
        //        transform.Translate(Vector3.down * slowScrollSpeed * Time.deltaTime);
        //    }
        //}

        //if (mousePosY >= Screen.height - slowScrollDistance && transform.position.y < gateTop)
        //{
        //    if (mousePosY >= Screen.height - fastScrollDistance)
        //    {
        //        transform.Translate(Vector3.up * fastScrollSpeed * Time.deltaTime);
        //    }
        //    else
        //    {
        //        transform.Translate(Vector3.up * slowScrollSpeed * Time.deltaTime);
        //    }
        //}
    }
}
