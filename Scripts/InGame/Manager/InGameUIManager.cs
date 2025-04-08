using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lop.Game
{
    // TODO: GameOver 패널 추가
    public class InGameUIManager : MonoBehaviour
    {
        private int selectIndex = 0;

        [Header("Timer")]
        [SerializeField] private Slider slider_timer;
        [SerializeField] private Slider slider_shield;
        private float timerCurTime;
        private float shieldCurTime;
        private float timerMaxTime;
        public float TimerCurTime => timerCurTime;

        [Header("Pause")]
        [SerializeField] private Button btn_pause;
        [SerializeField] private GameObject pnl_pause;
        [SerializeField] private Button btn_continue;
        [SerializeField] private Button btn_replay;
        [SerializeField] private Button btn_exit;
        [SerializeField] private Transform checkBoxTransform;
        private bool isPause;

        [Header("Score")]
        [SerializeField] private TMP_Text txt_score;
        [SerializeField] private TMP_Text txt_comboCount;
        [SerializeField] private TMP_Text txt_plusScore;
        public float plusTimerValueByMagellanicAttack = 1; // += 0.008f;
        private int score;
        private int combo;

        [Header("Progress")]
        [SerializeField] private Slider slider_progress;
        private Transform playerTransform;
        private float mapMaxX;

        [Header("Tutorial")]
        [SerializeField] private GameObject tutorial_key_show;

        public bool IsStop { get; set; }

        [Header("Multi")]
        [SerializeField] private GameObject pnl_diePenalty;
        [SerializeField] private GameObject pnl_gameclaer;
        private int penguinNumber;

        [SerializeField] private Image img_mySkillIcon;

        [SerializeField] private Image img_otherSkillIcon;
        [SerializeField] private TMP_Text txt_rivalSkillName;
        [SerializeField] private Animator skillAnimator;

        [SerializeField] private TMP_Text txt_pauseCount;

        private GameManagerParent gameManager;
        private GameObject myPenguin;
        private PenguinBase myPenguinScript;

        private void Awake()
        {
            if(PlayerPrefs.GetInt(PlayerPrefsKey.My_PlayStageID) == 0)
            {
                //if(tutorial_key_show != null) tutorial_key_show.SetActive(true);
            }
            OnClickButtons();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                gameManager = GameManager.Instance;
            }
            else
            {
                gameManager = MultiGameManager.Instance;
            }

            myPenguin = gameManager.GetMyPenguin(penguinNumber);
            myPenguinScript = myPenguin.GetComponent<PenguinBase>();

        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                OnPause();
            }
            if (IsStop) return;

            TimerUpdate();
            ProgressUpdate();
        }

        #region Progress
        public void InitMaxMapX(Transform playerTransform, int x)
        {
            this.playerTransform = playerTransform;
            mapMaxX = x;
        }
        private void ProgressUpdate()
        {
            if (slider_progress == null) return;
            slider_progress.value = Mathf.FloorToInt(playerTransform.position.x) / mapMaxX;
        }
        #endregion

        #region Score
        public void ScoreUpdate()
        {
            txt_score.text = score.ToString("D4");
        }
        public void ComboUpdata(float combo)
        {
            txt_comboCount.text = "X" + combo.ToString();
        }
        public void AddScore(int value)
        {
            score += value;
            txt_plusScore.text = (value < 0 ? "-" : "+") + Mathf.Abs(value).ToString();
            txt_plusScore.GetComponent<Animator>().SetTrigger("Play");
            ScoreUpdate();
        }
        #endregion

        #region Timer
        private void TimerUpdate()
        {
            if (timerCurTime <= 0)
            {
                gameManager.Gameover(penguinNumber);
            }
            else
            {
                if (myPenguinScript.MyPenguinName == PenguinName.Gentoo && myPenguinScript.IsFever) return;

                timerCurTime -= plusTimerValueByMagellanicAttack * Time.deltaTime;
                shieldCurTime -= plusTimerValueByMagellanicAttack * Time.deltaTime;
            }

            slider_shield.value = shieldCurTime / timerMaxTime;
            slider_timer.value = timerCurTime / timerMaxTime;
        }

        public IEnumerator Co_LoadLeaderBoard()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(SceneNameString._10_LeaderBoard);
        }
        public void InitTimer(float maxTime)
        {
            timerMaxTime = maxTime;
            shieldCurTime = timerMaxTime;
            timerCurTime = timerMaxTime;
        }
        public void AddShield(float value)
        {
            shieldCurTime += value;

            if (shieldCurTime > timerMaxTime)
            {
                shieldCurTime = timerMaxTime;
            }

            if((shieldCurTime - timerCurTime) > value) // @초에 N만큼 생성된다고 할때, 최대 N까지만 생성 (N = 30)
            {
                shieldCurTime = timerCurTime + value;
            }
        }
        public void AddTime(float value)
        {
            if(value < 0)
            {
                shieldCurTime += value;
                if (timerCurTime > shieldCurTime)
                {
                    timerCurTime = shieldCurTime;
                }
            }
            else
            {
                shieldCurTime += value;
                timerCurTime += value;

                if(timerCurTime > timerMaxTime)
                    timerCurTime = timerMaxTime;
                if (shieldCurTime > timerMaxTime)
                    shieldCurTime = timerMaxTime;
            }
        }
        #endregion

        #region Pause
        private void OnClickButtons()
        {
            btn_pause?.onClick.AddListener(() => OnPause());
            btn_continue?.onClick.AddListener(() => OnContinue());
            btn_replay?.onClick.AddListener(() => OnClickRePlay());
            btn_exit?.onClick.AddListener(() => OnClickExit());
        }
        public void OnPause()
        {
            SoundManager.Instance.Play_ButtonStop();

            if (isPause == true || pnl_pause == null) return;

            SoundManager.Instance.Stop_BGM();
            selectIndex = 0;
            pnl_pause.SetActive(true);
            txt_pauseCount.gameObject.SetActive(false);

            Time.timeScale = 0f;
            isPause = true;
        }
        private void OnContinue()
        {
            if (isPause == false) return;

            pnl_pause.SetActive(false);
            StartCoroutine(Co_PauseCount());
        }
        public IEnumerator Co_PauseCount()
        {
            txt_pauseCount.gameObject.SetActive(true);
            SoundManager.Instance.Stop_BGM();
            SoundManager.Instance.Play_CountDown();
            Time.timeScale = 0;
            isPause = true;

            int time = 3;
            float realTime;

            while (time > 0)
            {
                txt_pauseCount.text = time.ToString();
                realTime = 1;
                while (realTime > 0)
                {
                    realTime -= Time.unscaledDeltaTime;
                    yield return null;
                }
                yield return null;
                time -= 1;
            }

            txt_pauseCount.text = "Go!";
            SoundManager.Instance.Play_BGM();
            isPause = false;
            Time.timeScale = 1;

            float t = 1;
            while (txt_pauseCount.gameObject.activeSelf == true && t > 0)
            {
                t -= Time.deltaTime;
                yield return null;
            }
            txt_pauseCount.gameObject.SetActive(false);
        }
        private void OnClickRePlay()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneNameString._06_InGame);
        }
        private void OnClickExit()
        {
            Time.timeScale = 1f;
            SoundManager.Instance.Stop_BGM();
            SceneManager.LoadScene(SceneNameString._04_StoryMode);
        }
        #endregion

        #region Multi
        public void SetMulti(int penguinNumber)
        {
            this.penguinNumber = penguinNumber;
        }
        public void SetGameOverPenaltyPanel()
        {
            pnl_diePenalty.SetActive(true);
        }
        public void SetGameClearPanel()
        {
            pnl_gameclaer.SetActive(true);
        }
        public void SetSkillEvent(Sprite sprite, string name, string skillName)
        {
            skillAnimator.SetTrigger("Event");
            txt_rivalSkillName.text = name + "님의" + System.Environment.NewLine + skillName + "!";
            img_otherSkillIcon.sprite = sprite;
        }
        public void SetMySkillIcon(Sprite sprite)
        {
            img_mySkillIcon.sprite = sprite;
        }
        #endregion
    }
}