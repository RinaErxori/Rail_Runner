using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class SmartLaneRunner : MonoBehaviour
{
    [Header("Track Settings")]
    [Tooltip("Добавьте дорожки в порядке сверху вниз (0 - верхняя)")]
    [SerializeField] private Transform[] lanes;

    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float laneSwitchSpeed = 12f;
    [SerializeField] private float laneSwitchThreshold = 0.05f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyDepletionRate = 5f;
    [SerializeField] private float energyLogThreshold = 10f;

    [Header("Obstacle Settings")]
    [SerializeField] private string obstacleTag = "Obstacle";

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private float effectDuration = 2f;

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
    private bool _gameOverDisplayed = false; // Флаг для отображения менюшки один раз

    public event Action OnGameOver;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.freezeRotation = true;
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator не найден на объекте игрока!");
            enabled = false;
            return;
        }
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        InitializeLaneSystem();
        _currentEnergy = maxEnergy;
        _lastLoggedEnergy = maxEnergy;
        LogEnergyStatus();
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
        SetAnimationState("Run");
    }

    private void Update()
    {
        if (_isOutOfEnergy) return;

        HandleInput();
        UpdateEnergy();
        UpdateAnimation();
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
        Debug.LogWarning("Энергия кончилась! Игра приостановлена.");
        TriggerGameOver();
    }

    public void KillPlayer()
    {
        if (_isOutOfEnergy) return;
        _isOutOfEnergy = true;
        Debug.LogWarning("Игрок столкнулся с препятствием! Игра приостановлена.");
        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        _isOutOfEnergy = true;
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
        }

        SetAnimationState("Fall");
        PlayDeathEffect();

        StartCoroutine(HandleGameOverSequence());
    }

    private IEnumerator HandleGameOverSequence()
    {
        float fallDuration = 0f;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Fall"))
        {
            fallDuration = stateInfo.length;
            Debug.Log($"Анимация Fall проигрывается, длительность: {fallDuration} сек.");
        }
        else
        {
            Debug.LogWarning("Анимация Fall не запущена! Проверьте Animator Controller.");
        }

        float waitDuration = Mathf.Max(fallDuration, effectDuration);
        yield return new WaitForSecondsRealtime(waitDuration);

        if (!_gameOverDisplayed)
        {
            _gameOverDisplayed = true;
            Time.timeScale = 0f;
            OnGameOver?.Invoke();
            Debug.Log("Менюшка проигрыша отображена.");
        }
    }

    private void UpdateAnimation()
    {
        if (_isOutOfEnergy) return;

        animator.SetFloat("speedMultiplier", runSpeed / 12f);
        if (!_isGrounded)
        {
            SetAnimationState("Jump");
        }
        else if (_rb.linearVelocity.x > 0)
        {
            SetAnimationState("Run");
        }
    }

    private void SetAnimationState(string state)
    {
        if (animator != null)
        {
            animator.SetBool("isJumping", state == "Jump");
            animator.SetBool("isFalling", state == "Fall");
            animator.SetBool("isRunning", state == "Run");
            Debug.Log($"Установлено состояние анимации: {state}");
        }
        else
        {
            Debug.LogError("Animator отсутствует при попытке установить состояние!");
        }
    }

    private void PlayDeathEffect()
    {
        if (deathEffect != null)
        {
            ParticleSystem effectInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
            effectInstance.Play();
            Debug.Log("Эффект смерти воспроизведен.");
            Destroy(effectInstance.gameObject, effectDuration);
        }
        else
        {
            Debug.LogWarning("Эффект смерти (deathEffect) не назначен в инспекторе!");
        }
    }

    public void RestartLevel()
    {
        // Полная перезагрузка сцены
        Time.timeScale = 1f; // Снимаем паузу перед перезагрузкой
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _gameOverDisplayed = false; // Сбрасываем флаг для следующего раза
        Debug.Log("Сцена перезапущена.");
    }

    public void AddEnergy(float amount)
    {
        float oldEnergy = _currentEnergy;
        _currentEnergy = Mathf.Clamp(_currentEnergy + amount, 0f, maxEnergy);

        if (Mathf.Abs(_currentEnergy - oldEnergy) > 0.1f)
        {
            Debug.Log($"Получено энергии: +{amount:F1}. Теперь энергии: {_currentEnergy:F1}/{maxEnergy}");
            _lastLoggedEnergy = _currentEnergy;
        }
    }

    public float GetCurrentEnergy()
    {
        return _currentEnergy;
    }

    public float GetMaxEnergy()
    {
        return maxEnergy;
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
        else if (collision.gameObject.CompareTag(obstacleTag))
        {
            Debug.Log($"Столкновение с препятствием! Тег: {collision.gameObject.tag}");
            KillPlayer();
        }
    }
}