using UnityEngine;
using System.Collections;
using System.Data;

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
        IDbCommand dbCommand = DBConnectionSingleton.getIDbCommand();

        public void showDateUpdatePanel()
        {
            dateUpdatePanel.SetActive(true);
        }

        public void dateUpdateBtnClicked()
        {
            updateDatabase();
            dateUpdatePanel.SetActive(false);
        }

        void updateDatabase()
        {

            // 1. 데이터베이스 갱신 
            // UPDATE FARM SET GROW_LEFT_TIME-=1; -- 성장까지 남은 시간 -로 카운트함 
            // UPDATE FARM SET IS_WATERED_TODAY=0; -- 물 안 준 상태로 등록 
            // 시장 구현하게 된다면 갱신내용 추가할 것 
            string sqlQuery;

            // 사용자의 마지막 시간으로부터 접속하지 않은 시간을 계산한다. 

            // FARM - GROW_LEFT_TIME 값 UPDATE 
            sqlQuery = "UPDATE FARM SET GROW_LEFT_TIME-=1;";
            simpleExecuteQuery(sqlQuery);

            // FARM - IS_WATERED_TODAY 값 UPDATE 
            sqlQuery = "UPDATE FARM SET IS_WATERED_TODAY=0;";
            simpleExecuteQuery(sqlQuery);

            // INF0 - LAST_ACCESS_DATE 오늘로  
            sqlQuery = "UPDATE INFO SET LAST_ACCESS_DATE=?;";
            simpleExecuteQuery(sqlQuery);

            // 2. 데이터베이스 변경에 따른 ui 갱신 

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