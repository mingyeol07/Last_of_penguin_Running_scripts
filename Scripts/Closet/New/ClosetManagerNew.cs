using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lop.Game
{
    public class ClosetManagerNew : MonoBehaviour
    {
        [SerializeField] private PenguinData[] penguinDatas;
        private int curPenguinId = 0;

        #region DataPanel
        [SerializeField] private TMP_Text txt_penguinName;

        [Header("Individuality")]
        [SerializeField] private TMP_Text txt_individuality_1;
        [SerializeField] private GameObject[] img_individuality_stars_1;
        [Space(10f)]
        [SerializeField] private TMP_Text txt_individuality_2;
        [SerializeField] private GameObject[] img_individuality_stars_2;

        [Header("Ability")]
        [SerializeField] private Image img_passive;
        [SerializeField] private TMP_Text txt_passive_name;
        [SerializeField] private TMP_Text txt_passive;
        [SerializeField] private Image img_passive_thumbnail;
        [Space(10f)]
        [SerializeField] private Image img_fever;
        [SerializeField] private TMP_Text txt_fever_name;
        [SerializeField] private TMP_Text txt_fever;
        [SerializeField] private Image img_fever_thumbnail;
        [Space(10f)]
        [SerializeField] private Image img_battle;
        [SerializeField] private TMP_Text txt_battle_name;
        [SerializeField] private TMP_Text txt_battle;
        [SerializeField] private Image img_battle_thumbnail;
        #endregion

        [Header("Buttons")]
        [SerializeField] private Button btn_home;

        [SerializeField] private Button btn_save;
        [SerializeField] private Button btn_undo;

        [SerializeField] private Button btn_moveLeft;
        [SerializeField] private Button btn_moveRight;

        [SerializeField] private Transform penguinParentTransform;
        private float penguinMoveTime = 0.1f;

        // 저장 / 되돌리기 안내메세지
        [Header("Syetem Message")]
        [SerializeField] private TMP_Text txt_systemMessage; // 안내 메시지 ("저장되었습니다.")
        private float curSystemMessageColorTime;
        private float maxSystemMessageColorTime = 1;

        // 상단 펭귄 썸네일을 가리키는 화살표
        [Header("Thumbnail / Arrow")]
        [SerializeField] private RectTransform pointToCurPenguinArrow;
        private readonly float[] pointRectPositionX = { -450,  -300, -150, 0, 150, 300, 450 };
        private readonly float[] pointPenguinParentPosX = { -100, -105, -110, -115, -120, -125, -130 };

        private float thumbnailMoveTime = 0.1f;
        //[SerializeField] private PenguinThumbnail[] penguinThumbnails;

        private float undoSpeed = 0.02f;
        private bool isMove = false;

        [SerializeField] private RectTransform selectStar;

        [SerializeField] private Button[] icons;

        private void Awake()
        {
            InitOnClickButtons();
        }
        private void Start()
        {
            // 이전에 골랐던 펭귄으로 초기화
            OnClickUndoButton();
            InitCurrentPenguinData(PlayerPrefs.GetInt(PlayerPrefsKey.My_PenguinID, curPenguinId));
            InitStar();
        }
        private void Update()
        {
            UpdateSystemMessageColor();
        }

        // 시스템메세지 컬러 자연스럽게 사라지기
        private void UpdateSystemMessageColor()
        {
            // 업데이트에서 돌린 이유는.. 연속으로 다다다닥 누를 경우를 대비
            if (curSystemMessageColorTime > 0)
            {
                curSystemMessageColorTime -= Time.deltaTime;
                float t = curSystemMessageColorTime / maxSystemMessageColorTime;
                txt_systemMessage.color = new Color(0, 1, 0, t);
            }
        }

        /// <summary>
        /// 버튼 초기화
        /// </summary>
        private void InitOnClickButtons()
        {
            btn_moveLeft.onClick.AddListener(() =>
            {
                StartCoroutine(OnClickMovePenguinLeft());
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_moveRight.onClick.AddListener(() =>
            {
                StartCoroutine(OnClickMovePenguinRight());
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_save.onClick.AddListener(() =>
            {
                OnClickSaveButton();
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_undo.onClick.AddListener(() =>
            {
                OnClickUndoButton();
                SoundManager.Instance.Play_ButtonClick();
            });
            btn_home.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneNameString._02_Main);
                SoundManager.Instance.Play_ButtonClick();
            });

            icons[0].onClick.AddListener(() => { StartCoroutine(UndoMyPenguin(0, undoSpeed)); });
            icons[1].onClick.AddListener(() => { StartCoroutine(UndoMyPenguin(1, undoSpeed)); });
            icons[2].onClick.AddListener(() => { StartCoroutine(UndoMyPenguin(2, undoSpeed)); });
            icons[3].onClick.AddListener(() => { StartCoroutine(UndoMyPenguin(3, undoSpeed)); });
            icons[4].onClick.AddListener(() => { StartCoroutine(UndoMyPenguin(4, undoSpeed)); });
            icons[5].onClick.AddListener(() => { StartCoroutine(UndoMyPenguin(5, undoSpeed)); });
            icons[6].onClick.AddListener(() => { StartCoroutine(UndoMyPenguin(6, undoSpeed)); });
        }

        /// <summary>
        /// 내가 저장한 펭귄을 썸네일에 별 표시 해주는 함수
        /// </summary>
        private void InitStar()
        {
            selectStar.anchoredPosition = new Vector2(pointRectPositionX[PlayerPrefs.GetInt(PlayerPrefsKey.My_PenguinID, curPenguinId)], 0);
        }

        /// <summary>
        ///  지금 펭귄 인덱스를 받아 data패널들을 수정
        /// </summary>
        /// <param name="targetDataIndex"></param>
        private void InitCurrentPenguinData(int targetDataIndex)
        {
            PenguinData curPenguinData = penguinDatas[targetDataIndex];

            txt_penguinName.text = curPenguinData.PenguinName;

            txt_individuality_1.text = curPenguinData.Individuality_1_name;
            for (int i = 0; i < 5; i++)
            {
                if (curPenguinData.Individuality_1_value > i)
                {
                    img_individuality_stars_1[i].SetActive(true);
                }
                else
                {
                    img_individuality_stars_1[i].SetActive(false);
                }
            }

            txt_individuality_2.text = curPenguinData.Individuality_2_name;
            for (int i = 0; i < 5; i++)
            {
                if (curPenguinData.Individuality_2_value > i)
                {
                    img_individuality_stars_2[i].SetActive(true);
                }
                else
                {
                    img_individuality_stars_2[i].SetActive(false);
                }
            }

            img_passive.sprite = curPenguinData.PassiveAbility_sprite;
            img_passive_thumbnail.sprite = curPenguinData.PassiveAbility_sprite;
            txt_passive_name.text = curPenguinData.PassiveAbility_name;
            txt_passive.text = curPenguinData.PassiveAbility;

            img_fever.sprite = curPenguinData.FeverAbility_sprite;
            img_fever_thumbnail.sprite = curPenguinData.FeverAbility_sprite;
            txt_fever_name.text = curPenguinData.FeverAbility_name;
            txt_fever.text = curPenguinData.FeverAbility;

            img_battle.sprite = curPenguinData.BattleAbility_sprite;
            img_battle_thumbnail.sprite = curPenguinData.BattleAbility_sprite;
            txt_battle_name.text = curPenguinData.BattleAbility_name;
            txt_battle.text = curPenguinData.BattleAbility;
        }

        private void OnClickSaveButton()
        {
            curSystemMessageColorTime = maxSystemMessageColorTime;

            selectStar.anchoredPosition = new Vector2(pointRectPositionX[curPenguinId], 0);

            PlayerPrefs.SetInt(PlayerPrefsKey.My_PenguinID, curPenguinId);
        }

        private void OnClickUndoButton()
        {
            StartCoroutine(UndoMyPenguin(PlayerPrefs.GetInt(PlayerPrefsKey.My_PenguinID, curPenguinId), 0.2f));
        }

        private IEnumerator UndoMyPenguin(int targetPenguinId, float moveSpeed = 0)
        {
            if (targetPenguinId < curPenguinId)
            {
                yield return StartCoroutine(OnClickMovePenguinLeft(targetPenguinId));
            }
            else if (targetPenguinId > curPenguinId)
            {
                yield return StartCoroutine(OnClickMovePenguinRight(targetPenguinId));
            }
        }


        private IEnumerator OnClickMovePenguinLeft(int targetPosIndex = 99)
        {
            if (curPenguinId <= 0 || isMove) yield break;

            isMove = true;

            if (targetPosIndex == 99) targetPosIndex = curPenguinId - 1;

            InitCurrentPenguinData(targetPosIndex);

            StartCoroutine(MoveThumbnailArrow(targetPosIndex));
            yield return StartCoroutine(MovePenguinModel(targetPosIndex));

            curPenguinId = targetPosIndex;
            isMove = false;
        }

        private IEnumerator OnClickMovePenguinRight(int targetPosIndex = 99)
        {
            if (curPenguinId >= penguinDatas.Length - 1 || isMove) yield break;

            isMove = true;

            if (targetPosIndex == 99) targetPosIndex = curPenguinId + 1;

            InitCurrentPenguinData(targetPosIndex);

            StartCoroutine(MoveThumbnailArrow(targetPosIndex));
            yield return StartCoroutine(MovePenguinModel(targetPosIndex));

            curPenguinId = targetPosIndex;
            isMove = false;
        }

        // 썸네일을 가리키는 화살 표시 이동
        private IEnumerator MoveThumbnailArrow(int nextMoveIndex)
        {
            int distance = Mathf.Abs(curPenguinId - nextMoveIndex);
            
            float curTime = 0;
            float maxTime = thumbnailMoveTime * distance;

            Vector2 previoutRectAnchor = pointToCurPenguinArrow.anchoredPosition;

            while (curTime < maxTime)
            {
                curTime += Time.deltaTime;
                float t = curTime / maxTime;

                pointToCurPenguinArrow.anchoredPosition = new Vector2(Mathf.Lerp(previoutRectAnchor.x, pointRectPositionX[nextMoveIndex], t), previoutRectAnchor.y);
                yield return null;
            }

            pointToCurPenguinArrow.anchoredPosition = new Vector2(pointRectPositionX[nextMoveIndex], previoutRectAnchor.y);
        }

        // 펭귄 모델 이동
        private IEnumerator MovePenguinModel(int nextMoveIndex)
        {
            int distance = Mathf.Abs(curPenguinId - nextMoveIndex);

            float curTime = 0;
            float maxTime = penguinMoveTime * distance;

            float previousTransformX = penguinParentTransform.position.x;
            float nextTransformX = pointPenguinParentPosX[nextMoveIndex];

            while (curTime < maxTime)
            {
                curTime += Time.deltaTime;
                float t = curTime / maxTime;

                penguinParentTransform.position = new Vector2(Mathf.Lerp(previousTransformX, nextTransformX, t), 0);

                yield return null;
            }

            penguinParentTransform.position = new Vector2(nextTransformX, 0);
        }
    }
}