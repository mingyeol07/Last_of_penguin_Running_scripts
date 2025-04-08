using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game {
    public class PenguinAdelie : PenguinBase
    {
        // 경감되는 데미지 퍼센트 밸류
        private float damagePercentValue = 8.5f;
        public float DamagePercentValue => damagePercentValue;
        
        // 경감되는 낭떠러지 데미지 퍼센트
        private float cliffPercentValue = 12f;
        public float CliffPercentValue => cliffPercentValue;

        protected override void Start()
        {
            myPenguinName = PenguinName.Adlie;

            base.Start();
        }
    }
}