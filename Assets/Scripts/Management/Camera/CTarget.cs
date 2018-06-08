using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTarget : MonoBehaviour
{
    [SerializeField]
    private List<Transform> m_splitTargets = new List<Transform>();

    [SerializeField]
    private CUnit m_unit = null;
    public CUnit Unit { get { return m_unit; } }

    public int TransIdx { get; private set; }
    public int TransCount { get { return m_splitTargets.Count; } }

    public Transform GetTransform(bool isNegative = false)
    {
        if (m_splitTargets.Count <= 0)
            return transform;

        if(isNegative)
        {
            TransIdx++;
            if (TransIdx >= m_splitTargets.Count)
            {
                TransIdx = 0;
            }
            return m_splitTargets[TransIdx];
        }

        return m_splitTargets[TransIdx];
    }

}
