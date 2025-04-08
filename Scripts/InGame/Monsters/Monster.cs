using UnityEngine;

namespace Lop.Game
{
    // 몬스터 상위 클래스
    public class Monster : MonoBehaviour
    {
        protected Animator animator;
        [SerializeField] private BoxCollider2D triggerCollider;
        [SerializeField] private BoxCollider2D attackCollider;
        [SerializeField] private ParticleSystem particle;
        protected bool isTrigger;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Player") && !isTrigger)
            {
                PlayerInRange();
            }
        }

        private void PlayerInRange()
        {
            isTrigger = true;
            if (triggerCollider != null) triggerCollider.enabled = false;
            Attack();
        }

        public virtual void Attack()
        {
            animator.SetTrigger("Attack");
        }

        public void EnableAttackColliderEvent()
        {
            attackCollider.enabled = true;
            if (particle != null) particle.Play();
        }
    }
}