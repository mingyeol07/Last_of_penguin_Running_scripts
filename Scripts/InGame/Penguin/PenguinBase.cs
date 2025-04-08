using GoogleMobileAds.Api;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.Rendering.DebugUI;


namespace Lop.Game
{
    public class KeyType
    {
        public const string None = "";

        public const string Keyboard = "Keyboard";
        public const string Keyboard_1 = "Keyboard_1";
        public const string Keyboard_2 = "Keyboard_2";

        public const string PS = "PS";
        public const string PS_1 = "PS_1";
        public const string PS_2 = "PS_2";

        public const string Xbox = "Xbox";
        public const string Xbox_1 = "Xbox_1";
        public const string Xbox_2 = "Xbox_2";
    }
    public enum PenguinName
    {
        Default,
        Rockhopper,
        Emperor,
        Magellanic,
        Gentoo,
        King,
        Adlie,
    }

    public class PenguinBase : MonoBehaviour
    {
        protected PenguinName myPenguinName;
        public PenguinName MyPenguinName => myPenguinName;

        private IState currentState;

        #region strings
        private const string obstacleTag = "Obstacle";
        private const string cliffTag = "Cliff";
        private const string clearFlagTag = "ClearFlag";
        private const string groundTag = "Ground";

        private readonly int hashFlyAway = UnityEngine.Animator.StringToHash("FlyAway");
        private readonly int hashSlide = UnityEngine.Animator.StringToHash("isSliding");
        private readonly int hashGroundTrig = UnityEngine.Animator.StringToHash("isGroundTrig");
        private readonly int hashJumpTrig = UnityEngine.Animator.StringToHash("doJump");
        private readonly int hashDie = UnityEngine.Animator.StringToHash("isDie");

        private const string meshAlpha = "_Alpha";
        #endregion

        #region Component Variables
        protected Rigidbody2D rigid;
        protected ConstantForce2D constantForce2D;
        private Animator animator;
        public Animator Animator => animator;

        [SerializeField] private BoxCollider2D standingCollider;
        [SerializeField] private BoxCollider2D slideCollider;

        [SerializeField] private Collider2D body_standingCollider;
        [SerializeField] private Collider2D body_slideCollider;

        [SerializeField] private GameObject feverEffect;
        [SerializeField] private SkinnedMeshRenderer meshRenderer;

        [SerializeField] private TMP_Text txt_timeEvent;

        [SerializeField] private PlayerInput playerInput;
        private Animator timeAnimator;
        #endregion

        #region Player Stats
        protected float gravitationalAcceleration = 9.8f + 9.8f + 9.8f + 9.8f + 9.8f;
        // 29.4 => 49;
        // 49 => 68.6

        // 13.72 => 17.64 
        // 9.8 * 1.4 => 9.8 * 1.8
        // => 9.8 * 2.2
        private float jumpPower = 9.8f * 1.9f;
        private int jumpCount = 1;
        public int JumpCount => jumpCount;  
        protected int maxJumpCount = 1;
        public int MaxJumpCount => maxJumpCount;

        private Queue jumpBuffer = new Queue();
        public Queue JumpBuffer => jumpBuffer;

        private float invincibilityTime = 1.5f;

        // 6=>8
        // 8=>10
        protected float moveSpeed = 8f;
        private float defaultSpeed;
        private float feverSpeed = 12;

        private const int emperorDestroyObstacleScore = 3000;
        private const float groundDetectedOffset = 0.5f;

        protected bool isDie;
        public bool IsDie => isDie;

        private bool isClear;

        private Vector3 DropReturnPosition;
        #endregion

        #region Input Variables
        private bool isInputSlide;
        public bool IsInputSlide => isInputSlide;

        private bool isInputJump;
        public bool IsInputJump { get => isInputJump; set => isInputJump = value; }

        private bool isGround;
        public bool IsGround => isGround;

        private bool isJump;
        public bool IsJump => isJump;

        protected bool isFever;
        public bool IsFever=> isFever;

        private bool isFall;
        public bool IsFall => isFall;

        private bool isInvincibility;
        public bool IsInvincibility => isInvincibility;
        #endregion

