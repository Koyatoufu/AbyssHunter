using UnityEngine;

public class CSpellItem : CUseItem
{
    [SerializeField]
    private int m_effectIndex = 0;
    public int EffectIndex { get { return m_effectIndex; } }

    [SerializeField]
    public Stat.Element m_element;
    public Stat.Element Element { get { return m_element; } }

}
