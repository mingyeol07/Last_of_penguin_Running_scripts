using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lop.Game
{
    public class MultiLeaderboard : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] txt_names;
        [SerializeField] private TMP_Text[] txt_scores;
        //[SerializeField] private GameObject[,] playPenguins;

        [SerializeField] private Button btn_home;
        [SerializeField] private Button btn_restart;

        private int[] sortedScores = new int[4];
        private int[] sortedPenguinIDs = new int[4];
        private string[] sortedNames = new string[4];

        private void Awake()
        {
            btn_home.onClick.AddListener(()=> { SceneManager.LoadScene(SceneNameString._02_Main); });
            btn_restart.onClick.AddListener(() => 
            {
                int[] penguinIDs = MultiSaver.Instance.PenguinIDArray;

                Destroy(MultiSaver.Instance);

                MultiSaver.Instance.PenguinIDArray = penguinIDs;

                SceneManager.LoadScene(SceneNameString._09_MultiSelectPenguins2); 
            });
        }

        private void Start()
        {
            SoundManager.Instance.Play_Win();
            SoundManager.Instance.Play_Fireworks();

            var indexedArray = MultiSaver.Instance.ScoreArray
            .Select((value, index) => new { Value = value, Index = index })
            .OrderByDescending(x => x.Value) // 값 기준 내림차순 정렬
            .ToArray();

            for (int i = 0; i < txt_names.Length; i++) {
                sortedScores[i] = indexedArray[i].Value;
                sortedNames[i] = MultiSaver.Instance.NameArray[indexedArray[i].Index];
                sortedPenguinIDs[i] = MultiSaver.Instance.PenguinIDArray[indexedArray[i].Index];
            }

            for (int i =0; i < txt_names.Length; i++) {
                //playPenguins[i,sortedPenguinIDs[i]].SetActive(true);
                txt_names[i].text = sortedNames[i];
                txt_scores[i].text = sortedScores[i].ToString();
            }
        }
        private void OnDestroy()
        {
            SoundManager.Instance.Play_MainBGM();
        }
    }
}