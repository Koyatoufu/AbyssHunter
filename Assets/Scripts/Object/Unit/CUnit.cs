using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CStatus))]
[RequireComponent(typeof(CMovement))]
[RequireComponent(typeof(CActionCtrl))]
public abstract class CUnit : MonoBehaviour
{
    public virtual int UnitIdx { get { return 0; } }

    protected CStatus m_status;
    public CStatus Status { get { return m_status; } }

    protected CMovement m_movement;
    public CMovement Movement { get { return m_movement; } }

    protected CActionCtrl m_actCtrl;
    public CActionCtrl ActCtrl { get { return m_actCtrl; } }

    protected string m_unitTag;
    public string UnitTag { get { return m_unitTag; } }

    public virtual CWeaponHook MainHook { get { return null; }}
    public virtual CWeaponHook SubHook { get { return null; } }

    public CSubLevel StayLevel { get; set; }
    public CSpawnPoint SpawnedPoint { get; set; }

    [SerializeField]
    protected bool m_isInvincible = false;
    public bool IsInvincible { get { return m_isInvincible; } }

    [SerializeField]
    protected GameObject m_unitUIPrefab;
    [SerializeField]
    protected CUnitUI m_unitUI;
    public CUnitUI UnitUI { get { return m_unitUI; } }

    public CUseItem CurUseItem { get; set; }

    protected virtual void Start()
    {
        m_status = GetComponent<CStatus>();
        m_movement = GetComponent<CMovement>();
        m_movement.Init(this);
        m_actCtrl = GetComponent<CActionCtrl>();
        m_actCtrl.Init(this);
    }

    protected virtual void Update() { }

    protected abstract void FixedUpdate();

    public virtual bool GetDamage(Stat.AttackStat attackStat,HitType hitType = HitType.Normal,CUnit unit = null, ActionAnim actionAnim = null)
    {
        bool isDead = false;

        if (m_status.CurState == State.Dead)
            return false;

        float fMotionMul = actionAnim != null ? actionAnim.fMotionMagnifi : 1f;
        if (m_status.GetDamage(attackStat,out isDead,fMotionMul))
        {    
            if(m_unitUI!=null)
            {
                m_unitUI.SetHPGuageFill(m_status.GetHpAmount());
            }
            MainHookClose();
            m_actCtrl.ResetAttack();
            m_actCtrl.GetHitAction(hitType);
            m_actCtrl.Target = unit;
            return true;
        }
        
        if(isDead)
        {
            DeathProcess();
        }

        return false;
    }

    protected virtual void DeathProcess()
    {
        MainHookClose();
        m_actCtrl.ResetAttack();
        m_actCtrl.DoDeathAction();
        m_actCtrl.Target = null;
        if (m_unitUI != null)
        {
            m_unitUI.SetHPGuageFill(m_status.GetHpAmount());
        }
    }

    protected void MainHookClose()
    {
        if (MainHook != null)
            MainHook.CloseAllCollider();
    }

    public abstract void ResetUnit();

    public void ShowUI()
    {
        if(m_unitUI!=null)
        {
            m_unitUI.gameObject.SetActive(true);
        }
    }

    public void HideUI()
    {
        if(m_unitUI!=null)
        {
            m_unitUI.gameObject.SetActive(false);
        }
    }
}