using UnityEngine;
using System.Data;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    /*
     * 날짜가 바뀌었을 때 동작을 관리
     * (팝업창, 데이터베이스 update) 
     * 
     */
    public class DateChanger : MonoBehaviour
    {
        public GameObject dateUpdatePanel;

        IDbCommand dbCommand;
        GameObject farm;
        GameObject farmChild;
        GameObject crops;

        Transform cropsParents;

        String sqlQuery;

        public Text SaveLogText;

        // 게임이 시작되면 밭 모습 초기화 
        // 인벤토리 구현은 우선순위에 따라 계획 예정 
        private void Awake()
        {
            dbCommand = DBConnectionSingleton.getIDbCommand();

            accessFirstTimeUpdate();

            // 밭 이름 미리 지정해둠 
            String[] ttag = { "farm1", "farm2", "farm3", "farm4", "farm5" };
            String tmpTag;

            // 밭을 모두 순회하며 저장되어있는 농작물 심어줌 
            foreach (String t in ttag)
            {
                // 1. 이름이 같은 밭의 GameObject 가져옴 
                farm = GameObject.Find(t);

                // 1. for문을 돌면서 PlayerPrefs의 키 값 가져옴 
                for (int i = 0; i < 16; i++)
                {
                    // 2. key string 만듦
                    tmpTag = t + i.ToString();

                    // 3. PlayerPrefs에서 저장된 값 가져옴 > sqlite에서 가져옴
                    //tmpStr = PlayerPrefs.GetString(tmpTag);
                    sqlQuery = "SELECT ID FROM CROPS WHERE ID=(SELECT CROP_ID FROM FARM WHERE ID='" + tmpTag + "')";
                    // view를 만들거나 join table을 생성하는 쪽이 더 시스템 부하가 덜할 것 같다. 

                    dbCommand.CommandText = sqlQuery;
                    int Ttag = Convert.ToInt32(dbCommand.ExecuteScalar());

                    //Debug.Log(Ttag);
                    // 4. 해당 키 값이랑 일치하는 태그를 배열에서 찾아줌 
                    // 일치한다면 instantiate함 
                    if (Ttag == cropsConstants.mushroom)
                    {
                        // 5. instantiate로 프리팹 추가 
                        crops = Instantiate(Resources.Load("Prefabs/Mushroom"), new Vector3(0, 0, 0),
                            Quaternion.identity) as GameObject;

                        // 6. 추가한 프리팹의 부모 GameObject를 찾아줌 
                        cropsParents = farm.transform.GetChild(i).gameObject.GetComponent<Transform>();

                        // 7. 부모 아래에 자식으로 넣어줌 
                        crops.transform.parent = cropsParents;

                        // 8. 문제가 생겨서.... local 단위로 크기를 다시 맞춰줌 
                        crops.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        crops.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);

                        // 9. 자식 프리팹에 태그 지정해줌 
                        crops.gameObject.tag = "mushroom";
                    }
                    else if (Ttag == cropsConstants.carrot)
                    {
                        // 5. instantiate로 프리팹 추가 
                        crops = Instantiate(Resources.Load("Prefabs/carrot_1"), new Vector3(0, 0, 0),
                            Quaternion.identity) as GameObject;

                        // 6. 추가한 프리팹의 부모 GameObject를 찾아줌 
                        cropsParents = farm.transform.GetChild(i).gameObject.GetComponent<Transform>();

                        // 7. 부모 아래에 자식으로 넣어줌 
                        crops.transform.parent = cropsParents;

                        // 8. 문제가 생겨서.... local 단위로 크기를 다시 맞춰줌 
                        crops.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        crops.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                        // 9. 자식 프리팹에 태그 지정해줌 
                        crops.gameObject.tag = "carrot";
                    }
                    else if (Ttag == cropsConstants.onion)
                    {
                        // 5. instantiate로 프리팹 추가 
                        crops = Instantiate(Resources.Load("Prefabs/onion_1"), new Vector3(0, 0, 0),
                            Quaternion.identity) as GameObject;

                        // 6. 추가한 프리팹의 부모 GameObject를 찾아줌 
                        cropsParents = farm.transform.GetChild(i).gameObject.GetComponent<Transform>();

                        // 7. 부모 아래에 자식으로 넣어줌 
                        crops.transform.parent = cropsParents;

                        // 8. 문제가 생겨서.... local 단위로 크기를 다시 맞춰줌 
                        crops.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        crops.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                        // 9. 자식 프리팹에 태그 지정해줌 
                        crops.gameObject.tag = "onion";
                    }
                }
            }
        }

        public void showDateUpdatePanel()
        {
            dateUpdatePanel.SetActive(true);

        }

        public void dateUpdateBtnClicked()
        {
            SceneManager.LoadScene("MainScene");
        }

        // 사용자가 게임에 막 접속했을 때 동작하는 메소드 
        void accessFirstTimeUpdate()
        {
            // info에 저장된 날짜와 오늘 날짜를 비교한다. 
            // 날짜가 다르다면 데이터베이스를 업데이트하고 아니면 그냥 넘어간다. 
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string pre = DateTime.Now.ToString("yyyy-MM-dd");
            string sqlQuery = "SELECT LAST_ACCESS_DATE FROM INFO;";
            dbCommand.CommandText = sqlQuery;
            var reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                pre = reader.GetString(0);
                // 오늘 날짜랑 같다면 아무것도 하지 않는다. 아니면 db 업데이트 수행
                
            }

            reader.Close();

            if (!today.Equals(pre))
            {
                TimeSpan timeSpan = Convert.ToDateTime(today) - Convert.ToDateTime(pre);
                updateDatabase(timeSpan.Days);
            }
        }

        // 사용자가 접속 중에 날짜가 바뀌었을 때 동작하는 메소드 
        void updateDatabase(int passedDate)
        {

            // < 데이터베이스 갱신 >
            // UPDATE FARM SET GROW_LEFT_TIME-=1; -- 성장까지 남은 시간 -로 카운트함 
            // UPDATE FARM SET IS_WATERED_TODAY=1; -- 물 안 준 상태로 등록 
            // 시장 랜덤항목 구현하게 된다면 갱신내용 추가할 것 
            string sqlQuery;

            // 1. FARM 테이블에서 GROW_LEFT_TIME의 값을 -1한다. 
            // FARM - GROW_LEFT_TIME 값 UPDATE 
            sqlQuery = "UPDATE FARM SET GROW_LEFT_TIME=GROW_LEFT_TIME-" + passedDate + ";";
            simpleExecuteQuery(sqlQuery);

            // 1-1. first/second/third/grow period 값이랑 비교해서 해당되면 grow state를 +1 한다 
            sqlQuery = "SELECT GROW_LEFT_TIME, FIRST_STATE_STARTDATE, SECOND_STATE_STARTDATE, THIRD_STATE_STARTDATE, GROW_PERIOD, FARM.ID " +
                "FROM FARM, CROPS WHERE FARM.CROP_ID = CROPS.ID AND CROP_ID IS NOT NULL;";
            dbCommand.CommandText = sqlQuery;
            var reader = dbCommand.ExecuteReader();

            ArrayList sqlQueryList = new ArrayList();
            
            // 1.2. if문으로 값 비교 
            while (reader.Read())
            {
                int growLeftTime = reader.GetInt32(4) - reader.GetInt32(0);

                if (growLeftTime == reader.GetInt32(1)) // first state
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=1 WHERE ID='"+ Convert.ToString(reader.GetString(5))+"';";
                }
                else if(growLeftTime == reader.GetInt32(2)) // second state
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=2 WHERE ID='" + Convert.ToString(reader.GetString(5)) + "';";
                }
                else if (growLeftTime == reader.GetInt32(3)) // third state 
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=3 WHERE ID='" + Convert.ToString(reader.GetString(5)) + "';";
                }
                else if (growLeftTime >= reader.GetInt32(4)) // done 
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=4 WHERE ID='" + Convert.ToString(reader.GetString(5)) + "';";
                }
                sqlQueryList.Add(sqlQuery);
            }

            reader.Close();

            foreach (string query in sqlQueryList)
            {
                simpleExecuteQuery(query);
            }

            sqlQueryList.Clear();

            // 2. FARM 테이블의 LAST_WATERED_DATE가 CROPS의 WATER_PERIOD값을 넘겼다면 농작물을 시든 상태로 전환한다. 
            // 2-1. 마지막으로 물을 준 날짜, 물 주는 제한기간 값 가져옴 
            sqlQuery = "SELECT LAST_WATERED_DATE, WATER_PERIOD, FARM.ID FROM FARM, CROPS WHERE FARM.CROP_ID=CROPS.ID;";
            dbCommand.CommandText = sqlQuery;
            reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {

                // 2-2. 현재 날짜와 마지막으로 물 준 날짜의 차이 구함 
                TimeSpan timeSpan = DateTime.Now - Convert.ToDateTime(reader.GetString(0));

                int dateDiff = timeSpan.Days;

                // 2-3. 물을 줄 기간을 지났다면 grow_state 값을 -1(시든 상태)로 한다. 
                if(dateDiff > reader.GetInt32(1))
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=-1, CROP_ID=3 WHERE ID='" + Convert.ToString(reader.GetString(2)) + "';";
                    sqlQueryList.Add(sqlQuery);
                }

            }

            reader.Close();

            foreach (string query in sqlQueryList)
            {
                simpleExecuteQuery(query);
            }

            // 3. FARM 테이블에서 IS_WATERED_TODAY 값을 모두 0으로 바꾼다. 
            // INF0 - LAST_ACCESS_DATE 오늘로 
            sqlQuery = "UPDATE FARM SET IS_WATERED_TODAY=0 WHERE CROP_ID IS NOT NULL;";
            simpleExecuteQuery(sqlQuery);

            // 4. INFO 테이블에서 LAST_ACCESS_DATE 값을 오늘 날짜로 바꾼다.
            sqlQuery = "UPDATE INFO SET LAST_ACCESS_DATE=DATE('now', 'localtime');";
            simpleExecuteQuery(sqlQuery);

            // 5. 데이터베이스 변경에 따른 ui 갱신 
            // 밭에 대해서만 갱신해주면 되는건데... 아님 위에꺼 돌 때 같이 처리해주는건?? 
            // 오... 씬 이동으로 강제 갱신하기 
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }

        int simpleExecuteQuery(string sqlQuery)
        {
            int result = 0;

            dbCommand.CommandText = sqlQuery;
            dbCommand.ExecuteNonQuery();

            return result;
        }
    }
}