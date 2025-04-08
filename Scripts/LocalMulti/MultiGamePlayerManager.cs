using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lop.Game
{
    public class MultiGamePlayerManager : MonoBehaviour
    {
        public BackGroundScroll BackGroundScroll;
        public BackGroundScroll ForeGroundScroll;

        public MultiAttackCoolManager AttackCoolManager;
        public InGameUIManager GameUIManager;
        public FeverManager FeverManager;
        public StageManager StageManager;
        public ScoreCalculationManager ScoreManager;
        public VibratorManager vibratorManager;

        public CameraShake Shaker;
        public FollowCam FollowCam;

        public Image Img_hitEffect;
        public GameObject Player;

        public float MaxTime = 60;
        public float DamagePercent = 10;
        public float CliffDamagePercent = 15;
        public int Score;

        public bool isClear;

        public Vector2 PenguinSpawnPosition = new Vector2(5f, 20f); // 5,20 || 5,57.5

        private float magellanicDebuffMinValue = 0.02f;
        private float magellanicDebuffAddValue = 0.008f;

        private float kingDestroyBoxOffsetX = 8;
        private Vector2 kingDestroyBoxSize = new Vector2(30, 15);

        private int emperorDestroyBoxOffsetX = 6;

        private float gentooAttackValue = 5;

        [Header("Rival Skill Effect")]
        [SerializeField] private GameObject adelie_Hammer;
        [SerializeField] private GameObject rock_Box;
        [SerializeField] private GameObject em_Spoon;
        [SerializeField] private GameObject ma_Sword;
        [SerializeField] private GameObject gen_Slingshot;
        [SerializeField] private GameObject king_Folk;
        [SerializeField] private GameObject de_Axe;
        [SerializeField] private Transform rivalSpawnPos;
        [SerializeField] private GameObject twinkle;

        // 어택을 받았을 때 함수들
        public void DefaultAttack(int casterID)
        {
            StartCoroutine(SpawnRivalPenguin(de_Axe, casterID));
            //5초간 장애물 크기 1.2배 증가
            StageManager.DefaultAttackTime();
        }
        public void RockhopperAttack(int casterID)
        {
            StartCoroutine(SpawnRivalPenguin(rock_Box, casterID));
            //2.5초간 이동속도 감소 40%
            Player.GetComponent<PenguinBase>().RockhopperAttackToMe();
        }
        public void EmperorAttack(int casterID)
        {
            StartCoroutine(SpawnRivalPenguin(em_Spoon, casterID));
            List<GameObject> list = new List<GameObject>();
            int count = 0;
            int playerX = (int)Player.transform.position.x + emperorDestroyBoxOffsetX;

            List<GameObject> previousObjectsList = StageManager.PreviousObjects.ToList();
            previousObjectsList.Reverse();

            List<GameObject> currentObjectsList = StageManager.CurrentObjects.ToList();
            currentObjectsList.Reverse();

            foreach (GameObject go in previousObjectsList)
            {
                if (go != null && (int)go.transform.position.x == playerX + count)
                {
                    list.Add(go);
                    count++;
                    if (count >= 3) break;
                }
            }
            if (count < 3)
            {
                foreach (GameObject go in currentObjectsList)
                {
                    if (go != null && (int)go.transform.position.x == playerX + count)
                    {
                        list.Add(go);
                        count++;
                        if (count >= 3) break;
                    }
                }
            }

            for (int i =0; i< list.Count; i++)
            {
                Destroy(list[i]);
            }
        }
        public void MagellanicAttack(int casterID)
        {
            StartCoroutine(SpawnRivalPenguin(ma_Sword, casterID));
            if (GameUIManager.plusTimerValueByMagellanicAttack >= magellanicDebuffMinValue) { return; }

            if (GameUIManager.plusTimerValueByMagellanicAttack >= magellanicDebuffAddValue * 2) { GameUIManager.plusTimerValueByMagellanicAttack = magellanicDebuffMinValue;  return; }

            GameUIManager.plusTimerValueByMagellanicAttack += magellanicDebuffAddValue;
        }
        public void GentooAttack()
        {
            // 한 펭귄 빼고 펭귄들의 시간 5초 증가 
            GameUIManager.AddTime(gentooAttackValue);
        }
        public void ShowGentooAttack(int casterID)
        {
            StartCoroutine(SpawnRivalPenguin(gen_Slingshot, casterID));
        }
        public void KingAttack(int casterID)
        {
            StartCoroutine(SpawnRivalPenguin(king_Folk, casterID));
            // 화면 내의 모든 생성 삭제
            Vector2 boxOffset = new Vector2(Player.transform.position.x + kingDestroyBoxOffsetX, Player.transform.position.y);
            Vector2 boxSize = kingDestroyBoxSize;

            Collider2D[] scores = Physics2D.OverlapBoxAll(boxOffset, boxSize, 0, LayerMask.GetMask("Score"));

            for(int i =0; i < scores.Length; i++)
            {
                Destroy(scores[i].gameObject);
            }
        }
        public void AdlieAttack(int casterID)
        {
            StartCoroutine(SpawnRivalPenguin(adelie_Hammer , casterID));
            // 펭귄 1초동안 기절
            Player.GetComponent<PenguinBase>().AdelieAttackToMe();
            StartCoroutine(Co_BackGroundStop());
        }

        private IEnumerator Co_BackGroundStop()
        {
            BackGroundScroll.IsStop = true;
            ForeGroundScroll.IsStop = true;

            yield return new WaitForSeconds(1);

            BackGroundScroll.IsStop = false;
            ForeGroundScroll.IsStop = false;
        }

        private IEnumerator SpawnRivalPenguin(GameObject penguin, int casterID)
        {
            GameObject peng = Instantiate(penguin, rivalSpawnPos);

            peng.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("_OutlineColor", casterID == 1 ? Color.red : Color.green);
            peng.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.SetColor("_OutlineColor", casterID == 1 ? Color.red : Color.green);

            Instantiate(twinkle, peng.transform);

            yield return new WaitForSeconds(1);

            Destroy(peng);
            Destroy(Instantiate(twinkle, rivalSpawnPos), 0.5f);
        }
    }
}