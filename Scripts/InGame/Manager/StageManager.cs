using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using TMPro;

namespace Lop.Game
{
    public class StageManager : MonoBehaviour
    {
        private StageData stageData = null;

        [SerializeField] private Transform camTransform;
        // 반드시 enum형 ObjectType에 기재된 오브젝트의 순서를 따라야 한다.
        [SerializeField] private GameObject[] gameObjectArray = null;
        [SerializeField] private GameObject[] jellyObjectArray = null;
        [SerializeField] private int playStageId;
        public int PlayStageId => playStageId;

        private int chunkCount = 0; // 청크 수
        private int starCount = 0;
        private const int chunkSize = 40; // 청크를 소환하는 기준
        private const int spawnDistance = 20; // 청크를 소환할 때 카메라의 위치를 잡기 위한 변수
        private const float jellySpawnOffsetY = 0.5f;

        private int stageLength; // 스테이지의 길이
        public int StageLength => stageLength;

        private Stack<GameObject> currentObjects = new Stack<GameObject>();
        private Stack<GameObject> previousObjects = new Stack<GameObject>();
        public Stack<GameObject> CurrentObjects => currentObjects;
        public Stack<GameObject> PreviousObjects => previousObjects;
        private List<GameObject> jellyList = new List<GameObject>();
        private bool isJellySpawned = false;

        // Endless
        [SerializeField] private bool isEndlessMode;
        public bool IsEndlessMode => isEndlessMode;

        private bool stageChangeFlag; // 스테이지가 변경됨을 감지하는 플래그
        private int nextStageStartX;

        [SerializeField] private int endlessStartStageID;
        [SerializeField] private TMP_Text txt_round;

        // Multi
        [Header("Local Multi")]
        [SerializeField] private float spawnOffsetY;
        [SerializeField] private bool isMultiRandomStage;
        private int penguinNumber;
        private int currentStageCount;

        // beta
        private int[] ranStageID2 = new int[2];

        #region 다른 멀티 플레이어 정보
        private bool otherPenguinIsDefaultPenguin; // 상대 펭귄이 디폴트일때
        private bool isSizeUpTimeByDefaultPenguin; // 디폴트펭귄의 스킬발동 때
        private List<GameObject> sizeUpObjects = new List<GameObject>();
        private Vector3 upObjectSizeByDefaultPenguinAttack = new Vector3(1.2f, 1.2f, 1.2f);
        private Vector3 returnObjectSizeByDefaultPenguinAttack = Vector3.one;

        private bool otherPenguinIsAdeliePenguin;

        private void Start()
        {
            StartCoroutine(GetAdelie());
        }

        private IEnumerator GetAdelie()
        {
            yield return new WaitForSeconds(0.1f);

            var adelie = GameManagerParent.Instance.GetMyPenguin(penguinNumber).GetComponent<PenguinBase>();

            otherPenguinIsAdeliePenguin = adelie.MyPenguinName == PenguinName.Adlie;
        }
        #endregion

        private void Awake()
        {
            if (isMultiRandomStage)
            {
                ranStageID2 = MultiSaver.Instance.RandomStageID2;
                currentStageCount = 0;
                SetPath(ranStageID2[currentStageCount]);
            }
            else if (isEndlessMode)
            {
                SetPath(endlessStartStageID);
            }
            else
            {
                SetPath(playStageId);
            }
        }

        private void SetPath(int stageId = 99)
        {
            string path;

            if (stageId == 99) playStageId = PlayerPrefs.GetInt(PlayerPrefsKey.My_PlayStageID);
            else playStageId = stageId;

            path = Path.Combine(Application.streamingAssetsPath, "StageData_" + playStageId + ".json");
            LoadStageData(path);
        }

        private void LoadStageData(string path)
        {
            if (File.Exists(path))
            {
                string jsonContent = File.ReadAllText(path);
                stageData = MapJsonUtility.JsonToMap(jsonContent);
            }
            else
            {
                if (isEndlessMode)
                {
                    SetPath(0);
                }
                throw new Exception("스테이지 데이터가 없습니다.");
            }

            InitStage();
        }

