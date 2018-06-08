using UnityEngine;

public class CObstacle : MonoBehaviour
{
    [SerializeField]
    private GameObject m_modernGo = null;

    [SerializeField]
    private GameObject m_brokenGo = null;

    [SerializeField]
    private Collider m_collider = null;

    [SerializeField]
    private AudioSource m_bokenAudio = null;

    void OnCollisionEnter(Collision other)
    {
        CollisionCheck(other);
    }

    void OnCollisionStay(Collision other)
    {
        CollisionCheck(other);
    }

    void CollisionCheck(Collision other)
    {
        if (other.gameObject.GetComponent<CAttackCollider>() != null)
        {
            if (m_modernGo != null && m_brokenGo != null)
            {
                m_modernGo.SetActive(false);
                m_brokenGo.SetActive(true);
            }
            if (m_bokenAudio != null && CSoundManager.Inst != null)
                CSoundManager.Inst.EffectPlay(m_bokenAudio);
            if (m_collider != null)
            {
                m_collider.isTrigger = true;
            }
            return;
        }

        CActionCtrl actionCtrl = other.gameObject.GetComponent<CActionCtrl>();

        if (actionCtrl != null && actionCtrl.InAction)
        {
            if (m_modernGo != null && m_brokenGo != null)
            {
                m_modernGo.SetActive(false);
                m_brokenGo.SetActive(true);
            }
            if (m_bokenAudio != null && CSoundManager.Inst != null)
                CSoundManager.Inst.EffectPlay(m_bokenAudio);
            if (m_collider != null)
            {
                m_collider.isTrigger = true;
            }
            return;
        }
    }
}
