using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class FollowCam : MonoBehaviour
    {
        private GameObject player;
        [SerializeField] private float camY = 3.25f;
        [SerializeField] private float camX = 4;

        public bool CanMove = true;

        public void SetFollowTarget(GameObject target)
        {
            player = target;
        }

        private void LateUpdate()
        {
            if (player != null && CanMove)
                transform.localPosition = new Vector3(player.transform.position.x + camX, camY, -10);
        }

        public IEnumerator Co_fallCameraMove()
        {
            yield return new WaitForSeconds(0.8f);

            Vector3 PlayerPos = player.transform.position;
            Debug.Log(PlayerPos);
            float Duration = 1.4f;
            float Elapsed = 0;

            while (Elapsed < Duration)
            {
                Elapsed += Time.deltaTime;
                float t = Elapsed / Duration;

                transform.localPosition =  Vector3.Lerp(transform.localPosition, new Vector3(player.transform.position.x + camX,camY),t);

                yield return null; // 매 프레임마다 업데이트
            }
            CanMove = true;
        }
    }
}