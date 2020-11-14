using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuBtnHandler : MonoBehaviour
{
    public GameObject menuPanel;

    public void MenuBtnClicked()
    {
        // 패널 활성화 
        // 패널 활성화된 후 바깥 터치하면 종료 
        menuPanel.SetActive(true);
    }

    public void InvenBtnClicked()
    {
        menuPanel.SetActive(false);
        SceneManager.LoadScene("InventoryScene");
    }

    public void backBtnClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void exitBtnClicked()
    {
        Application.Quit();
    }

    public void closeBtnClicked()
    {
        // 패널 활성화 
        // 패널 활성화된 후 바깥 터치하면 종료 
        menuPanel.SetActive(false);
    }
}
