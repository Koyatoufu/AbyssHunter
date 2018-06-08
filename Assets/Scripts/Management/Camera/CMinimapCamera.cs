using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CMinimapCamera : MonoBehaviour
{
    [SerializeField]
    private CUnit m_target = null;

    [SerializeField]
    private float m_followSpeed = 9f;

    Camera m_camera = null;

    void Awake()
    {
        m_camera = GetComponent<Camera>();
        m_camera.enabled = false;
    }

    void Start()
    {
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        while(!GameManager.Inst.IsGameStart)
        {
            yield return null;
        }

        GameObject playerGo = null;

        while(playerGo==null)
        {
            playerGo = GameObject.FindWithTag("Player");
            yield return null;
        }
        
        CPlayerUnit player = playerGo.GetComponent<CPlayerUnit>();

        m_target = player;

        m_camera.enabled = true;
    }

    void FixedUpdate()
    {
        if (m_target == null)
            return;

        CheckTargetState();
        FollowTarget();
    }

    void FollowTarget()
    {
        Vector3 followPos = m_target.transform.position;
        followPos.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, followPos, Time.fixedDeltaTime * m_followSpeed);
    }

    void CheckTargetState()
    {
        if (m_target.Status.CurState == State.Dead)
            return;

        if (GameManager.Inst == null)
            return;

        if (!GameManager.Inst.IsGameEnd)
            return;

        m_camera.enabled = false;
    }
}
