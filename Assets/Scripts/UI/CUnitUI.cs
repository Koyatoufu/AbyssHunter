using UnityEngine;
using UnityEngine.UI;

public abstract class CUnitUI : MonoBehaviour
{
    [SerializeField]
    protected CUnit m_unit = null;

    [SerializeField]
    protected Image m_hpGuageImage = null;

    public virtual void Init(CUnit unit)
    {
        m_unit = unit;
    }

    public virtual void SetHPGuageFill(float fAmount)
    {
        if (m_hpGuageImage == null)
            return;

        m_hpGuageImage.fillAmount = fAmount;
    }

    public virtual void SetStaminaGuageFill(float fAmount){}

    public abstract void Process();

    public virtual void BuffIn(BuffType buffType,Stat.Element element = Stat.Element.None) {}

    public virtual void ResetUI()
    {
        if (m_hpGuageImage == null)
            return;

        m_hpGuageImage.fillAmount = 1f;
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
