using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    // 바다표범
    public class Walrus : Monster
    {
        public override void Attack()
        {
            base.Attack();
            SoundManager.Instance.Play_WalrusBite();
        }
    }
}