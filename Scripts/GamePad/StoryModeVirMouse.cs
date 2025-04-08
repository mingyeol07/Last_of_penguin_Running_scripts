using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Lop.Game {
    public class StoryModeVirMouse : MonoBehaviour {
        [SerializeField] private Button homeButton;
        [SerializeField] private _04_StoryMode storyModeScene;

        private Transform penguinTransform;

        [SerializeField] private ScreenStage[] screenStages;
        private ScreenStage curStage;

        private int stageIndex = 0;
        private int stageIndexMax;

        void Start() {
            stageIndexMax = screenStages.Length - 1;
            penguinTransform = storyModeScene.PenguinParent;
        }

        public void OnBack(InputValue value) {
            homeButton.onClick.Invoke();
        }

        public void OnClick(InputValue value) {
            curStage.OnClick();
        }

        public void OnMovePenguin(InputValue value) {
            Vector2 input = value.Get<Vector2>();

            MoveMouse(input.x > 0);
        }

        public void MoveMouse(bool isRight) {

            if (isRight) {
                if (++stageIndex > stageIndexMax) {
                    stageIndex = 0;
                }
            }
            else {
                if (--stageIndex < 0) {
                    stageIndex = stageIndexMax;
                }
            }

            curStage = screenStages[stageIndex];
            IsUIOutsideViewportAndChangeCam(curStage.transform, Camera.main);

            penguinTransform.position = curStage.transform.position;
        }

        private void IsUIOutsideViewportAndChangeCam(Transform element, Camera uiCamera) {

            Vector3 viewportPos = uiCamera.WorldToViewportPoint(element.position);
            if(viewportPos.x < 0 ) {
                storyModeScene.MoveCamLeft();
            }
            else if(viewportPos.x > 1) {
                storyModeScene.MoveCamRight();
            }
        }
    }
}

