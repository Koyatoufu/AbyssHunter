using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHitEffect : CParticle
{
    [SerializeField]
    private float m_shakeTime = 0.1f;

    [SerializeField]
    private float m_shakeMagnitude = 0.5f;

    public override void Show(CUnit unit)
    {
        base.Show(unit);

        StartCoroutine(CameraFunc.ShakeCoroutine(m_shakeTime, m_shakeMagnitude));
    }
}
