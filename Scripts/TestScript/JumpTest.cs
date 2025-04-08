using UnityEngine;

public class JumpTest : MonoBehaviour
{
    public float jumpForce = 5f; // 점프할 때의 힘
    public float gravityScale = 2f; // 중력 스케일
    public float horizontalSpeed = 2f; // 수평 이동 속도
    public float jumpDistance = 1f; // 점프할 거리 (1의 배수로 이동할 거리)
    private Rigidbody2D rb;
    private bool isGrounded = true; // 펭귄이 바닥에 있는지 확인하는 변수
    [SerializeField] private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        // 수평 속도는 계속 유지하여 오른쪽으로 이동
        rb.velocity = new Vector2(horizontalSpeed, rb.velocity.y);

        // 스페이스바를 눌렀을 때 점프 시작
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        // 수직 속도만 점프 높이에 맞게 변경
        anim?.SetTrigger("doJump");
        rb.velocity = new Vector2(rb.velocity.x, CalculateJumpVelocity());
        isGrounded = false; // 점프 중으로 변경
    }

    private float CalculateJumpVelocity()
    {
        // 주어진 중력과 점프 거리에 맞춰 수직 속도를 계산하여 착지 지점이 정확히 맞도록 조정
        return Mathf.Sqrt(2 * jumpDistance * Mathf.Abs(Physics2D.gravity.y) * gravityScale);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 착지 확인 (바닥과 충돌 시)
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim?.SetTrigger("isGroundTrig");
        }
    }
}