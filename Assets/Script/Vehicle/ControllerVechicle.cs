using System;
using UnityEngine;

public class ControllerVechicle : MonoBehaviour, IDamageable
{
    [Header("Vehicle Settings")]
    [SerializeField] private float _maxHp = 30f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _acceleration = 2f;

    private float _currentHp;
    private float _currentSpeed;
    private bool _activateRide;
    private bool _isDead;

    public float MaxHp => _maxHp;
    public float CurrentHp => _currentHp;
    public bool IsDead => _isDead;
    public float CurrentSpeed => _currentSpeed;
    public event Action<float, float> OnHpChanged;

    private Vector3 startPosition = new Vector3(-4.95781f, 0.18f, -2.93687f);

    private void Start()
    {
        ResetVehicle();
    }

    private void Update()
    {
        if (!_activateRide || _isDead) return;

        _currentSpeed = Mathf.Lerp(_currentSpeed, _speed, _acceleration * Time.deltaTime);
        transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime, Space.World);
    }

    public void ActivateRide(bool isRide)
    {
        _activateRide = isRide;
        if (!isRide) _currentSpeed = 0f;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHp -= damage;
        _currentHp = Mathf.Max(0, _currentHp);

        OnHpChanged?.Invoke(_currentHp, _maxHp);
        _currentSpeed = Mathf.Max(0f, _currentSpeed - 2.5f);
        if (_currentHp <= 0)
        {
            _isDead = true;
            _activateRide = false;
        }
    }
    public void ResetVehicle()
    {
        _currentHp = _maxHp;
        _currentSpeed = 0f;
        _isDead = false;
        _activateRide = false;
        transform.position = startPosition;
        OnHpChanged?.Invoke(_currentHp, _maxHp);
    }
}