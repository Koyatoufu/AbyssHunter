using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CInventory : MonoBehaviour
{
    #region Instance

    private Data.InventoryInfo m_inventoryInfo;
    public Data.InventoryInfo InventoryInfo { get { return m_inventoryInfo; } }

    public int CurItemSelect { get; set; }
    public int CurSpellSelect { get; set; }

    #region Equipment

    private CWearableMgr m_wearableMgr = new CWearableMgr();
    public CWearableMgr WearableCtrl { get { return m_wearableMgr; } }

    private CWeapon m_mainEquip = null;
    public CWeapon MainEquip { get { return m_mainEquip; } }

    private CWeapon m_subEquip = null;
    public CWeapon SecondEquip { get { return m_subEquip; } }

    private CWeaponHook m_MainHook;
    public CWeaponHook MainHook { get { return m_MainHook; } }

    private CWeaponHook m_subHook;
    public CWeaponHook SubHook { get { return m_subHook; } }

    #endregion

    [SerializeField]
    private Transform[] m_Slots;

    private CUnit m_unit;
    private CActionCtrl m_actionCtrl;

    #endregion

    public void Initialized(CUnit unit)
    {
        CDataManager dataMgr = CDataManager.Inst;

        if (dataMgr == null)
            return;

        m_inventoryInfo = dataMgr.PlayerRecord.InventoryInfo;

        m_unit = unit;
        m_actionCtrl = unit.GetComponent<CActionCtrl>();

        if (m_unit == null)
            return;

        // 장비 아이템을 장착
        InitSlot();
        InitWeaponSlot(dataMgr);

        m_wearableMgr.Init(m_actionCtrl.Anim.gameObject,m_inventoryInfo);
        m_unit.Status.ResisterSet(m_wearableMgr.TotalResister);
    }
    
    #region ItemsFunction
    public CItemBase ItemUse(int nIdx)
    {
        if(nIdx<0||nIdx>=InventoryInfo.itemList.Count)
            return null;

        if (InventoryInfo.itemList[nIdx] == null)
            return null;

        CItemBase item = InventoryInfo.itemList[nIdx].item;

        InventoryInfo.itemList[nIdx].count -= 1;

        if (InventoryInfo.itemList[nIdx].count < 1)
        {
            InventoryInfo.itemList.RemoveAt(nIdx);
            
            CurItemSelect = 0;
        }

        return item;
    }
    public ItemData ItemOut(int nIdx)
    {
        if (nIdx >= InventoryInfo.itemList.Count)
            return null;

        if (InventoryInfo.itemList[nIdx] == null)
            return null;

        ItemData itemData = InventoryInfo.itemList[nIdx];

        InventoryInfo.itemList.RemoveAt(nIdx);

        return itemData;
    }
    public void ItemAdd(ItemData itemData)
    {
        for(int i=0;i< InventoryInfo.itemList.Count;i++)
        {
            if(itemData.item== InventoryInfo.itemList[i].item)
            {
                InventoryInfo.itemList[i].count += itemData.count;
                return;
            }
        }

        InventoryInfo.itemList.Add(itemData);
    }
    #endregion

    #region SpellsFunction
    public CSpellItem SpellUse(int nIdx)
    {
        if (nIdx >= InventoryInfo.spellList.Count)
            return null;

        if (InventoryInfo.spellList[nIdx] == null)
            return null;

        CItemBase item = InventoryInfo.spellList[nIdx].item;

        InventoryInfo.spellList[nIdx].count -= 1;

        if (InventoryInfo.spellList[nIdx].count < 1)
        {
            InventoryInfo.spellList.RemoveAt(nIdx);

            CurSpellSelect = 0;
        }

        return item as CSpellItem;
    }
    public ItemData SpellOut(int nIdx)
    {
        if (nIdx >= InventoryInfo.spellList.Count)
            return null;

        if (InventoryInfo.spellList[nIdx] == null)
            return null;

        ItemData spellData = InventoryInfo.spellList[nIdx];

        InventoryInfo.spellList.RemoveAt(nIdx);

        for (int i = 0; i < m_inventoryInfo.itemList.Count; i++)
        {
            if (m_inventoryInfo.itemList[i].item == spellData.item)
            {
                m_inventoryInfo.itemList.RemoveAt(i);
                break;
            }
        }

        return spellData;
    }
    public void SpellAdd(ItemData itemData)
    {
        if(!(itemData.item is CSpellItem))
        {
            return;
        }

        for(int i=0;i< InventoryInfo.spellList.Count;i++)
        {
            if(InventoryInfo.spellList[i].item==itemData.item)
            {
                InventoryInfo.spellList[i].count += itemData.count;
                return;
            }
        }

        InventoryInfo.spellList.Add(itemData);
    }
    #endregion

    #region EquipmentFunction
    public CWearable ChangeWearable(CWearable wearable)
    {
        if (wearable == null)
            return null;

        CWearable returnWear = m_wearableMgr.ChangeEquip(wearable);
        return returnWear;
    }
    public CWeapon EquipWeapon(CWeapon weapon, SlotIdx slotIdx = SlotIdx.Right, bool isSub = false)
    {
        CWeapon outWeapon = null;

        #region CheckNull

        if (weapon == null)
        {
            Debug.LogError("Don't Equip null Weapon");
            return outWeapon;
        }

        GameObject weaponObj = Instantiate(weapon.Prefab);

        if (weaponObj == null)
        {
            Debug.LogError(weapon + ".prefab is null");
            return outWeapon;
        }

        weaponObj.name = weapon.Prefab.name;

        CWeaponHook weaponHook = weaponObj.GetComponent<CWeaponHook>();

        if (weaponHook == null)
        {
            Debug.LogError(weapon + "is not Contained WeaponHook");
            return outWeapon;
        }

        #endregion

        if (!isSub)
        {
            if (m_MainHook != null)
            {
                outWeapon = m_MainHook.Weapon;
                Destroy(m_MainHook.gameObject);
                m_mainEquip = null;
                m_MainHook = null;
            }

            m_MainHook = weaponHook;
            m_MainHook.Init(m_unit, weapon);
            m_mainEquip = weapon;
            m_inventoryInfo.curWeaponIdx = m_mainEquip.ItemIdx;
        }
        else
        {
            if (m_subHook != null)
            {
                outWeapon = m_subHook.Weapon;
                Destroy(m_subHook.gameObject);
                m_subHook = null;
                m_subEquip = null;
            }

            m_subHook = weaponHook;
            m_subHook.Init(m_unit, weapon);
            m_subEquip = weapon;
            m_inventoryInfo.subWeaponIdx = m_subEquip.ItemIdx;
        }

        WeaponSlotSetting(weaponHook, slotIdx);

        if(m_actionCtrl.Anim!=null)
            m_actionCtrl.Anim.SetInteger("WeaponIdx", (int)m_mainEquip.WeaponType);

        //TODO: 방패 혹은 오브젝트를 두개를 껴야하는 경우의 관련 처리
        return outWeapon;
    }

    void InitSlot()
    {
        // TODO: 임시로 태그 설정으로 슬롯을 검색
        GameObject[] slotObjs = GameObject.FindGameObjectsWithTag("Slots");

        m_Slots = new Transform[slotObjs.Length];

        for (int i = 0; i < slotObjs.Length; i++)
        {
            int nIdx = -1;

            switch (slotObjs[i].name)
            {
                case "handSlot_l":
                    nIdx = (int)SlotIdx.Left;
                    break;
                case "handSlot_r":
                    nIdx = (int)SlotIdx.Right;
                    break;
                case "backSlot":
                    nIdx = (int)SlotIdx.Back;
                    break;
                case "waistSlot":
                    nIdx = (int)SlotIdx.Waist;
                    break;
            }

            if (nIdx != -1)
            {
                m_Slots[nIdx] = slotObjs[i].transform;
            }
        }
    }
    void InitWeaponSlot(CDataManager dataMgr)
    {
        // TODO: PlayerData에서 설정 장비 가져오기

        int nWIdx = m_inventoryInfo.curWeaponIdx;

        CWeapon weapon = dataMgr.EquipmentContainer.GetEquipment(PartsType.WEAPON, nWIdx) as CWeapon;

        EquipWeapon(weapon);

        int nSecondWIdx = m_inventoryInfo.subWeaponIdx;

        CWeapon subWeapon = dataMgr.EquipmentContainer.GetEquipment(PartsType.WEAPON,nSecondWIdx) as CWeapon;

        if (subWeapon != null)
            EquipWeapon(subWeapon, subWeapon.WaitSlot, true);
    }

    public void ChangeMainSub()
    {
        if (m_MainHook == null)
            return;

        if (m_subHook == null)
            return;

        StartCoroutine(ChangeMainSubCoroutine());
        
    }
    IEnumerator ChangeMainSubCoroutine()
    {
        CWeapon tempWeapon = m_mainEquip;
        m_mainEquip = m_subEquip;
        m_subEquip = tempWeapon;

        CWeaponHook tempHook = MainHook;
        m_MainHook = SubHook;
        m_subHook = tempHook;

        m_actionCtrl.Anim.SetInteger("WeaponIdx", (int)m_mainEquip.WeaponType);

        yield return new WaitForSeconds(0.5f);

        WeaponSlotSetting(m_MainHook, SlotIdx.Right);
        WeaponSlotSetting(m_subHook, m_subHook.Weapon.WaitSlot);

        yield return null;
    }

    public bool IsNoEquip(PartsType type,bool isSub=false)
    {
        switch (type)
        {
            case PartsType.MAX:
                return false;
            case PartsType.WEAPON:
                {
                    if (isSub)
                        return IsNoSub();
                    else
                        return IsNoHand();
                }
            default:
                {
                    return m_wearableMgr.IsNoEquip(type);
                }
        }
    }

    bool IsNoHand()
    {
        if (m_mainEquip == null)
            return true;

        if (m_MainHook == null)
            return true;

        if (m_mainEquip.ItemIdx == 0)
            return true;

        return false;
    }

    bool IsNoSub()
    {
        if (m_subEquip == null)
            return true;
        if (m_subHook == null)
            return true;
        if (m_subEquip.ItemIdx == 0)
            return true;

        return false;
    }

    void WeaponSlotSetting(CWeaponHook weaponHook, SlotIdx slotIdx)
    {
        if (weaponHook == null)
            return;

        CWeapon weapon = weaponHook.Weapon;

        if (weapon == null)
            return;

        weaponHook.transform.parent = m_Slots[(int)slotIdx];

        weaponHook.gameObject.transform.localPosition = weapon.Prefab.transform.localPosition;
        weaponHook.gameObject.transform.localRotation = weapon.Prefab.transform.localRotation;
        weaponHook.gameObject.transform.localScale = weapon.Prefab.transform.localScale;
    }
    #endregion
}

public enum SlotIdx
{
    Left,
    Right,
    Back,
    Waist,
    Max
}

public class ItemData
{
    public CItemBase item;
    public int count;
}