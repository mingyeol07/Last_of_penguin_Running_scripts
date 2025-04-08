using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    // 건들이면 떨어지는 타일
    public class BrokenIceTile : MonoBehaviour
    {
        private Rigidbody2D rigid;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();

            rigid.gravityScale = 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                rigid.gravityScale = 2;
            }
        }
    }
}
