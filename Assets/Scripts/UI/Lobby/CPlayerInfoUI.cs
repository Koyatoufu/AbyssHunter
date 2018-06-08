using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CPlayerInfoUI : MonoBehaviour
{

    [SerializeField]
    private Text m_textName = null;

    [SerializeField]
    private Image m_imageVow = null;

    [SerializeField]
    private Sprite[] m_iconVow = null;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(InitCoroutine());
	}

    IEnumerator InitCoroutine()
    {
        while(CDataManager.Inst==null)
        {
            yield return null;
        }

        Data.PlayerInfo playerInfo = null;

        while(playerInfo==null)
        {
            playerInfo = CDataManager.Inst.PlayerRecord.PlayerInfo;
            yield return null;
        }

        if (m_textName != null)
        {
            m_textName.text = string.IsNullOrEmpty(playerInfo.name)?"Player":playerInfo.name;
        }
            
        if(m_imageVow!=null)
        {
            m_imageVow.sprite = m_iconVow[(int)playerInfo.gender];
        }
        
        yield return null;
    }
	
}
