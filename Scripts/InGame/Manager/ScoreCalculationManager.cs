using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class ScoreCalculationManager
    {
        private bool isSquid;

        #region SCORE
        private const int REDFISHSCORE = -6666;
        private const int BLUEFISHSCORE = 1000;
        private const int SHELLFISHSCORE = 10000;
        private const int SHRIMPSCORE = 3000;
        private const int SQUIDSCORE = 555;

        private float comboMultipleValue = 1;
        private const float comboValue = 1.5f;
        #endregion

        public float ComboMultipleValue => comboMultipleValue;
        public float ComboValue => comboValue;

        /// <summary>
        /// 내가 먹은 스코어의 타입을 받고 점수를 계산하여 반환
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int CalculationScore(ScoreType type)
        {
            int scoreRange = 0;

            if (type != ScoreType.Squid)
            {
                comboMultipleValue = 1;
                isSquid = false;
            }

            switch (type)
            {
                case ScoreType.None:
                    break;
                case ScoreType.Fish_red:
                    scoreRange = REDFISHSCORE;
                    break;
                case ScoreType.Fish_blue:
                    scoreRange = BLUEFISHSCORE;
                    break;
                case ScoreType.Shellfish:
                    scoreRange = SHELLFISHSCORE;
                    break;
                case ScoreType.Shrimp:
                    scoreRange = SHRIMPSCORE;
                    break;
                case ScoreType.Squid:
                    scoreRange = SQUIDSCORE;

                    //if(isSquid)
                    //{
                    //    comboMultipleValue += 1f;
                    //    scoreRange = (int)(SQUIDSCORE * comboMultipleValue * comboValue);
                    //}
                    
                    isSquid = true;
                    break;
            }

            return scoreRange;
        }
    }
}
