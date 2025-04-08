using Lop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class StageStar : Item
    {
        [SerializeField] private GameObject bezierStar;
        private int number;

        public void SetStarNumber(int number)
        {
            this.number = number;

            if(number > 2)
            {

            }
        }

        protected override void ContactPlayer()
        {
            SoundManager.Instance.Play_GetStar();
            //GameManagerParent.Instance.GetStageStar(number);
        }
    }
}