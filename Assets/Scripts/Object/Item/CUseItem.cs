using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUseItem : CItemBase
{
    [SerializeField]
    protected UseAnimType m_useAnimType;
    public UseAnimType UseAnimType { get { return m_useAnimType; } }

    [SerializeField]
    protected string m_effectName;
    public string EffectName { get { return m_effectName; } }

    [SerializeField]
    private int m_maxCount = 10;
    public int MaxCount { get { return m_maxCount; } }

    [SerializeField]
    private UseItemType m_useType = UseItemType.RECORVE;
    public UseItemType UseItemType { get { return m_useType; } }

    [SerializeField]
    private BuffType m_buffType = BuffType.None;
    public BuffType BuffType { get{ return m_buffType; } }

    [SerializeField]
    private int m_value = 10;
    public int Value { get { return m_value; } }
}

public enum UseItemType
{
    RECORVE,
    STAMINA,
    BUFF,
    SHOOT,
    BIND
}

public enum BuffType
{
    None,
    Element,
    Strength,
    Guard,
    Speed,
}

public enum UseAnimType
{
    DRINK,
    EAT,
    GRIB,
    BUFF,
    THROW,
    MAX
}
