using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lop.Game
{
    public class MultiAttackCoolManager : MonoBehaviour
    {
        [SerializeField] private Image img_coolValue;
        private float curCool;
        private float maxCool = 100000;

        private bool onAttack;
        public bool OnAttack => onAttack;

        public void AddAttackCool(float addValue)
        {
            curCool += addValue;

            float cool = curCool / maxCool;
            img_coolValue.fillAmount = cool;

            if (curCool >= maxCool)
            {
                onAttack = true;
            }
        }

        public void ResetCool()
        {
            curCool = 0;
            img_coolValue.fillAmount = 0;
            onAttack = false;
        }
    }
}