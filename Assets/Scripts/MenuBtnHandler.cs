using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
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

            // 플레이어의 마지막 위치 저장 

            LoadingSceneManager.LoadScene("InventoryScene");
            //SceneManager.LoadScene("InventoryScene");
        }

        public void backBtnClicked()
        {
            LoadingSceneManager.LoadScene("SampleScene");
            //SceneManager.LoadScene("SampleScene");
        }

        public void exitBtnClicked()
        {
            Application.Quit();
        }

        public void closeBtnClicked()
        {
            // 패널 비활성화 
            // 패널 활성화된 후 버튼 터치하면 종료 
            menuPanel.SetActive(false);
        }
    }
}