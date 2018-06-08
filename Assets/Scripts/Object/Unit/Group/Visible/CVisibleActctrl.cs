using System.Collections;
using UnityEngine;

public class CVisibleActctrl : CActionCtrl
{
    public override void Init(CUnit unit)
    {
        base.Init(unit);
    }

    public override void FixedProcess()
    {
        base.FixedProcess();
    }

    public override void ResetActionUnit()
    {
        base.ResetActionUnit();
    }

    public override void ResetAttack()
    {
        base.ResetAttack();
    }

    public override void RestAction()
    {
        return;
    }

    protected override void DetectedAction()
    {
        return;
    }

    protected override void DetectedItemAction()
    {
        return;
    }

    protected override void DudgeHandle()
    {
        base.DudgeHandle();
    }

    protected override void MoveHandle()
    {
        return;
    }

    protected override IEnumerator RestCorutine()
    {
        yield return null;
    }
}
