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

    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyDepletionRate = 5f;
    [SerializeField] private float energyLogThreshold = 10f; // Логировать каждые N единиц энергии

    private int _currentLane;
    private float _targetZPosition;
    private bool _isSwitchingLanes = false;
    private bool _isGrounded = true;
    private Rigidbody _rb;
    private float _currentEnergy;
    private bool _isOutOfEnergy = false;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private float _lastLoggedEnergy;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.freezeRotation = true;
        }

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        InitializeLaneSystem();
        _currentEnergy = maxEnergy;
        _lastLoggedEnergy = maxEnergy;
        LogEnergyStatus(); // Логируем начальное состояние энергии
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
        _targetZPosition = lanes[_currentLane].position.z;
    }

    private void Update()
    {
        if (_isOutOfEnergy) return;

        HandleInput();
        UpdateEnergy();
    }

    private void FixedUpdate()
    {
        if (_isOutOfEnergy) return;

        HandleLaneSwitching();
        MoveForward();
    }

    private void UpdateEnergy()
    {
        if (_isOutOfEnergy) return;

        float previousEnergy = _currentEnergy;
        _currentEnergy -= energyDepletionRate * Time.deltaTime;

        // Логируем изменение энергии если прошло больше energyLogThreshold
        if (Mathf.Abs(_currentEnergy - _lastLoggedEnergy) >= energyLogThreshold ||
            (_lastLoggedEnergy > 0 && _currentEnergy <= 0))
        {
            LogEnergyStatus();
            _lastLoggedEnergy = _currentEnergy;
        }

        if (_currentEnergy <= 0f)
        {
            _currentEnergy = 0f;
            OutOfEnergy();
        }
    }

    private void LogEnergyStatus()
    {
        float percentage = (_currentEnergy / maxEnergy) * 100f;
        Debug.Log($"Энергия: {_currentEnergy:F1}/{maxEnergy} ({percentage:F0}%)");
    }

    private void OutOfEnergy()
    {
        _isOutOfEnergy = true;
        Debug.LogWarning("Энергия кончилась! Уровень перезапустится через 3 секунды");

        _rb.linearVelocity = Vector3.zero;
        Invoke("RestartLevel", 3f);
    }

    private void RestartLevel()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        _currentEnergy = maxEnergy;
        _lastLoggedEnergy = maxEnergy;
        _isOutOfEnergy = false;

        Debug.Log("Уровень перезапущен. Энергия восстановлена!");
        InitializeLaneSystem();
    }

    /// <summary>
    /// Добавляет указанное количество энергии
    /// </summary>
    /// <param name="amount">Количество добавляемой энергии</param>
    public void AddEnergy(float amount)
    {
        float oldEnergy = _currentEnergy;
        _currentEnergy = Mathf.Clamp(_currentEnergy + amount, 0f, maxEnergy);

        if (Mathf.Abs(_currentEnergy - oldEnergy) > 0.1f) // Логируем только если энергия действительно изменилась
        {
            Debug.Log($"Получено энергии: +{amount:F1}. Теперь энергии: {_currentEnergy:F1}/{maxEnergy}");
            _lastLoggedEnergy = _currentEnergy;
        }
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
            Vector3 targetPosition = new Vector3(
                transform.position.x,
                transform.position.y,
                _targetZPosition
            );

            Vector3 newVelocity = new Vector3(
                _rb.linearVelocity.x,
                _rb.linearVelocity.y,
                (targetPosition - transform.position).normalized.z * laneSwitchSpeed
            );

            _rb.linearVelocity = newVelocity;

            if (Mathf.Abs(transform.position.z - _targetZPosition) <= laneSwitchThreshold)
            {
                _isSwitchingLanes = false;
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _rb.linearVelocity.y, 0);
            }
        }
    }

    private void MoveForward()
    {
        _rb.linearVelocity = new Vector3(runSpeed, _rb.linearVelocity.y, _rb.linearVelocity.z);
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