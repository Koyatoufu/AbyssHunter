using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class CConnectPoint : MonoBehaviour
{
    [SerializeField]
    private int m_nNearIdx = 0;
    public int NearLevelIdx { get { return m_nNearIdx; } }

    [SerializeField]
    private CSubLevel m_rootLevel = null;
    public CSubLevel RootLevel { get { return m_rootLevel; } }

    [SerializeField]
    private bool m_isLoadLevel = true;

    private Collider m_collider;

    void Awake()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = false;
    }

    void OnTriggerEnter(Collider other)
    {
        CUnit unit = other.GetComponent<CUnit>();
        if (unit!=null)
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        if(m_nNearIdx<1)
        {
            Debug.LogError("nearLevel is not settings");
            return;
        }

        CRootLevel rootLevel = CRootLevel.Inst;
        if(rootLevel!=null)
        {
            if(m_isLoadLevel)
            {
                rootLevel.LoadSubLevel(m_nNearIdx, this);
            }
            else
            {
                ColliderModeChange();
            }
        }
    }

    public void ColliderModeChange()
    {
        if(m_collider!=null)
        {
            m_collider.isTrigger = true;
            if(m_collider.isTrigger)
            {
                m_collider.enabled = false;
            }
        }
    }
}
