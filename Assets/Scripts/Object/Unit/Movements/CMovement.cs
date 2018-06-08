using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public abstract class CMovement : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody m_rBody;

    protected CActionCtrl m_actCtrl;
    protected CStatus m_status;

    public Vector3 MoveDir { get; set; }
    public Vector3 Rotation { get; set; }

    #region navMeshWays
    [SerializeField]
    protected List<Transform> m_moveWays = null;
    public List<Transform> MoveWays { get { return m_moveWays; } set { m_moveWays = value; } }

    public int CurMoveWayIdx { get; protected set; }
    public int CurScareWayIdx { get; protected set; }
    #endregion

    [SerializeField]
    protected LayerMask groundMask = 10;

    public bool IsOnGround { get; protected set; }

    protected CUnit m_unit;

    protected MoveState m_moveState = MoveState.NORMAL;
    public MoveState CurMoveState { get { return m_moveState; } set { m_moveState = value; } }

    public virtual void Init(CUnit unit)
    {
        m_unit = unit;

        m_rBody = GetComponent<Rigidbody>();

        m_rBody.angularDrag = 999f;
        m_rBody.drag = 4f;
        m_rBody.constraints = RigidbodyConstraints.FreezeRotation;

        m_actCtrl = GetComponent<CActionCtrl>();
        m_status = GetComponent<CStatus>();
    }

    public virtual void FixedProcess()
    {
        if (m_status == null)
            return;
        if (m_status.CurState == State.Dead)
            return;
        if (m_actCtrl == null)
            return;

        IsOnGround = OnGroundCheck();

        Rotate();
        Move();
    }

    protected bool OnGroundCheck()
    {
        Vector3 org = transform.position + Vector3.up * 0.3f;

        RaycastHit hit;
        bool isOnGround = false;

        Debug.DrawRay(org, Vector3.down * 0.5f);
        if (Physics.Raycast(org, Vector3.down, out hit, 0.5f, groundMask))
        {
            isOnGround = true;
            if(m_actCtrl!=null&& !m_actCtrl.InAction)
            {
                Vector3 newPoint = transform.position;
                newPoint.y = hit.point.y;
                transform.position = newPoint;
            }
        }

        return isOnGround;
    }

    protected abstract void Move();

    protected abstract void Rotate();

    
}
