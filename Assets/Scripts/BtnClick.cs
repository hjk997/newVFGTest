using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 버튼을 눌렀을 때 플레이어를 앞으로 이동, 혹은 좌우로 회전시킵니다. 
 **/
public class BtnClick : MonoBehaviour
{
    
    public Transform tr;

    public float speed = 20;
    public bool go, right, left;

    // Update is called once per frame
    void Update()
    {
        if (go)
        {
            tr.Translate(Vector3.forward*speed*Time.deltaTime);
        }
        if (left){
            tr.Rotate(new Vector3(0, -50, 0) * Time.deltaTime);

        }
        if (right){
            tr.Rotate(new Vector3(0, 50, 0) * Time.deltaTime);
        }
    }

    public void LeftBtnClickedUp()
    {
        left = false;
    }
    public void LeftBtnClickedDown()
    {
        left = true;
    }
    public void RightBtnClickedUp()
    {
        right = false;
    }
    public void RightBtnClickedDown()
    {
        right = true;
    }

    public void UpBtnClickedUp()
    {
        go = false;
    }

    public void UpBtnClickedDown()
    {
        go = true;
    }

}
