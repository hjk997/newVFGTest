using System;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using System.Collections.Generic;

namespace Assets.Scripts
{
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

        // 농작물 위에 뜨는 동그라미 
        public Transform sphere;

        // 가장 가까운 농작물의 위치 저장하는 변수 
        private Transform target;

        //private Transform closestEnemy = null;
        private float dist;
        private float min = 4000.0f;

        // 버튼에 생기는 도구 이미지 
        Sprite[] toolImage;

        // 도구가 바뀔 때 상단에 뜨는 안내문 txt 
        public Text toolInfoTxt;

        public GameObject sickleObject;
        public GameObject waterCanObject;
        public GameObject changeSeedBtn;
        public GameObject toolBtn;

        private AudioClip harvestBgm;

        private Animator waterCanAnim;
        private Animator sickleAnim;

        public Transform sickleTr;

        private int toolOpt = 1;
        private int seedOpt = 0;

        private static string[] state = { "btnChange", "seed", "sickleBtn", "wateringCanBtn"};
        private string totalTag = "farm1345";

        private bool detectTarget = false;

        private Dictionary<int, SeedDetails> seedList;
        string cropNameLocal = "";

        // database connection 객체 
        IDbCommand dbCommand;

        // Start is called before the first frame update
        void Start()
        {
            dbCommand = DBConnectionSingleton.getIDbCommand();
            OBtnPublisher.Instance.onChange += DoSmt;
            waterCanAnim = waterCanObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
            sickleAnim = sickleObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
            cropInfoPanel.SetActive(false);

            // 이미지 초기화
            toolImage = new Sprite[4];

            for(int i = 0; i < state.Length; i++)
            {
                toolImage[i] = Resources.Load<Sprite>("Image/" + state[i]);
            }
            
            // 인벤토리에 있는 씨앗 목록을 불러온다. 
            string sqlQuery = "SELECT ITEM_NAME, LEFT_NUM FROM INVENTORY WHERE LEFT_NUM!=0 AND ITEM_TYPE=5;";
            dbCommand.CommandText = sqlQuery;
            var reader = dbCommand.ExecuteReader();
            int tmp = 0;

            changeSeedBtn.GetComponent<SeedDetails>().seedList = new Dictionary<int, SeedDetails>();

            while (reader.Read())
            {
                SeedDetails seedDetails = this.gameObject.AddComponent<SeedDetails>();

                seedDetails.SeedName = reader.GetString(0);
                seedDetails.LeftNum = reader.GetInt32(1);

                // 씨앗 목록 만들기 
                changeSeedBtn.GetComponent<SeedDetails>().seedList.Add(tmp, seedDetails);

                tmp += 1;
            }

            reader.Close();
            showSeedChangeBtnImage();

            harvestBgm = Resources.Load("Sound/harvest", typeof(AudioClip)) as AudioClip;
            AudioManager.Inst.AddToPlaylist(harvestBgm);
        }

        // Update is called once per frame
        void Update()
        {
            switch (toolOpt)
            {
                /* 선택 없음 */
                case 1:
                    toolInfoTxt.text = " ";

                    cropInfoPanel.SetActive(false);
                    waterCanObject.SetActive(false);
                    sickleObject.SetActive(false);
                    sphere_obj.SetActive(false);
                    changeSeedBtn.SetActive(false);

                    break;
                /* 호미 */
                case 2:
                    //toolTxt.text = secondState;
                    toolInfoTxt.text = "<b>빨간 동그라미가 뜰 때까지 농작물에 가까이 다가가세요.</b>";

                    waterCanObject.SetActive(false);
                    sickleObject.SetActive(false);

                    // 씨앗 바꾸는 버튼 표시하기 
                    changeSeedBtn.SetActive(true);

                    // 일정 거리 안에 있는 농작물 중 가장 가까운 거 알려주는 함수 
                    getClosestCrops();
                    break;
                /* 낫 */
                case 3:
                    //toolTxt.text = thirdState;
                    toolInfoTxt.text = "<b>빨간 동그라미가 뜰 때까지 농작물에 가까이 다가가세요.</b>";

                    changeSeedBtn.SetActive(false);
                    waterCanObject.SetActive(false);
                    sickleObject.SetActive(true);

                    getClosestCrops();
                    break;

                /* 물뿌리개 */
                case 4:
                    //toolTxt.text = fourthState;
                    toolInfoTxt.text = "<b>빨간 동그라미가 뜰 때까지 농작물에 가까이 다가가세요.</b>";
                    waterCanObject.SetActive(true);
                    sickleObject.SetActive(false);

                    //2번째 인자 몇초가 지나고 함수를 처음 호출할것인가 3번째 인자 얼마나 자주 호출할것인가
                    //바로호출하고 1초마다 반복호출
                    getClosestCrops();
                    break;

                default:
                    toolInfoTxt.text = "";
                    cropInfoPanel.SetActive(false);
                    waterCanObject.SetActive(false);
                    sickleObject.SetActive(false);
                    sphere_obj.SetActive(false);
                    break;
            }
        }

