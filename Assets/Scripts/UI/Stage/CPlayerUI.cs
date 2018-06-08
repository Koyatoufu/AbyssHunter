using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CPlayerUI : CUnitUI
{
    CInventory m_inventory = null;
    CInputCtrl m_input = null;

    #region Vows
    [Header("Vows")]
    [SerializeField]
    private Image m_vowIcon = null;
    [SerializeField]
    private Sprite[] m_vowIcons = null;
    #endregion
    #region Stats
    [Header("PlayerStatus")]
    [SerializeField]
    private Image m_StaminaGuageImage = null;

    [SerializeField]
    private Image m_ItemSlotIcon = null;
    [SerializeField]
    private Text m_ItemSlotCount = null;
    int m_itemIdx = 0;

    [SerializeField]
    private Image m_spellSlotIcon = null;
    [SerializeField]
    private Text m_spellSlotCount = null;
    int m_spellIdx = 0;

    [SerializeField]
    private Image m_weaponIcon = null;

    [SerializeField]
    private GameObject m_menuGo = null;

    [SerializeField]
    private Sprite m_emptyIcon = null;

    #endregion
    #region Buff

    private enum BuffIconType
    {
        Fire,
        Ice,
        Thunder,
        Strength,
        Guard,
        Speed,
        None
    }
    [Header("Buff")]
    [SerializeField]
    private Sprite[] m_buffIcons = null;

    [SerializeField]
    private GameObject m_buffItem = null;

    [SerializeField]
    private Transform m_buffParent = null;
    #endregion

    [SerializeField]
    private AudioSource m_slotSource = null;
    [SerializeField]
    private AudioClip m_slotClip = null;
    [SerializeField]
    private AudioClip m_showClip = null;

    public override void Init(CUnit unit)
    {
        CPlayerUnit player = unit as CPlayerUnit;
        
        if (player == null)
            return;

        base.Init(unit);

        if(m_vowIcon!=null&&m_vowIcons.Length>=2)
        {
            m_vowIcon.sprite = m_vowIcons[(int)player.Gender];
        }

        m_input = player.InputCtrl;
        m_inventory = player.Inventory;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (m_menuGo != null)
            m_menuGo.SetActive(false);

        InventoryCheck();
    }

    public override void Process()
    {
        if (m_unit == null)
            return;

        if (m_unit.Status.CurState == State.Dead)
        {
            if(gameObject.activeSelf)
            {
                SetHPGuageFill(0f);
                this.gameObject.SetActive(false);
            }

            return;
        }

        InputCheck();
        InventoryCheck();

        SetHPGuageFill(m_unit.Status.GetHpAmount());
        SetStaminaGuageFill(m_unit.Status.GetStaminaAmount());
    }

    void InputCheck()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Cursor.visible = !Cursor.visible;
            
            if (m_menuGo != null)
                m_menuGo.SetActive(Cursor.visible ? true : false);
        }

        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;

        if (m_input == null)
            return;

        if(m_input.InputData.dPadX>0.3f)
        {
            WeaponMainSubChange();
        }
        else if (m_input.InputData.dPadX < -0.3f)
        {
            SelectSpellChange();
        }

        if (Mathf.Abs(m_input.InputData.dPadY) > 0.5f)
        {
            SelectItemChange();
        }
    }

    public override void SetHPGuageFill(float fAmount)
    {
        base.SetHPGuageFill(fAmount);
    }

    public override void SetStaminaGuageFill(float fAmount)
    {
        if (m_StaminaGuageImage == null)
            return;

        m_StaminaGuageImage.fillAmount = fAmount;
    }

    void InventoryCheck()
    {
        if (m_inventory == null)
            return;

        m_itemIdx = m_inventory.CurItemSelect;
        m_spellIdx = m_inventory.CurSpellSelect;

        if (m_weaponIcon != null)
        {
            m_weaponIcon.sprite = m_inventory.MainEquip==null ? m_emptyIcon: m_inventory.MainEquip.IconImg;
        }
        if(m_ItemSlotIcon!=null)
        {

            if (m_inventory.InventoryInfo.itemList == null || m_inventory.InventoryInfo.itemList.Count < 1)
            {
                m_ItemSlotIcon.sprite = m_emptyIcon;
                m_ItemSlotCount.text = "";
            }
            else
            {
                m_ItemSlotIcon.sprite = m_inventory.InventoryInfo.itemList[m_itemIdx].item.IconImg;
                m_ItemSlotCount.text = m_inventory.InventoryInfo.itemList[m_itemIdx].count.ToString();
            }
                
        }
        if(m_spellSlotIcon!=null)
        {
            if (m_inventory.InventoryInfo.spellList == null || m_inventory.InventoryInfo.spellList.Count < 1)
            {
                m_spellSlotIcon.sprite = m_emptyIcon;
                m_spellSlotCount.text = "";
            }
            else
            {
                m_spellSlotIcon.sprite = m_inventory.InventoryInfo.spellList[m_spellIdx].item.IconImg;
                m_spellSlotCount.text = m_inventory.InventoryInfo.spellList[m_spellIdx].count.ToString();
            }
        }   
    }

    void WeaponMainSubChange()
    {
        if (m_inventory == null)
            return;

        //TODO: 메인과 서브의 아이콘을 바꾼다
        m_inventory.ChangeMainSub();

        m_weaponIcon.sprite = m_emptyIcon;
        if(m_weaponIcon!=null)
        {
            m_weaponIcon.sprite = m_inventory.MainEquip.IconImg;
        }

        EffectSoundPlay(m_slotClip);
    }

    void SelectItemChange(bool flag = false)
    {
        if (m_inventory == null)
            return;

        if (m_inventory.InventoryInfo.itemList.Count < 1)
            return;

        if (flag)
        {
            m_itemIdx--;
            if (m_itemIdx < 0)
            {
                m_itemIdx = m_inventory.InventoryInfo.itemList.Count - 1;
            }
        }
        else
        {
            m_itemIdx++;
            if (m_itemIdx >= m_inventory.InventoryInfo.itemList.Count)
            {
                m_itemIdx = 0;
            }
        }

        m_inventory.CurItemSelect = m_itemIdx;
        EffectSoundPlay(m_slotClip);
    }

    void SelectSpellChange()
    {
        if (m_inventory == null)
            return;

        if(m_inventory.InventoryInfo.spellList.Count<1)
        {
            return;
        }

        m_spellIdx++;

        if (m_spellIdx >= m_inventory.InventoryInfo.spellList.Count)
            m_spellIdx = 0;

        m_inventory.CurSpellSelect = m_spellIdx;
        EffectSoundPlay(m_slotClip);
    }

    public void ShowChracterInfo()
    {
        if(m_menuGo!=null&&m_menuGo.activeSelf)
        {
            m_menuGo.SetActive(false);
        }

        if(CStageUIManager.Inst!=null)
        {
            CStageUIManager.Inst.InventoryUI.TurnEquipInfo(true);
        }

        EffectSoundPlay(m_showClip);
    }

    public void ShowInventory()
    {
        if (m_menuGo != null && m_menuGo.activeSelf)
        {
            m_menuGo.SetActive(false);
        }

        if (CStageUIManager.Inst != null)
        {
            CStageUIManager.Inst.InventoryUI.TurnPouch(true);
        }

        EffectSoundPlay(m_showClip);
    }

    public void ReturnLobby()
    {
        if(CQuestManager.Inst!=null)
        {
            CQuestManager.Inst.QuestReset();
            return;
        }
    }

    public override void BuffIn(BuffType buffType,Stat.Element element = Stat.Element.None)
    {
        BuffIconType iconType = BuffIconType.None;
        switch(buffType)
        {
            case BuffType.Element:
                {
                    if (element == Stat.Element.None)
                        return;

                    switch(element)
                    {
                        case Stat.Element.Fire:
                            iconType = BuffIconType.Fire;
                            break;
                        case Stat.Element.Ice:
                            iconType = BuffIconType.Ice;
                            break;
                        case Stat.Element.Thunder:
                            iconType = BuffIconType.Thunder;
                            break;
                    }
                }
                break;
            case BuffType.Strength:
                iconType = BuffIconType.Strength;
                break;
            case BuffType.Guard:
                iconType = BuffIconType.Guard;
                break;
            case BuffType.Speed:
                iconType = BuffIconType.Speed;
                break;
        }

        StartCoroutine(BuffIconCoroutine(iconType));
    }
    
    IEnumerator BuffIconCoroutine(BuffIconType type)
    {
        if (type == BuffIconType.None)
            yield break;

        GameObject buffItem = Instantiate(m_buffItem, m_buffParent);
        buffItem.name = m_buffItem.name;

        Image iconImage = buffItem.GetComponent<Image>();
        if (iconImage == null)
            yield break;
        iconImage.sprite = m_buffIcons[(int)type];

        yield return new WaitForSeconds(10f);

        iconImage.sprite = null;
        Destroy(buffItem);

        yield return null;
    }

    public void EffectSoundPlay(AudioClip clip)
    {
        if (CSoundManager.Inst != null && m_slotSource != null)
            CSoundManager.Inst.EffectPlay(m_slotSource, clip);
    }
}
