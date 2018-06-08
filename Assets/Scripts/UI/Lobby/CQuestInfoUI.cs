using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class CQuestInfoUI : MonoBehaviour
{

    [SerializeField]
    private GameObject m_questItemPrefab = null;

    [SerializeField]
    private Transform m_questItemParant = null;

    private List<CQuestListItem> m_questItems = new List<CQuestListItem>();

    CDataManager m_dataMgr = null;

    private Data.QuestInfo? m_selectedQuest = null;
    public Data.QuestInfo? SelectedQuest { get { return m_selectedQuest; } }

    private void Start()
    {
        m_dataMgr = CDataManager.Inst;

        if (m_dataMgr == null)
        {
            Debug.Log("Datamanager is missing");
            return;
        }

        if(m_questItemPrefab==null)
        {
            m_questItemPrefab = Resources.Load<GameObject>("Prefabs/UI/QuestContent");

            if(m_questItemPrefab==null)
            {
                Debug.LogError("QuestItem Prefab is missing");
            }
        }

        CreateQuestItems();
    }

    void CreateQuestItems()
    {
        Data.QuestContainer questContainer = m_dataMgr.QuestContainer;

        for(int i=0;i<questContainer.QuestCount;i++)
        {
            GameObject questItemGO = Instantiate<GameObject>(m_questItemPrefab,m_questItemParant);
            CQuestListItem questItem = questItemGO.GetComponent<CQuestListItem>();

            if (questItem == null)
                continue;

            questItem.Init(i);
        }
    }

    void ClearQuestItem()
    {
        m_questItems.Clear();
    }

    public void QuestSet(Data.QuestInfo questInfo)
    {
        m_selectedQuest = questInfo;
    }

    public void QuestReset()
    {
        m_selectedQuest = null;
    }
}