        #region Multi
        /// <summary>
        /// 플레이어가 멀티게임에서 몇번째 플레이어인지 저장, 혼자라면 0
        /// </summary>
        private int playerNumber;
        public int PlayerNumber => playerNumber;

        private bool isStun;
        private float rockhopperAttackTime;
        private const float defaultRockhopperAttackTime = 2.5f;
        private const float defaultRockhopperDownSpeedValue = 0.6f; // * moveSpeed;
        private bool isPenalty;
        public bool IsPenalty => isPenalty;

        [SerializeField] private ParticleSystem slowEffect;
        [SerializeField] private ParticleSystem stunEffect;
        #endregion

        protected GameManagerParent gameManager;

        private void Awake()
        {
            timeAnimator = txt_timeEvent.transform.GetComponent<Animator>();    
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            constantForce2D = GetComponent<ConstantForce2D>();
        }

        protected virtual void Start()
        {
            //if(playerNumber != 0)
            //{
            //    혼자할때 기본값
            //    moveSpeed = 11f;
            //    gravitationalAcceleration = 68.6f + 9.8f + 9.8f;
            //    jumpPower = 9.8f * 2.7f;
            //    feverSpeed = 18;
            //}

            constantForce2D.force = new Vector2(0, -gravitationalAcceleration);
            defaultSpeed = moveSpeed;
            isFall = false;

            SetState(new RunState());

            gameManager = GameManager.Instance != null ? GameManager.Instance : MultiGameManager.Instance;
            StartCoroutine(Co_SetLocationRecord());
        }

        protected virtual void Update()
        {
            // 뉴 인풋 사용시 Inputs함수 안함
            //Inputs();

            currentState?.Update();
            UpdateAnimatorParameters();

            // multi
            if(playerNumber != 0)
            {
                if (rockhopperAttackTime > 0)
                {
                    rockhopperAttackTime -= Time.deltaTime;
                }
                else if (rockhopperAttackTime <= 0 && moveSpeed != defaultSpeed)
                {
                    moveSpeed = defaultSpeed;
                    slowEffect.gameObject.SetActive(false);
                }
            }
        }

        private void FixedUpdate()
        {
            if (isDie || isClear || isStun) return;

            float speed = isFever ? feverSpeed : moveSpeed;
            rigid.velocity = new Vector2(speed, rigid.velocity.y);
        }

        public void SetState(IState nextState) // FSM 상태 지정
        {
            currentState?.OnExit();

            currentState = nextState;

            currentState.OnEnter(this);
        }

        private IEnumerator Co_JumpBuffer()
        {
            jumpBuffer.Enqueue(0);
            yield return new WaitForSeconds(0.2f);
            if(jumpBuffer.Count > 0) jumpBuffer.Dequeue();
        }
        public void OnJump(InputValue value) {

            if (isDie || isClear || isStun) return;

            isInputJump = value.isPressed;
        }

        public void OnSlide(InputValue value) {
            if (isDie || isClear || isStun) return;

            isInputSlide = value.isPressed;
        }

        public void OnAttack(InputValue value) {
            if (playerNumber == 0) return;
            if (isDie || isClear) return;

            GameManagerParent.Instance.GetComponent<MultiGameManager>().Attack(myPenguinName, playerNumber);
        }

        public void Jump()
        {
            if (jumpCount > 0)
            {
                SoundManager.Instance.Play_PlayerJumpSound();
                jumpCount--;
                StartCoroutine(Co_JumpedCheck());
                isGround = false;
                isInputJump = false;
                jumpBuffer.Clear();

                rigid.velocity = Vector2.zero;
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

                animator.ResetTrigger(hashGroundTrig);
                animator.SetTrigger(hashJumpTrig);
            }
        }

        private IEnumerator Co_JumpedCheck()
        {
            isJump = true;
            yield return new WaitForSeconds(0.1f);
            isJump = false;
        }

        // 슬라이드할때 아닐때 콜라이더 변경
        #region Collider Set
        public void SwitchColliderToSlide()
        {
            standingCollider.enabled = false;
            body_standingCollider.enabled = false;

            slideCollider.enabled = true;
            body_slideCollider.enabled = true;
        }

