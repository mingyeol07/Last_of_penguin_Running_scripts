using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class ScoreItem : Item
    {
        #region type
        [SerializeField] private ScoreType type;
        private const string blueFishTag = "BlueFish";
        private const string redFishTag = "RedFish";
        private const string shellFishTag = "Shellfish";
        private const string squidFishTag = "Squid";
        private const string shrimpFishTag = "Shrimp";

        private void Awake()
        {
            // 인스펙터에서 enum값을 설정하지 못하는 버그때문에 태그를 사용해서 enum값 수정
            if(gameObject.CompareTag(blueFishTag))
            {
                type = ScoreType.Fish_blue;
            }
            else if (gameObject.CompareTag(redFishTag))
            {
                type = ScoreType.Fish_red;
            }
            else if (gameObject.CompareTag(shellFishTag))
            {
                type = ScoreType.Shellfish;
            }
            else if (gameObject.CompareTag(squidFishTag))
            {
                type = ScoreType.Squid;
            }
            else if (gameObject.CompareTag(shrimpFishTag))
            {
                type = ScoreType.Shrimp;
            }
        }
        #endregion

        protected override void ContactPlayer()
        {
            gameManager.GetScore(type, 0, contactPenguinNumber);

            SoundManager.Instance.Play_EatFish();
        }
    }
}