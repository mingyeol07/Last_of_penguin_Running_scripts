using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    // 화면에 보여지는 스테이지
    public class ScreenStage : MonoBehaviour
    {
        [SerializeField] private _04_StoryMode storyModeManager;
        [SerializeField] private GameObject img_stars_Parent;
        [SerializeField] private GameObject go_lock;
        [SerializeField] private int stageLevel;
        private bool isStageUnLock;

        public static int lastStageStarNumber = 88;


        public void Initialize(bool isUnLock, int starNumber=99)
        {
            if (!isUnLock) return;

            isStageUnLock = true;
            go_lock.SetActive(false);

            // 해금된 마지막 스테이지라면 별을 보여주지 않는다.
            if (starNumber == lastStageStarNumber) return;

            img_stars_Parent.SetActive(true);

            // 얻은 별이 없을 경우(할당되지 않았을 경우) 스프라이트를 변경하지 않고 돌아간다.
            if (starNumber == 99) return;

            img_stars_Parent.transform.GetChild(starNumber).GetComponent<SpriteRenderer>().sprite = storyModeManager.Sprite_Star;
        }

        private void OnMouseDown()
        {
            OnClick();
        }

        public void OnClick() {
            if (isStageUnLock) {
                PlayerPrefs.SetInt(PlayerPrefsKey.My_PlayStageID, stageLevel);
                storyModeManager.StartStage();
            }
        }
    }
}