using UnityEngine;
using UnityEngine.UI;

public class CEnemyUI : CUnitUI
{
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
