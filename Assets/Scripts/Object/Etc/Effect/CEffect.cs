using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEffect : MonoBehaviour
{
    [SerializeField]
    protected EffectType m_type;

    [SerializeField]
    protected int m_effectIdx = 0;

    [SerializeField]
    protected float m_destroyTime = 1f;

    [SerializeField]
    private AudioSource m_audioSource = null;

    protected float m_fPastTime = 0f;

    protected CUnit m_unit = null;
    public CUnit Unit { get { return m_unit; } }

    public virtual void Show(CUnit unit)
    {
        m_unit = unit;
        gameObject.SetActive(true);
        StartCoroutine(EffectCoroutine());
        if (m_audioSource != null)
        {
            if (CSoundManager.Inst != null)
                CSoundManager.Inst.EffectPlay(m_audioSource);
            else
                m_audioSource.Play();
        }
    }

    public void Hide()
    {
        m_unit = null;
        m_fPastTime = 0f;
        //TODO: ObjectPool에 집어 넣는다.
        CObjectPool.Inst.PooledObject(gameObject, m_type, m_effectIdx);
    }

    protected virtual IEnumerator EffectCoroutine()
    {
        while(m_fPastTime<m_destroyTime)
        {
            yield return null;
            m_fPastTime += Time.deltaTime;
        }

        Hide();
    }
}

public enum EffectType
{
    Particle,
    Projectile,
}