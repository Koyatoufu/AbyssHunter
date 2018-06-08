using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyMovementShoot : CNpcMovement
{
    public override void Init(CUnit unit)
    {
        base.Init(unit);
    }

    public override void FixedProcess()
    {
        base.FixedProcess();
    }

    protected override void Move()
    {
        base.Move();
    }

    protected override bool MoveTargetCheck()
    {
        if (m_unit.Status.CurState == State.Normal)
        {
            if (m_moveWays == null)
                return false;

            if (m_moveWays.Count < 1)
                return false;

            if (m_moveTarget == null)
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

            m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed * 0.75f;

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
                    CurMoveWayIdx = m_moveWays.Count - 1;
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

            return true;
        }
        else if (m_unit.Status.CurState == State.Attack)
        {
            if (m_moveTarget == null)
            {
                return false;
            }   
        }

        return false;
    }

    protected override void Rotate()
    {
        base.Rotate();
    }
}
