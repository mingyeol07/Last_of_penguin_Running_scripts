using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace Lop.Game
{
    public class GameManager : GameManagerParent
    {
        private GameObject player;
        private PenguinBase playerScript;

        [SerializeField] private FollowCam followCam;
        [SerializeField] private Image img_hitEffect;
        [SerializeField] private BackGroundScroll backGroundScroll;
        [SerializeField] private BackGroundScroll foreGroundScroll;
        [Tooltip("펭귄 ID 순서대로 정렬")]
        [SerializeField] private GameObject[] Penguins;
        private readonly Vector2 penguinSpawnPosition = new Vector2(2.5f, 3.5f);

        [SerializeField] private InGameUIManager gameUIManager;
        [SerializeField] private StageManager stageManager;
        [SerializeField] private FeverManager feverManager;
        private VibratorManager vibratorManager;
        private ScoreCalculationManager scoreManager;

        private float maxTime = 60;

        private float damagePercent = 10;
        private float cliffDamagePercent = 15;

        // 35%확률로 지금 내 시간의 1/5만큼 얻는다.
        private float getWatchPercent = 0.35f;
        private float getWatchRandomValue = (20f / 100f);
        private float getWatchValue = 4;

        private int hitDamageScore = 1000;

        private float emperorReviveTimePercent = 30;

        private bool isEndlessMode;
        public bool IsEndlessMode => isEndlessMode;

        // 화면에 몇개를 얻엇는지 알려주는 스타
        [SerializeField] private GameObject[] getstars;

        #region test
        // 테스트를 위한 무적
        [SerializeField] private bool isMuJuck;
        #endregion

        #region Important Save
        private int score;
        private bool[] isGetStarArray = new bool[3];
        #endregion

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        private void Initialize()
        {
            int selectedPenguinID = PlayerPrefs.GetInt(PlayerPrefsKey.My_PenguinID);
            player = Instantiate(Penguins[selectedPenguinID], penguinSpawnPosition, Quaternion.Euler(0, 90, 0));
            playerScript = player.GetComponent<PenguinBase>();

            isEndlessMode = stageManager.IsEndlessMode;
            scoreManager = new ScoreCalculationManager();
            vibratorManager = new VibratorManager();

            feverManager.SetPlayer(player.GetComponent<PenguinBase>());

            followCam.SetFollowTarget(player);

            if (player.TryGetComponent(out PenguinKing king))
            {
                maxTime += king.PlusTime;
            }

            if (player.TryGetComponent(out PenguinAdelie adelie))
            {
                damagePercent = adelie.DamagePercentValue;
                cliffDamagePercent = adelie.CliffPercentValue;
            }

            gameUIManager.InitTimer(maxTime);
        }

        private void Start()
        {
            gameUIManager.InitMaxMapX(player.transform, stageManager.StageLength);
            SoundManager.Instance.Play_SuperRunBGM();

            player.transform.position = penguinSpawnPosition;
            StartCoroutine(gameUIManager.Co_PauseCount());
        }

        private void Update()
        {
            #region 치트
            if (Input.GetKeyDown(KeyCode.F3))
            {
                GetWatch(100);
            }
            if(Input.GetKeyDown(KeyCode.F2))
            {
                GetFeverStar();
                GetFeverStar();
                GetFeverStar();
                GetFeverStar();
                GetFeverStar();
                GetFeverStar();
                GetFeverStar();
            }
            #endregion
        }

        public override GameObject GetMyPenguin(int playerNumber = 0) {
            return player;
        }

        public override void GetScore(ScoreType type,  int customValue = 0, int playerNumber = 0)
        {
            int addValue = scoreManager.CalculationScore(type);
            score += addValue;

            //if (type == ScoreType.Squid)
            //{
            //    float multipleValue = scoreManager.ComboMultipleValue * scoreManager.ComboValue;
            //    gameUIManager.ComboUpdata(multipleValue);
            //}
            //else
            //{
            //    gameUIManager.ComboUpdata(0);
            //}

            if(type == ScoreType.None && customValue > 0)
            {
                gameUIManager.AddScore(customValue);
                gameUIManager.ScoreUpdate();

                return;
            }

            gameUIManager.AddScore(addValue);
            gameUIManager.ScoreUpdate();
        }

        public override void GetShield(float percent, int playerNumber = 0 )
        {
            float value = maxTime * (percent / 100);

            gameUIManager.AddShield(value);
        }

        public void GetTimePacent(float percent)
        {
            float value = maxTime * (percent / 100);

            gameUIManager.AddTime(value);
        }

        public void GetTimeRandom()
        {
            float value;
            float randomValue = UnityEngine.Random.Range(0f, 1f);

            if (randomValue < getWatchPercent)
            {
                value = gameUIManager.TimerCurTime * getWatchRandomValue;
            }
            else
            {
                value = getWatchValue;
            }

            playerScript.GetTimeEvent(value);
            gameUIManager.AddTime(value);
        }

        public override void GetWatch(float percent = 999, int playerNumber =0)
        {
            float value;

            if (percent == 999)
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);

                if (randomValue < getWatchPercent)
                {
                    value = gameUIManager.TimerCurTime * getWatchRandomValue;
                }
                else
                {
                    value = getWatchValue;
                }

                player.GetComponent<PenguinBase>().GetTimeEvent(value);
            }
            else
            {
                value = maxTime * (percent / 100);
            }

            gameUIManager.AddTime(value);
        }

        public override void GetFeverStar(int playerNumber = 0)
        {
            feverManager.AddFeverGauge();
        }

        public void GetStageStar(int number)
        {
            if (number > 2) return;
            isGetStarArray[number] = true;
            getstars[number].SetActive(true);
        }

        public override void PlayHit(int playerNumber = 0)
        {
            if (isMuJuck) return;

            Camera.main.GetComponent<CameraShake>().StartShake(0.3f);
            SoundManager.Instance.Play_PlayerHitSound();
            //vibratorManager.PlayVibrate(0);
            StartCoroutine(Co_HitEffect());

            score += -hitDamageScore;
            gameUIManager.AddScore(-hitDamageScore);
            GetWatch(-damagePercent);
        }

        public override void PlayFall(int playerNumber = 0)
        {
            if (isMuJuck) return;

            //vibratorManager.PlayVibrate(0);

            StartCoroutine(PlayFallEffect());
            StartCoroutine(followCam.Co_fallCameraMove());
            followCam.CanMove = false;

            GetWatch(-cliffDamagePercent);
        }

        public override void SetBackgroundMove(bool value)
        {
            backGroundScroll.IsStop = value;
            foreGroundScroll.IsStop = value;
        }

        IEnumerator PlayFallEffect()
        {
            yield return new WaitForSeconds(0.3f);

            SoundManager.Instance.Play_PlayerHitSound();
            

            img_hitEffect.color = new Color(1, 1, 1, 1);  // 처음에 완전한 불투명도
            float time = 0;
            float fadeDuration = 2.4f;  // 페이드아웃이 지속되는 시간

            // 알파값이 서서히 줄어들도록 처리
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float alphaValue = Mathf.Lerp(1, 0, time / fadeDuration);  // 1에서 0으로 서서히 감소
                img_hitEffect.color = new Color(1, 1, 1, alphaValue);

                yield return null;  // 한 프레임 대기
            }

            img_hitEffect.color = new Color(1, 1, 1, 0);  // 완전히 투명해짐
        }

        public override IEnumerator Co_HitEffect(int playerNumber = 0)
        {
            img_hitEffect.color = new Color(1, 1, 1, 1);  // 처음에 완전한 불투명도
            float time = 0;
            float fadeDuration = 1f;  // 페이드아웃이 지속되는 시간

            // 알파값이 서서히 줄어들도록 처리
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float alphaValue = Mathf.Lerp(1, 0, time / fadeDuration);  // 1에서 0으로 서서히 감소
                img_hitEffect.color = new Color(1, 1, 1, alphaValue);

                yield return null;  // 한 프레임 대기
            }

            img_hitEffect.color = new Color(1, 1, 1, 0);  // 완전히 투명해짐
        }

        public override void Gameover(int playerNumber = 0)
        {
            if (player.GetComponent<PenguinBase>().IsDie) return;

            player.GetComponent<PenguinBase>().GameoverAction();
            backGroundScroll.IsStop = true;
            foreGroundScroll.IsStop = true;
            gameUIManager.IsStop = true;

            if (player.TryGetComponent(out PenguinEmperor emperor))
            {
                if (emperor.IsRevive == false)
                {
                    // Rockhopper일 경우 부활 1회
                    StartCoroutine(emperor.Co_Revive());
                    return;
                }
            }

            // gameover
            SoundManager.Instance.Stop_BGM();
            PlayerPrefs.SetInt(PlayerPrefsKey.My_PlayScore, score);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_1, isGetStarArray[0] ? 1 : 0);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_2, isGetStarArray[1] ? 1 : 0);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_3, isGetStarArray[2] ? 1 : 0);
            StartCoroutine(gameUIManager.Co_LoadLeaderBoard());
        }

        public override void GameClear(int playerNumber = 0) // 깃발에 닿앗을 때
        {
            backGroundScroll.IsStop = true;
            foreGroundScroll.IsStop = true;
            gameUIManager.IsStop = true;

            PlayerPrefs.SetInt(PlayerPrefsKey.My_PlayScore, score);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_1, isGetStarArray[0] ? 1 : 0);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_2, isGetStarArray[1] ? 1 : 0);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_3, isGetStarArray[2] ? 1 : 0);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_PlayStageIsClear, 1);
            StartCoroutine(gameUIManager.Co_LoadLeaderBoard());
        }

        #region 특정 펭귄 전용 함수
        public override void RockhopperAddFeverTime(float value, int playerNumber = 0)
        {
            feverManager.SetAdditionalFeverTime(value);
        }

        public override void EmperorRevive(int playerNumber = 0)
        {
            backGroundScroll.IsStop = false;
            foreGroundScroll.IsStop = false;
            gameUIManager.IsStop = false;

            GetWatch(emperorReviveTimePercent);
        }
        #endregion
    }
}