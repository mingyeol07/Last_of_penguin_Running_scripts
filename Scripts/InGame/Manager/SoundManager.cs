using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class SoundManager : MonoBehaviour
    {
        //싱글톤
        private static SoundManager instance;
        public static SoundManager Instance { get { return instance; } }

        [SerializeField] private AudioSource myAudio; //효과음용
        [SerializeField] private AudioSource bgmAudio; //배경 음악용

        [Space(10)]
        [SerializeField] private AudioClip clip_playerHit;
        [SerializeField] private AudioClip clip_playerJump;
        [SerializeField] private AudioClip clip_playerSlide;

        [Space(10)]
        [SerializeField] private AudioClip clip_buttonClick;
        [SerializeField] private AudioClip clip_buttonStop;
        [SerializeField] private AudioClip clip_countDown;

        [Space(10)]
        [SerializeField] private AudioClip bgm_mainTheme;
        [SerializeField] private AudioClip bgm_superRunning;
        [SerializeField] private AudioClip bgm_playerFever;

        [Space(10)]
        [SerializeField] private AudioClip clip_walrusBark;
        [SerializeField] private AudioClip clip_sharkBark;

        [Space(10)]
        [SerializeField] private AudioClip clip_flyAway;

        [Space(10)]
        [SerializeField] private AudioClip clip_win;
        [SerializeField] private AudioClip bgm_fireworks;

        [Space(10)]
        [SerializeField] private AudioClip clip_getStar;
        [SerializeField] private AudioClip clip_getTimer;
        [SerializeField] private AudioClip clip_eatFish;

        [Space(10)]
        [SerializeField] private AudioClip clip_battleSkill;
        [SerializeField] private AudioClip clip_emperorSkill;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        void Start()
        {
            bgmAudio.loop = true;
        }

        #region BGM
        public bool IsPlayMainBGM()
        {
            return bgmAudio.clip == bgm_mainTheme;
        }
        public void Stop_BGM()
        {
            bgmAudio.Stop();
        }
        public void Play_BGM()
        {
            bgmAudio.Play();
        }
        public void Play_MainBGM()
        {
            bgmAudio.clip = bgm_mainTheme;
            bgmAudio.Play();
        }
        public void Play_SuperRunBGM()
        {
            bgmAudio.clip = bgm_superRunning;
            bgmAudio.Play();
        }
        public void Play_Fireworks()
        {
            bgmAudio.clip = bgm_fireworks;
            bgmAudio.Play();
        }
        public void Play_FeverBGM()
        {
            bgmAudio.clip = bgm_playerFever;
            bgmAudio.Play();
        }
        #endregion

        #region SFX
        public void Play_WalrusBite()
        {
            myAudio.PlayOneShot(clip_walrusBark);
        }
        public void Play_SharkBite()
        {
            myAudio.PlayOneShot(clip_sharkBark);
        }
        public void Play_Flyaway()
        {
            myAudio.PlayOneShot(clip_flyAway);
        }
        public void Play_Win()
        {
            myAudio.PlayOneShot(clip_win);
        }
        public void Play_GetStar()
        {
            myAudio.PlayOneShot(clip_getStar);
        }
        public void Play_GetTimer()
        {
            myAudio.PlayOneShot(clip_getTimer);
        }
        public void Play_EatFish()
        {
            myAudio.PlayOneShot(clip_eatFish);
        }
        public void Play_CountDown()
        {
            myAudio.PlayOneShot(clip_countDown);
        }
        #endregion

        #region PlayerSound
        public void Play_PlayerHitSound()
        {
            myAudio.PlayOneShot(clip_playerHit);
        }
        public void Play_PlayerJumpSound()
        {
            myAudio.PlayOneShot(clip_playerJump);
        }
        public void Play_PlayerSlideSound()
        {
            myAudio.PlayOneShot(clip_playerSlide);
        }
        #endregion

        #region Battle
        public void Play_BattleSkill()
        {
            myAudio.PlayOneShot(clip_battleSkill);
        }
        public void Play_EmperorSkill()
        {
            myAudio.PlayOneShot(clip_emperorSkill);
        }
        #endregion

        #region SystemSound
        public void Play_ButtonClick()
        {
            if (clip_buttonClick == null) return;
            myAudio.PlayOneShot(clip_buttonClick);
        }
        public void Play_ButtonStop()
        {
            if (clip_buttonStop == null) return;
            myAudio.PlayOneShot(clip_buttonStop);
        }
        #endregion
    }
}