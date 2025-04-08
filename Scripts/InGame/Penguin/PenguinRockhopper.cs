using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Lop.Game
{
    public class PenguinRockhopper : PenguinBase
    {
        // 시작할 때 부스트를 쓰는 스킬
        [SerializeField] private float boostTime = 3;
        private float addFeverTime = 2;

        protected override void Start()
        {
            myPenguinName = PenguinName.Rockhopper;
            StartRush();
            base.Start();

            gameManager.RockhopperAddFeverTime(addFeverTime, PlayerNumber);
        }

        private void StartRush()
        {
            StartCoroutine(Co_BoostTime());
        }

        private IEnumerator Co_BoostTime()
        {
            StartFever();
            yield return new WaitForSeconds(boostTime);
            StopFever();
        }
    }
}