        public void SwitchColliderToStand()
        {
            slideCollider.enabled = false;
            body_slideCollider.enabled = false;

            standingCollider.enabled = true;
            body_standingCollider.enabled = true;
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(obstacleTag))
            {
                if(isFever)
                {
                    if(collision.TryGetComponent(out Animator animator))
                    {
                        animator.SetTrigger(hashFlyAway);
                    }
                    else
                    {
                        //animator.transform.parent.parent.GetComponent<Animator>().SetTrigger("FlyAway");
                    }

                    if (myPenguinName == PenguinName.Emperor)
                    {
                        gameManager.GetScore(ScoreType.None, emperorDestroyObstacleScore, playerNumber);
                    }
                }
                else if(!isInvincibility)
                {
                    gameManager.PlayHit(playerNumber);
                    StartCoroutine(Co_InvincibilityFade());
                }
            }
            else if(collision.gameObject.CompareTag(cliffTag))
            {
                if(!isFever && !isInvincibility)
                {
                    gameManager.PlayFall(playerNumber);
                    isGround = false;
                    StartCoroutine(Co_InvincibilityFade());
                }

                if (isDie)
                {
                    rigid.velocity = Vector2.zero;
                    constantForce2D.force = Vector3.zero;
                }
                else
                {
                    //ContactGround();
                    jumpCount = 1;
                    rigid.velocity = Vector2.zero;

                    if (!isFall)
                    {
                        StartCoroutine(Co_RestorePlayerPosition());
                        StartCoroutine(Co_InvincibilityFade());
                    }
                    

                    //rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                }
            }
            //else if(collision.gameObject.CompareTag("Teleport"))
            //{
            //    transform.position = new Vector3(transform.position.x, transform.position.y + 25, transform.position.z);
            //}
            else if (collision.gameObject.CompareTag(clearFlagTag) && !isClear)
            {
                isClear = true;
                StartCoroutine(Co_ClearWait());

                //if (playerNumber == 0 && GameManager.Instance.IsEndlessMode) return;

                gameManager.GameClear(playerNumber);
            }
            //else if(collision.gameObject.CompareTag("MonsterTrigger"))
            //{
            //    if(collision.transform.parent.transform.parent.TryGetComponent<Monster>(out var monster))
            //    {
            //        monster.Attack();
            //    }
            //}
        }

        // 펭귄이 떨어졌을 때를 감지하여 이동
        IEnumerator Co_SetLocationRecord()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, LayerMask.GetMask("Ground"));

            if (hit.collider != null)
            {
                DropReturnPosition = transform.position;
                DropReturnPosition.x -= 2;
                DropReturnPosition.y += 1;
            }

            yield return new WaitForSeconds(0.2f);

