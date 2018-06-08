using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAnimationHook : MonoBehaviour {

    private CUnit m_unit;
    private Rigidbody rBody;
    private CActionCtrl m_actCtrl;

    private Animator m_anim;

    private bool m_isDudge = false;
    public bool IsDudge { get { return m_isDudge; } }
    private float m_fDudgeTime = 0f;

    [SerializeField]
    private AnimationCurve m_dudgeCurve;

    [SerializeField]
    private float rMultiplier = 2f;
    public float RMultiplier { get { return rMultiplier; } set { rMultiplier = value; } }

    [SerializeField]
    private AudioSource m_audioSource = null;

    public void Init(CUnit unit,CActionCtrl actCtrl)
    {
        m_unit = unit;
        m_actCtrl = actCtrl;
        rBody = actCtrl.RBody;
        m_anim = GetComponent<Animator>();

        if(m_dudgeCurve==null)
        {
            m_dudgeCurve = new AnimationCurve();
        }
    }

    void OnAnimatorMove()
    {
        if (m_unit == null)
            return;
        if (m_unit.Status.CurState == State.Dead)
            return;

        if (m_actCtrl == null)
            return;
        if (m_actCtrl.InAction == false || m_actCtrl.CanMove)
            return;

        rBody.drag = 0f;

        if (RMultiplier == 0f)
            RMultiplier = 1f;

        Vector3 velocity;

        if (m_isDudge)
        {
            m_fDudgeTime += Time.fixedDeltaTime;
            float fZValue = m_dudgeCurve.Evaluate(m_fDudgeTime);
            Vector3 dudgeVel = Vector3.forward * fZValue;
            if (m_actCtrl.MoveAmount == 0f)
                dudgeVel = Vector3.back * fZValue;
            Vector3 relateDir = transform.TransformDirection(dudgeVel);
            velocity = (relateDir * RMultiplier) / (Time.fixedDeltaTime * 6.25f);
        }
        else
        {
            Vector3 deltaPos = m_anim.deltaPosition;
            velocity = (deltaPos * rMultiplier) / (Time.fixedDeltaTime * 2.5f);

            if (m_unit.Status.CurState == State.Hit || m_unit.Status.CurState == State.Dead)
            {
                velocity = deltaPos * rMultiplier;
            }

        }

        // 문제 발생시 수정
        velocity.y = rBody.velocity.y;

        rBody.velocity = velocity;
    }

    #region Dudge
    public void InitForDudge()
    {
        if(!m_isDudge)
        {
            m_isDudge = true;
            m_fDudgeTime = 0f;
        }
    }
    public void CloseDudge()
    {
        if (!m_isDudge)
            return;

        rMultiplier = 2f;
        m_isDudge = false;
    }
    #endregion

    #region CancelAttack
    public void CancelAttack()
    {
        m_actCtrl.CancelAttack();
    }
    public void ResetAttack()
    {
        m_actCtrl.ResetAttack();
    }
    #endregion

    #region OpenCloseCollider
    public void OpenDamageCollider()
    {
        if(m_unit.MainHook!=null)
            m_unit.MainHook.OpenAllAttackCollider();   
    }
    public void OpenSubAttackCollider()
    {
        if(m_unit.SubHook!=null)
            m_unit.SubHook.OpenAllAttackCollider();
    }
    public void CloseDamageCollider()
    {
        if (m_unit.MainHook != null)
            m_unit.MainHook.CloseAllAttackCollier();
    }
    public void CloseSubAttackCollider()
    {
        if (m_unit.SubHook != null)
            m_unit.SubHook.CloseAllAttackCollier();
    }

    public void OpenGuardCollider()
    {
        if (m_unit.MainHook != null)
            m_unit.MainHook.OpenGuardCollider();
    }
    public void OpenSubGuardCollider()
    {
        if (m_unit.SubHook != null)
            m_unit.SubHook.OpenGuardCollider();
    }


    public void CloseGuardCollider()
    {
        if (m_unit.MainHook != null)
            m_unit.MainHook.CloseAllCollider();
    }
    public void CloseSubGuardCollider()
    {
        if (m_unit.SubHook != null)
            m_unit.SubHook.CloseAllCollider();
    }

    #endregion

    public void AudioEffectPlay(AudioClip clip)
    {
        if (m_audioSource == null)
            return;
        
        if(CSoundManager.Inst==null)
        {
            m_audioSource.clip = clip;
            m_audioSource.loop = false;
            m_audioSource.Play();
        }
        else
        {
            CSoundManager.Inst.EffectPlay(m_audioSource, clip);
        }
    }

    public void HideWeapon()
    {
        m_unit.MainHook.gameObject.SetActive(false);
    }
    public void ShowWeapon()
    {
        m_unit.MainHook.gameObject.SetActive(true);
    }

    public void ItemUse()
    {
        if (m_unit == null)
            return;

        if (m_unit.CurUseItem == null)
            return;

        GameObject effObj = null;
        CParticle effect = null;

        switch (m_unit.CurUseItem.UseItemType)
        {
            case UseItemType.RECORVE:
                {
                    m_unit.Status.ChargeHp(m_unit.CurUseItem.Value);
                    effObj = CObjectPool.Inst.GetEffect(EffectType.Particle, 2);
                    if (effObj == null)
                        break;
                    effect = effObj.GetComponent<CParticle>();
                    effect.Show(m_unit);
                    effect.transform.position = transform.position;
                }
                break;
            case UseItemType.STAMINA:
                {
                    m_unit.Status.StaminaCharge(m_unit.CurUseItem.Value);
                    effObj = CObjectPool.Inst.GetEffect(EffectType.Particle, 3);
                    if (effObj == null)
                        break;
                    effect = effObj.GetComponent<CParticle>();
                    effect.Show(m_unit);
                    effect.transform.position = transform.position;
                }
                break;
            case UseItemType.BUFF:
                {
                    switch (m_unit.CurUseItem.BuffType)
                    {
                        case BuffType.Element:
                            {
                                if (m_unit.MainHook != null)
                                {
                                    int nElement = Random.Range(5, 8);
                                    effObj = CObjectPool.Inst.GetEffect(EffectType.Particle, nElement);

                                    if (effObj == null)
                                        break;

                                    effect = effObj.GetComponent<CParticle>();
                                    effect.Show(m_unit);
                                    effect.transform.parent = m_unit.MainHook.transform;
                                    effect.transform.localPosition = Vector3.zero;

                                    if(m_unit.UnitUI!=null)
                                        m_unit.UnitUI.BuffIn(m_unit.CurUseItem.BuffType, (Stat.Element)(nElement - 8));
                                }
                            }
                            break;
                        case BuffType.None:
                            break;
                        default:
                            {
                                effObj = CObjectPool.Inst.GetEffect(EffectType.Particle, 4);
                                if (effObj == null)
                                    break;
                                effect = effObj.GetComponent<CParticle>();
                                effect.Show(m_unit);
                                effect.transform.parent = transform;
                                effect.transform.localPosition = Vector3.zero;
                                if(m_unit.UnitUI!=null)
                                    m_unit.UnitUI.BuffIn(m_unit.CurUseItem.BuffType);
                            }
                            break;
                    }
                }
                break;
            case UseItemType.BIND:
                break;
            case UseItemType.SHOOT:
                break;
        }

        m_unit.CurUseItem = null;
    }

}
