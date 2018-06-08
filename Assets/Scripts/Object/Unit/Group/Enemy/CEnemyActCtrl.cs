using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CEnemyActCtrl : CNpcActCtrl
{
    [SerializeField]
    protected float m_fDetectRange = 5f;

    protected bool CanDudge { get; set; }

    [SerializeField]
    protected LayerMask m_targetMask = 0;

    [SerializeField]
    protected float m_fWaitLimit = 1f;
    protected float m_fWaitTime = 0f;

    public override void Init(CUnit unit)
    {
        base.Init(unit);
    }

    public override void FixedProcess()
    {
        InAction = m_anim.GetBool("InAction");
        CanMove = m_anim.GetBool("CanMove");

        if (m_unit.Status.CurState == State.Dead)
            return;

        DetectedAction();
        DudgeHandle();
        MoveHandle();
    }

    public override void RestAction()
    {
        StartCoroutine(RestCorutine());
    }

    protected override void DudgeHandle()
    {
        if (!CanDudge)
            return;

        if (InAction||!CanMove)
            return;

        float amount = (MoveAmount > 0.3f) ? 1f : 0f;
        if (amount != 0)
        {
            if (m_movement.MoveDir == Vector3.zero)
                m_movement.MoveDir = transform.forward;
            Quaternion targetRot = Quaternion.LookRotation(m_movement.MoveDir);
            transform.rotation = targetRot;
            m_animHook.RMultiplier = m_unit.Status.BaseStst.dudgeSpeed;
        }
        else
        {
            m_movement.MoveDir = -transform.forward;
            m_animHook.RMultiplier = 1.3f;
        }

        m_anim.SetFloat("Vertical", amount);

        CanMove = false;
        InAction = true;
        m_anim.CrossFade("Dudges", 0.2f);

        m_animHook.InitForDudge();
    }

    protected override void MoveHandle()
    {
        if (InAction || !CanMove)
        {
            m_anim.SetFloat("Vertical", 0f);
            m_anim.SetFloat("Horizontal", 0f);
            m_anim.SetBool("IsRun", false);
            return;
        }

        m_anim.SetFloat("Vertical", MoveAmount);
        m_anim.SetBool("IsRun", IsRun);
    }

    protected override IEnumerator RestCorutine()
    {
        base.RestCorutine();

        yield return null;
    }

    protected override void DetectedAction()
    {
        //Debug.Log(m_unit.Status.CurState);

        switch (m_unit.Status.CurState)
        {
            case State.Normal:
                {
                    Vector3 pos = transform.position;
                    pos.y += 0.5f;
                    pos += transform.forward;

                    RaycastHit[] hits = 
                        Physics.SphereCastAll(pos, m_fDetectRange, transform.forward, m_fDetectRange , m_targetMask);

                    if (hits.Length < 1)
                    {
                        m_target = null;
                        return;
                    }

                    for (int i=0;i<hits.Length;i++)
                    {
                        Vector3 dir = hits[i].point - transform.position;
                        dir = dir.normalized;

                        float fDot = Vector3.Dot(transform.forward, dir);

                        if(fDot < 0.4f)
                        {
                            continue;
                        }

                        CUnit unit = hits[i].transform.GetComponent<CPlayerUnit>();

                        if(unit==null)
                        {
                            continue;
                        }

                        if (m_target == null)
                        {
                            m_target = unit;
                            continue;
                        }

                        if(Vector3.Distance(hits[i].transform.position,transform.position)<=
                            Vector3.Distance(m_target.transform.position,transform.position))
                        {
                            m_target = unit;
                        }
                    }

                    if(m_target!=null)
                    {
                        m_unit.Status.CurState = State.Attack;
                    }

                }
                break;
            case State.Attack:
                {
                    if (!CanAttack)
                    {
                        return;
                    }

                    if (m_unit.MainHook == null)
                    {
                        m_unit.Status.CurState = State.Normal;
                        return;
                    }
                        
                    if(m_target==null)
                    {
                        m_unit.Status.CurState = State.Normal;
                        return;
                    }
                    if(m_target.Status.CurState==State.Dead)
                    {
                        m_unit.Status.CurState = State.Normal;
                        return;
                    }

                    if (Vector3.Distance(transform.position, m_target.transform.position) > m_missingRange)
                    {
                        CanAttack = false;
                        m_unit.Status.CurState = State.Normal;
                        return;
                    }
                    if (Vector3.Distance(transform.position, m_target.transform.position) > ActionRange)
                    {
                        return;
                    }

                    //TODO: 플레이어가 유닛이 있는 레벨에서 벗어나면 노멀상태로

                    if (m_unit.MainHook.Weapon.ActionList.Count < 1)
                    {
                        m_unit.Status.CurState = State.Normal;
                        return;
                    }

                    Vector3 targetDir = Target.transform.position - transform.position;
                    targetDir.y = 0f;
                    if (targetDir == Vector3.zero)
                        targetDir = transform.forward;
                    Quaternion targetRot = Quaternion.LookRotation(targetDir);
                    transform.rotation = targetRot;

                    int nIdx = Random.Range(0, m_unit.MainHook.Weapon.ActionList.Count);
                    CAction action = m_unit.MainHook.Weapon.ActionList[nIdx];
                    m_pastAnim = action.CurAction();
                    m_pastActType = action.Type;

                    m_anim.Play(m_pastAnim.animName);
                    
                    InAction = true;
                    CanMove = false;
                    CanAttack = false;

                    m_anim.SetBool("InAction", InAction);
                    m_anim.SetBool("CanMove", CanMove);
                }
                break;
            default:
                break;
        }
    }

    public override void ResetAttack()
    {
        CancelAttack();

        if (m_pastActType == ActionType.Max)
            return;

        List<CAction> actionList = m_unit.MainHook.Weapon.ActionList;

        for (int i = 0; i < actionList.Count; i++)
        {
            actionList[i].ResetActionAnim();
        }

        m_pastActType = ActionType.Max;
        m_pastAnim = null;
    }

    public override bool CancelAttack()
    {
        if (CanAttack)
            return false;

        CanAttack = true;
        return true;
    }

    protected override IEnumerator HitCoroutine()
    {
        m_unit.MainHook.CloseAllCollider();

        yield return new WaitForSeconds(2f);

        m_unit.Status.CurState = State.Normal;

        yield return null;
    }
}
