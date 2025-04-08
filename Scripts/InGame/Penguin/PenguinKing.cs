using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lop.Game
{
    public class PenguinKing : PenguinBase
    {
        // 최대 시간 늘어나는 크기
        private float plusTime = 10;
        public float PlusTime => plusTime;

        // 앞의 장애물들이 생선이 됨
        // 앞의 장애물들을 감지하는 박스 크기
        private Vector2 checkBoxPoint = new Vector2(7, 4);
        private Vector2 CheckBoxSize = new Vector2(2, 13);

        [Tooltip("피버 때 장애물이 변하는 피쉬의 오브젝트")]
        [SerializeField] private GameObject fishOnFever;

        protected override void Start()
        {
            myPenguinName = PenguinName.King;

            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            
            // 피버 때 박스크기만큼 앞의 장애물을 찾아내고 생선 소환
            if(isFever)
            {
                Collider2D obstacleTop = Physics2D.OverlapBox((Vector2)transform.position + checkBoxPoint, CheckBoxSize, 0, LayerMask.GetMask("Obstacle_Top"));
                if (obstacleTop != null)
                {
                    Destroy(obstacleTop.gameObject);
                    for(int i = 0; i< 5; i++)
                    {
                        SpawnFishWithRandom(obstacleTop.transform.position);
                    }
                }

                Collider2D obstacleBot = Physics2D.OverlapBox((Vector2)transform.position + checkBoxPoint, CheckBoxSize, 0, LayerMask.GetMask("Obstacle_Bottom"));
                if (obstacleBot != null)
                {
                    Destroy(obstacleBot.gameObject);
                    for(int i = 0; i < 3; i++)
                    {
                        SpawnFishWithRandom(obstacleBot.transform.position);
                    }
                }
            }
        }

        private void SpawnFishWithRandom(Vector2 position)
        {
            float ranX = Random.Range(-1f, 1f);
            float ranY = Random.Range(-1f, 1f);

            Instantiate(fishOnFever, new Vector2(position.x + ranX, position.y + ranY), Quaternion.identity);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position + checkBoxPoint, CheckBoxSize);
        }
    }
}
