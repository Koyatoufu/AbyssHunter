using System.Collections;
using System.Collections.Generic;
using Stat;
using UnityEngine;

public class CProjectileCollider : CBaseCollider
{


    public override void Init(CUnit unit, CWeapon weapon)
    {
        base.Init(unit, weapon);
    }

    protected override void OnTriggerEnter(Collider other)
    {

    }
}
