using UnityEngine;

namespace Lop.Game
{
    public class JumpState : IState
    {
        private PenguinBase player;
        private bool isQuickLand;

        void IState.OnEnter(PenguinBase player)
        {
            this.player = player;
            isQuickLand = false;
            player.Jump();
        }

        void IState.OnExit()
        {
            
        }

        void IState.Update()
        {
            if(player.MyPenguinName == PenguinName.Default)
            {
                if (player.IsInputJump)
                {
                    player.SetState(new JumpState());
                }
            }

            if(player.MyPenguinName == PenguinName.Magellanic)
            {
                if ((player.IsInputJump && !isQuickLand && !player.IsJump) || (player.IsInputSlide && !isQuickLand))
                {
                    isQuickLand = true;
                    player.GetComponent<PenguinMagellanic>().QuickLanding();
                    player.SetState(new RunState());
                }
            }

            if (player.IsGround == true)
            {
                if (player.IsInputSlide)
                    player.SetState(new SlideState());
                else 
                    player.SetState(new RunState());
            }
        }
    }
}