        public void ToolBtnClicked()
        {
            toolOpt++;

            if (toolOpt > 4)
            {
                toolOpt = 1;
            }
            toolBtn.GetComponent<Image>().sprite = toolImage[toolOpt - 1];
        }

        public void showSeedChangeBtnImage()
        {

            if (seedList == null)
            {
                seedList = changeSeedBtn.GetComponent<SeedDetails>().seedList;
            }


            // 현재 대상 씨앗을 가져온다. 
            SeedDetails seed = seedList[seedOpt];

            // 이미지 출력  
            Image image = changeSeedBtn.transform.GetChild(0).GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>("Image/" + seed.SeedName);

            cropNameLocal = seed.SeedName;

            seedOpt++;

            if (seedList.Count <= seedOpt)
            {
                seedOpt = 0;
            }


        }

        // 사용자로부터 가장 가까운 농작물 위치를 찾는다. 
        void getClosestCrops()
        {
            // 가장 가까운 밭을 찾고 그 안에서만 탐색을 하는 것도 좋은 방법일 것 같긴 함...
            // 이 문제에 대해서는 다음에 고려해보기로 

            // 밭 이름 미리 지정해둠 
            string[] ttag = { "farm1", "farm2", "farm3", "farm4", "farm5" };
            string tmpTag = "";

            // 거리 측정에 필요한 값 
            float closestDistSqr = Mathf.Infinity; //infinity 실제값?
            Transform closestCrops = null;
            // min: 농작물이 6000.0f 안에는 있어야 체크할 수 있음 
            min = 6000.0f;

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

            // 태그가 달라졌을 때만 정보창 업데이트 
            if (!totalTag.Equals(tmpTag))
            {
                totalTag = tmpTag;
                target = closestCrops;
                getCropsUpdate();
            }
        }

