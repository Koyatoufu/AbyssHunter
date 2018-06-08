using System.Collections.Generic;
using UnityEngine;

public class CAction : ScriptableObject
{
    [SerializeField]
    private ActionType type = ActionType.Max;
    public ActionType Type { get { return type; } }

    [SerializeField]
    private List<ActionAnim> m_steps = new List<ActionAnim>();

    private int m_nStepIdx = 0;

    public ActionAnim CurAction()
    {
        if (m_steps.Count <= 0)
        {
            Debug.LogError("Not settings action steps");
            return null;
        }

        ActionAnim actionAnim = m_steps[m_nStepIdx];

        m_nStepIdx++;
        if (m_nStepIdx >= m_steps.Count)
        {
            ResetActionAnim();
        }

        return actionAnim;
    }
    public void ResetActionAnim()
    {
        m_nStepIdx = 0;
    }
}

[System.Serializable]
public class ActionAnim
{
    public string animName;

    public float fMotionMagnifi;

    public bool isAttack = true;

    public bool canHold;
    public HitType hitType = HitType.None;

    public bool canNormalCancel;
    public bool canStrongCancel;
    public bool canSubCancel;
    public bool canUniqueCancel;

}

public enum ActionType
{
    Normal,
    Strong,
    Sub,
    Unique,
    Max
}

public enum HitType
{
    None,
    Normal,
    Blow,
    Rise,
    Press
}