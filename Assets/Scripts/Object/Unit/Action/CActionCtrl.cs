using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CMovement))]
public abstract class CActionCtrl : MonoBehaviour
{
    #region Instance

    [SerializeField]
    protected Collider m_detectCol = null;

    protected CUnit m_target = null;
    public CUnit Target { get { return m_target; } set { m_target = value; } }

    protected Animator m_anim;
    public Animator Anim { get { return m_anim; } }

    protected Rigidbody m_rBody;
    public Rigidbody RBody { get { return m_rBody; } }
    
    protected CAnimationHook m_animHook;
    protected CMovement m_movement;

    protected CUnit m_unit;

    protected bool m_isDeathStart = false;

    protected bool m_isStiff;
    [SerializeField]
    protected const float STIFF_MAX = 1f;

    [SerializeField]
    protected float m_actionRange = 3f;
    public float ActionRange { get { return m_actionRange; } }

    [SerializeField]
    protected float m_missingRange = 20f;

    #endregion

    #region ActionAnim

    protected ActionAnim m_pastAnim;
    public ActionAnim PastAnim { get { return m_pastAnim; } }

    protected ActionType m_pastActType;
    public ActionType PastActionType { get { return m_pastActType; } }

    #endregion

    #region NOInctance_Properties

    public bool InAction { get; protected set; }
    public bool IsUseItem { get; protected set; }
    public bool CanMove { get; protected set; }
    public bool CanAttack { get; set; }
    public bool IsRun { get; set; }
    public bool IsSub { get; protected set; }
    public bool IsSitting { get; protected set; }
    public float MoveAmount { get; set; }

    #endregion

    public virtual void Init(CUnit unit)
    {
        if (unit == null)
            return;

        m_unit = unit;

        
        m_rBody = GetComponent<Rigidbody>();
        m_detectCol = GetComponent<Collider>();

        m_movement = GetComponent<CMovement>();
        m_anim = GetComponentInChildren<Animator>();
        if (m_anim != null)   
        {
            m_anim.applyRootMotion = false;

            m_animHook = GetComponentInChildren<CAnimationHook>();
            if (m_animHook == null)
                m_animHook = m_anim.gameObject.AddComponent<CAnimationHook>();

            m_animHook.Init(unit, this);
        }
        
        CanAttack = true;
    }

    public void Process()
    {
        if (m_anim == null)
            return;

        m_anim.SetBool("IsOnGround", m_movement.IsOnGround);
    }

    public virtual void FixedProcess()
    {
        if (m_anim == null)
            return;

        InAction = m_anim.GetBool("InAction");
        CanMove = m_anim.GetBool("CanMove");
        IsUseItem = m_anim.GetBool("IsUseItem");
    }

    protected abstract void DetectedAction();
    protected abstract void DetectedItemAction();

    public virtual void GetHitAction(HitType hitType)
    {
        if (hitType == HitType.None)
            return;

        int nHitIdx = (int)hitType;

        bool isDown = m_anim.GetBool("IsDown");
        if (isDown == true && (hitType != HitType.Press || hitType != HitType.Rise))
            return;

        string hitName = "Hit" + nHitIdx;

        m_anim.Play(hitName);

        if (m_unit.MainHook != null&&!m_unit.MainHook.gameObject.activeSelf)
            m_unit.MainHook.gameObject.SetActive(true);
    }
    public virtual void DoDeathAction()
    {
        if (m_unit.Status.CurState != State.Dead)
            return;

        m_anim.Play("Death");

        if(m_detectCol!=null)
            m_detectCol.enabled = false;

        m_rBody.isKinematic = true;
    }

    public virtual void ResetActionUnit()
    {
        m_anim.Play("empty_hit");
        m_anim.SetBool("IsRun", false);
        m_anim.SetBool("IsDown", false);

        m_isDeathStart = false;
        IsRun = false;
        ResetAttack();

        if (m_detectCol != null)
            m_detectCol.enabled = true;

        if (m_animHook != null)
            m_animHook.CloseDudge();

        m_rBody.isKinematic = false;

    }

    protected abstract void MoveHandle();
    protected virtual void DudgeHandle() { }

    public abstract void RestAction();
    protected abstract IEnumerator RestCorutine();
    public virtual bool CancelAttack() { return true; }
    public virtual void ResetAttack() { }
}
