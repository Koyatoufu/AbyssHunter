using UnityEngine;
using System.Collections;

public class CStatus : MonoBehaviour
{
    [SerializeField]
    private State m_state;
    public State CurState { get { return m_state; } set { m_state = value; } }

    [SerializeField]
    protected Stat.Base m_baseStat;
    public Stat.Base BaseStst { get { return m_baseStat; } }

    [SerializeField]
    private readonly float m_fExhuatStamina = 2f;

    private bool m_canUseStamina = true;

    private bool m_isInit = false;

    public void Init(Stat.Base baseStat)
    {
        if (m_isInit)
            return;

        m_baseStat = baseStat;
        m_isInit = true;
    }

    public void ResisterSet(Stat.Resister resister)
    {
        m_baseStat.resister = resister;
    }

    public bool GetDamage(Stat.AttackStat attackStat, out bool isDead, float fMotionMulti = 0.5f)
    {
        //TODO: 모션에 따른 배율이 정확히 들어오는지 확인
        int nDamage = CalculateAttributes.CaculateDamage(attackStat,fMotionMulti,m_baseStat.resister);
        
        if (nDamage <= 0)
        {
            isDead = false;
            return false;
        }
        
        int nCaculate = m_baseStat.hp - nDamage;

        if (nCaculate<=0)
        {
            m_baseStat.hp = 0;
            m_state = State.Dead;
            isDead = true;
            return false;
        }

        m_baseStat.hp = nCaculate;

        //TODO: 아머 조건에 따라서 
        m_state = State.Hit;
        isDead = false;
        return true;
    }
    public void ChargeHp(int nValue)
    {
        if (nValue <= 0)
            return;

        m_baseStat.hp += nValue;
        if(m_baseStat.hp>m_baseStat.maxHp)
        {
            m_baseStat.hp = m_baseStat.maxHp;
        }
    }

    public void StaminaCharge(float fValue)
    {
        m_baseStat.stamina += fValue;

        if (m_baseStat.stamina >= m_baseStat.maxStamina)
        {
            m_baseStat.stamina = m_baseStat.maxStamina;
        }
    }
    public void StaminaReduce(float fValue)
    {
        m_baseStat.stamina -= fValue;

        if (m_baseStat.stamina <= 0)
        {
            m_baseStat.stamina = 0;
        }
        
        if(m_baseStat.stamina<m_fExhuatStamina)
        {
            StartCoroutine(WaitUseableStemina());
        }
    }
    public bool IsUsableStamina(float fValue)
    {
        if (!m_canUseStamina)
            return false;

        if(m_baseStat.stamina-fValue >=0)
        {
            return true;
        }

        return false;
    }
    IEnumerator WaitUseableStemina()
    {
        //TODO: 헐떡 거리는 액션을 취할 수 있도록 한다.
        m_canUseStamina = false;
        yield return null;
        yield return new WaitForSeconds(5f);

        m_canUseStamina = true;
    }

    public void ResetState()
    {
        m_state = State.Normal;

        m_baseStat.hp = m_baseStat.maxHp;
        m_baseStat.stamina = m_baseStat.maxStamina;
    }

    public void RestState()
    {
        if(m_state!=State.Normal)
            return;

        m_baseStat.hp = m_baseStat.maxHp;
        m_baseStat.stamina = m_baseStat.maxStamina;

        m_state = State.Rest;
    }

    public float GetHpAmount()
    {
        return (float) m_baseStat.hp / (float)m_baseStat.maxHp;
    }
    public float GetStaminaAmount()
    {
        return (float) m_baseStat.stamina / (float)m_baseStat.maxStamina;
    }
    
}

public enum State { Normal, Attack, Scare, Rest, Hit, Down, Groggy, Dead, Item }