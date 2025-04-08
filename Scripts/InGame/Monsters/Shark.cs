using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    // 샤크
    public class Shark : Monster
    {
        public override void Attack()
        {
            base.Attack();
            SoundManager.Instance.Play_SharkBite();
        }
    }
}