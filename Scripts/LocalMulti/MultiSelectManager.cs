using Lop.Game;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class MultiPlayerInfo {
    public Transform penguinParent;
    public RectTransform selectArrow;
    public TMP_Text txt_systemMessage;
    public MultiAbilityIcon abilityIcon;
    public InputField inputField;
    public int penguinID = 0;
    public bool isSelect = false;
}

/// <summary>
/// 멀티 환경에서 캐릭터 선택하는 씬
/// </summary>
public class MultiSelectManager : MonoBehaviour
{
    [SerializeField] private MultiPlayerInfo[] playerInfos;

    public static MultiSelectManager instance;

    [SerializeField] private PenguinData[] penguinDatas;

    [SerializeField] private GameObject multiSavePrefab;
    [SerializeField] private TMP_Text txt_timer;
    [SerializeField] private Button btn_home;

    private float timerTime = 30;
    private int readyTimerTime = 5; // 모두 레디 했을 때 시작까지 세는 시간
    private bool isModifying;

    private bool isDone;

    private int maxPenguinId = 6;
    private int minPenguinId = 0;

    private int arrowRectPositionY = 120;
    private int overlapSelectArrowX = 30;

    private readonly float[] modelRectPositionX = { -100, -105, -110, -115, -120, -125, -130 };
    private readonly float[] arrowRectPositionX = { -600, -400, -200, 0, 200, 400, 600 };

    private void Awake() {
        btn_home.onClick.AddListener(() => { SceneManager.LoadScene(SceneNameString._02_Main); });

        Instantiate(multiSavePrefab).GetComponent<MultiSaver>();

        for (int i =0;i < playerInfos.Length;i++) {
            var playerInfo = playerInfos[i];

            playerInfo.inputField?.onValueChanged.AddListener((string text) => { isModifying = true; });
            playerInfo.inputField?.onEndEdit.AddListener((string text) => { InputFieldValueChanged(i, text); });
            playerInfo.inputField?.onSubmit.AddListener((string text) => { InputFieldValueChanged(i, text); });

            playerInfo.txt_systemMessage.text = "준비하세요!";
            playerInfo.txt_systemMessage.color = Color.gray;

            if (MultiSaver.Instance != null) {
                playerInfo.penguinID = MultiSaver.Instance.PenguinIDArray[i];
            }

            InitSelect(i, playerInfo);

            MultiSaver.Instance.NameArray[i] = "Player " + i;
        }
    }

    private void Update() {

        CountDown();
    }
    private void CountDown()
    {
        int h = 0, m = 0;
        if ((int)timerTime >= 60)
        {
            h = (int)timerTime / 60;
            m = (int)timerTime % 60;
        }
        else
        {
            h = 0;
            m = (int)timerTime;
        }
        txt_timer.text = string.Format("{0:D2}:{1:D2}", h, m);
        if (timerTime <= 0 && !isDone)
        {
            isDone = true;

            Fade.Out(Fade.FADE_TIME_DEFAULT, () =>
            {
                if (playerInfos.Length > 2) LoadingSceneController.LoadScene(SceneNameString._06_LocalMulti4);
                else LoadingSceneController.LoadScene(SceneNameString._06_LocalMulti2);
            });
        }
        else if (!isDone)
        {
            timerTime -= Time.deltaTime;
        }
    }

    private void InputFieldValueChanged(int user, string text) {
        MultiSaver.Instance.NameArray[user] = text;
        isModifying = false;
    }

    private void InitSelect(int user, MultiPlayerInfo playerInfo) {
        SetPenguinParentPos(playerInfo.penguinParent, playerInfo.penguinID);
        SetArrowPos(user);
        playerInfo.abilityIcon?.InitSpeechBubble(penguinDatas[playerInfo.penguinID]);
    }

    private void SetPenguinParentPos(Transform penguinParent, int penguinID) {
        penguinParent.position = new Vector2(modelRectPositionX[penguinID], penguinParent.position.y);
    }

    private void SetArrowPos(int movePenguinIndex) {
        int penguinID = playerInfos[movePenguinIndex].penguinID;
        HashSet<int> processedIDs = new HashSet<int>(); // 이미 처리한 펭귄 ID 저장

        for (int i = 0; i < playerInfos.Length; i++) {
            if (processedIDs.Contains(playerInfos[i].penguinID)) continue; // 중복 체크

            int id = playerInfos[i].penguinID;
            processedIDs.Add(id); // 현재 ID 처리 완료

            // ID가 같은 다른 펭귄 찾기
            List<int> samePenguins = new List<int>();

            for (int j = 0; j < playerInfos.Length; j++) {
                if (playerInfos[j].penguinID == id) {
                    samePenguins.Add(j);
                }
            }

            // 같은 ID를 가진 펭귄이 여러 개면 좌우로 나누기
            if (samePenguins.Count > 1) {
                playerInfos[samePenguins[0]].selectArrow.anchoredPosition = new Vector2(arrowRectPositionX[id] + overlapSelectArrowX, arrowRectPositionY);
                playerInfos[samePenguins[1]].selectArrow.anchoredPosition = new Vector2(arrowRectPositionX[id] - overlapSelectArrowX, arrowRectPositionY);
            }
            else {
                playerInfos[samePenguins[0]].selectArrow.anchoredPosition = new Vector2(arrowRectPositionX[id], arrowRectPositionY);
            }
        }
    }

    public void MoveRight(int userIndex) {
        if (playerInfos[userIndex].penguinID == maxPenguinId || playerInfos[userIndex].isSelect || isModifying) return;

        playerInfos[userIndex].penguinID++;
        InitSelect(userIndex, playerInfos[userIndex]);
    }

    public void MoveLeft(int userIndex) {
        if (playerInfos[userIndex].penguinID == minPenguinId || playerInfos[userIndex].isSelect || isModifying) return;

        playerInfos[userIndex].penguinID--;
        InitSelect(userIndex, playerInfos[userIndex]);
    }

    public void SelectPenguin(int userIndex) {
        if (AllPlayerSelected() || isModifying) return;

        if (playerInfos[userIndex].isSelect) {

            playerInfos[userIndex].txt_systemMessage.color = Color.gray;
            playerInfos[userIndex].isSelect = false;
        }
        else {
            playerInfos[userIndex].txt_systemMessage.text = "준비가 완료되었습니다.";
            playerInfos[userIndex].txt_systemMessage.color = Color.green;
            playerInfos[userIndex].isSelect = true;
            MultiSaver.Instance.PenguinIDArray[userIndex] = playerInfos[userIndex].penguinID;

            if (AllPlayerSelected()) {
                timerTime = readyTimerTime;
                playerInfos[userIndex].txt_systemMessage.text = "잠시 후 시작합니다!";
            }
        }
    }

    private bool AllPlayerSelected() {
        for(int i =0; i < playerInfos.Length; i++) {
            if (!playerInfos[i].isSelect) return false;
        }
        return true;
    }

    public void BackHome() {
        btn_home.onClick.Invoke();
    }
}
