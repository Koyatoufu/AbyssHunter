using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CParticle : CEffect
{
    private ParticleSystem m_particle = null;

    private void Awake()
    {
        m_particle = GetComponent<ParticleSystem>();
    }

    public override void Show(CUnit unit)
    {
        base.Show(unit);

        if (m_particle != null)
            m_particle.Play();
    }

    protected override IEnumerator EffectCoroutine()
    {
        return base.EffectCoroutine();
    }
}
