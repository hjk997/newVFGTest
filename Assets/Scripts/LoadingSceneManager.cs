using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * 로딩 Scene을 불러올 수 있는 클래스입니다...
 */
public class LoadingSceneManager : MonoBehaviour

{
    public static string nextScene = "SampleScene"; // 다음으로 향할 Scene명입니다...
    public const int loadingTimeSecond = 3; // 로딩 최종 시간입니다...
    public const float loadingPerSecond = 1f;
    public static int count = 0;
    private AsyncOperation op;
    [SerializeField] Image progressBar; // 진행률 막대입니다..


    private void Start()
    {
        op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        count = 0;
        InvokeRepeating("LoadScene", loadingPerSecond, loadingTimeSecond );
    }

    /**
     * 정적 함수로 LoadingSceneManager.LoadScene("다음 Scene명");을 넣으시면 됩니다...
     */
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }


    void LoadScene()
    {
        count += 1;
        progressBar.fillAmount = count / (float) loadingTimeSecond ;
        if (count == loadingTimeSecond)
        {
            op.allowSceneActivation = true;
        }
    }
}