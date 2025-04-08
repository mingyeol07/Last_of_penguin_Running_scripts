using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class PenguinEmperor : PenguinBase
    {
        private bool isRevive;
        public bool IsRevive => isRevive;

        protected override void Start()
        {
            myPenguinName = PenguinName.Emperor;

            base.Start();
        }

        public IEnumerator Co_Revive()
        {
            isRevive = true;
            yield return new WaitForSeconds(1);
            gameManager.EmperorRevive(PlayerNumber);
            isDie = false;
            
            if (constantForce2D.force == Vector2.zero)
            {
                constantForce2D.force = new Vector2(0, -gravitationalAcceleration);
                rigid.velocity = Vector2.up * 20;
            }
            StartCoroutine(Co_InvincibilityFade());
        }
    }
}