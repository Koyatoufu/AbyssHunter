using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CInventory))]
public class CPlayerUnit : CUnit {

    [SerializeField]
    private GameObject m_cameraObj;

    CInputCtrl m_inputCtrl;
    public CInputCtrl InputCtrl { get { return m_inputCtrl; } }
    public CPlayerCamera CameraCtrl { get; private set; }

    protected int m_nDeathCount;
    public int DeathCount { get { return m_nDeathCount; } }

    protected const int DEATHCOUNT_MAX = 3;

    [SerializeField]
    protected CInventory m_inventory;
    public CInventory Inventory { get { return m_inventory; } }

    public override CWeaponHook MainHook { get{ if (m_inventory == null) return null; return m_inventory.MainHook; }   }

    public Gender Gender { get; private set; }

    void Awake()
    {
        if(m_cameraObj==null)
        {
            m_cameraObj = Resources.Load("Prefabs/CameraCtrl") as GameObject;
        }

        GameObject camObj = Instantiate(m_cameraObj);
        camObj.name = "CameraCtrl";

        m_cameraObj = camObj;

        CameraCtrl = m_cameraObj.GetComponent<CPlayerCamera>();
    }

    protected override void Start ()
    {
        m_unitTag = "Player";
        gameObject.tag = m_unitTag;

        CDataManager dataMgr = CDataManager.Inst;

        if (dataMgr == null)
            return;

        Data.PlayerInfo playerInfo = dataMgr.PlayerRecord.PlayerInfo;

        GameObject genderPrefab = null;
        if (playerInfo.gender == Gender.Female)
        {
            genderPrefab = Resources.Load<GameObject>("Prefabs/Character/Player/Female");
            Gender = Gender.Female;
        }
        else
        {
            genderPrefab = Resources.Load<GameObject>("Prefabs/Character/Player/Male");
            Gender = Gender.Male;
        }

        if (genderPrefab == null)
        {
            return;
        }

        GameObject genderGO = Instantiate(genderPrefab,transform);
        genderGO.name = genderPrefab.name;

        base.Start();

        m_inputCtrl = GetComponent<CInputCtrl>();
        CameraCtrl.SetCamera(m_inputCtrl, this);

        m_inventory = GetComponent<CInventory>();
        m_inventory.Initialized(this);

        if (m_unitUIPrefab)
        {
            GameObject uiObj = Instantiate<GameObject>(m_unitUIPrefab, CStageUIManager.Inst.transform);
            uiObj.name = m_unitUIPrefab.name;

            m_unitUI = uiObj.GetComponent<CUnitUI>();
            if (m_unitUI != null)
                m_unitUI.Init(this);
        }
    }

    protected override void FixedUpdate()
    {
        m_actCtrl.FixedProcess();
        m_movement.FixedProcess();

        if(!m_actCtrl.IsRun && !m_actCtrl.InAction)
        {
            m_status.StaminaCharge(Time.deltaTime * 10f);
        }
    }

    protected override void Update()
    {
        m_inputCtrl.InputProcess();
        m_actCtrl.Process();
        if (m_unitUI == null)
            return;
        m_unitUI.Process();
    }

    protected override void DeathProcess()
    {
        m_actCtrl.DoDeathAction();
        
        if(CQuestManager.Inst!=null)
        {
            CQuestManager.Inst.AddDeathCount(this);

            if(!CQuestManager.Inst.IsQuestFailed())
            {
                StartCoroutine(DeathMessage());
            }
        }
        else
        {
            Invoke("ResetUnit", 5f);
        }        
    }

    IEnumerator DeathMessage()
    {
        CMessageUI messageUI = CStageUIManager.Inst.MessageUI;

        yield return new WaitForSeconds(1f);

        messageUI.ShowMainMessage("You Died", 6f, Color.red, MessageClipType.PlayerDied);
        
        yield return new WaitForSeconds(6f);

        ResetUnit();

        yield return new WaitForSeconds(1f);

        messageUI.ShowSubMessage("Remain : " + CQuestManager.Inst.RemainCount, 3f, MessageClipType.Base);
    }

    public override void ResetUnit()
    {
        if (m_status != null)
            m_status.ResetState();

        if (m_actCtrl != null)
        {
            m_actCtrl.ResetActionUnit();
        }

        if(m_unitUI!=null)
        {
            m_unitUI.gameObject.SetActive(true);
            m_unitUI.ResetUI();
        }

        CurUseItem = null;

        if(GameManager.Inst!=null)
        {
            if(GameManager.Inst.StartPoint!=null)
            {
                transform.position = GameManager.Inst.StartPoint.position;
            }
            else
            {
                transform.position = Vector3.zero;
            }
        }
        else
        {
            transform.position = Vector3.zero;
        }
    }

    
}
