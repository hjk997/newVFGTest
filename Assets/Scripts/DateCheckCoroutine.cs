using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts
{
    public class DateCheckCoroutine : MonoBehaviour
    {
        String preTime, nowTime;

        void OnEnable()
        {
            //InvokeRepeating("Func1", 0, 1.0f);
            // 현재 날짜 저장 
            preTime = DateTime.Now.ToString("dd");

            // 코루틴 호출 
            StartCoroutine(checkDateTime());
        }

        IEnumerator checkDateTime()
        {
            // 1분 주기로 현재 날짜를 조회한다. 
            yield return new WaitForSeconds(60f);
            nowTime = DateTime.Now.ToString("dd");

            // 날짜 바뀜 
            if (!nowTime.Equals(preTime))
            {
                // 게임 내 시간 멈추기 
                Time.timeScale = 0;

                // 팝업창 띄워서 갱신 정보를 받도록 한다. 
                DateChanger dateChanger = new DateChanger();

                dateChanger.showDateUpdatePanel();

            }
            
            Debug.Log(" 이전시간: " + preTime + " 현재시간: " + nowTime);

            preTime = nowTime;

        }

    }
}