using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CRootLevel m_mainLevel = null;
    public CRootLevel MainLevel { get { return m_mainLevel; } }

    [SerializeField]
    private static string g_curUserName;
    static string CurUserName { get { return g_curUserName; } set { if (string.IsNullOrEmpty(value)) return; g_curUserName = value; } }

    public bool IsGameStart { get; private set; }
    public bool IsGameEnd { get; private set; }

    private CQuestManager m_questManager = null;
    private CStageUIManager m_stageUI = null;

    private Transform m_startPoint = null;
    public Transform StartPoint { get { return m_startPoint; } }

    #region Singletone

    public static GameManager Inst { get; private set; }

    void Awake()
    {
        if(Inst!=null)
        {
            Destroy(this.gameObject);
            return;
        }

        Inst = this;
    }

    #endregion

    void Start()
    {
        m_questManager = CQuestManager.Inst;

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        m_stageUI = CStageUIManager.Inst;

        while(m_stageUI==null)
        {
            yield return null;
        }

        if (m_stageUI != null)
        {
            while(m_stageUI.MessageUI==null)
            {
                yield return null;
            }

            while(!m_stageUI.MessageUI.IsLoaded)
            {
                yield return null;
            }
        }

        #region ObjectPoolWait

        while (CObjectPool.Inst == null)
        {
            yield return null;
        }

        while (CObjectPool.Inst.IsInit == false)
        {
            yield return null;
        }

        #endregion

        //TODO: 플레이어 생성(임시)
        while (CRootLevel.Inst==null)
        {
            yield return null;
        }

        if (!IsGameStart)
        {
            #region BossRespon

            CUnitData targetData = m_questManager.QuestTarget;
            
            if(targetData!=null)
            {
                if(m_questManager.SelectQuest!=null&&m_questManager.SelectQuest.Value.mainTargetType==UnitType.Boss)
                {
                    GameObject bossGo = CObjectPool.Inst.GetUnit(UnitType.Boss, targetData.Index);

                    if(bossGo==null)
                    {
                        IsGameStart = true;
                        yield break;
                    }

                    CBossUnit boss = bossGo.GetComponent<CBossUnit>();

                    if (boss!=null)
                    {
                        boss.ResetUnit();

                        while (!CRootLevel.Inst.SubLevels.ContainsKey(targetData.StayLevel[0]))
                        {
                            yield return null;
                        }

                        CSubLevel startLevel = CRootLevel.Inst.SubLevels[targetData.StayLevel[0]];
                        
                        bossGo.transform.parent = null;
                        boss.ResetUnit();
                        boss.StayLevel = startLevel;
                        bossGo.transform.position = startLevel.SpwanPoints == null ?
                            startLevel.transform.position + Vector3.up * 5f : startLevel.SpwanPoints[0].transform.position + Vector3.up * 2f;
                        bossGo.transform.rotation = Quaternion.identity;
                        bossGo.SetActive(true);

                        if(startLevel.SpwanPoints!=null)
                        {
                            while (boss.Movement == null)
                            {
                                yield return null;
                            }

                            boss.Movement.MoveWays = startLevel.SpwanPoints[0].GetWay(0);
                        }
                    }
                    else
                    {
                        Destroy(bossGo);
                        CLoadingScene.LoadingScene("Lobby");
                        yield break;
                    }
                }
            }
            #endregion
            IsGameStart = true;

            #region PlyaerCreateTemp

            GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Character/Player");
            GameObject startGo = GameObject.FindWithTag("StartPoint");
            m_startPoint = startGo == null ? null : startGo.transform;
            GameObject player = Instantiate(playerPrefab, m_startPoint == null ? Vector3.zero : m_startPoint.position, Quaternion.identity);
            player.name = playerPrefab.name;

            #endregion
        }

        yield return null;

        Data.QuestInfo? questInfo = m_questManager.SelectQuest;

        if (questInfo != null)
        {
            m_stageUI.MessageUI.ShowMainMessage(questInfo.Value.areaName, 3f, new Color(0.8f,0.5f,0f,0.5f), MessageClipType.QuestStart);
            yield return new WaitForSeconds(3f);
            m_stageUI.MessageUI.ShowSubMessage(questInfo.Value.questName, 3f);
            yield return null;
        }
        else
        {
            m_stageUI.MessageUI.ShowSubMessage("Quest Start", 3f);
            yield return null;
        }

    }
}
