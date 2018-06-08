using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stat;

public class CBossUnit : CEnemyUnit
{
    [SerializeField]
    private AudioClip m_bgmClip = null;
    public AudioClip BgmClip { get { return m_bgmClip; } }

    [SerializeField]
    private CWeaponHook m_subHook = null;
    public override CWeaponHook SubHook { get { return m_subHook;} }

    protected override void Start()
    {
        base.Start();

        m_unitTag = "Boss";
        gameObject.tag = m_unitTag;

        m_subHook.Init(this);

        if (m_unitUIPrefab!=null)
        {
            GameObject uiObj = Instantiate(m_unitUIPrefab, CStageUIManager.Inst.transform);
            uiObj.name = m_unitUIPrefab.name;

            m_unitUI = uiObj.GetComponent<CUnitUI>();
            if (m_unitUI != null)
            {
                m_unitUI.Init(this);
                m_unitUI.gameObject.SetActive(false);
            }
                
        }
    }

    protected override void Update()
    {
        m_actCtrl.Process();
        if (m_unitUI != null)
            m_unitUI.Process();
    }

    protected override void FixedUpdate()
    {
        m_actCtrl.FixedProcess();
        m_movement.FixedProcess();
    }

    public override void ResetUnit()
    {
        base.ResetUnit();

        if (CRootLevel.Inst != null)
            CRootLevel.Inst.BgmSource.Stop();
    }

    public override bool GetDamage(AttackStat attackStat, HitType hitType = HitType.Normal, CUnit unit = null, ActionAnim actionAnim = null)
    {
        bool isDead = false;

        if (m_status.CurState == State.Dead)
            return false;

        float fMotionMul = actionAnim != null ? actionAnim.fMotionMagnifi : 1f;
        if (m_status.GetDamage(attackStat, out isDead, fMotionMul))
        {
            if (m_unitUI != null)
            {
                m_unitUI.SetHPGuageFill(m_status.GetHpAmount());
            }
            MainHookClose();
            m_actCtrl.ResetAttack();
            m_actCtrl.GetHitAction(hitType);

            if (unit == null)
                return true;

            if (m_actCtrl.Target != null)
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

    protected override void DeathProcess()
    {
        base.DeathProcess();
        
        if(CRootLevel.Inst.BgmSource.isPlaying)
        {
            CRootLevel.Inst.BgmSource.Stop();
        }

        if(CQuestManager.Inst!=null)
        {
            if(UnitData!=null)
                CQuestManager.Inst.QuestCheck(UnitData);
        }
    }

}
