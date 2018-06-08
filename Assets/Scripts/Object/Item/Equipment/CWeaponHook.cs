using UnityEngine;
using System.Collections.Generic;

public class CWeaponHook : MonoBehaviour
{
    private List<CBaseCollider> m_colliders = new List<CBaseCollider>();

    [SerializeField]
    private CWeapon m_weapon;
    public CWeapon Weapon { get { return m_weapon; } }

    [SerializeField]
    private CUnit m_unit;

    private MeleeWeaponTrail m_meleeTrail;
    
    private void Awake()
    {
        InitCollider();
        m_meleeTrail = GetComponent<MeleeWeaponTrail>();
        if (m_meleeTrail != null)
            m_meleeTrail.Emit = false;
    }

    public void Init(CUnit unit,CWeapon weapon = null)
    {
        m_unit = unit;

        if (m_weapon == null)
        {
            if(weapon==null)
            {
                Debug.LogError("Weapon Data is null!");
                return;
            }
            m_weapon = weapon;
        }

        for(int i=0;i<m_colliders.Count;i++)
        {
            m_colliders[i].Init(m_unit,m_weapon);
        }
    }

    void InitCollider()
    {
        if(m_colliders.Count<=0)
        {
            foreach(Transform trans in transform)
            {
                CBaseCollider col = trans.GetComponent<CBaseCollider>();
                if (col == null)
                    continue;
                m_colliders.Add(col);
            }
        }

        CloseAllCollider();
    }

    public void CloseAllCollider()
    {
        for(int i=0;i<m_colliders.Count;i++)
        {
            m_colliders[i].gameObject.SetActive(false);
        }

        if (m_meleeTrail != null)
            m_meleeTrail.Emit = false;
    }

    public void CloseAllAttackCollier()
    {
        for (int i=0;i<m_colliders.Count;i++)
        {
            if(m_colliders[i] is CAttackCollider)
            {
                m_colliders[i].gameObject.SetActive(false);
            } 
        }

        if (m_meleeTrail != null)
            m_meleeTrail.Emit = false;
    }

    public void OpenAllAttackCollider()
    {
        for (int i = 0; i < m_colliders.Count; i++)
        {
            if(m_colliders[i] is CAttackCollider)
            {
                m_colliders[i].gameObject.SetActive(true);
            }
        }

        if (m_meleeTrail != null)
            m_meleeTrail.Emit = true;
    }

    public void CloseGuardCollider()
    {
        for (int i = 0; i < m_colliders.Count; i++)
        {
            if (m_colliders[i] is CGuardCollider)
            {
                m_colliders[i].gameObject.SetActive(true);
            }
        }
    }

    public void OpenGuardCollider()
    {
        for (int i = 0; i < m_colliders.Count; i++)
        {
            if (m_colliders[i] is CGuardCollider)
            {
                m_colliders[i].gameObject.SetActive(true);
            }
        }
    }
}
