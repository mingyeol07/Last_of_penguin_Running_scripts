using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class RunState : IState
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
            if (player.IsInputJump == true)
            {
                player.JumpBuffer.Clear();
                player.SetState(new JumpState());
            }

            if (player.IsInputSlide == true && player.IsGround)
            {
                player.SetState(new SlideState());
            }
        }
    }
}