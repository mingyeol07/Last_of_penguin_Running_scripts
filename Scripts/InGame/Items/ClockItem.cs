using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game {
    public class ClockItem : Item
    {
        protected override void ContactPlayer()
        {
            gameManager.GetWatch(999, contactPenguinNumber);

            SoundManager.Instance.Play_GetTimer();
        }
    }
}
