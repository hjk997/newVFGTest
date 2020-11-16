using UnityEngine;
using UnityEditor;
using System.Data;
using System;

namespace Assets.Scripts
{
    public class ShopController : MonoBehaviour
    {
        IDbCommand dbCommand;

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
            int stuffCost;

            // 1. coin이 모자라면 구매 불가능 팝업 창을 띄운다. 
            string sqlQuery = "SELECT LEFT_NUM FROM INVENTORY WHERE ITEM_NAME='coin'";
            dbCommand.CommandText = sqlQuery;
            int leftCoin = (int)dbCommand.ExecuteScalar();

            sqlQuery = "SELECT SELL_COST FROM MARKET WHERE STUFF_NAME='" + code + "'";
            dbCommand.CommandText = sqlQuery;
            stuffCost = (int)dbCommand.ExecuteScalar();

            if(leftCoin < stuffCost)
            {
                showFailedPopup();
                return -1;
            }
            // 2. coin이 충분하면 구매 처리 후 구매 완료 팝업창을 띄운다. 

            // 2-1. inventory의 coin 값 감소
            sqlQuery = "UPDATE INVENTORY SET LEFT_NUM-=" + Convert.ToString(stuffCost) + " WHERE ITEM_NAME='coin'";
            dbCommand.CommandText = sqlQuery;
            dbCommand.ExecuteNonQuery();

            // 2-2. inventory의 물건 재고 +1
            // 물건 이름이랑 TYPE 같이 찾아서 바꾸는 걸로 수정하기 
            sqlQuery = "UPDATE INVENTORY SET LEFT_NUM+=1 WHERE STUFF_NAME='" + code + "'";
            dbCommand.CommandText = sqlQuery;
            dbCommand.ExecuteNonQuery();

            showSuccessedPopup();
            return result;

        }

        private void showFailedPopup()
        {

        }

        private void showSuccessedPopup()
        {

        }
    }

}