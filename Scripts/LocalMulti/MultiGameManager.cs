using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

namespace Lop.Game
{
    public class MultiGameManager : GameManagerParent
    {
        [Header("공용")]
        [Tooltip("펭귄 ID 순서대로 정렬")]
        [SerializeField] private Button btn_replay;
        [SerializeField] private Button btn_exit;
        [SerializeField] private GameObject[] Penguins;
        [SerializeField] private MultiGamePlayerManager[] playerManagers;

        [SerializeField] private TMP_Text[] txt_playerNames;

        [SerializeField] private Sprite[] penguinIcons;
        [SerializeField] private PenguinData[] penguinDatas;

        [SerializeField] private Image[] img_playerIcons;

        protected override void Awake() {
            base.Awake();

            btn_exit.onClick.AddListener(() => { Time.timeScale = 1; SceneManager.LoadScene(SceneNameString._02_Main); });
            btn_replay.onClick.AddListener(() => { SceneManager.LoadScene(SceneNameString._06_LocalMulti2); });

            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < playerManagers.Length; i++)
            {
                MultiGamePlayerManager manager = playerManagers[i];

                manager.ScoreManager = new ScoreCalculationManager();
                manager.vibratorManager = new VibratorManager();

                int penguinId = MultiSaver.Instance.PenguinIDArray[i];
                manager.Player = Instantiate(Penguins[penguinId], manager.PenguinSpawnPosition, Quaternion.Euler(0, 90, 0));

                manager.FollowCam.SetFollowTarget(manager.Player);

                manager.GameUIManager.SetMulti(i + 1);

                manager.StageManager.SetMulti(i + 1);

                manager.FeverManager.SetPlayer(manager.Player.GetComponent<PenguinBase>());

                manager.Player.GetComponent<PenguinBase>().SetMultiPenguin(i + 1);

                if (manager.Player.TryGetComponent(out PenguinKing king))
                {
                    manager.MaxTime += king.PlusTime;
                }
                if (manager.Player.TryGetComponent(out PenguinAdelie adelie))
                {
                    manager.DamagePercent = adelie.DamagePercentValue;
                    manager.CliffDamagePercent = adelie.CliffPercentValue;
                }

                manager.GameUIManager.InitTimer(manager.MaxTime);

                txt_playerNames[i].text = MultiSaver.Instance.NameArray[i];
                img_playerIcons[i].sprite = penguinIcons[MultiSaver.Instance.PenguinIDArray[i]];
            }
        }

        private void Start()
        {
            SoundManager.Instance.Play_SuperRunBGM();

            for (int i = 0; i < playerManagers.Length; i++)
            {
                playerManagers[i].GameUIManager.InitMaxMapX(playerManagers[i].Player.transform, playerManagers[i].StageManager.StageLength);
                playerManagers[i].GameUIManager.SetMySkillIcon(penguinDatas[MultiSaver.Instance.PenguinIDArray[i]].BattleAbility_sprite);

                if (playerManagers[i].Player.GetComponent<PenguinBase>().MyPenguinName == PenguinName.Default)
                {
                    for (int j = 0; j < playerManagers.Length; j++)
                    {
                        if (j == i) continue;
                        playerManagers[i].StageManager.SetOtherPenguinIsDefault();
                    }
                }
            }

            StartCoroutine(playerManagers[MultiSaver.player1Index].GameUIManager.Co_PauseCount());
        }

        public override GameObject GetMyPenguin(int playerNumber = 0)
        {
            return playerNumber == 0 ? null : playerManagers[playerNumber - 1].Player;
        }

        public void SkillEvent(int penguinNumber, int targetPlayerNumber, int playerNumber)
        {
            MultiGamePlayerManager targetManager = playerManagers[targetPlayerNumber];

            targetManager.GameUIManager.SetSkillEvent
                (
                    penguinDatas[penguinNumber].BattleAbility_sprite,
                    MultiSaver.Instance.NameArray[playerNumber],
                    penguinDatas[penguinNumber].BattleAbility_name
                );
        }

        public override void GetScore(ScoreType type,int customValue = 0, int playerNumber =0)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            int addValue = manager.ScoreManager.CalculationScore(type);

