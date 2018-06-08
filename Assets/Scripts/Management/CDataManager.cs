using UnityEngine;

public class CDataManager : MonoBehaviour
{
    
    #region Data

    [SerializeField]
    private Data.PlayerRecord m_playerRecord = new Data.PlayerRecord();
    public Data.PlayerRecord PlayerRecord { get { return m_playerRecord; } }

    private Data.EquipmentContainer m_equipmentContainer = new Data.EquipmentContainer();
    public Data.EquipmentContainer EquipmentContainer { get { return m_equipmentContainer; } }

    private Data.ItemContainer m_itemContainer = new Data.ItemContainer();
    public Data.ItemContainer ItemContainer { get { return m_itemContainer; } }

    private Data.UnitContainer m_unitContainer = new Data.UnitContainer();
    public Data.UnitContainer UnitContainer { get { return m_unitContainer; } }

    private Data.QuestContainer m_questContainer = new Data.QuestContainer();
    public Data.QuestContainer QuestContainer { get { return m_questContainer; } }

    private Data.EffectContainier m_effectContainier = new Data.EffectContainier();
    public Data.EffectContainier EffectContainier { get { return m_effectContainier; } }

    #endregion

    #region Singletone

    public static CDataManager Inst { get; private set; }

    void Awake()
    {
        if(Inst!=null)
        {
            Destroy(this.gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(this.gameObject);

        m_equipmentContainer.Init();
        m_itemContainer.Init();
        m_unitContainer.Init();
        m_questContainer.Init();
        m_effectContainier.Init();
        m_playerRecord.Init(m_itemContainer);

    }

    void OnDestroy()
    {
        if (Inst != this)
            return;

        m_playerRecord.SaveAllData();
    }

    #endregion


}