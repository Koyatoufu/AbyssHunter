using UnityEngine;
using UnityEngine.UI;

public class CTitleScene : MonoBehaviour
{
    [SerializeField]
    private GameObject m_soundMgrPrefab;

    [SerializeField]
    private AudioSource m_bgmSource = null;
    [SerializeField]
    private AudioSource m_effectSource = null;

    private CSoundManager m_soundMgr;

    [SerializeField]
    private AudioClip m_titleBgm;
    [SerializeField]
    private GameObject m_mainMenuUI = null;
    [SerializeField]
    private GameObject m_optionUI = null;

    [SerializeField]
    private Slider m_bgmSlider = null;
    [SerializeField]
    private Slider m_effectSlider = null;

    void Awake()
    {
        if(CSoundManager.Inst==null)
        {
            if (m_soundMgrPrefab == null)
                m_soundMgrPrefab = Resources.Load("Prefabs/Etc/SoundManager") as GameObject;
            GameObject soundMgr = Instantiate(m_soundMgrPrefab);
            soundMgr.name = "SoundManager";
        }
    }

    void Start()
    {
        m_soundMgr = CSoundManager.Inst;

        m_bgmSource = GetComponent<AudioSource>();
        if(m_titleBgm==null)
        {
            m_titleBgm = Resources.Load("Sounds/Bgm/stress_test") as AudioClip;
        }

        m_mainMenuUI.SetActive(true);
        m_optionUI.SetActive(false);
    }

    public void TitleBgmPlay()
    {
        m_soundMgr.BgmPlay(m_bgmSource, m_titleBgm);
    }

    public void GameStart()
    {
        if (CDataManager.Inst == null)
            return;
        if (CDataManager.Inst.PlayerRecord == null)
            return;

        Data.PlayerInfo playerInfo = CDataManager.Inst.PlayerRecord.PlayerInfo;

        if (playerInfo == null)
            return;

        if (string.IsNullOrEmpty(playerInfo.name))
        {
            CLoadingScene.LoadingScene("CreateUser");
        }
        else
        {
            CLoadingScene.LoadingScene("Lobby");
        }

        m_soundMgr.StopBgm(m_bgmSource);
    }

    public void TurnMainMenuUI()
    {
        if (m_mainMenuUI != null)
            m_mainMenuUI.SetActive(!m_mainMenuUI.activeSelf);
    }

    public void TurnOptionUI()
    {
        if (m_optionUI != null)
            m_optionUI.SetActive(!m_optionUI.activeSelf);

        if (m_bgmSlider != null)
            m_bgmSlider.value = m_soundMgr.BgmVolume;
        if (m_effectSlider != null)
            m_effectSlider.value = m_soundMgr.EffectVolume;
    }

    public void SoundConfirm()
    {
        BgmVolumeChange();
        EffectVolumeChange();

        if (m_optionUI != null)
            m_optionUI.SetActive(false);
        if (m_mainMenuUI != null)
            m_mainMenuUI.SetActive(true);
    }

    public void BgmVolumeChange()
    {
        if (m_bgmSlider != null)
            CSoundManager.Inst.BgmVolume = m_bgmSlider.value;
        m_bgmSource.volume = CSoundManager.Inst.BgmVolume;
    }

    public void EffectVolumeChange()
    {
        if (m_effectSlider != null)
            CSoundManager.Inst.EffectVolume = m_effectSlider.value;
    }

    public void GameEnd()
    {
        Application.Quit();
    }

    public void EffectPlay(AudioClip clip)
    {
        if(m_effectSource!=null&&m_soundMgr!=null)
        {
            m_soundMgr.EffectPlay(m_effectSource, clip);
        }
    }
}