            //if (type == ScoreType.Squid)
            //{
            //    float multipleValue = manager.ScoreManager.ComboMultipleValue * manager.ScoreManager.ComboValue;
            //    manager.GameUIManager.ComboUpdata(multipleValue);
            //}
            //else
            //{
            //    manager.GameUIManager.ComboUpdata(0);
            //}

            if(type == ScoreType.None && customValue != 0)
            {
                manager.Score += customValue;
                manager.GameUIManager.AddScore(customValue);
                manager.AttackCoolManager.AddAttackCool(customValue);
                return;
            }
            else
            {
                manager.Score += addValue;
                manager.GameUIManager.AddScore(addValue);
                manager.AttackCoolManager.AddAttackCool(addValue);
            }
        }

        public override void GetShield(float percent, int playerNumber = 0)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            float value = manager.MaxTime * (percent / 100);
            manager.GameUIManager.AddShield(value);
        }

        public override void GetWatch(float percent, int playerNumber = 0)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            float value;
            value = manager.MaxTime * (percent / 100);
            manager.GameUIManager.AddTime(value);
        }

        public override void GetFeverStar(int playerNumber)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            manager.FeverManager.AddFeverGauge();
        }

        public override void PlayHit(int playerNumber)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            SoundManager.Instance.Play_PlayerHitSound();
            //manager.vibratorManager.PlayVibrate(playerNumber - 1);
            manager.Shaker.StartShake(0.3f);

            GetScore(ScoreType.None,  -1000, playerNumber);
            //GetWatch(playerNumber, -manager.DamagePercent);
            StartCoroutine(Co_HitEffect(playerNumber));
        }

        public override void PlayFall(int playerNumber)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            SoundManager.Instance.Play_PlayerHitSound();
            //manager.vibratorManager.PlayVibrate(playerNumber - 1);

            manager.Shaker.StartShake(0.3f);

            GetWatch(-manager.CliffDamagePercent, playerNumber);
            StartCoroutine(Co_HitEffect(playerNumber));
        }

        public override IEnumerator Co_HitEffect(int playerNumber)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            manager.Img_hitEffect.color = Color.white;  // 처음에 완전한 불투명도
            float time = 0;
            float fadeDuration = 1f;  // 페이드아웃이 지속되는 시간

            // 알파값이 서서히 줄어들도록 처리
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float alphaValue = Mathf.Lerp(1, 0, time / fadeDuration);  // 1에서 0으로 서서히 감소
                manager.Img_hitEffect.color = new Color(1, 1, 1, alphaValue);

                yield return null;  // 한 프레임 대기
            }

            manager.Img_hitEffect.color = new Color(1, 1, 1, 0);  // 완전히 투명해짐
        }

        public override void Gameover(int playerNumber)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            if (manager.Player.GetComponent<PenguinBase>().IsDie) return;

            manager.Player.GetComponent<PenguinBase>().GameoverAction();
            manager.BackGroundScroll.IsStop = true;
            manager.ForeGroundScroll.IsStop = true;
            manager.GameUIManager.IsStop = true;

            if (manager.Player.TryGetComponent(out PenguinEmperor emperor))
            {
                if (emperor.IsRevive == false)
                {
                    // Rockhopper일 경우 부활 1회
                    StartCoroutine(emperor.Co_Revive());
                    return;
                }
            }

            MultiSaver.Instance.ScoreArray[playerNumber - 1] = manager.Score;
            StartCoroutine(Co_ShowGameoverPanel(manager));
        }

        private IEnumerator Co_ShowGameoverPanel(MultiGamePlayerManager manager)
        {
            yield return new WaitForSeconds(2);

            for (int i = 0; i < playerManagers.Length; i++) {
                if (!playerManagers[i].BackGroundScroll.IsStop) {
                    // 다른 펭귄이 아직 클리어하지 못했을 때
                    break;
                }

                if (i == playerManagers.Length - 1) {
                    SoundManager.Instance.Stop_BGM();
                    StartCoroutine(Co_LoadMultiLeaderboard());
                }
            }
            //manager.GameUIManager.SetGameOverPenaltyPanel();

            //manager.GameUIManager.AddTime(manager.MaxTime);
            //manager.Player.GetComponent<PenguinBase>().MultiRevive();
        }

        public override void GameClear(int playerNumber) // 깃발에 닿앗을 때
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            manager.BackGroundScroll.IsStop = true;
            manager.ForeGroundScroll.IsStop = true;
            manager.GameUIManager.IsStop = true;
            manager.isClear = true;

            MultiSaver.Instance.ScoreArray[playerNumber - 1] = manager.Score;
            StartCoroutine(Co_ShowGameClearPanel(manager));
            for (int i = 0; i < playerManagers.Length; i++)
            {
                if (playerManagers[i].isClear == false)
                {
                    // 다른 펭귄이 아직 클리어하지 못했을 때
                    GetScore(ScoreType.None, 50000, playerNumber);
                }
                if(!playerManagers[i].BackGroundScroll.IsStop) {
                    break;
                }

                if (i == playerManagers.Length - 1)
                {
                    SoundManager.Instance.Stop_BGM();
                    StartCoroutine(Co_LoadMultiLeaderboard());
                }
            }
        }

        private IEnumerator Co_LoadMultiLeaderboard()
        {
            yield return new WaitForSeconds(2);
            if(playerManagers.Length >2) SceneManager.LoadScene(SceneNameString._10_MultiLeaderBoard4);
            else SceneManager.LoadScene(SceneNameString._10_MultiLeaderBoard2);
        }

        private IEnumerator Co_ShowGameClearPanel(MultiGamePlayerManager manager)
        {
            yield return new WaitForSeconds(2);
            manager.GameUIManager.SetGameClearPanel();
        }

        #region 특정 펭귄 전용 함수
        public override void RockhopperAddFeverTime(float value, int playerNumber)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            manager.FeverManager.SetAdditionalFeverTime(value);
        }

        public override void EmperorRevive(int playerNumber)
        {
            MultiGamePlayerManager manager = playerManagers[playerNumber - 1];

            manager.BackGroundScroll.IsStop = false;
            manager.ForeGroundScroll.IsStop = false;
            manager.GameUIManager.IsStop = false;

            GetWatch(30, playerNumber);
        }
        #endregion

        public void Attack(PenguinName penguinName, int playerNumber)
        {
            if (playerManagers[playerNumber - 1].AttackCoolManager.OnAttack == false) return;
            playerManagers[playerNumber - 1].AttackCoolManager.ResetCool();

            int targetPlayerNumber = Random.Range(0, playerManagers.Length);

            while(playerNumber - 1 == targetPlayerNumber)
            {
                targetPlayerNumber = Random.Range(0, playerManagers.Length);
            }

            SkillEvent((int)penguinName, targetPlayerNumber, playerNumber - 1);

            switch (penguinName)
            {
                case PenguinName.Default:
                    playerManagers[targetPlayerNumber].DefaultAttack(playerNumber);
                    SoundManager.Instance.Play_BattleSkill();
                    break;
                case PenguinName.Rockhopper:
                    playerManagers[targetPlayerNumber].RockhopperAttack(playerNumber);
                    SoundManager.Instance.Play_BattleSkill();
                    break;
                case PenguinName.Emperor:
                    playerManagers[targetPlayerNumber].EmperorAttack(playerNumber);
                    SoundManager.Instance.Play_EmperorSkill();
                    break;
                case PenguinName.Magellanic:
                    playerManagers[targetPlayerNumber].MagellanicAttack(playerNumber);
                    SoundManager.Instance.Play_BattleSkill();
                    break;
                case PenguinName.Gentoo:
                    playerManagers[targetPlayerNumber].ShowGentooAttack(playerNumber);
                    for (int j = 0; j < playerManagers.Length; j++)
                    {
                        if (j == targetPlayerNumber) continue;
                        playerManagers[j].GentooAttack();
                    }
                    SoundManager.Instance.Play_BattleSkill();
                    break;
                case PenguinName.King:
                    playerManagers[targetPlayerNumber].KingAttack(playerNumber);
                    SoundManager.Instance.Play_BattleSkill();
                    break;
                case PenguinName.Adlie:
                    playerManagers[targetPlayerNumber].AdlieAttack(playerNumber);
                    SoundManager.Instance.Play_BattleSkill();
                    break;
            }
        }
    }
}