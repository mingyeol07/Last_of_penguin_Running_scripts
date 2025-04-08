using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lop.Game
{
    public class _01_Title : MonoBehaviour
    {
        private void Start()
        {
            SoundManager.Instance.Play_MainBGM();
            Fade.In(0.5f);
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(SceneNameString._02_Main);
            }
        }
    }
}
