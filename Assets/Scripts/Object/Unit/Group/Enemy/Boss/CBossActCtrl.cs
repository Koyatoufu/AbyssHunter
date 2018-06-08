using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CBossActCtrl : CEnemyActCtrl
{
    private CBossUnit m_bossUnit = null;

    private NavMeshAgent m_agent = null;

    public override void Init(CUnit unit)
    {
        base.Init(unit);

        m_bossUnit = unit as CBossUnit;
        m_agent = GetComponent<NavMeshAgent>();
    }

    public override void FixedProcess()
    {
        InAction = m_anim.GetBool("InAction");
        CanMove = m_anim.GetBool("CanMove");

        DetectedAction();
        DudgeHandle();
        MoveHandle();
    }

    protected override void DetectedAction()
    {
        switch (m_unit.Status.CurState)
        {
            case State.Normal:
                {
                    if (m_target != null)
                    {
                        if(m_target.Status.CurState == State.Dead)
                        {
                            ReleaseTarget();
                        }
                        else
                        {
                            TargetFunc();
                            return;
                        }   
                    }

                    Vector3 pos = transform.position;
                    pos.y += 1f;
                    pos += transform.forward;

                    RaycastHit[] hits =
                        Physics.SphereCastAll(pos, m_fDetectRange, transform.forward, m_fDetectRange, m_targetMask);

                    if (hits.Length < 1)
                    {
                        m_target = null;
                        return;
                    }

                    for (int i = 0; i < hits.Length; i++)
                    {
                        Vector3 dir = hits[i].point - transform.position;
                        dir = dir.normalized;

                        float fDot = Vector3.Dot(transform.forward, dir);

                        if (fDot < 0.6f)
                        {
                            continue;
                        }

                        CUnit unit = hits[i].transform.GetComponent<CPlayerUnit>();

                        if (unit == null)
                        {
                            continue;
                        }

                        if (m_target == null && unit.Status.CurState!=State.Dead)
                        {
                            m_target = unit;
                            continue;
                        }

                        if (Vector3.Distance(hits[i].transform.position, transform.position) <=
                            Vector3.Distance(m_target.transform.position, transform.position))
                        {
                            if(unit!=null && unit.Status.CurState != State.Dead)
                            m_target = unit;
                        }
                    }

                    if (m_target != null)
                    {
                        TargetFunc();
                    }
                }
                break;
            case State.Attack:
                {
                    if (m_target == null || m_target.Status.CurState == State.Dead)
                    {
                        ReleaseTarget();
                        return;
                    }
                    
                    if (Vector3.Distance(transform.position, m_target.transform.position) > m_missingRange)
                    {
                        ReleaseTarget();
                        return;
                    }

                    if (!CanAttack||InAction)
                        return;

                    if (Vector3.Distance(transform.position, m_target.transform.position) > ActionRange)
                        return;

                    if (m_unit.MainHook.Weapon.ActionList.Count < 1)
                    {
                        ReleaseTarget();
                        return;
                    }

                    Vector3 targetDir = Target.transform.position - transform.position;
                    targetDir.y = 0f;
                    if (targetDir == Vector3.zero)
                        targetDir = transform.forward;
                    Quaternion targetRot = Quaternion.LookRotation(targetDir);
                    transform.rotation = targetRot;

                    int nIdx = 0;
                    nIdx = Random.Range(0, (int)ActionType.Unique);
                    if(m_unit.Status.GetHpAmount()<0.3f && Random.value<0.5f)
                    {
                        nIdx = (int)ActionType.Unique;
                    }
                    CAction action = m_unit.MainHook.Weapon.ActionList[nIdx];
                    m_pastAnim = action.CurAction();
                    m_pastActType = action.Type;

                    m_anim.Play(m_pastAnim.animName);
                    
                    InAction = true;
                    CanMove = false;
                    CanAttack = false;
                }
                break;
            default:
                break;
        }
    }

    protected override void DetectedItemAction()
    {
        switch (m_unit.Status.CurState)
        {
            case State.Normal:
                {
                    if (InAction || IsUseItem)
                        return;
                }
                break;
            case State.Attack:
                {
                    if (InAction||IsUseItem)
                        return;
                }
                break;
        }
    }

    protected override void DudgeHandle()
    {
        base.DudgeHandle();
    }

    protected override void MoveHandle()
    {
        if(InAction||!CanMove)
        {
            m_anim.SetFloat("Vertical", 0f);
            m_anim.SetFloat("Horizontal", 0f);
            m_anim.SetBool("IsRun", false);
            return;
        }

        m_anim.SetFloat("Vertical", m_movement.MoveDir.z * MoveAmount);
        m_anim.SetFloat("Horizontal", m_movement.MoveDir.x * MoveAmount);
        m_anim.SetBool("IsRun", IsRun);
    }

    protected override IEnumerator RestCorutine()
    {
        return base.RestCorutine();
    }

    private void TargetFunc()
    {
        m_unit.ShowUI();
        m_unit.Status.CurState = State.Attack;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (CRootLevel.Inst.BgmSource != null && !CRootLevel.Inst.BgmSource.isPlaying && CSoundManager.Inst != null)
            CSoundManager.Inst.BgmPlay(CRootLevel.Inst.BgmSource, m_bossUnit.BgmClip);
        if (agent != null)
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);
        }
    }

    private void ReleaseTarget()
    {
        CanAttack = false;
        m_unit.Status.CurState = State.Normal;
        m_movement.CurMoveState = MoveState.NORMAL;
        m_unit.HideUI();
        if (CRootLevel.Inst.BgmSource != null && CSoundManager.Inst != null)
            CSoundManager.Inst.StopBgm(CRootLevel.Inst.BgmSource);
        if (m_agent != null)
        {
            m_agent.SetDestination(transform.position);
            m_agent.isStopped = true;
        }
        m_target = null;
    }

}
