using UnityEngine;
using System.Collections.Generic;

public class CWeapon : CItemBase ,IEquipment
{
    [SerializeField]
    private WeaponAttached m_attachedType = WeaponAttached.HAND_RIGHT;
    public WeaponAttached AttachedType { get { return m_attachedType; } }

    [SerializeField]
    private Stat.AttackStat m_weaponStat = new Stat.AttackStat();
    public Stat.AttackStat WeaponStat { get { return m_weaponStat; } }

    [SerializeField]
    private WeaponType m_weaponType = WeaponType.HAND;
    public WeaponType WeaponType { get { return m_weaponType; } }

    [SerializeField]
    private GameObject m_subWeapon = null;
    public GameObject SubWeapon { get { return m_subWeapon; } }

    [SerializeField]
    private List<CAction> m_actionList = new List<CAction>((int)ActionType.Max);
    public List<CAction> ActionList { get { return m_actionList; } }

    private const int DIV_DURABILLITY = 50;
    [SerializeField]
    private int m_durabillity = 175;
    public int Durabillity
    {
        get
        {
            return m_durabillity;
        }
        set
        {
            if (m_durabillity < 0)
                return;
            m_durabillity = value;
        }
    }
    public DurabillityRank CurDurRank
    {
        get
        {
            //TODO: 현재 내구도 랭크는 내구도를 50으로 나눈 몫을 랭크로 지정하지만 차후 문제 시 변경
            DurabillityRank rank = (DurabillityRank)(m_durabillity/DIV_DURABILLITY);
            return rank;
        }
    }

    [SerializeField]
    private SlotIdx m_waitSlot = SlotIdx.Waist;
    public SlotIdx WaitSlot { get { return m_waitSlot; } }

    [SerializeField]
    protected PartsType m_partsType;
    public PartsType PartsType { get { return m_partsType; } }
}

public enum WeaponAttached
{
    HAND_RIGHT,
    HAND_BOTH
}

public enum WeaponType
{
    HAND,
    SWORD,
    GREATSWORD,
    MACE,
    RANCE,
    BOW,
}

public enum DurabillityRank
{ E=0, D=1, C=2, B=3, A=4, S=5 }