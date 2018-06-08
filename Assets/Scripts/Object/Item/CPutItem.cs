using UnityEngine;
using UnityEngine.UI;

public class CPutItem : MonoBehaviour
{
    [SerializeField]
    private CItemBase m_item = null;

    [SerializeField]
    private GameObject m_itemInfoGO = null;
    [SerializeField]
    private Text m_itemText = null;
    [SerializeField]
    private Image m_itemIcon = null;

    [SerializeField]
    private AudioSource m_effectSource = null;

    private Collider m_collider = null;

    private float m_lossTime = 0f;
    private float m_pastTime = 0f;

    void Awake()
    {
        m_collider = GetComponent<Collider>();
    }

    void Start()
    {
        if (m_itemInfoGO != null)
            m_itemInfoGO.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf || m_lossTime == 0f)
            return;

        m_pastTime += Time.deltaTime;

        if (m_pastTime >= m_lossTime)
            PooledThis();
    }

    public void SetItemData(CItemBase itemData)
    {
        if (m_collider != null)
            m_collider.enabled=true;

        m_item = itemData;

        if(m_item!=null)
        {
            m_itemText.text = m_item.Name;
            m_itemIcon.sprite = m_item.IconImg;
        }

        m_lossTime = Random.Range(10f, 15f);
        m_pastTime = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CPlayerUnit>())
        {
            if (m_itemInfoGO != null)
                m_itemInfoGO.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        CPlayerUnit player = other.GetComponent<CPlayerUnit>();

        if (player!=null)
        {
            if (Input.GetButtonUp("Submit"))
            {
                if (m_item != null)
                {
                    if (m_item is IEquipment)
                    {
                        IEquipment equipment = m_item as IEquipment;

                        Data.EquipmentBoxInfo equipmentInfo = new Data.EquipmentBoxInfo();
                        equipmentInfo.partsType = equipment.PartsType;
                        equipmentInfo.index = m_item.ItemIdx;

                        CDataManager.Inst.PlayerRecord.BoxInfo.equipmentList.Add(equipmentInfo);
                    }
                    else
                    {
                        ItemData item = new ItemData();
                        item.item = m_item;
                        item.count = 1;

                        player.Inventory.ItemAdd(item);
                    }
                }

                if(m_itemInfoGO!=null)
                    m_itemInfoGO.SetActive(false);

                if (CSoundManager.Inst != null && m_effectSource != null)
                    CSoundManager.Inst.EffectPlay(m_effectSource);

                if (m_collider != null)
                    m_collider.enabled = false;

                Invoke("PooledThis",0.5f);
                
                return;
            }
        }
    }

    void PooledThis()
    {
        m_item = null;
        m_itemText.text = "Get Item";
        m_itemIcon.sprite = null;
        CObjectPool.Inst.PooledItem(gameObject, this);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CPlayerUnit>())
        {
            if (m_itemText != null)
                m_itemInfoGO.SetActive(false);
        }
    }
}
