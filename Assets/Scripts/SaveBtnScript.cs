using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /**
 * PlayerPrefs를 사용한 저장 기능 구현.
 * 추후 SQLlite로 바꿀 의향 있음.. 
 **/
    public class SaveBtnScript : MonoBehaviour
    {
        GameObject farm;
        GameObject farmChild;
        GameObject crops;

        Transform cropsParents;
        Transform prefab;

        IDbCommand IDbCommand;

        String sqlQuery;

        public Text SaveLogText;


    }
}