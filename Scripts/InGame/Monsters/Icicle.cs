using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    // 고드름 떨어지는 기믹 (구현 X)
    public class Icicle : MonoBehaviour
    {
        private Rigidbody2D rigid;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();

            rigid.gravityScale = 0;
        }

        private void Update()
        {
            if(Physics2D.OverlapBox(transform.position, new Vector2(1, 12), 0, LayerMask.GetMask("PenguinSurfaceBody")))
            {
                //rigid.gravityScale = 2;
            }
        }
    }
}