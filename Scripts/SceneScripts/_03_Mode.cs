using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lop.Game
{
    public class _03_Mode : MonoBehaviour
    {
        [SerializeField] private Button btn_storyMode;
        [SerializeField] private Button btn_makerMode;
        [SerializeField] private Button btn_home;

        private void Awake()
        {
            OnClickButtons();
        }

        private void OnClickButtons()
        {
            btn_storyMode.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneNameString._04_StoryMode);
                SoundManager.Instance.Play_ButtonClick();
            });
            //btn_makerMode.onClick.AddListener(() => { SceneManager.LoadScene(SceneNameString._05_MakerMode); });
            btn_home.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneNameString._02_Main);
                SoundManager.Instance.Play_ButtonClick();
            });
        }
    }
}