//using Lop.Editor.TileClass;
//using Lop.Structure;
//using LopRunning.ObjectTile;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//namespace Lop.Game
//{
//    public class Test_TutorialMap : MonoBehaviour
//    {
//        private StageData createStage;
//        [SerializeField] private GameObject[] objectArray;
//        [SerializeField] private Transform objectsParent;
//        [SerializeField] private string saveStage_Name;
//        [SerializeField] private string saveStage_Descrtiption;
//        [SerializeField] private int saveStage_Id;

//        private void Awake()
//        {
//            createStage = MapJsonUtility.JsonToMap(ReadMapInScene());
//        }

//        private void Start()
//        {
//            TestLoadMapObject();
//            TestLoadMapJelly();
//        }

//        private void TestLoadMapObject()
//        {
//            int startX = 0; // chunkCount가 0부터 시작되므로 그대로 사용
//            int endX = createStage.ObjectsInStage.GetLength(0); // 다음 청크의 시작

//            for (int x = startX; x < endX; x++)
//            {
//                for (int y = 0; y < createStage.ObjectsInStage.GetLength(1); y++)
//                {
//                    Lop.Game.Tile.TileData tile = createStage.ObjectsInStage[x, y];
//                    if (tile.ObjectTypeId != 0)
//                    {
//                        GameObject go = Instantiate(objectArray[tile.ObjectTypeId], transform);
//                        go.transform.position = new Vector2(x, y);
//                    }
//                }
//            }

//            TestLoadMapJelly();
//        }

//        private void TestLoadMapJelly()
//        {
//            for (int i = 0; i < createStage.JellyList.Count; i++)
//            {
//                GameObject go = Instantiate(objectArray[(int)createStage.JellyList[i].type], transform);
//                go.transform.position = createStage.JellyList[i].pos;
//            }
//        }

//        private string ReadMapInScene()
//        {
//            string stageName = saveStage_Name;
//            int stageId = saveStage_Id;
//            string stageDescription = saveStage_Descrtiption;
//            GridList<Lop.Editor.TileData> gridList = new GridList<Lop.Editor.TileData>(700, 15);
//            Lop.Editor.JellyList jellyList = new Lop.Editor.JellyList();
//            SetTile(ref gridList, 680, 1, ObjectType.ClearFlag);
//            //오브젝트 y는 0~7까지 보임

//            GameObject[] childrenObjects = objectsParent.GetComponentsInChildren<GameObject>();
//            foreach(GameObject child in childrenObjects)
//            {
//                for(int i =0; i < objectArray.Length; i++)
//                {
//                    if (PrefabUtility.GetCorrespondingObjectFromSource(child) == objectArray[i])
//                    {
//                        if (child.CompareTag("FeverStar") || child.CompareTag("TimeItem") ||
//                            child.CompareTag("BlueFish") || child.CompareTag("RedFish") || child.CompareTag("Shellfish") || child.CompareTag("Shrimp") || child.CompareTag("Squid"))
//                        {
//                            SetItem(ref jellyList, child.transform.position.x, child.transform.position.y, (JellyType)i);
//                        }
//                        else
//                        {
//                            SetTile(ref gridList, (int)child.transform.position.x, (int)child.transform.position.y, (ObjectType)i);
//                        }
//                        break;
//                    }
//                }
//            }

//            string stageData = MapJsonUtility.MapToJson(stageId, stageName, stageDescription, 0, gridList, jellyList);

//            string path = Path.Combine(Application.streamingAssetsPath, "StageData_" + stageId + ".json");
//            File.WriteAllText(path, stageData);

//            return stageData;
//        }

//        private void SetTile(ref GridList<Lop.Editor.TileData> gridList, int x, int y, ObjectType type)
//        {
//            Lop.Editor.TileData objectData = new Lop.Editor.TileData
//            {
//                tile = new TestTile((int)type)
//            };
//            gridList[x, y] = objectData; // 인덱스 수정
//        }

//        private void SetItem(ref Lop.Editor.JellyList jellyList, float x, float y, JellyType type)
//        {
//            Lop.Editor.JellyListElement item = new Lop.Editor.JellyListElement();
//            item.pos = new Vector2(x, y);
//            item.type = type;
//            jellyList.jellyList.Add(item);
//        }
//    }
//}