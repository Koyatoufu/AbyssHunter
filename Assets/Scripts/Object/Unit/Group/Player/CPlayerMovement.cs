using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CInputCtrl))]
public class CPlayerMovement : CMovement
{
    protected CInputCtrl m_input;
    protected CPlayerCamera m_camCtrl;

    public override void Init(CUnit unit)
    {
        base.Init(unit);
        m_input = GetComponent<CInputCtrl>();
        m_camCtrl = GetComponent<CPlayerUnit>().CameraCtrl;
    }

    public override void FixedProcess()
    {
        base.FixedProcess();
    }

    protected override void Move()
    {
        m_actCtrl.IsRun = false;

        m_rBody.drag = (m_actCtrl.MoveAmount > 0f || IsOnGround == false) ? 0f : 4f;

        if (!IsOnGround)
            return;
        if (!m_actCtrl.CanMove)
            return;
        
        float vertical = m_input.InputData.vertical;
        float horizontal = m_input.InputData.horizontal;

        Vector3 forward = m_camCtrl.transform.forward * vertical;
        Vector3 horizon = m_camCtrl.transform.right * horizontal;

        Vector3 moveVelocity = forward + horizon;
        //moveVelocity.Normalize();
        MoveDir = moveVelocity.normalized;

        m_actCtrl.MoveAmount = Mathf.Abs(vertical)+Mathf.Abs(horizontal);

        moveVelocity *= m_status.BaseStst.moveSpeed;
        if (m_actCtrl.IsUseItem)
        {
            m_actCtrl.MoveAmount *= 0.5f;
            moveVelocity *= 0.5f;
        }
        else
        {
            if (m_actCtrl.MoveAmount!=0 && m_input.InputData.btnInputs[(int)InputIndex.RB])
            {
                if(m_unit.Status.IsUsableStamina(2f))
                {
                    moveVelocity *= 2f;
                    m_actCtrl.IsRun = true;
                    m_unit.Status.StaminaReduce(5f * Time.deltaTime);
                }
            }
        }
        
        moveVelocity.y = 0f;

       m_rBody.velocity = moveVelocity;
    }

    protected override void Rotate()
    {
        Vector3 targetDir = MoveDir;//(IsLockOn && LockOnTrans != null && !IsRun) ? LockOnTrans.position - transform.position : MoveDir;
        targetDir = (m_camCtrl.IsLockOn && m_camCtrl.LockOnTarget != null && !m_actCtrl.IsRun && !m_actCtrl.InAction) ? 
            m_camCtrl.LockOnTarget.transform.position - transform.position:MoveDir;
        targetDir.y = 0f;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;
        Quaternion targetRot =
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.fixedDeltaTime * 5f * m_actCtrl.MoveAmount);
        transform.rotation = targetRot;
    }

}
