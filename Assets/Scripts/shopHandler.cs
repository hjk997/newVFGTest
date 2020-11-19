using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class shopHandler : MonoBehaviour
    {
        private ShopController shopController;

        public GameObject shopScript;
        public GameObject shopPanel;
        public GameObject defaultPanel;

        public GameObject noticePanel;
        public GameObject buyPanel;
        public GameObject sellPanel;

        private void Awake()
        {
            shopController = shopScript.GetComponent<ShopController>();
            
            defaultPanel.SetActive(true);
            shopPanel.SetActive(false);

        }

        // 구매 페이지 출력하는 메소드 
        public void showBuyPanel()
        {
            int coin = shopController.getRecentCoin();

            defaultPanel.SetActive(false);
            shopPanel.SetActive(true);

            buyPanel.SetActive(true);
            sellPanel.SetActive(false);
            noticePanel.SetActive(false);

            // TODO : coinText에 남은 coin 출력
            Text coinText = buyPanel.transform.GetChild(7).transform.GetComponent<Text>();
            coinText.text = Convert.ToString(coin);
        }

        // 닫기 버튼 눌렀을 때 실행되는 메소드 
        public void closePanel()
        {
            defaultPanel.SetActive(true);
            shopPanel.SetActive(false);
        }

        // 행동 수행 시 팝업창 출력 
        public void showNoticePanel(string str)
        {
            Text text = noticePanel.transform.GetChild(1).gameObject.GetComponent<Text>();
            text.text = str;

            noticePanel.SetActive(true);
        }

        // 팝업창 닫기 
        public void closeNoticePanel()
        {
            noticePanel.SetActive(false);
        }

        // 구매 화면으로 전환 
        public void showBuyPanelBtnClicked()
        {
            sellPanel.SetActive(false);
            buyPanel.SetActive(true);
        }

        // 판매 화면으로 전환 
        public void showSellPanelClicked()
        {
            buyPanel.SetActive(false);
            sellPanel.SetActive(true);

            shopController.initializeSellPanel();

        }

        public void buyCarrotSeedBtnClicked()
        {
            // 코드와 함께 구매 처리하는 함수 출력 
            string result = shopController.buySomething("carrot");
            showNoticePanel(result);

            int coin = shopController.getRecentCoin();

            Text coinText = buyPanel.transform.GetChild(7).transform.GetComponent<Text>();
            coinText.text = Convert.ToString(coin);
        }

        public void buyOnionSeedBtnClicked()
        {
            string result = shopController.buySomething("onion");
            showNoticePanel(result);

            int coin = shopController.getRecentCoin();

            Text coinText = buyPanel.transform.GetChild(7).transform.GetComponent<Text>();
            coinText.text = Convert.ToString(coin);
        }

        public void buyEggBtnClicked()
        {
            string result = shopController.buySomething("egg");
            showNoticePanel(result);

            int coin = shopController.getRecentCoin();

            Text coinText = buyPanel.transform.GetChild(7).transform.GetComponent<Text>();
            coinText.text = Convert.ToString(coin);
        }

    }
}