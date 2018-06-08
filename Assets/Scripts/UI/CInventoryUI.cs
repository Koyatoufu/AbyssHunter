using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CInventoryUI : MonoBehaviour
{
    #region ItemPouch_Instance
    [Header("ItemPouch")]
    [SerializeField]
    private GameObject m_pouchGO = null;

    [SerializeField]
    private List<Image> m_bagIcons = new List<Image>();

    [SerializeField]
    private List<Image> m_spellbagIcons = new List<Image>();
    #endregion

    #region ItemBox_Instance
    [Header("ItemBox")]
    [SerializeField]
    private GameObject m_boxGO = null;

    [SerializeField]
    private List<Image> m_boxIcons = new List<Image>();

    [SerializeField]
    private List<Image> m_equpmentIcons = new List<Image>();
    
    [SerializeField]
    private Text m_itemPageText = null;

    [SerializeField]
    private GameObject m_itemBoxGO = null;
    [SerializeField]
    private GameObject m_equipBoxGO = null;

    #endregion
    #region Equipment_Instance
    [Header("EquipmentInfo")]
    [SerializeField]
    private GameObject m_EquipmentGO = null;

    [SerializeField]
    private List<Image> m_partsIcons = new List<Image>();

    #endregion

    CInventory m_inventory;
    [SerializeField]
    private CItemBox m_itemBox = null;

    [SerializeField]
    private Sprite m_emptyIcon = null;

    [SerializeField]
    private AudioSource m_btnSource = null;

    public void Init(CInventory inventory)
    {
        if(inventory==null)
        {
            Debug.Log("Inventory is null");
            return;
        }
        m_inventory = inventory;

        //TODO: 인벤토리 및 아이템 박스에 맞춰서 내용 갱신
        m_itemPageText.text = "1/1";

        UpdateInventoryIcon();
        UpdateBoxIcon();
    }

    public void Process()
    {
        if(m_pouchGO != null && m_pouchGO.activeSelf)
        {
            //TODO: 활성화 시만 포치 내용 갱신
            UpdateInventoryIcon();
        }
        if(m_boxGO!=null&&m_boxGO.activeSelf)
        {
            //TODO: 활성화 시만 박스 내용 갱신
            UpdateBoxIcon();
        }
        if(m_EquipmentGO!=null && m_EquipmentGO.activeSelf)
        {
            UpdatePartsIcon();
        }
    }

    #region IconChange

    public void BagIconChange(Sprite sprite,int nIdx)
    {
        if (nIdx > m_bagIcons.Count)
            return;

        if (sprite == null)
            return;

        m_bagIcons[nIdx].sprite = sprite;
        //아이템 정보 갱신
    }

    public void SpellBagIconChange(Sprite sprite, int nIdx)
    {
        if (nIdx > m_spellbagIcons.Count)
            return;

        if (sprite == null)
            return;

        m_bagIcons[nIdx].sprite = sprite;
        //인벤토리 정보 갱신
    }

    void PartsFunc(PartsType type, int nIdx, Data.EquipmentContainer container, bool isSub = false)
    {
        CItemBase parts = container.GetEquipment(type, nIdx) as CItemBase;

        if (isSub)
        {
            if (parts != null)
                m_partsIcons[m_partsIcons.Count - 1].sprite = parts.IconImg;
        }
        else
        {
            if (parts != null)
                m_partsIcons[(int)type].sprite = parts.IconImg;
        }

    }

    #endregion

    #region TurnUI
    public void TurnPouch(bool isTurn)
    {
        if(m_pouchGO!=null)
            m_pouchGO.SetActive(isTurn);
    }

    public void TurnBox(bool isTurn)
    {
        if (m_boxGO != null)
            m_boxGO.SetActive(isTurn);

        if(isTurn)
        {
            m_equipBoxGO.SetActive(false);
            m_itemBoxGO.SetActive(true);
        }
        else
        {
            m_equipBoxGO.SetActive(false);
            m_itemBoxGO.SetActive(false);
        }
    }

    public void TurnEquipBox(bool isTurn)
    {
        if (m_boxGO != null)
            m_boxGO.SetActive(isTurn);

        if (isTurn)
        {
            m_equipBoxGO.SetActive(true);
            m_itemBoxGO.SetActive(false);
        }
        else
        {
            m_equipBoxGO.SetActive(false);
            m_itemBoxGO.SetActive(false);
        }
    }

    public void TurnEquipInfo(bool isTurn)
    {
        if (m_EquipmentGO != null)
            m_EquipmentGO.SetActive(isTurn);
    }

    public void AllTurnOff()
    {
        if (m_pouchGO != null)
            m_pouchGO.SetActive(false);

        if (m_EquipmentGO != null)
            m_EquipmentGO.SetActive(false);

        if (m_boxGO != null)
        {
            m_boxGO.SetActive(false);
            m_equipBoxGO.SetActive(false);
            m_itemBoxGO.SetActive(false);
        }

    }
    #endregion

    #region UpdateIcons

    void UpdateInventoryIcon()
    {
        if (m_inventory == null)
            return;

        for(int i=0;i<m_bagIcons.Count;i++)
        {
            m_bagIcons[i].sprite = m_emptyIcon;
        }

        for(int i=0;i<m_spellbagIcons.Count;i++)
        {
            m_bagIcons[i].sprite = m_emptyIcon;
        }

        for(int i=0;i<m_inventory.InventoryInfo.itemList.Count;i++)
        {
            if (i > m_bagIcons.Count)
                break;

            if (m_inventory.InventoryInfo.itemList[i].item != null)
            {
                m_bagIcons[i].sprite = m_inventory.InventoryInfo.itemList[i].item.IconImg;
            }
        }

        for(int i=0;i<m_inventory.InventoryInfo.spellList.Count;i++)
        {
            if (i > m_spellbagIcons.Count)
                break;

            if(m_inventory.InventoryInfo.spellList[i].item!=null)
            {
                m_spellbagIcons[i].sprite = m_inventory.InventoryInfo.spellList[i].item.IconImg;
            }
        }
    }

    void UpdateBoxIcon()
    {
        if (m_itemBox == null)
            return;

        for (int i = 0; i < m_boxIcons.Count; i++)
        {
            m_boxIcons[i].sprite = m_emptyIcon;
        }

        for(int i=0;i<m_equpmentIcons.Count;i++)
        {
            m_equpmentIcons[i].sprite = m_emptyIcon;
        }

        //20개당 페이지 갱신
        Data.BoxInfo boxInfo = m_itemBox.BoxInfo;
        if(boxInfo!=null)
        {
            for (int i = 0; i < boxInfo.itemList.Count; i++)
            {
                if (i >= m_boxIcons.Count)
                    break;

                m_boxIcons[i].sprite = boxInfo.itemList[i].item.IconImg;
            }
            //16개당 페이지 갱신
            for (int i = 0; i < m_itemBox.Equipments.Count; i++)
            {
                if (i >= m_equpmentIcons.Count)
                    break;

                CItemBase itemBase = m_itemBox.Equipments[i] as CItemBase;

                m_equpmentIcons[i].sprite = itemBase.IconImg;
            }
        }
    }

    void UpdatePartsIcon()
    {
        if (m_inventory == null)
            return;
        if (m_inventory.InventoryInfo == null)
            return;

        Data.EquipmentContainer container = CDataManager.Inst.EquipmentContainer;

        int nPartIdx = 0;

        nPartIdx = m_inventory.InventoryInfo.curBodyIdx;
        PartsFunc(PartsType.BODY, nPartIdx, container);

        nPartIdx = m_inventory.InventoryInfo.curArmIdx;
        PartsFunc(PartsType.ARM, nPartIdx, container);

        nPartIdx = m_inventory.InventoryInfo.curLegIdx;
        PartsFunc(PartsType.LEG, nPartIdx, container);

        nPartIdx = m_inventory.InventoryInfo.curHeadIdx;
        PartsFunc(PartsType.HEAD, nPartIdx, container);

        nPartIdx = m_inventory.InventoryInfo.curWeaponIdx;
        PartsFunc(PartsType.WEAPON, nPartIdx, container);

        nPartIdx = m_inventory.InventoryInfo.subWeaponIdx;
        PartsFunc(PartsType.WEAPON, nPartIdx, container, true);
    }

    #endregion

    #region ItemsChangeFunc

    public void OutPutEquipment(int nIdx)
    {
        if (!m_boxGO.activeSelf)
            return;

        if (nIdx >= (int)PartsType.MAX)
            return;

        if (m_inventory == null)
            return;

        PartsType type = (PartsType)nIdx;

        if (m_inventory.IsNoEquip(type))
            return;

        IEquipment naked = CDataManager.Inst.EquipmentContainer.GetEquipment(type, 0);

        if (type==PartsType.WEAPON)
        {
            CWeapon hand = naked as CWeapon;
            if (hand == null)
                return;
            CWeapon weapon = m_inventory.EquipWeapon(hand);
            m_itemBox.InputEquip(weapon);
        }
        else
        {
            CWearable zeroParts = CDataManager.Inst.EquipmentContainer.GetEquipment(type, 0) as CWearable;
            if (zeroParts == null)
                return;
            CWearable wearable = m_inventory.ChangeWearable(zeroParts);

            m_itemBox.InputEquip(wearable);
        }

    }

    public void EquipmentChange(int nIdx)
    {
        if (m_inventory == null)
            return;
        if (m_itemBox == null)
            return;

        if (nIdx < 0 || nIdx >= m_equpmentIcons.Count)
            return;

        IEquipment equip = m_itemBox.OutPut(nIdx);

        if(equip is CWearable)
        {
            CWearable equipWear = equip as CWearable;

            CWearable inputWear = m_inventory.ChangeWearable(equipWear);
            m_itemBox.InputEquip(inputWear);
        }
        else if(equip is CWeapon)
        {
            CWeapon equipWeapon = equip as CWeapon;

            CWeapon inputWeapon = m_inventory.EquipWeapon(equipWeapon);
            m_itemBox.InputEquip(inputWeapon);
        }
    }

    public void PouchItemOut(int nIdx)
    {
        if (m_inventory == null)
            return;
        if (m_itemBox == null)
            return;

        if (!m_itemBoxGO.activeSelf)
            return;

        if (nIdx < 0 || nIdx >= m_bagIcons.Count)
            return;

        ItemData itemData = m_inventory.ItemOut(nIdx);

        if(itemData!=null&&itemData.item!=null)
        {
            m_itemBox.InputItem(itemData);
        }
    }

    public void PouchSpellOut(int nIdx)
    {
        if (m_inventory == null)
            return;
        if (m_itemBox == null)
            return;

        if (!m_itemBoxGO.activeSelf)
            return;

        if (nIdx < 0 || nIdx >= m_spellbagIcons.Count)
            return;

        ItemData spellData = m_inventory.SpellOut(nIdx);

        if(spellData!=null && spellData.item!=null)
        {
            m_itemBox.InputItem(spellData);
        }
    }

    public void InputPouch(int nIdx)
    {
        if (m_inventory == null)
            return;
        if (m_itemBox == null)
            return;

        if (nIdx < 0 || nIdx >= m_boxIcons.Count)
            return;

        int nCount = 0;

        CItemBase item = m_itemBox.GetItem(nIdx, out nCount, true);

        if(item is CSpellItem)
        {
            ItemData itemData = new ItemData();
            itemData.item = item;
            itemData.count = nCount;

            m_inventory.SpellAdd(itemData);
        }
        else
        {
            ItemData itemData = new ItemData();
            itemData.item = item;
            itemData.count = nCount;

            m_inventory.ItemAdd(itemData);
        }
    }

    #endregion

    public void BtnSoundPlay(AudioClip clip)
    {
        if (CSoundManager.Inst != null && m_btnSource != null)
            CSoundManager.Inst.EffectPlay(m_btnSource, clip);
    }

}
