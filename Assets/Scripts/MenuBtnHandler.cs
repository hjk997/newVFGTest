using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class MenuBtnHandler : MonoBehaviour
    {
        public GameObject menuPanel;
        public GameObject player;

        public void MenuBtnClicked()
        {
            // 패널 활성화 
            // 패널 활성화된 후 바깥 터치하면 종료 
            menuPanel.SetActive(true);
        }

        public void InvenBtnClicked()
        {
            Vector3 pos, rot;

            pos = player.transform.position;
            rot = player.transform.rotation.eulerAngles;

            // 플레이어의 마지막 위치 저장 
            PlayerPrefs.SetFloat("xPosition", pos.x);
            PlayerPrefs.SetFloat("yPosition", pos.y);
            PlayerPrefs.SetFloat("zPosition", pos.z);

            PlayerPrefs.SetFloat("xRotation", rot.x);
            PlayerPrefs.SetFloat("yRotation", rot.y);
            PlayerPrefs.SetFloat("zRotation", rot.z);



            menuPanel.SetActive(false);

            LoadingSceneManager.LoadScene("InventoryScene");
            //SceneManager.LoadScene("InventoryScene");
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