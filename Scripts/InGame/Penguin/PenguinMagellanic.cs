using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class PenguinMagellanic : PenguinBase
    {
        // 추가되는 스피드% 크기
        private float addPercentMoveSpeed = 5;

        // 착지 속도
        private float landingSpeed = 15;

        protected override void Start()
        {
            myPenguinName = PenguinName.Magellanic;
            moveSpeed += ((addPercentMoveSpeed / 100) * moveSpeed);

            base.Start();
        }

        public void QuickLanding()
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.down * landingSpeed, ForceMode2D.Impulse);
        }
    }
}