using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public const float MoveSpeed = 10f;        // 이동 속도
    public const float FlySpeed = 20f;        // 비행 속도
    public const float AscentSpeed = 5f;       // 상승/하강 속도
    public const float RotationSpeed = 100f;   // 회전 속도 (선택 사항)
    public float gravityScale = 1f;            // 비행 중 중력 스케일 (필요에 따라 조절)
    public const float GrvScl = 9.81f;         // 중력 가속도

    private Rigidbody rb;
    private bool isFlying = false;       // 현재 비행 상태인지 여부

    private Vector3 currentVelocity = Vector3.zero; // 현재 속도 (SmoothDamp 용)
    public float smoothTime = 0.1f;

    public GameObject LeftWingFold;
    public GameObject RightWingFold;
    public GameObject LeftWingUnfold;
    public GameObject RightWingUnfold;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody 컴포넌트가 캐릭터에 없습니다!");
            enabled = false; // 스크립트 비활성화
        }
    }

    void Update()
    {
        // 비행 시작/종료 입력 감지
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlying = !isFlying;
            rb.useGravity = !isFlying; // 비행 상태에 따라 중력 활성화/비활성화
            if (!isFlying)
            {
                rb.linearVelocity = Vector3.zero; // 착륙 시 속도 초기화 (선택 사항)
            }
        }
    }

    void FixedUpdate()
    {
        if (isFlying)
        {
            LeftWingUnfold.gameObject.SetActive(true);
            RightWingUnfold.gameObject.SetActive(true);
            LeftWingFold.gameObject.SetActive(false);
            RightWingFold.gameObject.SetActive(false);
            rb.useGravity = false;
            gravityScale = 0.5f;
            // 공중 이동 로직
            HandleFlyingMovement();
        }
        else
        {
            LeftWingUnfold.gameObject.SetActive(false);
            RightWingUnfold.gameObject.SetActive(false);
            LeftWingFold.gameObject.SetActive(true);
            RightWingFold.gameObject.SetActive(true);
            rb.useGravity = true;
            gravityScale = 1f;
            // 지상 이동 로직
            HandleMovement();
        }
    }

    void HandleFlyingMovement()
    {
        // 입력 값 받기
        float horizontalInput = Input.GetAxis("Horizontal"); // A, D, Left, Right
        float verticalInput = Input.GetAxis("Vertical");     // W, S, Up, Down
        float ascentInput = 0f;

        if (Input.GetKey(KeyCode.LeftShift)) // 상승 (Left Shift 키)
        {
            ascentInput = 1f;
        }
        if (Input.GetKey(KeyCode.LeftControl)) // 하강 (Left Control 키)
        {
            ascentInput = -1f;
        }

        if (Input.GetKey(KeyCode.RightShift))
        {
            ascentInput = AscentSpeed;
        }

        if (Input.GetKey(KeyCode.RightControl))
        {
            ascentInput = -AscentSpeed;
        }

        // 이동 방향 벡터 계산
        Vector3 moveDirection = new Vector3(horizontalInput, ascentInput, verticalInput).normalized;

        // 이동 속도 적용
        //Vector3 moveVelocity = moveDirection * moveSpeed;
        Vector3 targetVelocity = moveDirection * FlySpeed;
        
        // Rigidbody 에 속도 적용
        //rb.linearVelocity = transform.TransformDirection(moveVelocity); // 로컬 좌표계를 월드 좌표계로 변환
        rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, transform.TransformDirection(targetVelocity), ref currentVelocity, smoothTime);

        // 회전 (선택 사항 - 마우스 입력으로 Yaw 회전)
        if (Input.GetMouseButton(1)) // 마우스 오른쪽 버튼 클릭 시 회전
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, mouseX * RotationSpeed * Time.deltaTime);
        }

        // 중력 스케일 적용
        if (gravityScale > 0f) // 현실감을 위해 약하게 중력 적용
        {
            rb.AddForce(Vector3.down * gravityScale * GrvScl, ForceMode.Acceleration);
        }
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // 이동 방향 벡터 계산
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        Vector3 moveVelocity = moveDirection * MoveSpeed;
        rb.linearVelocity = transform.TransformDirection(moveVelocity); // 로컬 좌표계를 월드 좌표계로 변환
        // 중력 스케일 적용
        if (gravityScale > 0f)
        {
            rb.AddForce(Vector3.down * gravityScale * GrvScl, ForceMode.Acceleration);
        }
    }
}