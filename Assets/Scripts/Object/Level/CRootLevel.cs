using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CRootLevel : MonoBehaviour
{
    public static CRootLevel Inst { get; private set; }

    [SerializeField]
    private string m_rootName = null;
    public string RootName { get { return m_rootName; } set { if (!string.IsNullOrEmpty(m_rootName)) return; m_rootName = value; } }

    [SerializeField]
    private AudioSource m_bgmSource = null;
    public AudioSource BgmSource { get { return m_bgmSource; } }

    [SerializeField]
    private int m_nCurSubIdx = 1;
    public int CurSubIdx
    {
        get
        {
            return m_nCurSubIdx;
        }
        set
        {
            if (value < 1 && value >= m_Sublevels.Count)
                value = m_Sublevels.Count -1;
            m_nCurSubIdx = value;
        }
    }

    private CSubLevel m_curSubLevel = null;
    public CSubLevel CurrentSubLevel { get { return m_curSubLevel; } }

    private int m_nStartIdx = 1;
    public int StartIdx { get { return m_nStartIdx; } }

    private CSubLevel m_startLevel = null;
    public CSubLevel StartLevel { get { return m_startLevel; } }

    private Dictionary<int,CSubLevel> m_Sublevels = new Dictionary<int,CSubLevel>();
    public Dictionary<int,CSubLevel> SubLevels { get { return m_Sublevels; } }

    void Awake()
    {
        if(Inst!=null)
        {
            Destroy(this.gameObject);
            return;
        }

        Inst = this;
    }

    void Start()
    {
        string firstScene = m_rootName + 1.ToString();
        if(!SceneManager.GetSceneByName(firstScene).isLoaded)
        {
            SceneManager.LoadScene(firstScene, LoadSceneMode.Additive);
        }
    }

    public void LoadSubLevel(int nSubIdx, CConnectPoint point = null)
    {
        string subSceneName = m_rootName + nSubIdx;

        Scene? scene = SceneManager.GetSceneByName(subSceneName);

        if (scene==null)
        {
            Debug.LogError(subSceneName + "is not contained all Levels");
            return;
        }

        if(!SceneManager.GetSceneByName(subSceneName).isLoaded)
        {
            StartCoroutine(LoadSubLevelCoroutine(subSceneName, point));
        }
    }

    IEnumerator LoadSubLevelCoroutine(string sceneName, CConnectPoint point = null)
    {
        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        if (operation == null)
            yield break;

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            yield return null;

            if(operation.progress>=0.9f)
            {
                operation.allowSceneActivation = true;
                if(point!=null)
                {
                    point.ColliderModeChange();
                }
            }
        }

        yield return null;
    }

    public void SetCurrentLevel(CSubLevel subLevel)
    {
        subLevel.tag = "Processed";

        m_nCurSubIdx = subLevel.CurIdx;
        m_curSubLevel = subLevel;
    }

    public void ComeOutCurSubLevel(CSubLevel subLevel)
    {
        if (subLevel != m_curSubLevel)
            return;

        subLevel.tag = "SubLevel";

        m_nCurSubIdx = 0;
        m_curSubLevel = null;
    }

    

    public void SetStartLevel(CSubLevel subLevel)
    {
        if (!subLevel.IsLoaded)
            return;

        m_startLevel = subLevel;
    }
}
