using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CInputCtrl))]
public class CPlayerActCtrl : CActionCtrl
{
    protected CInputCtrl m_input;

    protected CPlayerCamera m_camCtrl;

    public override void Init (CUnit unit)
    {
        base.Init(unit);

        m_input = GetComponent<CInputCtrl>();
        m_camCtrl = GetComponent<CPlayerUnit>().CameraCtrl;
    }

    public override void FixedProcess()
    {
        base.FixedProcess();

        IsSub = false;
        m_anim.SetBool("IsSub",IsSub);

        DetectedAction();

        if (InAction)
        {
            m_anim.applyRootMotion = true;
            m_anim.SetFloat("Vertical", 0f);
            m_anim.SetFloat("Horizontal", 0f);
            return;
        }

        if (!CanMove)
        {
            m_anim.SetFloat("Vertical", 0f);
            m_anim.SetFloat("Horizontal", 0f);
            return;
        }

        DetectedItemAction();
        DudgeHandle();
        MoveHandle();
    }

    protected override void DetectedAction()
    {
        if (!CanAttack || IsUseItem ||!m_movement.IsOnGround)
            return;

        if (!m_input.InputData.btnInputs[(int)InputIndex.X] && !m_input.InputData.btnInputs[(int)InputIndex.Y] && 
                !m_input.InputData.btnInputs[(int)InputIndex.LB] && !m_input.InputData.btnInputs[(int)InputIndex.LT])
            return;

        if (m_unit.MainHook == null)
            return;

        CWeapon weapon = m_unit.MainHook.Weapon;
        if (weapon == null)
            return;

        ActionAnim actAnim = null;
        ActionType type = ActionType.Max;

        #region InputCheck
        if (m_input.InputData.btnInputs[(int)InputIndex.X])
        {
            type = ActionType.Normal;
            actAnim = weapon.ActionList[(int)type].CurAction();
        }
        if (m_input.InputData.btnInputs[(int)InputIndex.Y])
        {
            type = ActionType.Strong;
            actAnim = weapon.ActionList[(int)type].CurAction();
        }
        if (m_input.InputData.btnInputs[(int)InputIndex.LB])
        {
            type = ActionType.Sub;
            actAnim = weapon.ActionList[(int)type].CurAction();
        }
        if (m_input.InputData.btnInputs[(int)InputIndex.LT])
        {
            type = ActionType.Unique;
            actAnim = weapon.ActionList[(int)type].CurAction();
        }
        #endregion

        if (type == ActionType.Max)
            return;
        
        if (actAnim == null)
            return;

        if (string.IsNullOrEmpty(actAnim.animName))
            return;

        #region pastAttackTypeCheck
        if(m_pastAnim!=null)
        {
            switch (type)
            {
                case ActionType.Normal:
                    if (!m_pastAnim.canNormalCancel)
                        return;
                    break;
                case ActionType.Strong:
                    if (!m_pastAnim.canStrongCancel)
                        return;
                    break;
                case ActionType.Sub:
                    if (!m_pastAnim.canSubCancel)
                        return;
                    break;
                case ActionType.Unique:
                    if (!m_pastAnim.canUniqueCancel)
                        return;
                    break;
            }
        }
        #endregion

        m_pastAnim = actAnim;
        m_pastActType = type;

        m_anim.Play(actAnim.animName);
        m_anim.applyRootMotion = true;

        if (type==ActionType.Sub)
        {
            //TODO: 주술 아이템 사용

            IsSub = true;
            m_anim.SetBool("IsSub", IsSub);
            return;
        }

        CanMove = false;
        InAction = true;

        if (m_pastAnim.isAttack)
        {
            CanAttack = false;
        }

        m_anim.SetBool("CanMove", CanMove);
        m_anim.SetBool("InAction", InAction);

    }

    protected override void DetectedItemAction()
    {
        if (!CanMove || IsUseItem || !m_movement.IsOnGround || InAction)
            return;

        if (!m_input.InputData.btnInputs[(int)InputIndex.B])
            return;
        
        CPlayerUnit player = m_unit as CPlayerUnit;
        if (player == null)
            return;
        CInventory inventory = player.Inventory;
        if (inventory == null)
            return;

        IsUseItem = true;
        m_anim.SetBool("IsUseItem", IsUseItem);

        CUseItem item = inventory.ItemUse(inventory.CurItemSelect) as CUseItem;
        int nIdx = (int)UseAnimType.MAX;

        if (item == null)
        {
            m_anim.Play("Item" + nIdx);
            return;
        }

        nIdx = (int)item.UseAnimType;
        
        // 아이템 사용 번호에 따라 아이템 액션 변경 및 던지는 물체의 경우 발사체 소환
        m_anim.Play("Item"+nIdx);

        m_unit.CurUseItem = item;
    }

    protected override void MoveHandle()
    {
        
        m_anim.applyRootMotion = false;
        float moveAmount = MoveAmount;

        if(m_camCtrl!=null&&m_camCtrl.IsLockOn)
        {
            m_anim.SetBool("IsLockOn", true);

            Vector3 relativeDir = transform.InverseTransformDirection(m_movement.MoveDir);
            float fH = relativeDir.x;
            float fV = relativeDir.z;

            //Debug.Log("movedir:"+m_movement.MoveDir);
            
            m_anim.SetFloat("Vertical", fV,0.4f,Time.fixedDeltaTime);
            m_anim.SetFloat("Horizontal", fH,0.4f,Time.fixedDeltaTime);
        }
        else
        {
            m_anim.SetBool("IsLockOn", false);
            m_anim.SetFloat("Vertical", moveAmount);
        }

        m_anim.SetBool("IsRun", IsRun);
    }

    protected override void DudgeHandle()
    {
        if (!m_input.InputData.btnInputs[(int)InputIndex.RT]||IsUseItem||m_animHook.IsDudge)
            return;

        if (!m_unit.Status.IsUsableStamina(10f))
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

        m_anim.SetFloat("DudgeAmount", amount);

        CanMove = false;
        InAction = true;
        m_anim.CrossFade("Dudges", 0.125f);

        m_animHook.InitForDudge();
        m_unit.Status.StaminaReduce(10f);
    }

    public override void RestAction()
    {
        if (InAction || !CanMove || IsUseItem)
            return;

        m_anim.Play("Sitting");
        m_anim.SetBool("IsRest",true);

        CanMove = false;
        InAction = true;

        StartCoroutine(RestCorutine()) ;
    }

    protected override IEnumerator RestCorutine()
    {
        m_unit.Status.RestState();

        yield return new WaitForSeconds(5f);

        m_unit.Status.CurState = State.Normal;
        m_anim.SetBool("IsRest",false);

        yield return null;
    }

    public override void ResetAttack()
    {
        CancelAttack();

        if (m_pastActType == ActionType.Max)
            return;

        List<CAction> actionList = m_unit.MainHook.Weapon.ActionList;

        for (int i=0;i<actionList.Count;i++)
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
}
