using UnityEngine;

public class CWearable : CItemBase,IEquipment
{
    [SerializeField]
    protected PartsType m_partsType;
    public PartsType PartsType { get { return m_partsType; } }

    [SerializeField]
    private Stat.Resister m_resister = new Stat.Resister();
    public Stat.Resister Resister { get { return m_resister; } }
}
