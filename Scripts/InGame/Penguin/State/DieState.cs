using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class DieState : IState
    {
        private PenguinBase player;

        void IState.OnEnter(PenguinBase player)
        {
            this.player = player;
        }

        void IState.OnExit()
        {

        }

        void IState.Update()
        {
            if(player.IsDie == false)
            {
                player.SetState(new RunState());
            }
        }
    }
}