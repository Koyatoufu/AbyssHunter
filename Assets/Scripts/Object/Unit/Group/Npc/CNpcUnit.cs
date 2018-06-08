using System.Collections;
using UnityEngine;

public class CNpcUnit : CUnit
{
    [SerializeField]
    protected CUnitData m_unitData = null;
    public CUnitData UnitData { get { return m_unitData; } }

    public override int UnitIdx { get{ if (m_unitData == null) return 0; return m_unitData.Index; } }

    protected override void Start()
    {
        base.Start();

        if (m_unitData != null)
            m_status.Init(m_unitData.BaseStat);
    }

    protected override void Update()
    {
        m_actCtrl.Process();
    }

    protected override void FixedUpdate()
    {
        m_actCtrl.FixedProcess();
        m_movement.FixedProcess();
    }

    public override void ResetUnit()
    {
        if (m_status != null)
            m_status.ResetState();

        if(m_actCtrl!=null)
        {
            m_actCtrl.ResetActionUnit();
        }

        if(m_unitUI!=null)
        {
            m_unitUI.gameObject.SetActive(true);
            m_unitUI.ResetUI();
        }   
    }

    protected override void DeathProcess()
    {
        base.DeathProcess();

        if (m_unitUI != null)
            m_unitUI.HideUI();
    }
}
