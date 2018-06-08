using UnityEngine;

public class CGuardCollider : CBaseCollider
{
    [SerializeField]
    private int m_dmgLimit = 40;
    private int m_nDmg = 0;

    [SerializeField]
    private float m_dmgReduce = 0.75f;

    [SerializeField]
    private Stat.Resister m_resister = new Stat.Resister();

    public override void Init(CUnit unit, CWeapon weapon)
    {
        base.Init(unit, weapon);
    }

    public override void ApponentColliderIn(Stat.AttackStat attackStat,HitType hitType, ActionAnim actionAnim = null)
    {
        float fMul = actionAnim == null ? 1f : actionAnim.fMotionMagnifi;
        int nDmg = CalculateAttributes.CaculateDamage(attackStat, fMul, m_resister);
        nDmg = (int)(nDmg*m_dmgReduce);
        m_nDmg += nDmg;

        if(m_nDmg>=m_dmgLimit)
        {
            m_nDmg = 0;
            m_unit.GetDamage(attackStat, hitType, null, actionAnim);
        }
    }
}
