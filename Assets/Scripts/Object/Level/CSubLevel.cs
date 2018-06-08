using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class CSubLevel : MonoBehaviour
{
    [SerializeField]
    private int m_nCurIdx = 1;
    public int CurIdx { get { return m_nCurIdx; } }

    [SerializeField]
    private List<CConnectPoint> m_connectLinks = new List<CConnectPoint>();
    public List<CConnectPoint> ConnectLinks { get { return m_connectLinks; } }

    [SerializeField]
    private List<CSpawnPoint> m_spawnPoints = new List<CSpawnPoint>(); 
    public List<CSpawnPoint> SpwanPoints { get { return m_spawnPoints; } }

    private bool m_isLoaded = false;
    public bool IsLoaded { get { return m_isLoaded; } }

    void Start()
    {
        StartCoroutine(LoadNearLevels());
    }

    IEnumerator OnStartLevel(bool isNearLoad = false,Dictionary<UnitType,int> spawnList = null)
    {
        if (spawnList!=null&&spawnList.Count>0)
        {
            if (m_spawnPoints.Count > 0)
            {
                int nCount = 0;
                int nMaxCount = m_spawnPoints.Count;

                foreach(KeyValuePair<UnitType,int> pair in spawnList)
                {
                    m_spawnPoints[nCount].Initialized(this, pair.Key, pair.Value);

                    if (nCount >= nMaxCount)
                        break;
                    yield return null;
                }
            }
        }
        else
        {
            if (m_spawnPoints.Count > 0)
            {
                for (int i = 0; i < m_spawnPoints.Count; i++)
                {
                    m_spawnPoints[i].Initialized(this);
                    yield return null;
                }
            }
        }
        

        m_isLoaded = true;

        while(CRootLevel.Inst == null)
        {
            yield return null;
        }

        CRootLevel.Inst.SubLevels.Add(m_nCurIdx,this);

        if (m_nCurIdx==CRootLevel.Inst.StartIdx)
        {
            CRootLevel.Inst.SetStartLevel(this);
        }

        yield return null;
        
    }

    IEnumerator LoadNearLevels()
    {
        yield return null;

        for (int i = 0; i < m_connectLinks.Count; i++)
        {
            yield return null;
            m_connectLinks[i].LoadNextLevel();
        }

        StartCoroutine(OnStartLevel());
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("In");
            CRootLevel.Inst.SetCurrentLevel(this);
        }
        else if (other.tag == "Boss")
        {
            //TODO: 보스의 현재 위치 레벨 전달
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Out");
            CRootLevel.Inst.ComeOutCurSubLevel(this);
        }
        else if (other.tag == "Boss")
        {
            //TODO: 보스가 현재 레벨에서 벗어낫음을 전달
        }
    }
}
