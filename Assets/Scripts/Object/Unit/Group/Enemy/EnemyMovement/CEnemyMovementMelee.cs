using UnityEngine;
using UnityEngine.AI;

public class CEnemyMovementMelee : CNpcMovement
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
        if (m_unit.Status.CurState == State.Dead|| !m_actCtrl.CanMove)
        {
            m_navAgent.speed = 0f;
            m_navAgent.isStopped = true;
            return;
        }

        if (MoveTargetCheck())
        {
            if (m_moveTarget != null)
                m_navAgent.SetDestination(m_moveTarget.position);
        }
    }

    protected override void Rotate()
    {
        base.Rotate();
    }

    protected override bool MoveTargetCheck()
    {
        if (m_unit.Status.CurState == State.Normal)
        {
            m_actCtrl.IsRun = false;
            m_navAgent.isStopped = false;

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

            m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed;

            if (Vector3.Distance(transform.position, m_moveTarget.position) <= m_actCtrl.ActionRange)
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
            m_actCtrl.IsRun = false;
            m_navAgent.isStopped = false;

            if (m_unit.ActCtrl == null || m_unit.ActCtrl.Target == null)
            {
                Debug.Log("NULL");
                m_unit.Status.CurState = State.Normal;
                return false;
            }

            if(m_actCtrl.InAction)
            {
                m_navAgent.isStopped = true;
                return false;
            }

            m_actCtrl.IsRun = true;

            m_navAgent.speed = m_unit.Status.BaseStst.moveSpeed * 1.5f;

            m_moveTarget = m_unit.ActCtrl.Target.transform;

            MoveDir = m_moveTarget.position - transform.position;
            MoveDir = MoveDir.normalized;

            if (Vector3.Distance(transform.position, m_moveTarget.position) <= m_actCtrl.ActionRange)
            {
                m_actCtrl.IsRun = false;
                m_actCtrl.CanAttack = true;
                m_navAgent.isStopped = true;
                return false;
            }

            return true;
        }

        return false;
    }
}
