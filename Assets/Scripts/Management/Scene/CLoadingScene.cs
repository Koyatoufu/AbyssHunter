using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CLoadingScene : MonoBehaviour
{
    static string nextSceneName = null;

    private float m_fTime = 0f;

    public void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadingScene(string sceneName)
    {
        nextSceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        //if (nextSceneName == null)
        //    nextSceneName = "Title";

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);

        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            yield return null;

            m_fTime += Time.deltaTime;

            if (operation.progress>=0.9f)
            {
                operation.allowSceneActivation = true;
                yield return null;
            }
        }
    }
	
}
