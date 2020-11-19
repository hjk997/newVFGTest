using UnityEngine;
using UnityEngine.UI;
using System.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class scrollViewController : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public Text coinText;

        List<List<itemDetails>> listData;
        
        public void Awake()
        {
            listData = new List<List<itemDetails>>();
            for(int i = 0; i < 5; i++)
            {
                listData.Add(new List<itemDetails>());
            }
            initScrollView();
        }

        // 호출 되면 데이터베이스에서 inventory 정보 들고오고 dictionary에 저장한다. 
        private void initScrollView()
        {

            IDbCommand DbCommand = DBConnectionSingleton.getIDbCommand();

            // db에서 전체 목록을 불러온다. 
            string sqlQuery = "SELECT ITEM_NAME, ITEM_TYPE, LEFT_NUM FROM INVENTORY WHERE LEFT_NUM!=0;";
            DbCommand.CommandText = sqlQuery;
            //SqliteDataReader reader = DbCommand.ExecuteReader();
            var reader = DbCommand.ExecuteReader();

            while (reader.Read())
            {
                itemDetails element = this.gameObject.AddComponent<itemDetails>();

                element.ItemName = reader.GetString(0);
                element.Type = reader.GetInt32(1);
                element.LeftNum = Convert.ToString(reader.GetInt32(2));

                if (name.Equals("coin"))
                {
                    coinText.text = "보유 코인: " + element.LeftNum;
                }
                else
                {
                    listData[element.Type - 1].Add(element);
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
                    image.sprite = Resources.Load<Sprite>("Image/"+name);

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

                    card.transform.SetParent(scrollRect.transform.GetChild(0).transform.GetChild(0).transform);
                    card.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

            }

        }

        public void totalPrint()
        { // code = 0

            printScrollView(-1);
        }

        public void cropsPrint()
        { // code = 1

            printScrollView(1);

        }

        public void foodPrint()
        { // code = 2

            printScrollView(2);
        }

        public void toolPrint()
        { // code = 3, 4

            printScrollView(4);
        }

        public void seedPrint()
        { // code = 5

            printScrollView(5);

        }


        void printScrollView(int exceptCode)
        {
            // 선택된 항목 외의 모든 요소를 전부 invisible 상태로 바꾼다.. 
            foreach (var pair in listData)
            {
                foreach (var element in pair)
                {
                    string tTag = element.ItemName + element.Type;
                    //Debug.Log(tTag);

                    // scrollView에서 태그 이름을 기반으로 객체를 하나씩 읽어온다. 
                    GameObject card = scrollRect.transform.Find("Viewport").transform.Find("Content").transform.Find(tTag).gameObject;

                    if ( (exceptCode == -1) || (element.Type == exceptCode) )
                    {
                        //Debug.Log("foreach: " + card.name);
                        //visible 상태로 전환
                        card.SetActive(true);
                    } 
                    else 
                    {
                        //invisible 상태로 전환 
                        card.SetActive(false);
                    }
                }
            }
        }
}
}