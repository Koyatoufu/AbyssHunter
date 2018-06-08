using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CMessageUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_subMsgObj = null;
    [SerializeField]
    private GameObject m_mainMsgObj = null;

    [SerializeField]
    private Text m_subMsgText = null;
    [SerializeField]
    private Text m_mainMsgText = null;

    [SerializeField]
    private AudioSource m_subMsgSource = null;

    [SerializeField]
    private AudioSource m_mainMsgSource = null;

    [SerializeField]
    private AudioClip[] m_msgClips = new AudioClip[(int)MessageClipType.MAX];

    public bool IsLoaded { get; private set; }

    void Start()
    {
        StartCoroutine(AllLoadedCourtine());
    }

    IEnumerator AllLoadedCourtine()
    {
        while(m_subMsgObj==null)
        {
            yield return null;
        }

        while(m_mainMsgObj==null)
        {
            yield return null;
        }

        IsLoaded = true;

        yield return null;
    }

    public void ShowSubMessage(string text,float fApearTime,MessageClipType clipType = MessageClipType.Base)
    {
        m_subMsgObj.SetActive(true);

        m_subMsgText.text = text;

        StartCoroutine(SubMessageCourtine(fApearTime, clipType));
    }

    IEnumerator SubMessageCourtine(float fWaitTime, MessageClipType clipType)
    {
        if (CSoundManager.Inst != null)
            CSoundManager.Inst.EffectPlay(m_subMsgSource,m_msgClips[(int)clipType]);

        yield return new WaitForSeconds(fWaitTime);

        m_subMsgObj.SetActive(false);

        yield return null;
    }

    public void ShowMainMessage(string text,float fApearTime,Color color, MessageClipType clipType = MessageClipType.Base)
    {
        m_mainMsgObj.SetActive(true);

        m_mainMsgText.text = text;
        m_mainMsgText.color = color;

        StartCoroutine(MainMessageCourtine(fApearTime,clipType));
    }

    IEnumerator MainMessageCourtine(float fWaitTime,MessageClipType clipType)
    {
        if(CSoundManager.Inst!=null)
            CSoundManager.Inst.EffectPlay(m_mainMsgSource, m_msgClips[(int)clipType]);

        yield return new WaitForSeconds(fWaitTime);

        m_mainMsgObj.SetActive(false);

        yield return null;
    }

}

public enum MessageClipType
{
    Base,
    QuestStart,
    PlayerDied,
    QuestClear,
    MAX
}
