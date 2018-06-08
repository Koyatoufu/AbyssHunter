using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private float m_spwanDelay = 30f;

    [SerializeField]
    private int m_maxCount = 5;
    
    [SerializeField]
    private CUnitData m_spawnBase = null;

    [SerializeField]
    private List<CUnit> m_spawnList = new List<CUnit>();

    [SerializeField]
    private List<Ways> m_ways = new List<Ways>();

    private bool m_isInit = false;
    private CSubLevel m_subLevel = null;

    //void Start()
    //{
    //    if (!m_isInit)
    //        Initialized(null);
    //}

    public void Initialized(CSubLevel level)
    {
        if (m_isInit)
            return;

        m_subLevel = level;

        m_isInit = true;
        StartCoroutine(SpawnCoroutine());

    }

    public void Initialized(CSubLevel level,UnitType unitType, int nUnitIdx)
    {
        if (m_isInit)
            return;

        m_subLevel = level;

        CDataManager dataMgr = CDataManager.Inst;
        if(dataMgr!=null)
        {
            CUnitData unit = dataMgr.UnitContainer.GetUnitData(unitType, nUnitIdx);

            if (unit != null)
                m_spawnBase = unit;
        }

        m_isInit = true;
        StartCoroutine(SpawnCoroutine());
        
    }

    IEnumerator SpawnCoroutine()
    {
        if(GameManager.Inst!=null)
        {
            while(CObjectPool.Inst==null)
            {
                yield return null;
            }

            while(!CObjectPool.Inst.IsInit)
            {
                yield return null;
            }

            while (!GameManager.Inst.IsGameEnd)
            {
                yield return null;

                if (m_spawnList.Count >= m_maxCount)
                {
                    continue;
                }

                if (m_spawnBase!=null)
                {
                    yield return new WaitForSeconds(m_spwanDelay);

                    GameObject unitObj = CObjectPool.Inst.GetUnit(m_spawnBase.UnitType,m_spawnBase.Index);

                    if (unitObj == null)
                    {
                        continue;
                    }

                    //GameObject unitObj = Instantiate(m_spawnBase.UnitPrefab, transform.position, Quaternion.identity);

                    CNpcUnit unit = unitObj.GetComponent<CNpcUnit>();

                    if (unit == null)
                    {
                        Debug.LogError(m_spawnBase + "not contained unit component");
                        break;
                    }

                    m_spawnList.Add(unit);
                    unitObj.transform.position = transform.position;
                    unitObj.transform.rotation = Quaternion.identity;
                    unitObj.SetActive(true);
                    unit.ResetUnit();

                    yield return null;

                    unit.StayLevel = m_subLevel;
                    unit.SpawnedPoint = this;

                    if (m_ways.Count < 1)
                        continue;

                    int nIdx = Random.Range(0, m_ways.Count);

                    unit.Movement.MoveWays = m_ways[nIdx].wayList;

                    yield return null;
                }

            }
        }

        yield return null;
    }

    public void RemoveUnitFromList(CUnit unit)
    {
        if (unit == null)
            return;

        if (!m_spawnList.Contains(unit))
            return;

        m_spawnList.Remove(unit);
    }

    public List<Transform> GetWay(int nIdx)
    {
        if (m_ways == null)
            return null;
        if (nIdx >= m_ways.Count)
            return null;

        return m_ways[nIdx].wayList;
    }

    [System.Serializable]
    private class Ways
    {
        [SerializeField]
        public List<Transform> wayList = new List<Transform>();
    }
}
