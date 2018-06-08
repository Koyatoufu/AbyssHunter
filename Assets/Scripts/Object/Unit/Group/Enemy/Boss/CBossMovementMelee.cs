using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class CBossMovementMelee : CEnemyMovementMelee
{
    private float m_fStayTime = 0f;

    [SerializeField]
    private float m_areaMoveTime = 300f;//분단위로 계산

    private float m_fRoundTime = 0f;
    
    private float m_roundAmount= 5f;

    private CBossUnit m_bossUnit = null;
    private int m_nCurSubRoot = 0;

    public override void Init(CUnit unit)
    {
        base.Init(unit);

        m_bossUnit = unit as CBossUnit;
        if(m_bossUnit==null)
        {
            //Destroy(this);
            return;
        }

        StartCoroutine(RootCoroutine());
    }
    private IEnumerator RootCoroutine()
    {
        while (CRootLevel.Inst == null)
            yield return null;
        
        while(CRootLevel.Inst.SubLevels.Count<m_bossUnit.UnitData.StayLevel[0])
            yield return null;

        for(int i=0;i<m_bossUnit.UnitData.StayLevel.Count;i++)
        {
            yield return null;
        }

        yield return null;
    }

    public override void FixedProcess()
    {
        base.FixedProcess();
    }

    protected override void Move()
    {
        if (m_unit.Status.CurState == State.Dead || !m_actCtrl.CanMove)
        {
            m_navAgent.speed = 0f;
            m_navAgent.isStopped = true;
            return;
        }

        if (MoveTargetCheck())
        {
            if (m_moveTarget != null)
                m_navAgent.SetDestination(m_moveTargetPos);
        }
    }

    protected override void Rotate()
    {
        Vector3 targetDir = MoveDir;
        targetDir.y = 0f;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;
        Quaternion targetRot =
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.fixedDeltaTime * 5f * m_actCtrl.MoveAmount);
        transform.rotation = targetRot;
    }

    protected override bool MoveTargetCheck()
    {
        switch (m_unit.Status.CurState)
        {
            case State.Normal:
                {
                    switch (m_moveState)
                    {
                        case MoveState.NORMAL:
                            {
                                //m_actCtrl.IsRun = false;
                                m_navAgent.isStopped = false;
                                m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed;

                                m_actCtrl.MoveAmount = 0f;

                                if (m_fStayTime >= m_areaMoveTime)
                                {
                                    m_nCurSubRoot++;
                                    if (m_nCurSubRoot >= m_bossUnit.UnitData.StayLevel.Count)
                                        m_nCurSubRoot = 0;
                                    int nIdx = m_bossUnit.UnitData.StayLevel[m_nCurSubRoot];
                                    m_unit.StayLevel = CRootLevel.Inst.SubLevels[nIdx];

                                    if (m_unit.StayLevel == null)
                                        return false;

                                    m_moveTarget = CRootLevel.Inst.SubLevels[nIdx].transform;
                                    m_moveTargetPos = m_moveTarget.position;
                                    
                                    m_moveState = MoveState.FOLLOW;
                                    m_fStayTime = 0f;

                                    return true;
                                }

                                m_fStayTime += Time.deltaTime;

                                if(m_moveWays!=null&&m_moveWays.Count>0&&m_moveTarget==null)
                                {
                                    CurMoveWayIdx = 0;
                                    m_moveTarget = m_moveWays[CurMoveWayIdx];
                                    m_moveTargetPos = m_moveTarget.position;
                                    return true;
                                }

                                if(m_moveTarget!=null)
                                {
                                    MoveDir = (m_moveTargetPos - transform.position).normalized;

                                    if(Vector3.Distance(m_moveTarget.position,transform.position)<2.5f)
                                    {
                                        CurMoveWayIdx++;
                                        if (m_moveWays.Count < 1)
                                        {
                                            CurMoveWayIdx = 0;
                                        }
                                        else
                                        {
                                            if (CurMoveWayIdx >= m_moveWays.Count)
                                                CurMoveWayIdx = 0;

                                            m_moveTarget = m_moveWays[CurMoveWayIdx];
                                            m_moveTargetPos = m_moveTarget.position;
                                            return true;
                                        }
                                    }
                                }
                                else
                                {
                                    MoveDir = Vector3.zero;
                                }

                                m_actCtrl.MoveAmount = MoveDir.magnitude * 0.75f;
                                
                            }
                            break;
                        case MoveState.FOLLOW:
                            {
                                //m_actCtrl.IsRun = false;
                                m_navAgent.isStopped = false;
                                m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed;
                                MoveDir = (m_moveTargetPos - transform.position).normalized;
                                m_actCtrl.MoveAmount = MoveDir.magnitude;

                                if(m_moveTarget == null || m_unit.StayLevel==null || m_moveTarget != m_unit.StayLevel.transform)
                                {
                                    m_moveState = MoveState.NORMAL;
                                    m_navAgent.isStopped = true;
                                    return false;
                                }

                                if(Vector3.Distance(transform.position,m_moveTarget.position)<3f)
                                {
                                    m_moveState = MoveState.NORMAL;
                                    m_navAgent.isStopped = true;
                                    m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed;

                                    if (m_unit.StayLevel.SpwanPoints.Count > 0)
                                    {
                                        m_moveWays = m_unit.StayLevel.SpwanPoints[0].GetWay(0);

                                    }   

                                    return false;
                                }
                            }
                            break;
                        case MoveState.ROUND:
                            {
                                //m_actCtrl.IsRun = false;
                                m_navAgent.isStopped = true;
                                m_moveState = MoveState.NORMAL;
                            }
                            break;
                    }
                }
                break;
            case State.Attack:
                {
                    switch (m_moveState)
                    {
                        case MoveState.NORMAL:
                            {
                                //m_actCtrl.IsRun = false;
                                m_navAgent.isStopped = false;

                                if (m_actCtrl.InAction)
                                    return false;

                                m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed;
                                m_moveTarget = m_actCtrl.Target.transform;
                                MoveDir = (m_moveTarget.position - transform.position).normalized;

                                if (Vector3.Distance(transform.position, m_moveTarget.position) > m_actCtrl.ActionRange)
                                {
                                    m_navAgent.isStopped = true;
                                    m_moveState = MoveState.FOLLOW;
                                    m_moveTargetPos = m_moveTarget.position + new Vector3(0f,0f,2f);
                                    return true;
                                }
                                else
                                {
                                    m_navAgent.isStopped = true;
                                    m_roundAmount = Random.Range(0f, 3f);
                                    Vector3 randomPos = Random.Range(0, 1) == 0 ? Vector3.right : Vector3.left;
                                    randomPos *= 5f;
                                    m_moveTargetPos = m_moveTarget.position + randomPos;
                                    m_moveState = MoveState.ROUND;
                                    return true;
                                }
                            }
                            //break;
                        case MoveState.FOLLOW:
                            {
                                //m_actCtrl.IsRun = true;
                                m_navAgent.isStopped = false;
                                MoveDir = (m_moveTarget.position - transform.position).normalized;
                                m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed * 1.2f;
                                m_actCtrl.MoveAmount = MoveDir.magnitude;

                                if (Vector3.Distance(transform.position,m_moveTarget.position)<=m_actCtrl.ActionRange)
                                {
                                    if(Random.value<0.3f)
                                    {
                                        m_moveState = MoveState.ROUND;
                                        m_roundAmount = Random.Range(1f, 2f);
                                        Vector3 randomPos = Random.Range(0, 1) == 0 ? new Vector3(1f, 0f, 1f) : new Vector3(-1, 0f, 1f);
                                        randomPos *= 5f;
                                        m_moveTargetPos = m_moveTarget.position + randomPos;
                                    }
                                    else
                                    {
                                        m_actCtrl.CanAttack = true;
                                        m_moveState = MoveState.NORMAL;
                                    }
                                }
                                else
                                {
                                    m_moveTargetPos = m_moveTarget.position;
                                    return true;
                                }
                            }
                            break;
                        case MoveState.ROUND:
                            {
                                //m_actCtrl.IsRun = false;
                                m_navAgent.isStopped = false;
                                m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed*0.75f;
                                MoveDir = (m_actCtrl.Target.transform.position - transform.position).normalized;
                                m_actCtrl.MoveAmount = MoveDir.magnitude;

                                if (m_fRoundTime>=m_roundAmount)
                                {
                                    m_fRoundTime = 0f;

                                    if (Vector3.Distance(transform.position, m_moveTargetPos) > m_actCtrl.ActionRange)
                                    {
                                        m_navAgent.isStopped = true;
                                        m_moveState = MoveState.FOLLOW;
                                        m_moveTargetPos = m_moveTarget.position + new Vector3(0f, 0f, 2f);
                                        return true;
                                    }
                                                                        
                                    m_actCtrl.CanAttack = true;
                                    m_moveState = MoveState.NORMAL;
                                    return false;
                                }
                                else
                                {
                                    m_fRoundTime += Time.fixedDeltaTime;
                                    if(Vector3.Distance(transform.position,m_moveTargetPos)<1f)
                                    {
                                        Vector3 randomPos = Random.Range(0, 1) == 0 ? new Vector3(1f, 0f, 1f) : new Vector3(-1, 0f, 1f);
                                        randomPos *= 3f;
                                        m_moveTargetPos = m_moveTarget.position + randomPos;
                                        return true;
                                    }
                                    return true;
                                }
                            }
                    }
                }
                break;
            case State.Scare:
                {
                    CSubLevel fleePlace = CRootLevel.Inst.SubLevels[m_bossUnit.UnitData.StayLevel[0]];
                    if (m_bossUnit.StayLevel==fleePlace)
                    {
                        if(Vector3.Distance(transform.position, m_moveTarget.position) <3f)
                        {
                            m_unit.Status.CurState = State.Normal;
                            m_moveWays = m_unit.StayLevel.SpwanPoints[0].GetWay(0);
                            m_moveTarget = m_moveWays[0];
                            return false;
                        }
                    }
                    else
                    {
                        m_moveTarget = fleePlace.SpwanPoints[0].transform;
                        m_unit.StayLevel = fleePlace;
                        return true;
                    }
                }
                break;
        }
        return false;
    }
}
