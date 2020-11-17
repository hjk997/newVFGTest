using UnityEngine;

namespace Assets.Scripts
{
    public class QuitScript : MonoBehaviour
    {

        //public string SceneToLoad;

        // Update is called once per frame
        int ClickCount = 0;
        void Update()
        {
            /* press to start 
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(SceneToLoad);
            }
            */

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClickCount++;
                
                if (!IsInvoking("DoubleClick"))
                    Invoke("DoubleClick", 1.0f);

            }
            else if (ClickCount == 2)
            {
                CancelInvoke("DoubleClick");
                Application.Quit();
            }

        }

        void DoubleClick()
        {
            ClickCount = 0;
        }

    }
}