            StartCoroutine(Co_SetLocationRecord());
        }

        // 떨어졌을 때 플레이어 포지션을 뒤로 이동시키는 함수
        IEnumerator Co_RestorePlayerPosition()
        {
            isFall = true;
            yield return new WaitForSeconds(0.6f);

            Vector3 DropPosition = DropReturnPosition;
            gameManager.SetBackgroundMove(true);

            float Duration = 0.8f;
            float Elapsed = 0;

            for (int i = 0; i < 4; i++)
            {
                while (Elapsed < Duration)
                {
                    transform.position = DropPosition;

                    rigid.velocity = Vector3.zero;

                    Elapsed += Time.deltaTime; // 시간을 증가시키며 선형 보간
                    yield return null; // 매 프레임마다 업데이트
                }

                yield return null;
            }
            isFall = false;
            gameManager.SetBackgroundMove(false);
        }

        // 펭귄이 타일 콜라이더 사이사이로 빠지는 버그가 있음.

        // TODO: Collision Enter 와 Stay를 썼을 때를 각각 비교해보기
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Collider2D hitCollider = collision.contacts[0].collider;

            if (myPenguinName == PenguinName.Default)
            {
                if (hitCollider.gameObject.CompareTag(groundTag) && jumpCount == 0 && hitCollider.transform.position.y + groundDetectedOffset <= transform.position.y)
                {
                    ContactGround();
                }
            }

            if (hitCollider.gameObject.CompareTag(groundTag) && !isGround && !isJump && hitCollider.transform.position.y + groundDetectedOffset <= transform.position.y)
            {
                ContactGround();
            }
        }

        //private void OnCollisionStay2D(Collision2D collision)
        //{
        //    Collider2D hitCollider = collision.contacts[0].collider;

        //    if(myPenguinName == PenguinName.Default )
        //    {
        //        if (hitCollider.gameObject.CompareTag(groundTag) && jumpCount == 0 && hitCollider.transform.position.y + groundDetectedOffset <= transform.position.y)
        //        {
        //            ContactGround();
        //        }
        //    }

        //    if (hitCollider.gameObject.CompareTag(groundTag) && !isGround && !isJump && hitCollider.transform.position.y + groundDetectedOffset <= transform.position.y)
        //    {
        //        ContactGround();
        //    }
        //}

        public void ContactGround()
        {
            isGround = true;
            jumpCount = maxJumpCount;

            animator.SetTrigger(hashGroundTrig);
        }

        // 골인지점에 닿았을 때 조금 잇다가 멈추는 연출
        private IEnumerator Co_ClearWait() {
            yield return new WaitForSeconds(0.3f);

            rigid.velocity = Vector2.zero;
            animator.SetTrigger("isClear");
        }

        /// <summary>
        /// 플레이어가 데미지를 받았을 때 투명해지는 함수
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Co_InvincibilityFade()
        {
            float time = 0;
            float lerpDuration = 0.2f;
            float startAlpha = 1.0f;
            float targetAlpha = 0.2f;

            isInvincibility = true;

            while (time < invincibilityTime)
            {
                float lerpTime = 0;

                while (lerpTime < lerpDuration)
                {
                    // 알파 값을 부드럽게 변화시키기
                    float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, lerpTime / lerpDuration);
                    meshRenderer.material.SetFloat(meshAlpha, currentAlpha);

                    lerpTime += Time.deltaTime; // 시간을 증가시키며 선형 보간
                    yield return null; // 매 프레임마다 업데이트
                }

                // 알파 값을 반전시킴 (0.5 -> 1 또는 1 -> 0.5)
                float temp = startAlpha;
                startAlpha = targetAlpha;
                targetAlpha = temp;

                time += lerpDuration; // 무적 시간 업데이트
            }

            meshRenderer.material.SetFloat(meshAlpha, 1);
            isInvincibility = false;
        }

        public void UpdateAnimatorParameters()
        {
            //animator.SetBool("isJump", isJump);
            animator.SetBool(hashSlide, isInputSlide && isGround);
            animator.SetBool(hashDie, isDie);
        }

        public virtual void StartFever()
        {
            feverEffect.SetActive(true);
            isFever = true;
        }

        public virtual void StopFever()
        {
            feverEffect.SetActive(false);
            isFever = false;
        }

        public void GetTimeEvent(float value)
        {
            txt_timeEvent.text = $"[+{value.ToString("F1")}]";
            timeAnimator.SetTrigger("Play");
        }

        public void GameoverAction()
        {
            rigid.velocity = Vector2.zero;
            isDie = true;
            SetState(new DieState());
        }

        #region Local Multi
        public void SetMultiPenguin(int playerNumber)
        {
            this.playerNumber = playerNumber;
        }
        public void MultiRevive()
        {
            // 멀티게임에서 지스타 시연용 부활

            //isDie = false;
            //isPenalty = true;

            //constantForce2D.force = new Vector2(0, -gravitationalAcceleration);
            //rigid.velocity = Vector2.up * 20;

            //StartCoroutine(Co_InvincibilityFade());
        }
        public void AdelieAttackToMe()
        {
            if (isStun) return;

            StartCoroutine(Co_Stun());
        }

        public void RockhopperAttackToMe()
        {
            rockhopperAttackTime = defaultRockhopperAttackTime;

            slowEffect.gameObject.SetActive(true);
            slowEffect.Play();

            moveSpeed *= defaultRockhopperDownSpeedValue;
        }

        private IEnumerator Co_Stun()
        {
            isStun = true;
            stunEffect.gameObject.SetActive(true);

            rigid.velocity = Vector2.zero;
            yield return new WaitForSeconds(1f);

            isStun = false;
            stunEffect.gameObject.SetActive(false);
        }
        #endregion
    }
}