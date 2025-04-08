using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lop.Game
{
    // 조이스틱 진동을 시험해보려다 끝내 구현해내지 못함
    public class VibratorManager : MonoBehaviour
    {
        private IEnumerator co_vivrate;

        public void PlayVibrate(int playerNumber)
        {
            if (Gamepad.all.Count <= playerNumber) return;

            Gamepad.all[playerNumber].SetMotorSpeeds(0.5f, 0.6f);
            if (co_vivrate != null)
            {
                StopCoroutine(co_vivrate);
            }
            co_vivrate = Co_StopVibrator(playerNumber);
            StartCoroutine(co_vivrate);
        }

        private IEnumerator Co_StopVibrator(int playerNumber)
        {
            Debug.Log("Stop");
            yield return new WaitForSeconds(1f);
            Gamepad.all[playerNumber].SetMotorSpeeds(0.0f, 0.0f);
        }
    }
}