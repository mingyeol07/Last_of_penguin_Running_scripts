using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game {
    public class FeverStar : Item
    {
        [SerializeField] private GameObject eff_getStar;

        protected override void ContactPlayer()
        {
            gameManager.GetFeverStar(contactPenguinNumber);

            Destroy(Instantiate(eff_getStar, transform.position, Quaternion.identity), 3f);
        }
    }
}