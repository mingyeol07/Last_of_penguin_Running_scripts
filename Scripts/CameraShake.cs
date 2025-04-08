using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game 
{
    public class CameraShake : MonoBehaviour
    {
        private float shakeSize = 0.1f;
        private Vector3 initialPosition;

        private void Start()
        {
            initialPosition = transform.localPosition;

            //StartCoroutine(Shake(shakeAmount, shakeTime));
        }

        public void StartShake(float setShakeTime = 1f)
        {
            StartCoroutine(ShakeStart(setShakeTime));
        }

        private IEnumerator ShakeStart(float duration)
        {
            float timer = 0;

            while (timer <= duration)
            {
                // y 축 값만 흔들리도록 Random.Range를 사용
                float randomY = Random.Range(initialPosition.y - shakeSize, initialPosition.y + shakeSize);

                // 기존의 x, z 값은 유지하고 y 값만 변경
                transform.localPosition = new Vector3(0, randomY, -10);

                timer += Time.deltaTime;

                if (Time.timeScale == 0) break;
                yield return null;
            }

            transform.localPosition = initialPosition;
        }
    }
}
