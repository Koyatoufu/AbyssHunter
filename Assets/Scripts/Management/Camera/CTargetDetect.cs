using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CTargetDetect : MonoBehaviour
{
    [SerializeField]
    private List<CTarget> m_targets = null;
    //public List<CTarget> Targets { get { return m_targets; } }

    [SerializeField]
    private LayerMask m_targetMask = 11;

    private CPlayerCamera m_camCtrl;
    private CUnit m_unit;

    private int m_nCurIdx = -1;

    public void Init(CPlayerCamera camCtrl,CUnit unit)
    {
        m_camCtrl = camCtrl;
        m_unit = unit;
    }

    public void ClearTargets()
    {
        m_nCurIdx = -1;
        m_targets.Clear();
        m_camCtrl.IsLockOn = false;
    }

    public void TargetDetection()
    {
        RaycastHit[] hit = Physics.SphereCastAll(transform.position, 20f, transform.forward,m_targetMask);

        if (hit.Length < 1)
        {
            ClearTargets();
            return;
        }

        for (int i = 0; i < hit.Length; i++)
        {
            CTarget temp = hit[i].collider.GetComponent<CTarget>();

            if (temp == null)
                continue;

            Vector3 tempDir = temp.transform.position - m_camCtrl.Camera.transform.position;
            
            float dot = Vector3.Dot(m_camCtrl.Camera.transform.forward, tempDir.normalized);

            if (dot >= 0.65f)
            {
                m_targets.Add(temp);
            }
        }

        if (m_targets.Count > 0)
        {
            m_camCtrl.IsLockOn = true;
        }
    }

    public CTarget GetTarget()
    {
        if (m_nCurIdx < 0)
            CloseTargetCheck();

        if (m_nCurIdx < 0)
        {
            ClearTargets();
            return null;
        }

        return m_targets[m_nCurIdx];
    }

    public CTarget CanTargetChange(bool isLeft = false)
    {
        if (m_targets.Count < 1)
        {
            ClearTargets();
            return null;
        }

        //int nBackIdx = m_nCurIdx;

        if(isLeft)
        {
            m_nCurIdx --;
        }
        else
        {
            m_nCurIdx ++;
        }

        if(m_nCurIdx>=m_targets.Count)
        {
            m_nCurIdx = 0;
        }
        else if(m_nCurIdx<0)
        {
            m_nCurIdx = m_targets.Count - 1;
        }

        return m_targets[m_nCurIdx];
    }

    void CloseTargetCheck()
    {
        int nIdx = -1;

        for (int i = 0; i < m_targets.Count; i++)
        {
            if (m_targets[i] == null)
                continue;

            if (m_targets[i].Unit == null)
                continue;

            if (m_targets[i].Unit.Status.CurState == State.Dead)
            {
                ClearTargets();
                return;
            }

            if (nIdx < 0)
            {
                nIdx = i;
                continue;
            }

            float fDistanceTarget = Vector3.Distance(m_unit.transform.position, m_targets[i].transform.position);
            float fDistanceTemp = Vector3.Distance(m_unit.transform.position, m_targets[i].transform.position);

            if (fDistanceTemp < fDistanceTarget)
            {
                nIdx = i;
            }
        }

        m_nCurIdx = nIdx;
    }

}
