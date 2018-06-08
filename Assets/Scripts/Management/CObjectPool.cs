using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CObjectPool : MonoBehaviour
{
    public class EffectPool
    {
        public GameObject prefab = null;
        public int nBufferAmount = 0;
        public Stack<GameObject> pooledList = null;
    }

    public class UnitPool
    {
        public CUnitData unitData = null;
        public int nBufferAmount = 0;
        public Stack<GameObject> pooledList = null;
    }

    public class ItemPool
    {
        public GameObject itemPrefab = null;
        public int nBufferAmount = 0;
        public Stack<GameObject> pooledList = new Stack<GameObject>();
    }

    #region Singletone

    public static CObjectPool Inst { get; private set; }

    void Awake()
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
    private int m_effectAmount = 15;

    [SerializeField]
    private int m_unitAmount = 5;

    [SerializeField]
    private bool m_canBeGrow = true;

    private Dictionary<EffectType, Dictionary<int, EffectPool>> m_effectPools = new Dictionary<EffectType, Dictionary<int, EffectPool>>();
    private Dictionary<UnitType, Dictionary<int, UnitPool>> m_unitPools = new Dictionary<UnitType, Dictionary<int, UnitPool>>();
    private ItemPool m_itemPool = new ItemPool();

    private GameObject m_effectParent = null;
    private GameObject m_unitParent = null;
    private GameObject m_itemParent = null;

    public bool IsInit { get; private set; }

    void Start()
    {
        if (IsInit == true)
            return;

        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        while(CDataManager.Inst==null)
        {
            yield return null;
        }

        InitEffectPools(CDataManager.Inst.EffectContainier);
        InitUnitPools(CDataManager.Inst.UnitContainer);
        InitItemPools();

        yield return null;

        IsInit = true;

        yield return null;
    }

    #region PoolCreate
    void InitEffectPools(Data.EffectContainier containier)
    {

        m_effectParent = new GameObject("Effects");
        m_effectParent.transform.parent = transform;

        EffectPoolCreate(m_effectParent.transform, EffectType.Particle, containier);

        EffectPoolCreate(m_effectParent.transform, EffectType.Projectile, containier);

    }
    void EffectPoolCreate(Transform parent,EffectType type,Data.EffectContainier containier)
    {
        m_effectPools[type] = new Dictionary<int, EffectPool>();

        for (int i = 0; i < containier.GetEffectCount(type); i++)
        {
            GameObject prefab = containier.GetEffects(type, i);
            if (prefab != null)
            {
                EffectPool pool = new EffectPool();
                pool.prefab = prefab;
                pool.nBufferAmount = m_effectAmount;
                pool.pooledList = new Stack<GameObject>();

                for (int j = 0; j < pool.nBufferAmount; j++)
                {
                    GameObject poolObj = Instantiate(prefab);
                    poolObj.name = prefab.name;
                    poolObj.transform.parent = parent;
                    poolObj.SetActive(false);
                    pool.pooledList.Push(poolObj);
                }

                m_effectPools[type].Add(i, pool);
            }
        }
    }
    void InitUnitPools(Data.UnitContainer container)
    {
        m_unitParent = new GameObject("Units");
        m_unitParent.transform.parent = transform;

        UnitPoolCreate(m_unitParent.transform, UnitType.Npc, container);
        UnitPoolCreate(m_unitParent.transform, UnitType.Enemy, container);
        UnitPoolCreate(m_unitParent.transform, UnitType.Boss, container);
    }
    void UnitPoolCreate(Transform parent,UnitType type,Data.UnitContainer container)
    {
        m_unitPools[type] = new Dictionary<int, UnitPool>();

        for (int i = 0; i < container.GetUnitCount(type) ;i++)
        {
            CUnitData unitData = container.GetUnitData(type, i);
            if (unitData != null)
            {
                UnitPool pool = new UnitPool();
                pool.unitData = unitData;
                pool.nBufferAmount = type!=UnitType.Boss ? m_unitAmount:1;
                pool.pooledList = new Stack<GameObject>();

                for (int j = 0; j < pool.nBufferAmount; j++)
                {
                    if (unitData.UnitPrefab == null)
                        break;
                    GameObject poolObj = Instantiate(unitData.UnitPrefab);
                    poolObj.name = unitData.UnitPrefab.name;
                    poolObj.transform.parent = parent;
                    poolObj.SetActive(false);
                    pool.pooledList.Push(poolObj);
                }

                m_unitPools[type].Add(i,pool);
            }
        }
    }

    void InitItemPools()
    {
        m_itemParent = new GameObject("ItemParent");
        m_itemParent.transform.parent = transform;

        m_itemPool.itemPrefab = Resources.Load<GameObject>("Prefabs/Etc/ItemObject");

        m_itemPool.nBufferAmount = 10;

        for(int i=0;i<m_itemPool.nBufferAmount;i++)
        {
            GameObject copyItem = Instantiate(m_itemPool.itemPrefab);
            copyItem.name = m_itemPool.itemPrefab.name;
            copyItem.SetActive(false);
            copyItem.transform.parent = m_itemParent.transform;

            m_itemPool.pooledList.Push(copyItem);
        }
    }

    #endregion

    public GameObject GetEffect(EffectType type,int nIdx)
    {
        EffectPool pool = m_effectPools[type][nIdx];
        if (pool == null)
            return null;
        if(pool.pooledList.Count<1)
        {
            if(m_canBeGrow)
            {
                GameObject newEffect = Instantiate(pool.prefab);
                newEffect.name = pool.prefab.name;
                return newEffect;
            }
            else
            {
                return null;
            }
        }

        GameObject outPut = pool.pooledList.Pop();
        outPut.transform.parent = null;
        //outPut.SetActive(true);

        return outPut;
    }

    public void PooledObject(GameObject effect, EffectType type,int nidx)
    {
        EffectPool pool = m_effectPools[type][nidx];
        if (pool == null)
        {
            Destroy(effect);
            return;
        }   
        
        if(pool.pooledList.Count>=pool.nBufferAmount)
        {
            if (!m_canBeGrow)
            {
                Destroy(effect);
                return;
            }
        }

        pool.pooledList.Push(effect);
        effect.transform.parent = m_effectParent.transform;
        effect.SetActive(false);
    }

    public GameObject GetUnit(UnitType type,int nIdx)
    {
        UnitPool unitPool = m_unitPools[type][nIdx];

        if (unitPool == null)
            return null;

        if(unitPool.pooledList.Count<1)
        {
            if(m_canBeGrow)
            {
                GameObject newUnit = Instantiate(unitPool.unitData.UnitPrefab);
                newUnit.name = unitPool.unitData.UnitPrefab.name;
                return newUnit;
            }
            else
            {
                return null;
            }
        }

        GameObject outPut = unitPool.pooledList.Pop();
        outPut.transform.parent = null;
        //outPut.SetActive(true);

        return outPut;
    }

    public void PooledObject(GameObject unitObj, CUnitData data, UnitType type, int nIdx)
    {
        UnitPool pool = m_unitPools[type][nIdx];

        if(pool==null)
        {
            Destroy(unitObj);
            return;
        }

        if(pool.pooledList.Count>=pool.nBufferAmount)
        {
            if(!m_canBeGrow)
            {
                Destroy(unitObj);
                return;
            }
        }

        unitObj.SetActive(false);
        unitObj.transform.parent = m_unitParent.transform;
        pool.pooledList.Push(unitObj);
    }

    public GameObject GetItem()
    {
        GameObject newItem = null;

        if (m_itemPool.pooledList.Count<1)
        {
            if(m_canBeGrow)
            {
                newItem = Instantiate(m_itemPool.itemPrefab);
                newItem.name = m_itemPool.itemPrefab.name;

                return newItem;
            }

            return null;
        }

        newItem = m_itemPool.pooledList.Pop();
        newItem.transform.parent = null;

        return newItem;
    }

    public void PooledItem(GameObject itemObj, CPutItem putItem)
    {
        if (putItem == null)
        {
            Destroy(itemObj);
            return;
        }

        if(m_itemPool.pooledList.Count>=m_itemPool.nBufferAmount)
        {
            if(!m_canBeGrow)
            {
                Destroy(itemObj);
                return;
            }
        }

        itemObj.transform.parent = m_itemParent.transform;
        m_itemPool.pooledList.Push(itemObj);
        itemObj.SetActive(false);
    }
}