        // 사용자와 가장 가까운 밭 위치 위에 동그라미 띄우고 
        // 해당 농작물에 대한 정보를 패널 형태로 출력한다. 
        void getCropsUpdate()
        {
            // cropsInfoPanel Text 순서: name> description> plantdate> nextdate> totaldate> iswatered
            Text[] cropsInfoText = cropInfoPanel.GetComponentsInChildren<Text>();
            Image[] cropsInfoImage = cropInfoPanel.GetComponentsInChildren<Image>();

            if (target != null)
            {
                // 농작물 위에 동그라미 
                Vector3 vec = target.position;
                vec.y += 10.0f;

                sphere.position = vec;
                sphere_obj.SetActive(true);

                detectTarget = true;

                // 이때... 농작물이 있다면 농작물의 상세 정보를 띄워줌 

                // 1. 농작물이 있는지 확인하기 위해... 밭 위치와 일치하는 태그 값을 가져옴 
                // 지금은 전역변수에 있는 태그값을 쓰고있는데... 나중에 한 쪽으로 수정하기 
                string tag = target.transform.parent.tag;

                // 2. 태그값과 target의 순번을 조합해서 database에서 자료를 찾음  
                string sqlQuery = "SELECT IFNULL(NAME, 'none'), DESCRIPTION, PLANT_DATE, GROW_LEFT_TIME, LAST_WATERED_DATE, IS_WATERED_TODAY" +
                    " FROM FARM LEFT OUTER JOIN CROPS ON FARM.CROP_ID = CROPS.ID WHERE FARM.ID = '" + totalTag + "';";
                dbCommand.CommandText = sqlQuery;
                var reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {

                    // if문을 통해 검사함 > 농작물이 없다면 정보창 표시 안 함 
                    if (reader.GetString(0).Equals("none"))
                    {
                        cropInfoPanel.SetActive(false);
                        break;
                    }
                    else
                    {

                        cropsInfoImage[1].sprite = Resources.Load<Sprite>("Image/" + reader.GetString(0));
                        cropsInfoText[0].text = String.Format("이름: {0}", reader.GetString(0));
                        cropsInfoText[1].text = reader.GetString(1);
                        cropsInfoText[2].text = String.Format("심은 날짜: {0}", reader.GetString(2));
                        cropsInfoText[3].text = String.Format("성장까지 남은 일 수: {0}", reader.GetInt32(3));
                        cropsInfoText[4].text = String.Format("마지막으로 물을 준 날짜: {0}", reader.GetString(4));

                        if (reader.GetString(0).Equals("mushroom"))
                        {
                            cropsInfoImage[2].sprite = Resources.Load<Sprite>("Image/nowater");
                            cropsInfoText[5].text = "돌이킬 수 없어요.";
                        }
                        else if (reader.GetInt32(5) == 0) // 물을 안 준 상태
                        {
                            cropsInfoImage[2].sprite = Resources.Load<Sprite>("Image/nowater");
                            cropsInfoText[5].text = "아직 물을 안 줬어요!";
                        }
                        else if (reader.GetInt32(5) == 1) // 물을 준 상태
                        {
                            cropsInfoImage[2].sprite = Resources.Load<Sprite>("Image/water");
                            cropsInfoText[5].text = "이미 물을 줬어요!";
                        }

                        // 패널을 화면에 띄움 
                        cropInfoPanel.SetActive(true);
                    }
                }

                reader.Close();
            }
            else
            {
                sphere_obj.SetActive(false);
                cropInfoPanel.SetActive(false);
                detectTarget = false;
            }
        }