        private async void InitStage()
        {
            // GameManager에 보낼 stageLength를 저장
            stageLength = stageData.ObjectsInStage.GetLength(0) + nextStageStartX;

            // 비동기 젤리생성
            isJellySpawned = true;
            await InstanceJellyAsync();
        }

        private async Task InstanceJellyAsync()
        {
            for (int i = 0; i < stageData.JellyList.Count; i++)
            {
                await Task.Delay(100);
                if (this != null)
                {
                    if (stageData.JellyList[i].type == 0) continue;

                    GameObject go = Instantiate(jellyObjectArray[(int)stageData.JellyList[i].type], transform);
                    go.transform.position = new Vector2(nextStageStartX + stageData.JellyList[i].pos.x - jellySpawnOffsetY, stageData.JellyList[i].pos.y - jellySpawnOffsetY + spawnOffsetY);

                    if (stageData.JellyList[i].type == JellyType.FeverStar_gold)
                    {
                        go.GetComponent<StageStar>().SetStarNumber(starCount);
                        starCount++;
                    }

                    jellyList.Add(go);
                }
            }

            isJellySpawned = false;
        }

        private void Update()
        {
            if (camTransform.position.x > nextStageStartX + (chunkSize * chunkCount) - spawnDistance)
            {
                DrawStage();
                chunkCount++;
            }

            if (otherPenguinIsDefaultPenguin)
            {
                sizeUpObjects.RemoveAll(item => item == null);

                if (isSizeUpTimeByDefaultPenguin)
                {
                    for (int i = 0; i < sizeUpObjects.Count; i++)
                    {
                        if (sizeUpObjects[i].transform.localScale != upObjectSizeByDefaultPenguinAttack)
                        {
                            sizeUpObjects[i].transform.localScale = upObjectSizeByDefaultPenguinAttack;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < sizeUpObjects.Count; i++)
                    {
                        if (sizeUpObjects[i].transform.localScale != returnObjectSizeByDefaultPenguinAttack)
                        {
                            sizeUpObjects[i].transform.localScale = returnObjectSizeByDefaultPenguinAttack;
                        }
                    }
                }
            }
        }

        private void DrawStage()
        {
            if (stageData == null) return;

            // stageData.ObjectsInStage의 길이가 아니라 실제 청크 개수를 체크해야 함

            if (IsEndlessMode || isMultiRandomStage)
            {
                // 스테이지가 변경되어야하는지 검사 
                ResetStageChangeIfNeeded();
                if (!stageChangeFlag && chunkCount * chunkSize >= stageData.ObjectsInStage.GetLength(0) - spawnDistance)
                {
                    // 청크 초기화
                    chunkCount = 0;

                    // 오브젝트 생성 최소위치 초기화
                    nextStageStartX += stageData.ObjectsInStage.GetLength(0);

                    // 스테이지 변경
                    if (isMultiRandomStage && ranStageID2.Length - 1 > currentStageCount)
                    {
                        currentStageCount++;
                        SetPath(ranStageID2[currentStageCount]);
                    }
                    if (IsEndlessMode && playStageId <= 20)
                    {
                        playStageId++;
                        txt_round.text = "Stage " + playStageId;
                        SetPath(playStageId);
                    }

                    // 플래그 초기화
                    stageChangeFlag = true;
                }
            }
            else
            {
                if (chunkCount * chunkSize >= stageData.ObjectsInStage.GetLength(0))
                    return;
            }
            
            Lop.Game.Tile.TileData[,] objectList = stageData.ObjectsInStage;

            // 이전 오브젝트 스택 비우기
            while (previousObjects.Count > 0)
            {
                Destroy(previousObjects.Pop());
            }

            // 젤리도 플레이어위치랑 비교해서 지우기
            if (!isJellySpawned)
            {
                // 카메라 위치에서 일정 범위 내의 젤리만 삭제하도록 수정
                for (int i = jellyList.Count - 1; i >= 0; i--)
                {
                    // jellyList[i]가 null인지 확인
                    if (jellyList[i] == null)
                    {
                        jellyList.RemoveAt(i);
                        continue;
                    }

                    // 카메라의 x 위치와 젤리의 x 위치를 비교하여 삭제
                    if (jellyList[i].transform.position.x < camTransform.position.x - spawnDistance)
                    {
                        Destroy(jellyList[i]);
                        jellyList.RemoveAt(i);
                    }
                }
            }

            // 현재 오브젝트 스택을 이전 오브젝트 스택으로 교체
            previousObjects = currentObjects;
            currentObjects = new Stack<GameObject>();

            // 청크 하나의 x최소값~x최대값을 가진 TileData 검사
            int startX = chunkSize * chunkCount; // chunkCount가 0부터 시작되므로 그대로 사용
            int endX = Mathf.Min(chunkSize * (chunkCount + 1), objectList.GetLength(0)); // 다음 청크의 시작

            // 마지막 청크일때, 끝까지 생성하기
            if(objectList.GetLength(0) - chunkSize * (chunkCount + 1) < chunkSize)
            {
                endX = objectList.GetLength(0);
            }

            // 오브젝트 생성
            for (int x = startX; x < endX; x++)
            {
                for (int y = 0; y < objectList.GetLength(1); y++)
                {
                    Lop.Game.Tile.TileData tile = objectList[x, y];
                    if (tile.ObjectTypeId != 0)
                    {
                        if (tile.ObjectTypeId == (int)ObjectType.ClearFlag)
                        {
                            if (isMultiRandomStage && currentStageCount < ranStageID2.Length - 1)
                            {
                                continue;
                            }
                            else if (isEndlessMode && playStageId != 20)
                            {
                                continue;
                            }
                        }

                        GameObject go = Instantiate(gameObjectArray[tile.ObjectTypeId], transform);
                        go.transform.position = new Vector2(nextStageStartX + x, y + spawnOffsetY);
                        currentObjects.Push(go);

                        #region 배틀모드일 때 상대방이 default펭귄인 경우,
                        if (otherPenguinIsDefaultPenguin)
                        {
                            if (go.CompareTag("Obstacle"))
                            {
                                sizeUpObjects.Add(go);
                            }
                        }
                        #endregion
                    }

                    #region 아델리펭귄의 경우 바닥깔기
                    if (otherPenguinIsAdeliePenguin)
                    {
                        if (tile.ObjectTypeId == 0 && y == 0)
                        {
                            GameObject go = Instantiate(gameObjectArray[(int)ObjectType.Ground_ice], transform);
                            go.transform.position = new Vector2(x, y);
                            go.GetComponent<SpriteRenderer>().color = Color.cyan;
                            currentObjects.Push(go);
                        }
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 스테이지를 변경할 수 있는지 검사
        /// </summary>
        private void ResetStageChangeIfNeeded()
        {
            // 카메라가 충분히 이동한 경우 플래그를 초기화하여 다음 전환을 준비
            if (camTransform.position.x > nextStageStartX + chunkSize)
            {
                stageChangeFlag = false;
            }
        }

        public void SetMulti(int penguinNumber)
        {
            this.penguinNumber = penguinNumber;
        }

        public void SetOtherPenguinIsDefault()
        {
            otherPenguinIsDefaultPenguin = true;
        }

        public void DefaultAttackTime()
        {
            StartCoroutine(SizeUp());
        }

        private IEnumerator SizeUp()
        {
            if (isSizeUpTimeByDefaultPenguin) yield break;
            isSizeUpTimeByDefaultPenguin = true;
            yield return new WaitForSeconds(2.5f);
            isSizeUpTimeByDefaultPenguin = false;
        }
    }
}