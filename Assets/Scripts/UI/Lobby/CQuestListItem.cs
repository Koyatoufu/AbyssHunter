using UnityEngine;
using UnityEngine.UI;

public class CQuestListItem : MonoBehaviour
{
    [SerializeField]
    private int m_questIndex = 0;

    [SerializeField]
    private Text m_questnameText = null;

    [SerializeField]
    private Image m_questTargetIcon = null;

    private Data.QuestInfo m_questInfo;

    [SerializeField]
    private AudioSource m_audioSource = null;

    public void Init(int nQuestIndex)
    {
        m_questIndex = nQuestIndex;

        CQuestManager questMgr = CQuestManager.Inst;

        Data.QuestInfo? info = CDataManager.Inst.QuestContainer.GetQuest(m_questIndex);
        if (info == null)
        {
            Debug.LogError("Info is Missing");
            return;
        }

        m_questInfo = info.Value;

        if (m_questnameText != null)
            m_questnameText.text = info.Value.questName;

        if (m_questTargetIcon != null)
        {
            CUnitData unitData = CDataManager.Inst.UnitContainer.GetUnitData(m_questInfo.mainTargetType, m_questInfo.mainTargetIdx);

            if(unitData!=null)
                m_questTargetIcon.sprite = unitData.IconImage;
        }
    }

    public void SelectQuest()
    {
        CLobbyManager.Inst.QuestInfoUI.QuestSet(m_questInfo);
    }

    public void SoundPlay()
    {
        if (m_audioSource != null && CSoundManager.Inst != null)
            CSoundManager.Inst.EffectPlay(m_audioSource);
    }
}
