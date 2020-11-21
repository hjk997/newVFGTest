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

        // Start is called before the first frame update
        // 게임이 시작되면 밭 모습 초기화 
        // 인벤토리 구현은 우선순위에 따라 계획 예정 
        void Start()
        {
            IDbCommand = DBConnectionSingleton.getIDbCommand();

            // 밭 이름 미리 지정해둠 
            String[] ttag = {"farm1", "farm2", "farm3", "farm4", "farm5"};
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

                    IDbCommand.CommandText = sqlQuery;
                    int Ttag = Convert.ToInt32(IDbCommand.ExecuteScalar());

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

    }
}