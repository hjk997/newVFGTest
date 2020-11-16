using UnityEngine;
using System.Collections;
using System.Data;
using System;

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

        private void Awake()
        {
            dbCommand = DBConnectionSingleton.getIDbCommand();
        }

        public void showDateUpdatePanel()
        {
            dateUpdatePanel.SetActive(true);
        }

        public void dateUpdateBtnClicked()
        {
            //updateDatabase();
            dateUpdatePanel.SetActive(false);
        }

        // 사용자가 게임에 막 접속했을 때 동작하는 메소드 


        // 사용자가 접속 중에 날짜가 바뀌었을 때 동작하는 메소드 
        void updateDatabase()
        {

            // < 데이터베이스 갱신 >
            // UPDATE FARM SET GROW_LEFT_TIME-=1; -- 성장까지 남은 시간 -로 카운트함 
            // UPDATE FARM SET IS_WATERED_TODAY=0; -- 물 안 준 상태로 등록 
            // 시장 구현하게 된다면 갱신내용 추가할 것 
            string sqlQuery;

            // 1. FARM 테이블에서 GROW_LEFT_TIME의 값을 -1한다. 
            // FARM - GROW_LEFT_TIME 값 UPDATE 
            sqlQuery = "UPDATE FARM SET GROW_LEFT_TIME-=1;";
            simpleExecuteQuery(sqlQuery);

            // 1-1. first/second/third/grow period 값이랑 비교해서 해당되면 grow state를 +1 한다 
            sqlQuery = "SELECT GROW_LEFT_TIME, FIRST_STATE_STARTDATE, SECOND_STATE_STARTDATE, THIRD_STATE_STARTDATE, GROW_PERIOD, FARM.ID " +
                "FROM FARM, CROPS WHERE FARM.CROP_ID = CROPS.ID AND CROP_ID IS NOT NULL;";
            dbCommand.CommandText = sqlQuery;
            var reader = dbCommand.ExecuteReader();

            // 1.2. if문으로 값 비교 
            while (reader.Read())
            {
                int growLeftTime = reader.GetInt32(4) - reader.GetInt32(0);

                if (growLeftTime == reader.GetInt32(1)) // first state
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=1 WHERE ID='"+ Convert.ToString(reader.GetInt32(5))+"';";
                    simpleExecuteQuery(sqlQuery);
                }
                else if(growLeftTime == reader.GetInt32(2)) // second state
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=2 WHERE ID='" + Convert.ToString(reader.GetInt32(5)) + "';";
                    simpleExecuteQuery(sqlQuery);
                }
                else if (growLeftTime == reader.GetInt32(3)) // third state 
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=3 WHERE ID='" + Convert.ToString(reader.GetInt32(5)) + "';";
                    simpleExecuteQuery(sqlQuery);
                }
                else if (growLeftTime >= reader.GetInt32(4)) // done 
                {
                    sqlQuery = "UPDATE FARM SET GROW_STATE=4 WHERE ID='" + Convert.ToString(reader.GetInt32(5)) + "';";
                    simpleExecuteQuery(sqlQuery);
                }
            }


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
                    sqlQuery = "UPDATE FARM SET GROW_STATE=-1 WHERE ID='" + Convert.ToString(reader.GetString(2)) + "';";
                    simpleExecuteQuery(sqlQuery);
                }

            }

            // 3. FARM 테이블에서 IS_WATERED_TODAY 값을 모두 0으로 바꾼다. 
            // INF0 - LAST_ACCESS_DATE 오늘로 
            sqlQuery = "UPDATE FARM SET IS_WATERED_TODAY=0;";
            simpleExecuteQuery(sqlQuery);

            // 4. INFO 테이블에서 LAST_ACCESS_DATE 값을 오늘 날짜로 바꾼다.
            sqlQuery = "UPDATE INFO SET LAST_ACCESS_DATE=DATE('now', 'localtime');";
            simpleExecuteQuery(sqlQuery);



            // 5. 데이터베이스 변경에 따른 ui 갱신 
            // 밭에 대해서만 갱신해주면 되는건데... 아님 위에꺼 돌 때 같이 처리해주는건?? 




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