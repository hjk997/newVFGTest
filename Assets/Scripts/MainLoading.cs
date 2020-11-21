using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLoading : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            LoadingSceneManager.LoadScene("SampleScene");
        }
    }
}
