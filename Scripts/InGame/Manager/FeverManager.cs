using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace Lop.Game
{
    public class FeverManager : MonoBehaviour
    {
        [SerializeField] private Slider slider_Fever;
        [SerializeField] private float maxGauge = 6;
        private float curFeverGauge;
        private PenguinBase player;

        // rockhopper의 능력_추가피버시간
        private float additionalFeverTime;

        public void SetPlayer(PenguinBase penguin)
        {
            player = penguin;
        }

        public void AddFeverGauge(int value = 1)
        {
            curFeverGauge += value;
            if (curFeverGauge > maxGauge) curFeverGauge = maxGauge;
            slider_Fever.value = curFeverGauge / maxGauge;

            if (curFeverGauge >= maxGauge && !player.IsFever)
            {
                SoundManager.Instance.Play_FeverBGM();
                player.StartFever();
                StartCoroutine(Co_FeverTimer());
            }
        }

        // 피버시간동안 피버 게이지 색이 랜덤으로 변함
        private IEnumerator Co_FeverTimer()
        {
            float totalDuration = maxGauge + additionalFeverTime;

            Image fill = slider_Fever.fillRect.GetComponent<Image>();
            float time = 0;
            float colorChangeTime = 0.5f;
            float randomH = Random.Range(0, 1f);
            // 색상(H), 채도(G), 명도(B)
            Color nextColor = Color.HSVToRGB(randomH, 1f, 1f);
            Color prevColor = fill.color;
            float t;

            while (true)
            {
                if (curFeverGauge > 0)
                {
                    // Fever gauge 감소
                    curFeverGauge -= Time.deltaTime * (maxGauge / totalDuration);
                    slider_Fever.value = curFeverGauge / maxGauge;
                }
                else
                {
                    break;
                }

                t = Mathf.Clamp01(time / colorChangeTime);
                fill.color = Color.Lerp(prevColor, nextColor, t);

                time += Time.deltaTime;

                if (time >= colorChangeTime)
                {
                    prevColor = nextColor;
                    randomH = Random.Range(0, 1f);
                    nextColor = Color.HSVToRGB(randomH, 1f, 1f);
                    time = 0;
                }

                yield return null;
            }

            fill.color = Color.white;
            curFeverGauge = 0;
            SoundManager.Instance.Play_SuperRunBGM();
            player.StopFever();
        }

        public void SetAdditionalFeverTime(float value)
        {
            additionalFeverTime = value;
        }
    }
}