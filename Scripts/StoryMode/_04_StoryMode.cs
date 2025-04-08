using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lop.Game
{
    public class _04_StoryMode : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;

        [SerializeField] private ScreenStage[] stageArray = new ScreenStage[21];
        private StoryProgressData storyProgress;

        [SerializeField] private Transform penguinParent;
        public Transform PenguinParent => penguinParent;
        [SerializeField] private GameObject[] penguins;

        [SerializeField] private Sprite sprite_star;
        public Sprite Sprite_Star { get { return sprite_star; } }

        [SerializeField] private Button btn_home;

        private float minCamPosX = -8;
        private float maxCamPosX = 7.9f;

        private void Awake()
        {
            string path = Path.Combine(Application.persistentDataPath, "StoryProgressData.json");

            if (File.Exists(path))
            {
                string loadJson = File.ReadAllText(path);
                storyProgress = JsonUtility.FromJson<StoryProgressData>(loadJson);
            }
            else
            {
                StoryProgressData storyProgressData = new StoryProgressData();
                storyProgressData.LastStage = 0;
                storyProgressData.StageStarArray = new List<StageStarData>();
                for(int i =0; i < 21; i++)
                {
                    StageStarData stageStarData = new StageStarData();
                    stageStarData.IsGetStars = new bool[3] { false, false, false };
                    storyProgressData.StageStarArray.Add(stageStarData);
                }

                string saveJson = JsonUtility.ToJson(storyProgressData, true);
                File.WriteAllText(path, saveJson);

                storyProgress = storyProgressData;
            }

            //#region 테스트
            //storyProgress = new StoryProgressData();
            //storyProgress.CurrentStage = 4;
            //#endregion

            GameObject myPenguin = penguins[PlayerPrefs.GetInt(PlayerPrefsKey.My_PenguinID)];

            myPenguin.transform.position = new Vector3(0, -0.1f, -5);
            myPenguin.transform.rotation = Quaternion.Euler(12, 170, 0);
            myPenguin.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

            Instantiate(myPenguin, penguinParent);

            btn_home.onClick.AddListener(() => 
            { 
                SceneManager.LoadScene(SceneNameString._02_Main);
                SoundManager.Instance.Play_ButtonClick();
            });

            float fixedAspectRatio = 16f/9f;
            float currentAspectRactio = (float) Screen.width / (float)Screen.height;

            // 가로비율이 더 길 경우
            if(currentAspectRactio > fixedAspectRatio)
            {
                minCamPosX = -4f;
                maxCamPosX = 3.9f;
            }

            mainCam.transform.position = new Vector3(minCamPosX, 0, -10);
        }
        
        private void Start()
        {
            if (SoundManager.Instance.IsPlayMainBGM() == false)
            {
                SoundManager.Instance.Play_MainBGM();
            }

            StartCoroutine(Co_SetStage());
            storyProgress.LastStage = 20; SetStages();
        }

        IEnumerator Co_SetStage()
        {
            yield return null;
            SetStages();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                storyProgress.LastStage = 20;
                SetStages();
            }

            MoveCam();
        }

        public void PenguinMove(Vector3 pos)
        {
            penguinParent.transform.position = pos;
        }

        private void MoveCam()
        {
            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                Vector3 newPosition = mainCam.transform.position;
                newPosition.x = Mathf.Clamp(newPosition.x - mouseX, minCamPosX, maxCamPosX);
                mainCam.transform.position = newPosition;
            }
        }

        // 조이스틱 환경에서 화면 전환
        public void MoveCamRight() {
            mainCam.transform.position = new Vector3(maxCamPosX, mainCam.transform.position.y, mainCam.transform.position.z);
        }
        public void MoveCamLeft() {
            mainCam.transform.position = new Vector3(minCamPosX, mainCam.transform.position.y, mainCam.transform.position.z);
        }

        private void SetStages()
        {
            for(int i = 0; i < storyProgress.LastStage + 1; i++)
            {
                if (i == storyProgress.LastStage)
                {
                    stageArray[storyProgress.LastStage].Initialize(true, ScreenStage.lastStageStarNumber);
                    stageArray[storyProgress.LastStage].transform.GetComponent<SpriteRenderer>().color = new Color(1, 0.6f, 0);
                    PenguinMove(stageArray[0].transform.position);
                }
                else if(i <  storyProgress.LastStage)
                {
                    // 열기
                    stageArray[i].Initialize(true);
                    stageArray[i].transform.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);

                    // 스타
                    for (int j = 0; j < 3; j++)
                    {
                        if (storyProgress.StageStarArray[i].IsGetStars[j] == true)
                        {
                            stageArray[i].Initialize(true,j);
                        }
                    }
                }
            }
        }

        public void StartStage()
        {
            //penguinParent.GetChild(0).transform.position = new Vector2(2.5f, 2);
            SoundManager.Instance.Play_ButtonClick();
            Fade.Out(Fade.FADE_TIME_DEFAULT, () =>
            {
                LoadingSceneController.LoadScene(SceneNameString._06_InGame);
            });
            //SceneManager.LoadScene(SceneNameString._06_InGame);
        }
    }
}