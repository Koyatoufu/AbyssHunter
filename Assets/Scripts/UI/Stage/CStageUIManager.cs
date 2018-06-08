using UnityEngine;
using System.Collections;

public class CStageUIManager : MonoBehaviour
{
    #region SingleTone
    public static CStageUIManager Inst { get; private set; }

    public void Awake()
    {
        if(Inst!=null)
        {
            Destroy(this.gameObject);
            return;
        }

        Inst = this;
    }
    #endregion

    [SerializeField]
    private GameObject m_invenUIObj = null;
    [SerializeField]
    private GameObject m_messageUIObj = null;

    private CInventoryUI m_InventoryUI = null;
    public CInventoryUI InventoryUI { get { return m_InventoryUI; } }

    private CMessageUI m_messageUI = null;
    public CMessageUI MessageUI { get { return m_messageUI; } }

    private CPlayerUnit m_player = null;

    void Start()
    {
        if(m_invenUIObj!=null)
        {
            GameObject invenUI = Instantiate(m_invenUIObj,transform);
            invenUI.name = m_invenUIObj.name;
            m_InventoryUI = invenUI.GetComponent<CInventoryUI>();
            StartCoroutine(InventoryUICoroutine());
        }
        if(m_messageUIObj!=null)
        {
            GameObject messageUI = Instantiate(m_messageUIObj, transform);
            messageUI.name = m_messageUIObj.name;
            m_messageUI = messageUI.GetComponent<CMessageUI>();
        }
    }

    IEnumerator InventoryUICoroutine()
    {
        while(m_player==null)
        {
            yield return null;

            GameObject playerGo = GameObject.FindWithTag("Player");
            if (playerGo == null)
                continue;

            m_player = playerGo.GetComponent<CPlayerUnit>();
        }

        while(m_player.Inventory==null)
        {
            yield return null;
        }

        m_InventoryUI.Init(m_player.Inventory);
    }

    void Update()
    {
        m_InventoryUI.Process();
    }
}
