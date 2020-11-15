using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class OBtnPublisher : MonoBehaviour
    {

        public delegate void ChangeState();
        public event ChangeState onChange;

        private static OBtnPublisher _instance;
        public static OBtnPublisher Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType(typeof(OBtnPublisher)) as OBtnPublisher;
                }
                return _instance;
            }
        }

        // Update is called once per frame
        public void PushOBtn()
        {
            if(onChange!= null)
            {
                onChange();
            }
        }
    }
}