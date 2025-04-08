using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lop.Game
{
    public class LoadingSceneController : MonoBehaviour
    {
        static string nextScene;

        [Header("LoadImage")]
        [SerializeField] private Image img_loadImage;
        [SerializeField] private Sprite[] sprite_howToPlay;

        [Header("LoadingBar")]
        [SerializeField] private Slider slider_progressBar;
        [SerializeField] private TMP_Text txt_tip;

        #region tip!s
        private string[] tips = 
            {
                "노는게 제일 좋아",
                "아델리 펭귄은 정말 맑은 눈을 가졌죠! 너무 뚫어져라 쳐다보진 말아주세요.",
                "킹 펭귄은 먹는 걸 무지 좋아합니다. 킹 펭귄이 생선을 뺏어먹을 지도 몰라요!",
                "젠투 펭귄은 남을 돕는걸 좋아해요. 하지만 항상 뜻대로 흘러가진 않죠.",
                "마젤란 펭귄과 달리기 내기를 하려면 우사인 펭귄정도 되야 할걸요?",
                "황제 펭귄은 미술적 감각이 뛰어납니다. 오징어 먹물로 그림그리기를 좋아해요!",
                "바위뛰기 펭귄의 용맹함의 근거는 멋진 눈썹에서 오죠!",
                "무엇을 하든 기본기를 확실히 다져야 합니다. 기본적이지만 최강이죠!",
                "아델리 펭귄의 뿅망치를 조심하세요! 순진한 얼굴에 속으면 큰 코 다칩니다.",
                "바닥을 조심하세요. 상어가 호시탐탐 펭귄을 덥칠 준비를 마쳤습니다.",
                "요즘 펭귄들의 핫 아이템은 조개입니다.",
                "펭귄 마을에 인기 아이스크림 가게를 아시나요?",
                "펭귄들이 서로 공격할 때 쓰는 무기는 장난감입니다.",
        };
        #endregion

        [Header("Load Duration")]
        [SerializeField] private float minLoadDuration = 0.5f; // 최소 로딩 시간 설정

        public static void LoadScene(string sceneName)
        {
            nextScene = sceneName;
            SceneManager.LoadScene("99.Loading");
        }

        void Start()
        {
            Fade.In(Fade.FADE_TIME_DEFAULT);
            SoundManager.Instance.Stop_BGM();

            int randIndex = 0;

            // 로딩이미지 랜덤
            randIndex = Random.Range(0, sprite_howToPlay.Length);
            img_loadImage.sprite = sprite_howToPlay[randIndex];

            // 팁 랜덤
            randIndex = Random.Range(0, tips.Length);
            txt_tip.text = "Tip. " + tips[randIndex];

            StartCoroutine(LoadSceneProgress());
        }

        private IEnumerator LoadSceneProgress()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
            op.allowSceneActivation = false;

            float timer = 0f;
            while (!op.isDone)
            {
                yield return null;

                if (op.progress < 0.9f)
                {
                    slider_progressBar.value = op.progress;
                }
                else
                {
                    timer += Time.unscaledDeltaTime / 2;

                    // 최소 로딩 시간 동안 천천히 증가시키기
                    float progress = Mathf.Lerp(0.9f, 1f, timer / minLoadDuration);
                    slider_progressBar.value = Mathf.Clamp01(progress);

                    if (timer >= minLoadDuration && slider_progressBar.value >= 1f)
                    {
                        op.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }
    }
}