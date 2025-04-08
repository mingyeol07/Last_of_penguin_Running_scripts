using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public interface IState
    {
        void OnEnter(PenguinBase player);

        void Update();

        void OnExit();
    }
}