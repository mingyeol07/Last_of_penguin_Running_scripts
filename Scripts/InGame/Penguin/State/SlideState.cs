using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class SlideState : IState
    {
        private PenguinBase player;

        void IState.OnEnter(PenguinBase player)
        {
            this.player = player;
            SoundManager.Instance.Play_PlayerSlideSound();
            player.SwitchColliderToSlide();
        }

        void IState.OnExit()
        {
            player.SwitchColliderToStand();
        }

        void IState.Update()
        {
            if (player.IsInputJump == true && player.IsGround)
            {
                player.JumpBuffer.Clear();
                player.SetState(new JumpState());
            }
            if (player.IsInputSlide == false)
            {
                player.SetState(new RunState());
            }
        }
    }
}
