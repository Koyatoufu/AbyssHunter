using System.Collections;
using System.Collections.Generic;
using Stat;
using UnityEngine;

[RequireComponent(typeof(CInventory))]
public class CVisibleUnit : CUnit
{
    protected CInventory m_inventory;
    public CInventory Inventory { get { return m_inventory; } }

    public override CWeaponHook MainHook { get { return null; } }

    protected override void Start()
    {
        m_unitTag = "Player";
        gameObject.tag = m_unitTag;

        CDataManager dataMgr = CDataManager.Inst;

        if (dataMgr == null)
            return;

        Data.PlayerInfo playerInfo = dataMgr.PlayerRecord.PlayerInfo;

        GameObject genderPrefab = null;
        if (playerInfo.gender == Gender.Female)
            genderPrefab = Resources.Load<GameObject>("Prefabs/Character/Player/Female");
        else
            genderPrefab = Resources.Load<GameObject>("Prefabs/Character/Player/Male");
        
        if (genderPrefab == null)
            return;

        GameObject genderGO = Instantiate(genderPrefab, transform);
        genderGO.name = genderPrefab.name;

        base.Start();

        m_inventory = GetComponent<CInventory>();
        m_inventory.Initialized(this);
    }

    protected override void Update()
    {
        return;
    }

    protected override void FixedUpdate()
    {
        return;
    }

    public override void ResetUnit(){}
    protected override void DeathProcess(){}

}
