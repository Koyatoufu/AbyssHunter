using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CQuestManager : MonoBehaviour
{
    public static CQuestManager Inst { get; private set; }

    [SerializeField]
    private string m_questName = null;
    public string QuestName { get { return m_questName; } }

    private Data.QuestInfo? m_selectQuest = null;
    public Data.QuestInfo? SelectQuest { get { return m_selectQuest; } }

    private CUnitData m_questTarget = null;
    public CUnitData QuestTarget { get { return m_questTarget; } }

    private bool m_isQuestStart = false;
    public bool IsQuestStart { get { return m_isQuestStart; } }

    private float m_fPastTime = 0f;
    public float PastTime { get { return m_fPastTime; } }

    public float LimitTime { get; private set; }

    private int m_checkCount = 0;

    private const int DeathCountBase = 3;
    private int m_deathLimit = DeathCountBase;
    private int m_deathCount = 0;

    public int RemainCount { get { return m_deathLimit - m_deathCount; } }

    private int m_playerCount = 0;

    void Awake()
    {
        if(Inst!=null)
        {
            Destroy(this.gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void QuestSet(Data.QuestInfo questInfo)
    {
        if(m_selectQuest==null)
            m_selectQuest = questInfo;

        m_questName = questInfo.questName;

        m_questTarget = CDataManager.Inst.UnitContainer.GetUnitData(questInfo.mainTargetType, questInfo.mainTargetIdx);
    }

    public void QuestStart()
    {
        StartCoroutine(QuestStartCoroutine());
    }

    IEnumerator QuestStartCoroutine()
    {
        CRootLevel rootLevel = null;

        while (rootLevel == null)
        {
            rootLevel = CRootLevel.Inst;
            yield return null;
        }

        CSubLevel subLevel = null;

        if (m_questTarget == null || m_questTarget.StayLevel.Count<1)
        {
            yield break;
        }

        int nTargetStart = m_questTarget.StayLevel[0];

        while (subLevel == null)
        {
            yield return null;

            if (rootLevel.SubLevels.Count <= nTargetStart)
                continue;

            subLevel = rootLevel.SubLevels[nTargetStart];
        }

        yield return null;
    }

    public void LateUpdate()
    {
        if (!m_isQuestStart)
            return;

        if(m_fPastTime%5<=0.2f)
        {
            CStageUIManager.Inst.MessageUI.ShowSubMessage("Time pasted: " + m_fPastTime, 1f);
        }

        if(m_fPastTime>=LimitTime)
        {
            QuestFailed();
            return;
        }

        m_fPastTime += Time.deltaTime;
    }

    void QuestFailed()
    {
        //플레이어 캐릭터 동작 정지
        StartCoroutine(ReturnLobbyCoroutine("Quest Failed",0.5f, 10f, 10f, 10f, MessageClipType.PlayerDied));
    }

    public bool IsQuestFailed()
    {
        if (m_deathCount >= m_deathLimit)
            return true;

        return false;
    }

    public void QuestReset()
    {
        m_checkCount = 0;
        m_deathCount = 0;
        m_deathLimit = DeathCountBase;
        m_selectQuest = null;
        m_questTarget = null;

        CLoadingScene.LoadingScene("Lobby");
    }

    public void QuestCheck(CUnitData targetData)
    {
        if (m_questTarget == null)
            return;

        if (m_questTarget != targetData)
            return;

        m_checkCount++;

        if(m_checkCount>=m_selectQuest.Value.mainTargetCount)
        {
            
            //TODO: 퀘스트 클리어 처리
            StartCoroutine(ReturnLobbyCoroutine("Quest Clear", 3f , 5f, 5f ,10f, MessageClipType.QuestClear));
        }
    }

    public void AddPlayerCount()
    {
        if (m_playerCount >= 4)
            return;

        m_playerCount++;
        m_deathLimit = m_playerCount * DeathCountBase;
    }

    public void AddDeathCount(CPlayerUnit player)
    {
        if (player == null)
            return;

        if (player.tag != "Player")
            return;

        m_deathCount++;

        if(m_deathCount>=m_deathLimit)
        {
            QuestFailed();
        }
    }
    
    IEnumerator ReturnLobbyCoroutine(string message,float fFirstDelay,float fMainDelay,float fSubDelay,float fResetDelay,MessageClipType clipType)
    {
        yield return new WaitForSeconds(fFirstDelay);

        CStageUIManager.Inst.MessageUI.ShowMainMessage( message , fMainDelay, Color.red, clipType);

        yield return new WaitForSeconds(5f);

        CStageUIManager.Inst.MessageUI.ShowSubMessage("Return to Lobby", fSubDelay);

        yield return new WaitForSeconds(fResetDelay);

        QuestReset();

        yield return null;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}