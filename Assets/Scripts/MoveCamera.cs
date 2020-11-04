using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 안 씁니다 
 **/
public class MoveCamera : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        float keyHorizontal = Input.GetAxis("Horizontal");
        float keyVertical = Input.GetAxis("Vertical");

        Debug.Log(keyHorizontal.ToString());
        Debug.Log(keyVertical.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
