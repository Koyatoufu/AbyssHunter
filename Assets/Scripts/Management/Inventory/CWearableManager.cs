using UnityEngine;

[System.Serializable]
public class CWearableMgr
{
    [SerializeField]
    private GameObject[] m_changeMeshes = new GameObject[(int)PartsType.WEAPON];

    [SerializeField]
    private CWearable[] m_Wearables = new CWearable[(int)PartsType.WEAPON];

    private GameObject m_wearTarget;
    private Stitcher m_stitcher;

    private Data.InventoryInfo m_inventoryInfo;

    private Stat.Resister m_totalResister;
    public Stat.Resister TotalResister { get { return m_totalResister; } }

    public CWearableMgr()
    {
        m_stitcher = new Stitcher();
    }

    public void Init(GameObject wearTarget,Data.InventoryInfo inventoryInfo)
    {
        m_wearTarget = wearTarget;
        m_inventoryInfo = inventoryInfo;

        InitEquip();
    }

    public CWearable ChangeEquip(CWearable wearable)
    {
        CWearable returnWear = null;

        if (wearable==null)
        {
            Debug.Log("Wearable is NullData");
            return returnWear;
        }

        if (m_wearTarget == null)
        {
            Debug.LogError("Missing WearTarget");
            return returnWear;
        }

        int nIdx = (int)wearable.PartsType;

        if (nIdx >= m_changeMeshes.Length)
        {
            Debug.LogError("Don't Use " + PartsType.WEAPON);
            return returnWear;
        }

        if(m_changeMeshes[nIdx]!=null)
        {
            GameObject.Destroy(m_changeMeshes[nIdx]);
        }
        
        m_changeMeshes[nIdx] = m_stitcher.Stitch(wearable.Prefab, m_wearTarget);
        returnWear = m_Wearables[nIdx];
        m_Wearables[nIdx] = wearable;

        switch (wearable.PartsType)
        {
            case PartsType.BODY:
                m_inventoryInfo.curBodyIdx = wearable.ItemIdx;
                break;
            case PartsType.ARM:
                m_inventoryInfo.curArmIdx = wearable.ItemIdx;
                break;
            case PartsType.LEG:
                m_inventoryInfo.curLegIdx = wearable.ItemIdx;
                break;
            case PartsType.HEAD:
                m_inventoryInfo.curHeadIdx = wearable.ItemIdx;
                break;
        }

        return returnWear;
    }

    void InitEquip()
    {
        CDataManager dataMgr = CDataManager.Inst;

        CWearable body = dataMgr.EquipmentContainer.GetEquipment(PartsType.BODY,m_inventoryInfo.curBodyIdx) as CWearable;
        CWearable arm = dataMgr.EquipmentContainer.GetEquipment(PartsType.ARM, m_inventoryInfo.curArmIdx) as CWearable;
        CWearable head = dataMgr.EquipmentContainer.GetEquipment(PartsType.HEAD, m_inventoryInfo.curHeadIdx) as CWearable;
        CWearable leg = dataMgr.EquipmentContainer.GetEquipment(PartsType.LEG, m_inventoryInfo.curLegIdx) as CWearable;

        ChangeEquip(body);
        ChangeEquip(arm);
        ChangeEquip(head);
        ChangeEquip(leg);

        m_totalResister = GetResisterAll();
    }

    public bool IsNoEquip(PartsType type)
    {
        if (type >= PartsType.WEAPON)
            return false;

        if (m_Wearables[(int)type] == null)
            return true;

        if (m_Wearables[(int)type].ItemIdx == 0)
            return true;

        return false;
    }

    private Stat.Resister GetResisterAll()
    {
        Stat.Resister resister = new Stat.Resister();

        for(int i=0;i<m_Wearables.Length;i++)
        {
            resister.strike += m_Wearables[i].Resister.strike;
            resister.slash += m_Wearables[i].Resister.slash;
            resister.fire += m_Wearables[i].Resister.fire;
            resister.ice += m_Wearables[i].Resister.ice;
            resister.thunder += m_Wearables[i].Resister.thunder;
        }

        return resister;
    }
}