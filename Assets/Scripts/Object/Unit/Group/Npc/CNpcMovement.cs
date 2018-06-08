using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class CNpcMovement : CMovement
{
    protected NavMeshAgent m_navAgent;
    protected Vector3 m_moveTargetPos;
    protected Transform m_moveTarget;

    protected bool m_switch = false;

    public override void Init(CUnit unit)
    {
        base.Init(unit);

        m_navAgent = GetComponent<NavMeshAgent>();
    }

    protected override void Move()
    {
        if (m_unit.Status.CurState != State.Normal && m_unit.Status.CurState!=State.Scare)
        {
            m_navAgent.speed = 0f;
            m_navAgent.isStopped = true;
            return;
        }

        if (MoveTargetCheck())
        {
            if(m_moveTarget!=null)
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
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.fixedDeltaTime * m_actCtrl.MoveAmount);
        transform.rotation = targetRot;
    }

    protected virtual bool MoveTargetCheck()
    {
        if (m_moveWays == null)
            return false;

        if (m_moveWays.Count < 1)
            return false;

        if(m_moveTarget==null)
        {
            m_moveTarget = m_moveWays[0];
            CurMoveWayIdx = 0;
            CurScareWayIdx = 0;
        }

        m_actCtrl.MoveAmount = 0f;

        if (m_moveTarget == null)
            return false;

        MoveDir = m_moveTarget.position - transform.position;
        MoveDir = MoveDir.normalized;

        m_actCtrl.MoveAmount = MoveDir.magnitude;
        
        if (m_unit.Status.CurState==State.Normal)
        {
            m_navAgent.isStopped = false;
            m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed;

            if (Vector3.Distance(transform.position, m_moveTarget.position) <= 0.5f)
            {
                if (!m_switch)
                    CurMoveWayIdx++;
                else
                    CurMoveWayIdx--;

                if (CurMoveWayIdx >= m_moveWays.Count)
                {
                    m_actCtrl.MoveAmount = 0f;
                    m_actCtrl.RestAction();
                    CurMoveWayIdx = m_moveWays.Count-1;
                    m_switch = true;
                }
                    
                if (CurMoveWayIdx < 0)
                {
                    m_actCtrl.MoveAmount = 0f;
                    m_actCtrl.RestAction();
                    CurMoveWayIdx = 0;
                    m_switch = false;
                }

            }

            m_moveTarget = m_moveWays[CurMoveWayIdx];
            m_moveTargetPos = m_moveTarget.position;

            return true;
        }
        else if(m_unit.Status.CurState==State.Scare)
        {
            m_navAgent.isStopped = false;
            m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed * 2f;

            m_actCtrl.IsRun = true;

            if(m_moveState == MoveState.SCARE)
            {
                if (Vector3.Distance(transform.position, m_moveTarget.position) <= 1f)
                {
                    CNpcUnit npc = m_unit as CNpcUnit;
                    
                    if (m_unit != null)
                    {
                        m_unit.StayLevel = null;
                        if (m_unit.SpawnedPoint != null)
                            m_unit.SpawnedPoint.RemoveUnitFromList(m_unit);
                        m_unit.SpawnedPoint = null;
                    }

                    m_actCtrl.IsRun = false;
                    CObjectPool.Inst.PooledObject(gameObject, npc.UnitData, npc.UnitData.UnitType, npc.UnitData.Index);
                    return false;
                }
            }
            else
            {
                if (m_unit == null || m_unit.StayLevel == null)
                {
                    m_moveTarget = m_moveWays[CurMoveWayIdx];
                }
                else
                {
                    if (m_unit.StayLevel.ConnectLinks.Count > 0)
                    {
                        int nRand = Random.Range(0, m_unit.StayLevel.ConnectLinks.Count);
                        m_moveTarget = m_unit.StayLevel.ConnectLinks[nRand].transform;
                        m_moveTargetPos = m_moveTarget.position;
                    }
                    else
                    {
                        m_moveTarget = m_moveWays[CurMoveWayIdx];
                        m_moveTargetPos = m_moveTarget.position;
                    }
                    
                }

                m_moveState = MoveState.SCARE;

                return true;

            }

            

        }

        return false;
    }
}

public enum MoveState
{
    NORMAL,
    ROUND,
    FOLLOW,
    SCARE,
}