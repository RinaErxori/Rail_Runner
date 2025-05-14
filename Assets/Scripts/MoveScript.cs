using UnityEngine;

public class SmartLaneRunner : MonoBehaviour
{
    [Header("Track Settings")]
    [Tooltip("Добавьте дорожки в порядке сверху вниз (0 - верхняя)")]
    [SerializeField] private Transform[] lanes;

    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float laneSwitchSpeed = 12f;
    [SerializeField] private float laneSwitchThreshold = 0.05f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;

    private int _currentLane;
    private float _xPosition;
    private float _targetZPosition;
    private bool _isSwitchingLanes = false;
    private bool _isGrounded = true;
    private Rigidbody _rb;

    private void Awake()
    {
        // Настройка компонента Rigidbody
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            _rb.isKinematic = false; // Изменено на false, чтобы использовать физику
            _rb.useGravity = true;   // Включаем гравитацию
            _rb.freezeRotation = true; // Запрещаем вращение
        }

        InitializeLaneSystem();
    }

    private void InitializeLaneSystem()
    {
        if (lanes == null || lanes.Length < 2)
        {
            Debug.LogError("Нужно минимум 2 дорожки!");
            enabled = false;
            return;
        }

        _currentLane = lanes.Length / 2;
        _xPosition = transform.position.x;
        _targetZPosition = lanes[_currentLane].position.z;
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleLaneSwitching();
        MoveForward();
    }

    private void HandleInput()
    {
        if (!_isSwitchingLanes)
        {
            if (Input.GetKeyDown(KeyCode.W) && _currentLane > 0)
            {
                StartLaneSwitch(_currentLane - 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) && _currentLane < lanes.Length - 1)
            {
                StartLaneSwitch(_currentLane + 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            Jump();
        }
    }

    private void StartLaneSwitch(int newLane)
    {
        _currentLane = newLane;
        _targetZPosition = lanes[newLane].position.z;
        _isSwitchingLanes = true;
    }

    private void HandleLaneSwitching()
    {
        if (_isSwitchingLanes)
        {
            // Плавное перемещение между дорожками
            float newZ = Mathf.Lerp(transform.position.z, _targetZPosition,
                                 laneSwitchSpeed * Time.fixedDeltaTime);

            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                newZ
            );

            if (Mathf.Abs(transform.position.z - _targetZPosition) <= laneSwitchThreshold)
            {
                _isSwitchingLanes = false;
            }
        }
    }

    private void MoveForward()
    {
        _xPosition += runSpeed * Time.fixedDeltaTime;

        // Плавное перемещение вперед
        float newX = Mathf.Lerp(transform.position.x, _xPosition, 10f * Time.fixedDeltaTime);

        transform.position = new Vector3(
            newX,
            transform.position.y,
            transform.position.z
        );
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        _isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            _isGrounded = true;
        }
    }
}
