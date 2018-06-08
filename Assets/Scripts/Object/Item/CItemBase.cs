using UnityEngine;
using System.Collections.Generic;

public abstract class CItemBase : ScriptableObject
{
    [SerializeField]
    protected Sprite m_iconImg;
    public Sprite IconImg { get { return m_iconImg; } }

    [SerializeField]
    protected int m_itemIdx;
    public int ItemIdx { get { return m_itemIdx; } }

    [SerializeField]
    protected string m_name;
    public string Name { get { return m_name; } }

    [SerializeField]
    protected GameObject m_prefab;
    public GameObject Prefab { get { return m_prefab; } }
}