using System.Collections;
using Stat;
using UnityEngine;

public class CEnemyUnit : CNpcUnit
{
    [SerializeField]
    protected CWeaponHook m_attackHook = null;
    public override CWeaponHook MainHook { get { return m_attackHook; } }

    protected override void Start()
    {
        base.Start();

        m_unitTag = "Enemy";
        gameObject.tag = m_unitTag;

        if(MainHook!=null)
            MainHook.Init(this);

        if (m_unitUI != null)
            m_unitUI.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        m_actCtrl.Process();
        if(m_unitUI!=null)
            m_unitUI.Process();
    }

    public override void ResetUnit()
    {
        if (m_status != null)
            m_status.ResetState();

        if (m_actCtrl != null)
        {
            m_actCtrl.ResetActionUnit();
        }

        if (m_unitUI != null)
        {
            m_unitUI.ResetUI();
            m_unitUI.gameObject.SetActive(false);
        }
    }

    public override bool GetDamage(AttackStat attackStat, HitType hitType = HitType.Normal, CUnit unit = null, ActionAnim actionAnim = null)
    {
        bool isDead = false;

        if (m_status.CurState == State.Dead)
            return false;

        float fMotionMul = actionAnim != null ? actionAnim.fMotionMagnifi : 1f;
        if (m_status.GetDamage(attackStat, out isDead, fMotionMul))
        {
            StartCoroutine(HitUiCoroutine());
            MainHookClose();
            m_actCtrl.ResetAttack();
            m_actCtrl.GetHitAction(hitType);

            if (unit == null)
                return true;

            if (m_actCtrl.Target!=null)
            {
                if (unit.tag == "Player")
                    m_actCtrl.Target = unit;
            }

            return true;
        }

        if (isDead)
        {
            DeathProcess();
        }

        return false;
    }

    private IEnumerator HitUiCoroutine()
    {
        if (m_unitUI != null)
        {
            m_unitUI.gameObject.SetActive(true);
            yield return null;
            m_unitUI.SetHPGuageFill(m_status.GetHpAmount());
            yield return new WaitForSeconds(0.5f);
            m_unitUI.HideUI();
            yield return null;
        }

        yield return null;
    }

    protected override void FixedUpdate()
    {
        m_actCtrl.FixedProcess();
        m_movement.FixedProcess();
    }

}
