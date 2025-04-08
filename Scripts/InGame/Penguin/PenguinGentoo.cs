using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class PenguinGentoo : PenguinBase
    {
        // 보호막을 얻는 스킬
        private float getShieldCycleTime = 30;
        private float shieldRangePercent = 5;

        protected override void Start()
        {
            myPenguinName = PenguinName.Gentoo;
            StartCoroutine(Co_GetShieldCycle());

            base.Start();
        }

        private IEnumerator Co_GetShieldCycle()
        {
            yield return new WaitForSeconds(getShieldCycleTime);

            gameManager.GetShield(shieldRangePercent, PlayerNumber);

            if(!IsDie) StartCoroutine(Co_GetShieldCycle());
        }
    }
}
