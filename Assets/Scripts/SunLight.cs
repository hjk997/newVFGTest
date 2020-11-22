using System;
using UnityEngine;

namespace Assets.Scripts
{
    /**
 * 밤낮 구현 소스( 리얼타임에 맞게 수정 필요) 
 **/
    public class SunLight : MonoBehaviour
    {
        public GameObject audioScript;
        AudioManager audioManager;
        float rot = 0.0004f;

        // Start is called before the first frame update
        void Start()
        {
            // 사용자의 현재 시간 알아온 뒤 그 시간에 맞춰서 
            // 현재 태양 위치를 조절해줘야 함 

            // 1. 현재 시간 값 얻어오기 
            int hour = Int32.Parse(DateTime.Now.ToString("HH"));
            AudioClip sunBgm = Resources.Load("Sound/day", typeof(AudioClip)) as AudioClip;
            AudioClip nightBgm = Resources.Load("Sound/night", typeof(AudioClip)) as AudioClip;

            //audioManager = audioScript.GetComponent<AudioManager>();

            //audioManager.AddToPlaylist(sunBgm);
            //audioManager.AddToPlaylist(nightBgm);

            AudioManager.Inst.AddToPlaylist(sunBgm);
            AudioManager.Inst.AddToPlaylist(nightBgm);


            // 2. 현재 시 값을 바탕으로 현재 태양의 위치 획득하기 
            if (5 < hour)
            {
                // Music: 낮
                AudioManager.Inst.PlayBGM(sunBgm, MusicTransition.Swift, 0, 1.0f);
                rot = (hour - 6) * 15;
            }
            else if (hour < 6)
            {
                // Music: 밤 
                AudioManager.Inst.PlayBGM(nightBgm, MusicTransition.Swift, 0, 1.0f);
                rot = (hour + 18) * 15;
            }

            transform.Rotate(new Vector3(rot, 0f, 0f), Space.World);
            transform.Rotate(new Vector3(285f, 0f, 0f), Space.World);
            Debug.Log("sun: " + rot.ToString());
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(0.0004f, 0f, 0f), Space.World);
        }
    }
}