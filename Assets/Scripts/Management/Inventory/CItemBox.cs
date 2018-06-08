using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CItemBox : MonoBehaviour
{
    private List<IEquipment> m_equipments = new List<IEquipment>();
    public List<IEquipment> Equipments { get { return m_equipments; } }

    [SerializeField]
    private CInventoryUI m_IventoryUI = null;

    private CDataManager m_dataMgr = null;

    private Data.BoxInfo m_boxInfo = null;
    public Data.BoxInfo BoxInfo { get { return m_boxInfo; } }

    private Data.EquipmentContainer m_equipmentContainer = null;

    private void Awake()
    {
        //TODO: 데이터 관리 클래스에서 박스안에 들어 있었던 아이템을 집어 넣는다.

        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        while (m_dataMgr == null)
        {
            m_dataMgr = CDataManager.Inst;
            yield return null;
        }

        m_boxInfo = m_dataMgr.PlayerRecord.BoxInfo;

        m_equipmentContainer = m_dataMgr.EquipmentContainer;

        EquipmentBoxInit();

        yield return null;
    }

    void EquipmentBoxInit()
    {
        if (m_boxInfo == null)
            return;

        for(int i=0;i<m_boxInfo.equipmentList.Count;i++)
        {
            IEquipment equipment = m_equipmentContainer.GetEquipment
                (m_boxInfo.equipmentList[i].partsType, m_boxInfo.equipmentList[i].index);

            if (equipment == null)
                continue;

            m_equipments.Add(equipment);
        }
    }

    public CItemBase GetItem(int nIdx, out int nCount, bool isOutPut = false)
    {
        nCount = 1;

        if (m_boxInfo == null)
            return null;

        if (m_boxInfo == null)
            return null;

        if (nIdx<0||nIdx>=m_boxInfo.itemList.Count)
            return null;
        
        CItemBase item = m_boxInfo.itemList[nIdx].item;

        if(isOutPut)
        {
            nCount = m_boxInfo.itemList[nIdx].count;
            m_boxInfo.itemList.RemoveAt(nIdx);
        }

        return item;
    }

    public void InputItem(ItemData itemData)
    {
        if (m_boxInfo==null)
            return;

        for(int i=0;i<m_boxInfo.itemList.Count;i++)
        {
            if(m_boxInfo.itemList[i].item==itemData.item)
            {
                m_boxInfo.itemList[i].count += itemData.count;
                return;
            }
        }

        m_boxInfo.itemList.Add(itemData);

    }

    public IEquipment OutPut(int nIdx)
    {
        if (m_boxInfo == null)
            return null;

        if (nIdx >= m_equipments.Count)
            return null;

        IEquipment equipment = m_equipments[nIdx];
        m_equipments.RemoveAt(nIdx);

        m_boxInfo.equipmentList.RemoveAt(nIdx);

        m_IventoryUI.Process();

        return equipment;
    }

    public void InputEquip(CItemBase equipmentBase)
    {
        if (m_boxInfo == null)
            return;

        IEquipment equipment = equipmentBase as IEquipment;

        if (equipment == null)
            return;
        if (equipmentBase.ItemIdx <= 0)
            return;

        m_equipments.Add(equipment);

        Data.EquipmentBoxInfo equipmentInfo = new Data.EquipmentBoxInfo();

        equipmentInfo.partsType = equipment.PartsType;
        equipmentInfo.index = equipmentBase.ItemIdx;

        m_boxInfo.equipmentList.Add(equipmentInfo);
    }

}
