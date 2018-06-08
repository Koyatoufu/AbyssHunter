using UnityEngine;
using System.Collections;

public class CArmorCollider : CBaseCollider
{
    [SerializeField]
    Stat.Resister m_armorResister = new Stat.Resister();
    public Stat.Resister ArmorResister { get { return m_armorResister; } }

    [SerializeField]
    Stat.ArmorStat m_armorStat = new Stat.ArmorStat();
    public Stat.ArmorStat ArmorStat { get { return m_armorStat; } }

    [SerializeField]
    private float m_armorBreakTime = 10f;

    [SerializeField]
    private SkinnedMeshRenderer m_armorSkin = null;

    [SerializeField]
    private GameObject m_copyParts = null;

    public override void Init(CUnit unit, CWeapon weapon)
    {
        base.Init(unit, weapon);
        //m_armorResister = m_unit.Status;
    }

    public override void ApponentColliderIn(Stat.AttackStat attackStat, HitType hitType,ActionAnim actionAnim = null)
    {
        m_unit.GetDamage(attackStat, hitType,null,actionAnim);

        if(m_armorStat.armorDamage>=m_armorStat.armorLimit)
        {
            StartCoroutine(ArmorBreakCoroutine());
        }
        else
        {
            float fMul = actionAnim == null ? 1f : actionAnim.fMotionMagnifi;
            m_armorStat.armorDamage += 
                CalculateAttributes.CaculateDamage(attackStat, fMul,m_armorResister);
        }
    }

    IEnumerator ArmorBreakCoroutine()
    {
        yield return new WaitForSeconds(m_armorBreakTime);
        //TODO: 액션에서 경직 액션을 받아와 자신의 유닛에 넘긴다. 
        if(m_armorStat.isSeperation)
        {
            //TODO: 부모와 분리가 가능한 물체이면 부모객체에서 분리시키고 약간의 물리행동을 주어 날려버린다.
            if(m_armorSkin!=null)
            {
                m_armorSkin.enabled = false;
                yield return null;
            }

            if (m_copyParts != null)
            {
                GameObject copyObj = Instantiate(m_copyParts, transform.position, transform.rotation);

                if (copyObj != null)
                {
                    #region RandomDirection
                    Rigidbody rBody = copyObj.GetComponent<Rigidbody>();
                    rBody.isKinematic = false;
                    int nRand = Random.Range(0, 4);
                    Vector3 dir = Vector3.forward + Vector3.up;
                    switch (nRand)
                    {
                        case 0:
                            dir += Vector3.forward;
                            break;
                        case 1:
                            dir += Vector3.up;
                            break;
                        case 2:
                            dir += Vector3.right;
                            break;
                        case 3:
                            dir += Vector3.left;
                            break;
                        default:
                            dir += Vector3.forward;
                            break;
                    }
                    rBody.AddForce(dir * 2f, ForceMode.Impulse);
                    #endregion
                }
                yield return null;
            }

            #region DisEnable
            CharacterJoint joint = GetComponent<CharacterJoint>();
            if (joint != null)
            {
                joint.enableCollision = false;
                joint.enablePreprocessing = false;
                joint.enableProjection = false;
            }
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;
            this.enabled = false;
            #endregion

            yield return null;
        }
        yield return null;
    }
}
