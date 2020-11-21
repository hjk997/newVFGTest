using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class RayScript : MonoBehaviour
    {
        Ray ray;
        RaycastHit hit;

        public GameObject shopScript;
        shopHandler shopHandler;

        bool isShopSelected = false;

        // Start is called before the first frame update
        void Start()
        {
            OBtnPublisher.Instance.onChange += DoSmt;
            shopHandler = shopScript.GetComponent<shopHandler>();
        }

        // Update is called once per frame
        void Update()
        {
            ray = new Ray(transform.position, transform.forward);

            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(ray.origin, ray.direction * 50, Color.red);

            if (Physics.Raycast(ray, out hit, 70, 1 << LayerMask.NameToLayer("grandma")))
            {
                //Debug.Log("There is something in front of the object!");
                isShopSelected = true;
            }
            else
            {
                isShopSelected = false;
            }

        }

        // O 버튼 눌렸을 때 행동함 
        private void DoSmt()
        {
            if (isShopSelected)
            {
                // Music : 시장 방문 
                // shopHandler 부르기
                shopHandler.showBuyPanel();
            }
        }

        public void exitBtnClicked()
        {
            shopHandler.closePanel();
        }
    }
}