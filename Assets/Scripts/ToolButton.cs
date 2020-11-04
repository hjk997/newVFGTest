using System;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    // 플레이어, 밭 위치, crops 위치... 
    public GameObject player;
    public GameObject sphere_obj;
    public GameObject crop_child;

    GameObject farm;
    GameObject farmChild;

    // 농작물 정보 알려주는 패널 
    public GameObject cropInfoPanel;

    public Transform sphere;

    // 가장 가까운 농작물의 위치 저장하는 변수 
    private Transform target;

    //private Transform closestEnemy = null;
    private float dist;
    private float min = 4000.0f;

    // 버튼에 생기는 도구 이름 
    public Text toolTxt;
    // 도구가 바뀔 때 상단에 뜨는 안내문 txt 
    public Text toolInfoTxt;

    // 농작물에 다가갔을 때 뜨는 info 문구 
    public Text cropsNameTxt;
    public Text cropsDateTxt;
    public Text cropsDueDateTxt;

    public GameObject sickleObject;
    public GameObject waterCanObject;

    private Animator waterCanAnim;
    private Animator sickleAnim;

    public Transform sickleTr;

    public int toolOpt = 1;

    private static string firstState = "선택 없음";
    private static string secondState = "호미";
    private static string thirdState = "낫";
    private static string fourthState = "물뿌리개";
    private string totalTag;

    private bool detectTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        OBtnPublisher.Instance.onChange += DoSmt;
        waterCanAnim = waterCanObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
        sickleAnim = sickleObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
        cropInfoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (toolOpt)
        {
            /* 선택 없음 */
            case 1:
                toolTxt.text = firstState;
                toolInfoTxt.text = " ";

                waterCanObject.SetActive(false);
                sickleObject.SetActive(false);
                sphere_obj.SetActive(false);

                break;
            /* 호미 */
            case 2:
                toolTxt.text = secondState;
                toolInfoTxt.text = "<b>빨간 동그라미가 뜰 때까지 농작물에 가까이 다가가세요.</b>";

                waterCanObject.SetActive(false);
                sickleObject.SetActive(false);

                // 일정 거리 안에 있는 농작물 중 가장 가까운 거 알려주는 함수 
                getClosestCrops();
                break;
            /* 낫 */
            case 3:
                toolTxt.text = thirdState;
                toolInfoTxt.text = "<b>빨간 동그라미가 뜰 때까지 농작물에 가까이 다가가세요.</b>";

                waterCanObject.SetActive(false);
                sickleObject.SetActive(true);
                
                getClosestCrops();
                break;

            /* 물뿌리개 */
            case 4:
                toolTxt.text = fourthState;
                toolInfoTxt.text = "<b>빨간 동그라미가 뜰 때까지 농작물에 가까이 다가가세요.</b>";
                waterCanObject.SetActive(true);
                sickleObject.SetActive(false);

                //2번째 인자 몇초가 지나고 함수를 처음 호출할것인가 3번째 인자 얼마나 자주 호출할것인가
                //바로호출하고 1초마다 반복호출
                getClosestCrops();
                break;

            default:
                toolInfoTxt.text = "";
                waterCanObject.SetActive(false);
                sickleObject.SetActive(false);
                sphere_obj.SetActive(false);
                break;
        }
        
    }

    public void ToolBtnClicked()
    {
        toolOpt++;
        
        if(toolOpt > 4)
        {
            toolOpt = 1;
        }
    }

    void getCropsUpdate()
    {
        if (target != null)
        {
            // 농작물 위에 동그라미 
            Vector3 vec = target.position;
            vec.y += 10.0f;

            sphere.position = vec;
            sphere_obj.SetActive(true);

            detectTarget = true;

            // 이때... 농작물이 있다면 농작물의 상세 정보를 띄워줌 

            // 1. 농작물이 있는지 확인하기 위해... PlayerPrefs에 일치하는 태그 값을 가져옴 
            string tag = target.transform.parent.tag;

            // 2. 태그값과 target의 순번을 조합해서 PlayerPrefs에서 자료를 찾음 
            // if문을 통해 검사함 
            if (totalTag.Equals("") || PlayerPrefs.GetString(totalTag).Equals("none"))
            {
                cropInfoPanel.SetActive(false);
            }
            else
            {
                // 농작물 정보에 대한 텍스트 값 설정 
                cropsNameTxt.text = PlayerPrefs.GetString(totalTag);
                cropsDateTxt.text = PlayerPrefs.GetString(totalTag + "date");
                cropsDueDateTxt.text = "??";

                // 패널을 화면에 띄움 
                cropInfoPanel.SetActive(true);

            }
        }
        else
        {
            sphere_obj.SetActive(false);
            detectTarget = false;
        }
    }

    void getClosestCrops()
    {
        // 가장 가까운 밭을 찾고 그 안에서만 탐색을 하는 것도 좋은 방법일 것 같긴 함...
        // 이 문제에 대해서는 다음에 고려해보기로 

        // 밭 이름 미리 지정해둠 
        string[] ttag = { "farm1", "farm2", "farm3", "farm4", "farm5" };
        string tmpTag = "";

        // 거리 측정에 필요한 값 
        float closestDistSqr = Mathf.Infinity;//infinity 실제값?
        Transform closestCrops = null;
        // min: 농작물이 4000.0f 안에는 있어야 체크할 수 있음 
        min = 4000.0f;

        // 1. 밭 순서대로 순회함 
        foreach (String t in ttag)
        {
            // 1. 이름이 같은 밭의 GameObject 가져옴 
            farm = GameObject.Find(t);

            // 2. 밭에 있는 Cylinder나 pile을 순서대로 순회함 
            for (int i = 0; i < 16; i++)
            {
                // 3. 자식에 하나씩 접근함 (Cylinder이거나 dirt_pile임) 
                farmChild = farm.transform.GetChild(i).gameObject;

                Vector3 objectPos = farmChild.transform.position;
                dist = (objectPos - player.transform.position).sqrMagnitude;

                // 5. 순회를 하면서 거리를 체크함 
                //농작물이 특정 거리 안으로 들어올때
                //if (dist < 4000.0f)
                //{
                if (dist < min)
                    {
                        closestDistSqr = dist;
                        closestCrops = farmChild.transform;
                        // 6. 임의의 태그명... 만들어줌 
                        tmpTag = t + i.ToString();
                        min = dist;
                    }
                //}
            }

        }

        totalTag = tmpTag;
        target = closestCrops;
        getCropsUpdate();
    }

    private void DoSmt()
    {
        // O 버튼을 눌렀을 때 동작하는 코드
        // detect된 밭에 대해서 심거나 없애거나 물을 주거나 함 
        if (detectTarget) {

            switch (toolOpt)
            {
                case 2: // 호미 
                    // 보고 있는 밭에 자식이 없다면 농작물 심음
                    if(target.childCount == 0)
                    {
                        crop_child = Instantiate(Resources.Load("Prefabs/Carrot_Fruit"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

                        // 7. 부모 아래에 자식으로 넣어줌 
                        crop_child.transform.parent = target;

                        // 8. 문제가 생겨서.... local 단위로 크기를 다시 맞춰줌 
                        crop_child.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        crop_child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                        // 9. 자식 프리팹에 태그 지정해줌 
                        crop_child.gameObject.tag = "carrot";
                    }

                    break;

                case 3: // 낫
                    // 보고 있는 밭에 농작물이 있고 수확이 가능하다면 농작물 수확
                    //Debug.Log(target.GetChild(0).name);
                   // crop_child = target.transform.Find(target.GetChild(0).name).gameObject;
                    sickleAnim.SetTrigger("isOBtnPushed");
                    //Destroy(crop_child);
                    break;

                case 4: // 물뿌리개 
                    // 보고 있는 밭에 농작물이 있다면 물을 줌 
                    Debug.Log("pushed");
                    waterCanAnim.SetTrigger("isOBtnPushed");
                    break;

                default: // 아마... 호출될 일 없을것임 
                    break;
            }
        } 
    }
}
