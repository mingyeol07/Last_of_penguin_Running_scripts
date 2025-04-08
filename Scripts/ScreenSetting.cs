using Lop.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSetting : MonoBehaviour
{
    private void Awake()
    {
        // 이 오브젝트를 새 씬 로드 시 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);

        // 씬이 로드될 때마다 OnSceneLoaded 함수 호출하도록 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 씬이 파괴될 때 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneNameString._00_Splash) return;
        if (scene.name == SceneNameString._01_Title) return;
        if (scene.name == SceneNameString._02_Main) return;
        if (scene.name == SceneNameString._06_LocalMulti2) return;
        if (scene.name == SceneNameString._06_InGame) return;
        if (scene.name == SceneNameString._09_MultiSelectPenguins2) return;
        if (scene.name == SceneNameString._10_LeaderBoard) return;
        if (scene.name == SceneNameString._10_MultiLeaderBoard2) return;
        if (scene.name == SceneNameString._09_Closet) return;
        if (scene.name == SceneNameString._04_StoryMode) return;

        // 씬 변경 시 실행하고 싶은 함수 호출
        SetResolution();
    }

    public void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }
}
