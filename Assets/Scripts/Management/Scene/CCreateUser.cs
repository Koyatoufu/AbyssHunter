using UnityEngine;
using System.Collections;

public class CCreateUser : MonoBehaviour
{
    [SerializeField]
    private CMessageUI m_messageUI = null;

    [SerializeField]
    private CVisibleUnit m_wearUnit = null;

    [SerializeField]
    private AudioSource m_bgmAudio = null;

    [SerializeField]
    private AudioSource m_buttonAudio = null;

    private int m_nCurset = 1;
    private int m_nNotset = 2;

    private string m_name = "";
    private Gender m_Gender = Gender.Female;

    void Start()
    {
        StartCoroutine(InitCoroutine());

        if(m_bgmAudio!=null)
        {
            if(CSoundManager.Inst!=null)
            {
                CSoundManager.Inst.BgmPlay(m_bgmAudio);
            }
        }
    }

    IEnumerator InitCoroutine()
    {
        SetPlayerGender(0);
        yield return null;

        while(m_wearUnit.Inventory==null)
        {
            yield return null;
        }

        CWeapon weapon = CDataManager.Inst.EquipmentContainer.GetEquipment(PartsType.WEAPON, m_nCurset) as CWeapon;

        m_wearUnit.Inventory.EquipWeapon(weapon);
    }

    public void SetUserName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            m_messageUI.ShowSubMessage("Please Input UserName", 2f);
            return;
        }

        m_name = name;
    }

    public void SetPlayerGender(int nGender)
    {
        if (nGender > (int)Gender.Male)
            return;
        m_Gender = (Gender)nGender;
    }

    public void SetPlayerFirstSet(int nSetIdx)
    {
        if (m_wearUnit.Inventory == null)
            return;

        m_nCurset = nSetIdx;
        m_nNotset = m_nCurset == 1 ? 2 : 1;

        //nSetIdx += m_playerInfo.gender == Gender.Male ? 100 : 0;

        CWearable[] wearables = new CWearable[(int)PartsType.WEAPON];

        for (int i = 0; i < (int)PartsType.WEAPON; i++)
        {
            wearables[i] = CDataManager.Inst.EquipmentContainer.GetEquipment((PartsType)i, nSetIdx) as CWearable;
            m_wearUnit.Inventory.ChangeWearable(wearables[i]);
        }

        CWeapon weapon = CDataManager.Inst.EquipmentContainer.GetEquipment(PartsType.WEAPON, 1) as CWeapon;

        m_wearUnit.Inventory.EquipWeapon(weapon);

    }

    public void SaveUserData()
    {
        if (string.IsNullOrEmpty(m_name))
        {
            m_messageUI.ShowSubMessage("Please Input UserName", 2f);
            return;
        }

        //m_nNotset += m_playerInfo.gender == Gender.Male ? 100 : 0;

        Data.BoxInfo boxInfo = CDataManager.Inst.PlayerRecord.BoxInfo;
        Data.InventoryInfo inventoryInfo = CDataManager.Inst.PlayerRecord.InventoryInfo;

        AddEquipmentInBox(boxInfo);
        EquipmentSet(inventoryInfo);

        inventoryInfo.itemList.Add(CreateItem(0));
        boxInfo.itemList.Add(CreateItem(2));

        CDataManager.Inst.PlayerRecord.SaveUserData(m_name, m_Gender);

        CLoadingScene.LoadingScene("Lobby");

        if (CSoundManager.Inst != null)
            CSoundManager.Inst.StopBgm(m_bgmAudio);
    }

    ItemData CreateItem(int nIdx,int nCount = 5)
    {
        ItemData itemData = new ItemData();
        itemData.item = CDataManager.Inst.ItemContainer.GetItem(nIdx);
        itemData.count = nCount;
        return itemData;
    }
    void EquipmentSet(Data.InventoryInfo inventoryInfo)
    {
        inventoryInfo.curArmIdx = m_nCurset;
        inventoryInfo.curBodyIdx = m_nCurset;
        inventoryInfo.curLegIdx = m_nCurset;
        inventoryInfo.curHeadIdx = m_nCurset;

        inventoryInfo.curWeaponIdx = 1;
        inventoryInfo.subWeaponIdx = m_nNotset + 1;
    }
    void AddEquipmentInBox(Data.BoxInfo boxInfo)
    {
        Data.EquipmentBoxInfo equipInfo = null;

        for (int i = 0; i < (int)PartsType.WEAPON; i++)
        {
            equipInfo = new Data.EquipmentBoxInfo();
            equipInfo.partsType = (PartsType)i;
            equipInfo.index = m_nNotset;

            boxInfo.equipmentList.Add(equipInfo);
        }

        equipInfo = new Data.EquipmentBoxInfo();
        equipInfo.partsType = PartsType.WEAPON;
        equipInfo.index = m_nCurset + 1;
        boxInfo.equipmentList.Add(equipInfo);
    }

    public void EffectPlay(AudioClip clip)
    {
        if (CSoundManager.Inst != null&&m_buttonAudio!=null)
            CSoundManager.Inst.EffectPlay(m_buttonAudio, clip);
    }
}