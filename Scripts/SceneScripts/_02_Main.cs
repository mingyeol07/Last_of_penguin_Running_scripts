using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;

namespace Lop.Game {
    public class _02_Main : MonoBehaviour {
        [SerializeField] private Animator canvasAnimator;

        [Header("Buttons")]
        [SerializeField] private Button btn_closet;
        //[SerializeField] private Button btn_store;
        //[SerializeField] private Button btn_lab;
        [SerializeField] private Button btn_play;
        [SerializeField] private Button btn_play_back;
        //[SerializeField] private Button btn_purchase_gem;
        [SerializeField] private Button btn_editor;
        [SerializeField] private Button btn_exit;

        [Header("PlayButtons")]
        [SerializeField] private Button btn_soloPlay;
        [SerializeField] private Button btn_localMultiTwoPeople;
        [SerializeField] private Button btn_localMultiFourPeople;
        //[SerializeField] private Button btn_networkMultiPlay;

        //[Header("Panel")]
        //[SerializeField] private GameObject pnl_purchase_gem;

        [Header("Option")]
        [SerializeField] private GameObject pnl_option;
        [SerializeField] private Toggle tog_option;
        [SerializeField] private Button btn_basicKey;
        [SerializeField] private Button btn_consoleKey_ps;
        [SerializeField] private Button btn_consoleKey_xbox;

        [Header("Setting")]
        [SerializeField] private Slider slider_sfx;
        [SerializeField] private Slider slider_bgm;
        [SerializeField] private AudioMixer audioMixer;

        private float minSliderValue = -40f;
        private float maxSliderValue = 0;
        private float defaultSliderValue = -20f;

        private const string bgmName = "BGM";
        private const string sfxName = "SFX";

        private void Awake() {
            OnClickButtons();
        }

        private void Start() {
            if (SoundManager.Instance.IsPlayMainBGM() == false) {
                SoundManager.Instance.Play_MainBGM();
            }

            audioMixer.GetFloat(bgmName, out float bgm);
            slider_bgm.value = bgm;

            audioMixer.GetFloat(sfxName, out float sfx);
            slider_sfx.value = sfx;
        }

        private void OnClickButtons() {
            btn_closet.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneNameString._09_Closet);
                SoundManager.Instance.Play_ButtonClick();
            });
            //btn_store.onClick.AddListener(() =>
            //{
            //    SceneManager.LoadScene(SceneNameString._08_Store);
            //    SoundManager.Instance.Play_ButtonClick();
            //});
            //btn_lab.onClick.AddListener(() =>
            //{
            //    SceneManager.LoadScene(SceneNameString._07_Fusion);
            //    SoundManager.Instance.Play_ButtonClick();
            //});
            btn_soloPlay.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneNameString._03_Mode);
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_localMultiTwoPeople.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneNameString._09_MultiSelectPenguins2);
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_localMultiFourPeople.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneNameString._09_MultiSelectPenguins4);
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_editor.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneNameString._99_SelectMapScene);
                SoundManager.Instance.Play_ButtonClick();
            });

            btn_play.onClick.AddListener(() =>
            {
                OnClickPlayButton();
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_play_back.onClick.AddListener(() =>
            {
                OnClickPlayBackButton();
                SoundManager.Instance.Play_ButtonClick();
            });

            //btn_purchase_gem.onClick.AddListener(() =>
            //{
            //    TogglePurchasePanel();
            //    SoundManager.Instance.Play_ButtonClick();
            //});

            btn_exit.onClick.AddListener(() =>
            {
                OnClickExit();
                SoundManager.Instance.Play_ButtonClick();
            });

            tog_option.onValueChanged.AddListener((bool on) =>
            {
                pnl_option.SetActive(on);
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_basicKey.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString(PlayerPrefsKey.My_KeyTypeID, KeyType.Keyboard);

                btn_basicKey.transform.GetChild(0).gameObject.SetActive(true);
                btn_consoleKey_ps.transform.GetChild(0).gameObject.SetActive(false);
                btn_consoleKey_xbox.transform.GetChild(0).gameObject.SetActive(false);
            });

            btn_consoleKey_ps.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString(PlayerPrefsKey.My_KeyTypeID, KeyType.PS);

                btn_basicKey.transform.GetChild(0).gameObject.SetActive(false);
                btn_consoleKey_ps.transform.GetChild(0).gameObject.SetActive(true);
                btn_consoleKey_xbox.transform.GetChild(0).gameObject.SetActive(false);
            });

            btn_consoleKey_xbox.onClick.AddListener(() =>
            {
                PlayerPrefs.SetString(PlayerPrefsKey.My_KeyTypeID, KeyType.Xbox);

                btn_basicKey.transform.GetChild(0).gameObject.SetActive(false);
                btn_consoleKey_ps.transform.GetChild(0).gameObject.SetActive(false);
                btn_consoleKey_xbox.transform.GetChild(0).gameObject.SetActive(true);
            });

            if (PlayerPrefs.GetString(PlayerPrefsKey.My_KeyTypeID) == KeyType.Keyboard) {
                btn_basicKey.transform.GetChild(0).gameObject.SetActive(true);
                btn_consoleKey_ps.transform.GetChild(0).gameObject.SetActive(false);
                btn_consoleKey_xbox.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (PlayerPrefs.GetString(PlayerPrefsKey.My_KeyTypeID) == KeyType.PS) {
                btn_basicKey.transform.GetChild(0).gameObject.SetActive(false);
                btn_consoleKey_ps.transform.GetChild(0).gameObject.SetActive(true);
                btn_consoleKey_xbox.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (PlayerPrefs.GetString(PlayerPrefsKey.My_KeyTypeID) == KeyType.Xbox) {
                btn_basicKey.transform.GetChild(0).gameObject.SetActive(false);
                btn_consoleKey_ps.transform.GetChild(0).gameObject.SetActive(false);
                btn_consoleKey_xbox.transform.GetChild(0).gameObject.SetActive(true);
            }

            // -40~0이 안정적!

            slider_sfx.minValue = minSliderValue;
            slider_sfx.maxValue = maxSliderValue;
            slider_sfx.value = defaultSliderValue;
            slider_sfx.onValueChanged.AddListener(OnSFXValueChanged);

            slider_bgm.minValue = minSliderValue;
            slider_bgm.maxValue = maxSliderValue;
            slider_bgm.value = defaultSliderValue;
            slider_bgm.onValueChanged.AddListener(OnBGMValueChanged);

            //for (int i =0; i < Gamepad.all.Count; i++) {
            //    if (Gamepad.all[i] == Gamepad.current)
            //    {
            //        Gamepad.all[i].SetMotorSpeeds(0.5f, 0.5f);
            //    }
            //}
        }

        private void OnSFXValueChanged(float volume) {
            if (volume == -40) audioMixer.SetFloat(sfxName, -80);
            else audioMixer.SetFloat(sfxName, volume);
        }

        private void OnBGMValueChanged(float volume) {
            if (volume == -40) audioMixer.SetFloat(bgmName, -80);
            else audioMixer.SetFloat(bgmName, volume);
        }

        private void TogglePurchasePanel() {
            //pnl_purchase_gem.SetActive(!pnl_purchase_gem.activeSelf);
        }
        private void OnClickPlayButton() {
            canvasAnimator.SetBool("IsPlay", true);
        }
        private void OnClickPlayBackButton() {
            canvasAnimator.SetBool("IsPlay", false);
        }
        private void OnClickExit() {
            Application.Quit();
        }
    }
}