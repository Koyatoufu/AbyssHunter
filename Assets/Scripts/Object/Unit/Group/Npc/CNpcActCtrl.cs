using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CNpcActCtrl : CActionCtrl
{
    [SerializeField]
    protected float m_fDeathWait = 3f;
    
    public override void FixedProcess()
    {
        InAction = m_anim.GetBool("InAction");
        CanMove = m_anim.GetBool("CanMove");

        MoveHandle();
    }

    public override void Init(CUnit unit)
    {
        base.Init(unit);
    }

    public override void GetHitAction(HitType hitType)
    {
        if (hitType == HitType.None)
            return;

        int nHitIdx = (int)hitType;

        bool isDown = m_anim.GetBool("IsDown");
        if (isDown == true && (hitType != HitType.Press || hitType != HitType.Rise))
            return;

        string hitName = "Hit" + nHitIdx;

        m_anim.Play(hitName);

        StartCoroutine(HitCoroutine());
    }

    protected virtual IEnumerator HitCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        m_unit.Status.CurState = State.Scare;

        yield return null;
    }

    public override void DoDeathAction()
    {
        base.DoDeathAction();

        if (m_isDeathStart)
            return;

        StartCoroutine(DeathCoroutine());
    }

    protected virtual IEnumerator DeathCoroutine()
    {
        CNpcUnit unit = m_unit as CNpcUnit;

        if (unit == null)
        {
            Destroy(gameObject);
            yield break;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if(agent!=null)
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);
            yield return null;
        }

        if(unit.UnitData!=null&&unit.UnitData.PutItems!=null)
        {
            GameObject putItemGo = CObjectPool.Inst.GetItem();
            putItemGo.transform.position = transform.position;
            putItemGo.transform.rotation = Quaternion.identity;
            putItemGo.SetActive(true);

            CPutItem putItem = putItemGo.GetComponent<CPutItem>();
            if (putItem != null)
            {
                CItemBase item = null;

                if (unit.UnitData.PutItems.Count > 0)
                {
                    int nIdx = Random.Range(0, unit.UnitData.PutItems.Count);
                    item = unit.UnitData.PutItems[nIdx];
                }

                if (item != null)
                    putItem.SetItemData(item);
            }
        }

        yield return null;

        m_isDeathStart = true;

        yield return new WaitForSeconds(m_fDeathWait);

        m_unit.StayLevel = null;
        if (m_unit.SpawnedPoint != null)
            m_unit.SpawnedPoint.RemoveUnitFromList(m_unit);
        m_unit.SpawnedPoint = null;

        CObjectPool.Inst.PooledObject(gameObject,unit.UnitData,unit.UnitData.UnitType,unit.UnitData.Index);

        yield return null;

    }

    public override void RestAction()
    {
        StartCoroutine(RestCorutine());
    }

    protected override void DetectedAction() {}

    protected override void DetectedItemAction() {}

    protected override void DudgeHandle() {}

    protected override void MoveHandle()
    {
        m_anim.SetFloat("Vertical", MoveAmount);
        m_anim.SetBool("IsRun", IsRun);
    }

    protected override IEnumerator RestCorutine()
    {
        float fRand = Random.Range(3f, 5f);

        m_unit.Status.CurState = State.Rest;

        yield return new WaitForSeconds(fRand);

        m_unit.Status.CurState = State.Normal;

        yield return null;
    }
}
