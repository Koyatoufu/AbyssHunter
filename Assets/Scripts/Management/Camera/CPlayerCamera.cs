using UnityEngine;

public class CPlayerCamera : MonoBehaviour
{
    private const float MOUSE_SPEED = 2f;
    private const float AXIS_SPEED = 7f;

    private const float MIN_ANGLE = -35f;
    private const float MAX_ANGLE = 35f;

    [SerializeField]
    private float m_turnSmoothing = .1f;
    [SerializeField]
    private float m_followSpeed = 9f;
    [SerializeField]
    private GameObject m_lockOnBill = null;

    private CUnit m_player = null;

    private Transform m_followTarget;
    private CInputCtrl m_input;
    private Transform m_lockOnTrans = null;
    public CTarget LockOnTarget { get; private set; }

    private Camera m_camera;
    public Camera Camera { get { return m_camera; } }
    private Transform m_pivot;
    private CTargetDetect m_targetDetect;

    private float m_fSmoothX;
    private float m_fSmoothY;
    private float m_fSmoothXVel;
    private float m_fSmoothYVel;

    private float m_fLookAngle;
    private float m_fTiltAngle;

    public bool IsLockOn { get; set; }

    public void SetCamera(CInputCtrl input,CUnit unit)
    {
        m_input = input;

        m_player = unit;
        m_followTarget = unit.transform;

        m_pivot = transform.GetChild(0);
        m_camera = m_pivot.GetComponentInChildren<Camera>();

        m_targetDetect = transform.GetComponent<CTargetDetect>();
        m_targetDetect.Init(this,unit);
    }

    void FixedUpdate()
    {
        float fDelta = Time.fixedDeltaTime;

        float fH = m_input.InputData.mouseAxisX;
        float fV = m_input.InputData.mouseAxisY;

        float fC_H = m_input.InputData.rightAxisX;
        float fC_V = m_input.InputData.rightAxisY;

        float fTargetSpd = MOUSE_SPEED;
        if (fC_H!=0f||fC_V!=0f)
        {
            fTargetSpd = AXIS_SPEED;
            fH = fC_H;
            fV = -fC_V;
        }
         
        FollowTarget(fDelta);
        LockOnProcess(fC_H);
        HandleRotation(fDelta,fV,fH,fTargetSpd);
    }

    void FollowTarget(float fDelta)
    { 
        if (m_followTarget == null)
            return;

        Vector3 followPos = m_followTarget.position;
        followPos.y += 1.0f;

        transform.position = Vector3.Lerp(transform.position, followPos, fDelta * m_followSpeed);
    }

    void HandleRotation(float fDelta,float fV,float fH,float fTargetSpd)
    {
        if(m_turnSmoothing>0f)
        {
            m_fSmoothX = Mathf.SmoothDamp(m_fSmoothX, fH, ref m_fSmoothXVel, m_turnSmoothing);
            m_fSmoothY = Mathf.SmoothDamp(m_fSmoothY, fV, ref m_fSmoothYVel, m_turnSmoothing);
        }
        else
        {
            m_fSmoothX = fH;
            m_fSmoothY = fV;
        }

        m_fTiltAngle -= m_fSmoothY * fTargetSpd;
        m_fTiltAngle = Mathf.Clamp(m_fTiltAngle, MIN_ANGLE, MAX_ANGLE);
        m_pivot.localRotation = Quaternion.Euler(m_fTiltAngle, 0f, 0f);
        
        m_fLookAngle += m_fSmoothX * fTargetSpd;

        if (IsLockOn)
        {
            m_fLookAngle = transform.eulerAngles.y;

            if (LockOnTarget != null)
            {
                if(m_player==null||m_player.Status.CurState==State.Dead)
                {
                    TarGetClear();
                    return;
                }

                if (LockOnTarget.Unit == null || LockOnTarget.Unit.Status.CurState == State.Dead || m_lockOnTrans==null || !LockOnTarget.gameObject.activeSelf)
                {
                    TarGetClear();
                    return;
                }

                if(m_lockOnBill!=null)
                    m_lockOnBill.transform.position = m_lockOnTrans.position;
                Vector3 lockOnDir = m_lockOnTrans.position - transform.position;
                lockOnDir.Normalize();
                if (lockOnDir == Vector3.zero)
                    lockOnDir = transform.forward;

                Quaternion lockOnRot = Quaternion.LookRotation(lockOnDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, lockOnRot, fDelta);
                return;
            }
        }

        transform.rotation = Quaternion.Euler(0f, m_fLookAngle, 0f);
    }

    void LockOnProcess(float fH)
    {
        if (m_input != null)
        {
            if (!IsLockOn)
            {
                if (m_input.InputData.btnInputs[(int)InputIndex.AXIS_R])
                {
                    m_targetDetect.TargetDetection();
                    LockOnTarget = m_targetDetect.GetTarget();

                    if (LockOnTarget!=null)
                    {
                        m_lockOnTrans = LockOnTarget.GetTransform();
                        if(m_lockOnBill!=null)
                        {
                            m_lockOnBill.SetActive(true);
                            //m_lockOnGo.transform.parent = m_lockOnTrans;
                        }
                    }
                }   
            }
            else
            {
                
                if (m_input.InputData.btnInputs[(int)InputIndex.AXIS_R])
                {
                    TarGetClear();
                    return;
                }
                
                if(Mathf.Abs(m_input.InputData.rightAxisX) > 0.1f || Input.GetKeyDown(KeyCode.Z))
                {
                    Transform changeTrans = LockOnTarget.GetTransform(true);

                    if(changeTrans!=m_lockOnTrans)
                    {
                        m_lockOnTrans = changeTrans;
                        return;
                    }

                    LockOnTarget = m_targetDetect.CanTargetChange(true);
                    m_lockOnTrans = LockOnTarget.GetTransform();
                }
            }
        }
    }

    void TarGetClear()
    {
        m_targetDetect.ClearTargets();
        LockOnTarget = null;
        m_lockOnTrans = null;

        if(m_lockOnBill!=null)
        {
            //m_lockOnGo.transform.parent = transform;
            m_lockOnBill.SetActive(false);
        }
    }

    
}
