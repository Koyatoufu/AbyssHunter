using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class CBaseCollider : MonoBehaviour
{
    protected CUnit m_unit;
    public CUnit CurUnit { get { return m_unit; } }

    protected CWeapon m_weapon;
    public CWeapon Weapon { get { return m_weapon; } }

    public virtual void Init(CUnit unit,CWeapon weapon)
    {
        m_unit = unit;
        m_weapon = weapon;
    }

    protected virtual void OnTriggerEnter(Collider other) { }

    public virtual void ApponentColliderIn(Stat.AttackStat attackStat,HitType hitType,ActionAnim actionAnim = null) { }
}
