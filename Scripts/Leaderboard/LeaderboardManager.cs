using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lop.Game
{
    public class LeaderboardManager : MonoBehaviour
    {
        private MyRecordDatasInStage myRecordDatas;

        private int nowStageID;
        private int nowScore;
        private int nowPenguinID;
        private bool[] nowIsGetStars = new bool[3];

        [SerializeField] private Button btn_home;
        [SerializeField] private Button btn_restart;
        [SerializeField] private Button btn_sign;

        [Header("PenguinIcons")]
        [SerializeField] private Sprite[] sprite_icons;
        [SerializeField] private Image[] img_penguinIcons;

        [Header("Leaderboards")]
        [SerializeField] private GameObject[] pnl_leaderBoard;
        [SerializeField] private TMP_Text[] txt_scores;
        [SerializeField] private TMP_Text[] txt_names;

        [Header("NowRecord")]
        [SerializeField] private TMP_Text txt_score;
        [SerializeField] private InputField inputField_name;
        [SerializeField] private GameObject[] stars;

        private void Awake()
        {
            btn_home.transform.parent.gameObject.SetActive(false);
            btn_restart.transform.parent.gameObject.SetActive(false);

            //inputField_name.onSubmit.AddListener(InputFieldValueChanged) ;

            btn_home.onClick.AddListener(()=> 
            {
                SoundManager.Instance.Play_ButtonClick();
                SceneManager.LoadScene(SceneNameString._04_StoryMode); 
            });
            btn_restart.onClick.AddListener(() =>
            {
                SoundManager.Instance.Play_ButtonClick();
                SceneManager.LoadScene(SceneNameString._06_InGame);
            });

            btn_sign.onClick.AddListener(() => { 
                InputFieldValueChanged(inputField_name.text); btn_sign.gameObject.SetActive(false);
                inputField_name.interactable = false;
            });
        }

        void Start()
        {
            SoundManager.Instance.Play_Win();
            SoundManager.Instance.Play_Fireworks();

            LoadMyRecord();

            // 현재 스테이지와 점수를 불러오기
            nowStageID = PlayerPrefs.GetInt(PlayerPrefsKey.My_PlayStageID);
            nowPenguinID = PlayerPrefs.GetInt(PlayerPrefsKey.My_PenguinID);
            nowScore = PlayerPrefs.GetInt(PlayerPrefsKey.My_PlayScore);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_PlayScore, 0);  // 점수를 초기화

            nowIsGetStars[0] = PlayerPrefs.GetInt(PlayerPrefsKey.My_GetStar_1) == 1;
            nowIsGetStars[1] = PlayerPrefs.GetInt(PlayerPrefsKey.My_GetStar_2) == 1;
            nowIsGetStars[2] = PlayerPrefs.GetInt(PlayerPrefsKey.My_GetStar_3) == 1;
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_1, 0);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_2, 0);
            PlayerPrefs.SetInt(PlayerPrefsKey.My_GetStar_3, 0);


            txt_score.text = nowScore.ToString();

            for (int i = 0; i < nowIsGetStars.Length; i++)
            {
                if(nowIsGetStars[i] == true)
                {
                    stars[i]?.SetActive(true);
                }
            }
            SaveStageStars();
            SetLeaderboardDatas();
        }

        private void InputFieldValueChanged(string text)
        {
            text = inputField_name.textComponent.text;

            btn_home.transform.parent.gameObject.SetActive(true);
            btn_restart.transform.parent.gameObject.SetActive(true);

            RecordData recordData = new();
            recordData.Name = text;
            recordData.Score = nowScore;
            recordData.PenguinID = nowPenguinID;

            myRecordDatas.RecordInStages[nowStageID].RecordDatas.Add(recordData);
            myRecordDatas.RecordInStages[nowStageID].RecordDatas.Sort((RecordData a, RecordData b) => { return b.Score.CompareTo(a.Score); });

            inputField_name.interactable = false;
            SaveMyRecord();

            SetLeaderboardDatas();
        }

        private void SetLeaderboardDatas()
        {
            for(int i = 0; i < txt_scores.Length; i++)
            {
                if (myRecordDatas.RecordInStages[nowStageID].RecordDatas.Count <= i)
                {
                    pnl_leaderBoard[i].SetActive(false);
                }
                else
                {
                    pnl_leaderBoard[i].SetActive(true);
                    txt_scores[i].text = myRecordDatas.RecordInStages[nowStageID].RecordDatas[i].Score.ToString("D7");
                    img_penguinIcons[i].sprite = sprite_icons[myRecordDatas.RecordInStages[nowStageID].RecordDatas[i].PenguinID];
                }
            }

            for(int i = 0; i < txt_names.Length; i++)
            {
                if (myRecordDatas.RecordInStages[nowStageID].RecordDatas.Count <= i)
                {
                    txt_names[i].text = "";
                    continue;
                }
                else
                {
                    txt_names[i].text = myRecordDatas.RecordInStages[nowStageID].RecordDatas[i].Name.ToString();
                }
            }
        }

        private void LoadMyRecord()
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, "MyRecordData.json");

                if (File.Exists(path))
                {
                    string jsonContent = File.ReadAllText(path);
                    myRecordDatas = JsonUtility.FromJson<MyRecordDatasInStage>(jsonContent);
                }
                else
                {
                    InitializeNewRecordData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load record data: " + e.Message);
                InitializeNewRecordData();  // 오류 시 데이터 초기화
            }
        }

        private void InitializeNewRecordData()
        {
            myRecordDatas = new MyRecordDatasInStage();
            myRecordDatas.RecordInStages = new List<StageRecordData>();
            for (int i = 0; i < 21; i++)
            {
                myRecordDatas.RecordInStages.Add(new StageRecordData());
            }
            SaveMyRecord();
        }

        private void SaveMyRecord()
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, "MyRecordData.json");
                string saveJson = JsonUtility.ToJson(myRecordDatas, true);
                File.WriteAllText(path, saveJson);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save record data: " + e.Message);
            }
        }

        private void SaveStageStars()
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, "StoryProgressData.json");
                string json;
                StoryProgressData progressData = null;

                if (File.Exists(path))
                {
                    json = File.ReadAllText(path);
                    progressData = JsonUtility.FromJson<StoryProgressData>(json);
                }
                else
                {
                    Debug.LogError("ProgressData가 없습니다.");
                }

                // 별점 업데이트

                StageStarData starData = new StageStarData();
                starData.IsGetStars = new bool[3];

                for (int i = 0; i < nowIsGetStars.Length; i++)
                {
                    if (nowIsGetStars[i])
                    {
                        starData.IsGetStars[i] = true;
                    }
                }

                for(int i = 0; i < 3; i++)
                {
                    if (progressData.StageStarArray[nowStageID].IsGetStars[i]) continue;

                    progressData.StageStarArray[nowStageID].IsGetStars[i] = starData.IsGetStars[i];
                }


                // 다음스테이지

                bool isClear = PlayerPrefs.GetInt(PlayerPrefsKey.My_PlayStageIsClear) != 0;
                PlayerPrefs.SetInt(PlayerPrefsKey.My_PlayStageIsClear, 0);

                if (isClear)
                {
                    if(progressData.LastStage <= nowStageID)
                    {
                        progressData.LastStage = nowStageID + 1;
                    }
                }


                // 저장
                json = JsonUtility.ToJson(progressData, true);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save stage stars: " + e.Message);
            }
        }

        private void OnDestroy()
        {
            SoundManager.Instance.Play_MainBGM();
        }
    }
}