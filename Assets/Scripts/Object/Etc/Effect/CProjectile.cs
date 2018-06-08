using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CProjectile : CEffect
{
    [SerializeField]
    private Stat.AttackStat m_attackStat = new Stat.AttackStat();
    public Stat.AttackStat AttackStat { get { return m_attackStat; } }

    [SerializeField]
    private float m_moveSpeed = 0f;

    [SerializeField]
    private Vector3 m_movedir = Vector3.forward;

    [SerializeField]
    bool m_isRandomDir = false;

    private Rigidbody m_rigidbody = null;

    public void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    public override void Show(CUnit unit)
    {

        base.Show(unit);

        if(m_isRandomDir)
        {
            m_rigidbody.AddForce(m_movedir,ForceMode.Impulse);
        }
    }

    protected override IEnumerator EffectCoroutine()
    {
        //if (m_rigidbody == null)
        //    m_rigidbody = GetComponent<Rigidbody>();

        while(m_fPastTime<m_destroyTime)
        {
            m_fPastTime += Time.deltaTime;
            Vector3.MoveTowards(transform.position, transform.position + m_movedir, m_moveSpeed);
            yield return null;
        }

        Hide();
        yield return null;
    }

}
