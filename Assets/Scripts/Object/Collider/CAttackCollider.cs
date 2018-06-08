using UnityEngine;

public class CAttackCollider : CBaseCollider
{
    public override void Init(CUnit unit,CWeapon weapon)
    {
        base.Init(unit,weapon);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (m_unit == null)
            return;

        HitType hitType = HitType.None;
        ActionAnim actionAnim = m_unit.ActCtrl.PastAnim;

        CBaseCollider baseCollider = other.GetComponent<CBaseCollider>();
        if(baseCollider!=null)
        {
            baseCollider.ApponentColliderIn(m_weapon.WeaponStat,hitType,actionAnim);

            if (baseCollider is CArmorCollider || baseCollider is CGuardCollider)
                return;
        }

        CUnit otherUnit = other.GetComponent<CUnit>();

        if (actionAnim != null)
        {
            hitType = m_unit.ActCtrl.PastAnim.hitType;
        }

        if (otherUnit != null && otherUnit.UnitTag != m_unit.UnitTag)
        {
            otherUnit.GetDamage(m_weapon.WeaponStat, hitType, m_unit, actionAnim);
            if(CObjectPool.Inst!=null)
            {
                GameObject hitObject = null;
                CParticle particle = null;
                switch(m_weapon.WeaponStat.physicsType)
                {
                    case Stat.PhysicalType.Slash:
                        hitObject = CObjectPool.Inst.GetEffect(EffectType.Particle, 1);
                        break;
                    case Stat.PhysicalType.Strike:
                        hitObject = CObjectPool.Inst.GetEffect(EffectType.Particle, 0);
                        break;
                }

                if (hitObject == null)
                    return;
                particle = hitObject.GetComponent<CParticle>();
                if (particle != null)
                {
                    particle.Show(m_unit);
                    hitObject.transform.position = transform.position;
                    hitObject.SetActive(true);
                }
                    
            }
        }
        
    }
}
