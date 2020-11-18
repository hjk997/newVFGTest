using UnityEngine;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ShopController : MonoBehaviour
    {
        IDbCommand dbCommand;

        public GameObject successedPanel;
        public GameObject failedPanel;
        public GameObject buyPanel;
        public GameObject sellPanel;

        List<List<itemDetails>> listData;
        public ScrollRect scrollRect;

        private void Awake()
        {
            dbCommand = DBConnectionSingleton.getIDbCommand();
        }

        public void buyCarrotSeedBtnClicked()
        {
            // 코드와 함께 구매 처리하는 함수 출력 
            buySomething("carrot");
        }

        public void buyOnionSeedBtnClicked()
        {
            buySomething("onion");
        }

        public void buyEggBtnClicked()
        {
            buySomething("egg");
        }

        // 실제 구매 수행 
        private int buySomething(string code)
        {
            int result = 0;
            int stuffCost = 10000000;
            int stuffType = 10;
            int leftCoin=0;

            // 1. coin이 모자라면 구매 불가능 팝업 창을 띄운다. 
            string sqlQuery = "SELECT LEFT_NUM FROM INVENTORY WHERE ITEM_NAME='coin';";
            dbCommand.CommandText = sqlQuery;
            // 나중에 찾아보고 ExecuteScalar로 바꾸기 
            var reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                leftCoin = reader.GetInt32(0);
            }

            reader.Close();

            sqlQuery = "SELECT STUFF_COST, STUFF_TYPE FROM MARKET WHERE STUFF_NAME='" + code + "';";
            dbCommand.CommandText = sqlQuery;
            reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                stuffCost = reader.GetInt32(0);
                stuffType = reader.GetInt32(1);
                
            }

            reader.Close();

            if (leftCoin < stuffCost)
            {
                showFailedPopup();
                return -1;
            }

            // 2. coin이 충분하면 구매 처리 후 구매 완료 팝업창을 띄운다. 

            // 2-1. inventory의 coin 값 감소
            sqlQuery = "UPDATE INVENTORY SET LEFT_NUM=LEFT_NUM-" + Convert.ToString(stuffCost) + " WHERE ITEM_NAME='coin';";
            dbCommand.CommandText = sqlQuery;
            dbCommand.ExecuteNonQuery();

            // 2-2. inventory의 물건 재고 +1
            // 물건 이름이랑 TYPE 같이 찾아서 바꾸는 걸로 수정하기 
            sqlQuery = "UPDATE INVENTORY SET LEFT_NUM=LEFT_NUM+1 WHERE ITEM_NAME='" + code + "' AND ITEM_TYPE="+ Convert.ToString(stuffType) +";";
            dbCommand.CommandText = sqlQuery;
            dbCommand.ExecuteNonQuery();

            showSuccessedPopup();
            return result;

        }

        private void sellSomething()
        {

        }

        private void showFailedPopup()
        {
            failedPanel.SetActive(true);
        }

        private void showSuccessedPopup()
        {
            successedPanel.SetActive(true);
        }

        public void successedOkBtnClicked()
        {
            successedPanel.SetActive(false);
        }

        public void failedOkBtnClicked()
        {
            failedPanel.SetActive(false);
        }

        public void showBuyPanelBtnClicked()
        {
            sellPanel.SetActive(false);
            buyPanel.SetActive(true);
        }

        public void showSellPanelClicked()
        {
            buyPanel.SetActive(false);
            sellPanel.SetActive(true);

            //printSellPanel();
        }

        private void printSellPanel()
        {
            // Scroll View에 판매 목록을 출력한다. 
            IDbCommand DbCommand = DBConnectionSingleton.getIDbCommand();

            // db에서 전체 목록을 불러온다. 
            string sqlQuery = "SELECT ITEM_NAME, ITEM_TYPE, LEFT_NUM FROM INVENTORY WHERE LEFT_NUM!=0;";
            DbCommand.CommandText = sqlQuery;
            //SqliteDataReader reader = DbCommand.ExecuteReader();
            var reader = DbCommand.ExecuteReader();

            while (reader.Read())
            {
                string name = reader.GetString(0);
                int type = reader.GetInt32(1);
                string leftNum = Convert.ToString(reader.GetInt32(2));

                if (name.Equals("coin"))
                {
                    
                }
                else
                {
                    listData[type - 1].Add(new itemDetails(name, leftNum, type));
                }
            }

            reader.Close();

            // 리스트에 모든 목록 출력 
            foreach (var pair in listData)
            {
                foreach (var element in pair)
                {
                    Text[] newText;
                    Image image;

                    string name = element.ItemName;
                    string leftNum = element.LeftNum;
                    string type = Convert.ToString(element.Type);

                    // scroll view에 해당 항목 넣어주기 
                    GameObject card = Instantiate(Resources.Load("Prefabs/inventoryList")) as GameObject;
                    card.name = name + type;

                    image = card.GetComponentsInChildren<Image>()[0];
                    image.sprite = Resources.Load<Sprite>("Image/" + name);

                    if (type.Equals("1")) // 농작물
                    {
                        name = "이름 : " + name + "(농작물)";

                    }
                    else if (type.Equals("5"))// 씨앗
                    {
                        name = "이름 : " + name + "(씨앗)";
                    }
                    else
                    {
                        name = "이름 : " + name;
                    }

                    leftNum = "남은 개수 : " + leftNum;

                    newText = card.GetComponentsInChildren<Text>();

                    newText[0].text = name;

                    newText[1].text = leftNum;

                    card.transform.SetParent(scrollRect.transform.GetChild(1).transform.GetChild(1).transform);
                    card.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

            }
        }
    }

}