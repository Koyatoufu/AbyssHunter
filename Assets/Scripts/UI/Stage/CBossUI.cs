using UnityEngine;
using UnityEngine.UI;

public class CBossUI : CUnitUI
{
    [SerializeField]
    private Text bossNameText = null;

    public override void Init(CUnit unit)
    {
        base.Init(unit);

        if(bossNameText!=null)
        {
            bossNameText.text = unit.name;
        }
    }

    public override void Process()
    {

    }

    public override void SetHPGuageFill(float fAmount)
    {
        base.SetHPGuageFill(fAmount);

        if (m_hpGuageImage.fillAmount <= 0f)
        {
            this.gameObject.SetActive(false);
        }
    }
}
