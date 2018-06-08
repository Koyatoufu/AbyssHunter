using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUnitData : ScriptableObject
{
    [SerializeField]
    private GameObject m_unitPrefab = null;
    public GameObject UnitPrefab { get { return m_unitPrefab; } }

    [SerializeField]
    private Sprite m_iconImage = null;
    public Sprite IconImage { get { return m_iconImage; } }

    [SerializeField]
    private int m_index = 0;
    public int Index { get { return m_index; } }

    [SerializeField]
    private string m_name = "";
    public string Name { get { return m_name; } }

    [SerializeField]
    private UnitType m_unitType = UnitType.Npc;
    public UnitType UnitType { get { return m_unitType; } }

    [SerializeField]
    private Stat.Base m_baseStat = new Stat.Base();
    public Stat.Base BaseStat { get { return m_baseStat; } }

    [SerializeField]
    private List<int> m_stayLevel = new List<int>();
    public List<int> StayLevel { get { return m_stayLevel; } }

    [SerializeField]
    private List<CItemBase> m_putItems = null;
    public List<CItemBase> PutItems { get { return m_putItems; } }
}

public enum UnitType
{
    Npc,
    Enemy,
    Boss
}