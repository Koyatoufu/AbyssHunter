using UnityEngine;

public class CSoundManager : MonoBehaviour
{
    private const string BGMVOLUME_SAVE = "BgmVolume";
    private const string EFFECTVOLUME_SAVE = "EffectVolume";
    
    private float m_bgmVolume = 1f;
    public float BgmVolume { get { return m_bgmVolume; } set { m_bgmVolume = Mathf.Clamp01(value); } }
    
    private float m_effectVolume = 1f;
    public float EffectVolume { get { return m_effectVolume; } set { m_effectVolume = Mathf.Clamp01(value); } }

    #region Singletone

    public static CSoundManager Inst { get; private set; }

    void Awake()
    {
        if(Inst!=null)
        {
            Destroy(this.gameObject);
            return;
        }

        Inst = this;

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    void Start()
    {
        if(PlayerPrefs.HasKey(BGMVOLUME_SAVE))
            m_bgmVolume = PlayerPrefs.GetFloat(BGMVOLUME_SAVE);
        if (PlayerPrefs.HasKey(EFFECTVOLUME_SAVE))
            m_effectVolume = PlayerPrefs.GetFloat(EFFECTVOLUME_SAVE);
    }

    public void BgmPlay(AudioSource source,AudioClip clip = null, ulong nDelay = 0, bool isLoop = true)
    {
        if (source == null)
            return;

        if(clip!=null)
        source.clip = clip;

        source.loop = isLoop;
        source.volume = m_bgmVolume;
        source.Play(nDelay);
    }

    public void EffectPlay(AudioSource source,AudioClip clip = null, ulong nDelay = 0)
    {
        if (source == null)
            return;

        if (clip != null)
            source.clip = clip;

        source.volume = m_effectVolume;
        source.Play(nDelay);
    }

    public void StopBgm(AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
    }

    public void ResumeBgm(AudioSource source)
    {
        if (source == null)
            return;

        if (!source.isPlaying&&source.clip!=null)
        {
            source.volume = m_bgmVolume;
            source.Play();
        }
    }

    public void SetEffectVolume(AudioSource source)
    {
        if (source == null)
            return;

        if (source.isPlaying)
            source.Stop();

        source.volume = m_effectVolume;
    }

    void OnDestroy()
    {
        PlayerPrefs.SetFloat(BGMVOLUME_SAVE, m_bgmVolume);
        PlayerPrefs.SetFloat(EFFECTVOLUME_SAVE, m_effectVolume);

        PlayerPrefs.Save();
    }
}
