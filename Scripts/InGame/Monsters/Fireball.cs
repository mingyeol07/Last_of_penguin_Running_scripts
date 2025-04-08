using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    // 앞으로 굴러가는 파이어볼
    public class Fireball : Monster
    {
        private void Update()
        {
            if(isTrigger)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * 3);
            }
        }
    }
}