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

        Dictionary<string, itemDetails> itemListData;
        public ScrollRect scrollRect;

        private void Awake()
        {
            dbCommand = DBConnectionSingleton.getIDbCommand();
        }

        public string buySomething(string code)
        {
            return buy(code);
        }

        // 실제 구매 수행 
        private string buy(string code)
        {
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
                // failed 
                return "가지고 있는 코인이 모자랍니다.";
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


            // success
            return "구매에 성공하였습니다.";

        }

        public int getRecentCoin()
        {
            int coin = 0;

            // db에서 보유 코인 수를 불러온다. 
            string sqlQuery = "SELECT LEFT_NUM FROM INVENTORY WHERE ITEM_NAME='coin';";
            dbCommand.CommandText = sqlQuery;
            var reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                coin = reader.GetInt32(0);
            }

            reader.Close();

            return coin;
        }


        public void initializeSellPanel()
        {
            if(itemListData == null)
            {
                // dataList 값 갱신 & sellStuffList 아래에 object 추가
                itemListData = new Dictionary<string, itemDetails>();
                printSellPanel();
            }
            else
            {
                // db access 후 dataList 값과 다르면 기존에 있는 object에서 내용만 바꾸기 
                updateSellPanel();
            }
            
        }

        private void printSellPanel()
        {
            // Scroll View에 판매 목록을 출력한다. 
            dbCommand = DBConnectionSingleton.getIDbCommand();

            // db에서 보유 코인 수를 불러온다. 
            int coin = getRecentCoin();

            Text coinText = sellPanel.transform.GetChild(0).GetComponent<Text>();
            coinText.text = Convert.ToString(coin);

            // db에서 농작물, 음식에 대한 전체 목록을 불러온다. 
            string sqlQuery = "SELECT ITEM_NAME, ITEM_TYPE, LEFT_NUM, ID FROM INVENTORY WHERE LEFT_NUM!=0 AND ITEM_TYPE IN (1, 2);";
            dbCommand.CommandText = sqlQuery;
            //SqliteDataReader reader = DbCommand.ExecuteReader();
            var reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                itemDetails element = this.gameObject.AddComponent<itemDetails>();

                element.ItemName = reader.GetString(0);
                element.Type = reader.GetInt32(1);
                element.LeftNum = Convert.ToString(reader.GetInt32(2));
                element.Id = reader.GetInt32(3);

                itemListData.Add(element.ItemName, element);
            }

            reader.Close();

            foreach (var pair in itemListData) {
                string name = pair.Value.ItemName;
                int type = pair.Value.Type;
                string leftNum = pair.Value.LeftNum;
                int price = 9999999;

                // 리스트에 모든 목록 출력 
                Text[] newText;
                Image image;
                Button sellBtn;

                // 물품에 대한 가격 값 가져오기 
                if (type == 1) // 농작물일 때 
                {
                    sqlQuery = "SELECT SELL_COST FROM CROPS WHERE NAME='" + name + "';";
                }
                else if(type == 2) // 음식일 때
                {
                    sqlQuery = "SELECT SELL_COST FROM FOOD WHERE FOOD_NAME='" + name + "';";
                }
                else {
                    continue;
                }

                dbCommand.CommandText = sqlQuery;
                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    price = reader.GetInt32(0);
                }

                reader.Close();

                // scroll view에 해당 항목 넣어주기 
                GameObject card = Instantiate(Resources.Load("Prefabs/sellStuffList")) as GameObject;
                card.name = name;

                image = card.GetComponentsInChildren<Image>()[0];
                image.sprite = Resources.Load<Sprite>("Image/" + name);

                name = "이름 : " + name + "(농작물)";
                leftNum = "남은 개수 : " + leftNum;

                newText = card.GetComponentsInChildren<Text>();

                newText[0].text = name;

                newText[1].text = leftNum;

                newText[2].text = "판매 가격: " + Convert.ToString(price);

                card.transform.SetParent(scrollRect.transform.GetChild(0).transform.GetChild(0).transform);
                card.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                // button 이벤트 추가 
                sellBtn = card.transform.GetChild(4).GetComponent<Button>();
                sellBtn.onClick.AddListener(()=>sellBtnClicked(pair.Value.Id, name, type, price, leftNum));
            }

            reader.Close();

        }

        private void sellBtnClicked(int id, string name, int type, int price, string leftNum)
        {
            // 선택된 물건을 1개씩 판매한다. 
            // range bar를 넣어서 여러개씩 판매할 수 있도록 고치면 좋겠지만... 일단 이렇다 

            // 판매할 수 없을 경우 경고창 > 근데 아마 판매할 수 없는 경우가 안 나올 것 
            if (leftNum.Equals("0"))
            {
                // fail 팝업창 출력
                return;
            }

            // 1. 선택한 물건 재고 감소 > 재고가 0이면 실행 안되어야 함 
            string sqlQuery = "UPDATE INVENTORY SET LEFT_NUM=LEFT_NUM-1 WHERE ID='" + id + "';";
            dbCommand.CommandText = sqlQuery;
            //SqliteDataReader reader = DbCommand.ExecuteReader();
            dbCommand.ExecuteNonQuery();

            // 2. 물건 판매 후 물건 가격만큼 코인 추가 
            // 2-1. 물건 판매가 알아내기 > 위에서 알아낸 거 사용 
            sqlQuery = "UPDATE INVENTORY SET LEFT_NUM=LEFT_NUM+" + price + " WHERE ITEM_NAME='coin';";
            dbCommand.CommandText = sqlQuery;
            //SqliteDataReader reader = DbCommand.ExecuteReader();
            dbCommand.ExecuteNonQuery();

            // 3. 판매 목록을 다시 가져온다. 
            updateSellPanel();
        }

        private void updateSellPanel()
        {
            // Scroll View에 판매 목록을 출력한다. 
            IDbCommand DbCommand = DBConnectionSingleton.getIDbCommand();

            // db에서 보유 코인 수를 불러온다. 
            int coin = getRecentCoin();

            Text coinText = sellPanel.transform.GetChild(0).GetComponent<Text>();
            coinText.text = Convert.ToString(coin);

            // db에서 농작물, 음식에 대한 전체 목록을 불러온다. 
            string sqlQuery = "SELECT ITEM_NAME, ITEM_TYPE, LEFT_NUM FROM INVENTORY WHERE ITEM_TYPE IN (1, 2);";
            DbCommand.CommandText = sqlQuery;
            //SqliteDataReader reader = DbCommand.ExecuteReader();
            var reader = DbCommand.ExecuteReader();

            while (reader.Read())
            {
                string name = reader.GetString(0);
                // db에 있는 값과 itemListData의 값을 비교한다. 
                // 값이 같다면 그냥 두고, 다르다면 list 값을 바꾼 후 object에서도 갱신한다. 
                if (itemListData.ContainsKey(name))
                {
                    string leftNum = Convert.ToString(reader.GetInt32(2));

                    // 값이 다를 때 
                    if (!itemListData[name].LeftNum.Equals(leftNum))
                    {
                        // list 정보 갱신
                        itemListData[name].LeftNum = leftNum;

                        // ui에 출력되는 object 값 갱신 
                        GameObject card = scrollRect.transform.GetChild(0).transform.GetChild(0).transform.Find(name).gameObject;

                        // leftNum이 0이면 object 삭제
                        if (leftNum.Equals("0"))
                        {
                            Destroy(card);
                        }
                        // 아니면 text 값만 변경 
                        else
                        {
                            Text newText;
                            newText = card.transform.GetChild(2).GetComponent<Text>();

                            newText.text = "남은 개수 : " + leftNum;
                        }
                    }
                }
            }
            reader.Close();


        }
    }

}