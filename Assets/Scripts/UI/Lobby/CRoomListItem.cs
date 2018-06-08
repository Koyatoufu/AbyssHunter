using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class CRoomListItem : MonoBehaviour
{
    [SerializeField]
    private string m_QuestName = null;
    [SerializeField]
    private Text m_roomNameText = null;

    [SerializeField]
    private AudioSource m_audioSource = null;

    public delegate void JoinRoomDelegate(MatchInfoSnapshot matchInfo);
    private JoinRoomDelegate m_joinRoomDelegate;

    private MatchInfoSnapshot m_match;

    public void SetUpRoom(MatchInfoSnapshot match, JoinRoomDelegate joinDelegate)
    {
        m_match = match;
        m_joinRoomDelegate = joinDelegate;

        if(m_roomNameText!=null)
            m_roomNameText.text = m_QuestName + match.name + "( " + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        if(m_joinRoomDelegate!=null && m_match!=null)
            m_joinRoomDelegate(m_match);
    }

    public void SoundPlay()
    {
        if (m_audioSource != null && CSoundManager.Inst != null)
            CSoundManager.Inst.EffectPlay(m_audioSource);
    }
}
