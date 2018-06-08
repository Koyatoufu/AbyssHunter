using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CLobbyManager : MonoBehaviour
{
    public static CLobbyManager Inst { get; private set; }

    [SerializeField]
    private CQuestInfoUI m_questInfoUI = null;
    public CQuestInfoUI QuestInfoUI { get { return m_questInfoUI; } }

    [SerializeField]
    private CJoinUI m_joinUI = null;
    public CJoinUI JoinUI { get { return m_joinUI; } }

    [SerializeField]
    private CInventoryUI m_InventoryUI = null;
    public CInventoryUI InventoryUI { get { return m_InventoryUI; } }

    [SerializeField]
    private CMessageUI m_messageUI = null;

    [SerializeField]
    private CVisibleUnit m_wearUnit = null;

    [SerializeField]
    private AudioSource m_bgmSource = null;

    [SerializeField]
    private AudioSource m_effectSource = null;

    NetworkManager m_manager = null;

    void Awake()
    {
        if (Inst != null)
        {
            Destroy(this);
            return;
        }

        Inst = this;
    }

    void Start()
    {
        m_manager = NetworkManager.singleton;

        StartCoroutine(StartCoroutine());

        if(CSoundManager.Inst!=null)
        {
            CSoundManager.Inst.BgmPlay(m_bgmSource);
        }
    }

    IEnumerator StartCoroutine()
    {
        while(m_wearUnit.Inventory==null)
        {
            yield return null;
        }

        m_InventoryUI.Init(m_wearUnit.Inventory);
        yield return null;
    }

    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            CreateQuest();
        }

        m_InventoryUI.Process();
    }

    public void CreateQuest()
    {
        //TODO: 퀘스트 생성 및 룸 생성

        Data.QuestInfo? questInfo = m_questInfoUI.SelectedQuest;

        if(questInfo==null)
        {
            if(m_messageUI!=null)
            {
                m_messageUI.ShowSubMessage("Don't Quest Selected",2f);
            }
            return;
        }

        CQuestManager.Inst.QuestSet(questInfo.Value);

        //TODO: 네트워크 매치 메이크 생성
        
        if (m_manager != null)
        {
            if (m_manager.matchMaker == null)
            {
                m_manager.StartMatchMaker();
            }

            //m_manager.matchMaker.CreateMatch(questInfo.Value.questName, 4, false, "", "", "", 0, 0, m_manager.OnMatchCreate);
            //return;
        }

        CSoundManager.Inst.StopBgm(m_bgmSource);
        CLoadingScene.LoadingScene("LevelRoot");
    }

    public void OpenQuestInfo()
    {
        m_questInfoUI.gameObject.SetActive(true);
        m_joinUI.gameObject.SetActive(false);
        m_InventoryUI.gameObject.SetActive(false);
    }

    public void OpenJoinInfo()
    {
        m_questInfoUI.QuestReset();
        m_questInfoUI.gameObject.SetActive(false);
        m_joinUI.gameObject.SetActive(true);
        m_InventoryUI.gameObject.SetActive(false);
    }

    public void OpenInventoryInfo()
    {
        m_questInfoUI.QuestReset();
        m_questInfoUI.gameObject.SetActive(false);
        m_joinUI.gameObject.SetActive(false);
        m_InventoryUI.AllTurnOff();
        m_InventoryUI.gameObject.SetActive(true);
        m_InventoryUI.TurnPouch(true);
        m_InventoryUI.TurnBox(true);
    }

    public void OpenEquipmentInfo()
    {
        m_questInfoUI.QuestReset();
        m_questInfoUI.gameObject.SetActive(false);
        m_joinUI.gameObject.SetActive(false);
        m_InventoryUI.AllTurnOff();
        m_InventoryUI.gameObject.SetActive(true);
        m_InventoryUI.TurnEquipBox(true);
        m_InventoryUI.TurnEquipInfo(true);
    }

    public void GoToTitle()
    {
        CLoadingScene.LoadingScene("Title");
    }

    public void EffectPlay(AudioClip clip)
    {
        if (CSoundManager.Inst != null && m_effectSource != null)
            CSoundManager.Inst.EffectPlay(m_effectSource,clip);
    }
}
