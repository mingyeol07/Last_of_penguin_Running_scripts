using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class PenguinDefault : PenguinBase
    {
        // 크기가 커지는 펭귄 스킬
        private Vector3 defaultScale = new Vector3(0.75f, 0.75f, 0.75f);
        private Vector3 feverScale = new Vector3(2.25f, 2.25f, 2.25f);

        protected override void Start()
        {
            myPenguinName = PenguinName.Default;
            maxJumpCount = 2;

            base.Start();
        }

        public override void StartFever()
        {
            transform.localScale = feverScale;

            base.StartFever();
        }

        public override void StopFever()
        {
            transform.localScale = defaultScale;

            base.StopFever();
        }
    }
}