        private void DoSmt()
        {
            // O 버튼을 눌렀을 때 동작하는 코드
            // detect된 밭에 대해서 심거나 없애거나 물을 주거나 함 
            if (detectTarget)
            {
                switch (toolOpt)
                {
                    case 2: // 씨앗 심기 
                        // 보고 있는 밭에 자식이 없다면 농작물 심음
                        if (target.childCount == 0)
                        {

                            // Music : 도구 사용?
                            AudioManager.Inst.PlayOneShot(AudioManager.Inst.LoadClip("Sound/tool"));
                            // 무슨 씨앗 심는건지 가져오기 

                            crop_child = Instantiate(Resources.Load("Prefabs/" + cropNameLocal  + "_1"), new Vector3(0, 0, 0),
                                Quaternion.identity) as GameObject;

                            // 부모 아래에 자식으로 넣어줌 
                            crop_child.transform.parent = target;

                            // 문제가 생겨서.... local 단위로 크기를 다시 맞춰줌 
                            crop_child.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                            crop_child.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                            // 자식 프리팹에 태그 지정해줌 
                            crop_child.gameObject.name = totalTag;
                            crop_child.gameObject.tag = cropNameLocal;

                            // 쿼리문 작성 후 execute
                            string sqlQuery = "UPDATE FARM SET CROP_ID = (SELECT ID FROM CROPS WHERE NAME='" +
                                   crop_child.gameObject.tag + "')," +
                                   "PLANT_DATE='" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                                "LAST_WATERED_DATE='" + DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                                "GROW_LEFT_TIME=(SELECT GROW_PERIOD FROM CROPS WHERE NAME='" +
                                   crop_child.gameObject.tag + "')," +
                                "GROW_STATE=0," +
                                "IS_WATERED_TODAY=0 WHERE ID='" + totalTag + "';";

                            dbCommand.CommandText = sqlQuery;
                            dbCommand.ExecuteNonQuery();

                            // 씨앗 개수 하나 빼줌 
                            sqlQuery = "UPDATE INVENTORY SET LEFT_NUM=LEFT_NUM-1 WHERE ITEM_NAME='"+cropNameLocal+"' AND ITEM_TYPE=5;";
                            dbCommand.CommandText = sqlQuery;
                            dbCommand.ExecuteNonQuery();

                            getCropsUpdate();
                        }

                        break;

                    case 3: // 낫

                        // Music : 도구 사용 & 농작물 사용 
                        //AudioManager.Inst.PlaySFX(harvestBgm, new Vector2(0f, 0f), 1);

                        AudioManager.Inst.PlayOneShot(AudioManager.Inst.LoadClip("Sound/tool"));
                        AudioManager.Inst.PlayOneShot(AudioManager.Inst.LoadClip("Sound/harvest"));

                        // 보고 있는 밭에 농작물이 있고 수확이 가능하다면 농작물 수확
                        //Debug.Log(target.GetChild(0).name);
                        if (target.childCount == 1)
                        {
                            // 먼저 인벤토리에 농작물 저장 
                            crop_child = target.transform.Find(target.GetChild(0).name).gameObject;
                            string tagNameSickle = target.GetChild(0).tag;

                            // TODO: 성장이 덜 되었으면 추가할 수 X 



                            // 가져온 태그명으로 농작물 이름 검색해서 inventory에 추가함 
                            string sqlQuerySickle = "UPDATE INVENTORY SET LEFT_NUM = LEFT_NUM + 1 WHERE ITEM_NAME = '" + tagNameSickle + "' AND iTEM_TYPE = 1;";
                            dbCommand.CommandText = sqlQuerySickle;
                            dbCommand.ExecuteNonQuery();

                            // 밭에서 농작물 정보 삭제함 
                            sqlQuerySickle = "UPDATE FARM SET CROP_ID = null," +
                                "PLANT_DATE=null," +
                                "LAST_WATERED_DATE=null," +
                                "GROW_LEFT_TIME=null," +
                                "GROW_STATE=null," +
                                "IS_WATERED_TODAY=null WHERE ID='" + totalTag + "';";

                            dbCommand.CommandText = sqlQuerySickle;
                            dbCommand.ExecuteNonQuery();

                            sickleAnim.SetTrigger("isOBtnPushed");
                            Destroy(crop_child);

                            getCropsUpdate();
                        }

                        break;

                    case 4: // 물뿌리개 

                        // Music : 도구 사용
                        AudioManager.Inst.PlayOneShot(AudioManager.Inst.LoadClip("Sound/tool"));
                        // 보고 있는 밭에 농작물이 있다면 물을 줌 
                        if (target.childCount == 1)
                        {
                            // 이름 값 가져오기 
                            string tagNameWaterCan = target.GetChild(0).name;


                            // 해당 농작물에 대한 IS_WATERED_TODAY 값을 1로 하고 오늘 날짜를 LAST_WATERED_DATE 값으로 한다. 
                            string sqlQuery = "UPDATE FARM SET IS_WATERED_TODAY=1, " +
                                "LAST_WATERED_DATE='" + DateTime.Now.ToString("yyyy-MM-dd") +
                                "' WHERE ID='" + totalTag + "' AND IS_WATERED_TODAY=0;";

                            dbCommand.CommandText = sqlQuery;
                            dbCommand.ExecuteNonQuery();

                            // 애니메이션
                            waterCanAnim.SetTrigger("isOBtnPushed");

                            getCropsUpdate();
                        }
                        break;

                    default: // 아마... 호출될 일 없을것임 
                        break;
                }
            }
        }
    }
}