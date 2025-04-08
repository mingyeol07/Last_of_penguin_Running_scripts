using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Lop.Game
{
    public class MultiSaver : MonoBehaviour
    {
        public static MultiSaver Instance;

        public static int player1Index = 0;
        public static int player2Index = 1;

        public string[] NameArray = new string[4];
        public int[] PenguinIDArray = new int[4];
        public int[] ScoreArray = new int[4];
        public PlayerInput[] playerInputs = new PlayerInput[4];

        public int[] RandomStageID2 ;

        //public string keyType_p1 = KeyType.Keyboard_1;
        //public string keyType_p2 = KeyType.Keyboard_2;

        private const int stageLength = 2;
        private const int randomStageMin = 2;
        private const int randomStageMax = 21;

        private void Awake()
        {
            if (null == Instance)
            {
                Instance = this;

                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }

            RandomStageIDSpawn();

            DontDestroyOnLoad(gameObject);
        }

        private void RandomStageIDSpawn() {
            if (RandomStageID2.Length < stageLength) {
                RandomStageID2 = new int[stageLength];
                for (int i = 0; i < RandomStageID2.Length; i++) {
                    RandomStageID2[i] = Random.Range(randomStageMin, randomStageMax);
                }
            }
        }

        private void Start() {
            SceneManager.sceneLoaded += LoadedsceneEvent;
        }

        private void LoadedsceneEvent(Scene scene, LoadSceneMode mode) {
            if (scene.name == SceneNameString._06_LocalMulti2 || scene.name == SceneNameString._06_LocalMulti4 || scene.name == SceneNameString._99_Loading
                || scene.name == SceneNameString._10_MultiLeaderBoard2 || scene.name == SceneNameString._10_MultiLeaderBoard4
                || scene.name == SceneNameString._09_MultiSelectPenguins2 || scene.name == SceneNameString._09_MultiSelectPenguins4) {
                return;
            }
            else {
                SceneManager.sceneLoaded -= LoadedsceneEvent;
                Destroy(gameObject);
            }
        }
    }
}