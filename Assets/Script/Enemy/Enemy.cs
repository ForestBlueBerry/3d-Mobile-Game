using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float _maxHp = 50f;
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _attackDistance = 2f;
    [SerializeField] private float _agrodistance = 25f;

    [Header("Visuals")]
    [SerializeField] private Renderer _meshRenderer;
    [SerializeField] private Material _hitMaterial;

    private float _currentHp;
    private Transform _targetCar;
    private bool _isDead;
    private IObjectPool<Enemy> _pool;

    private Material _originalMaterial;
    private float distanceToCar;
    private Animator animator;

    private void Awake()
    {
        if (_meshRenderer == null) _meshRenderer = GetComponentInChildren<Renderer>();
        if (_meshRenderer != null) _originalMaterial = _meshRenderer.sharedMaterial;
        animator = GetComponent<Animator>();
    }

    public void SetPool(IObjectPool<Enemy> pool) => _pool = pool;

    public void Init(Transform targetCar)
    {
        _targetCar = targetCar;
        _currentHp = _maxHp;
        _isDead = false;

        if (_meshRenderer != null) _meshRenderer.material = _originalMaterial;
    }

    private void Update()
    {
        if (_isDead || _targetCar == null) return;

        Vector3 targetPosition = new Vector3(_targetCar.position.x, transform.position.y, _targetCar.position.z);
        distanceToCar = Vector3.Distance(transform.position, targetPosition);
        if (distanceToCar > _agrodistance) return;

        if (distanceToCar > _attackDistance)
        {
            transform.LookAt(targetPosition);
            RunAnim(true);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
        }
        else
        {
            AttackCar();
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHp -= damage;
        PlayHitEffect().Forget();

        if (_currentHp <= 0) Die();
    }

    private async UniTaskVoid PlayHitEffect()
    {
        if (_meshRenderer == null || _hitMaterial == null) return;

        _meshRenderer.material = _hitMaterial;

        await UniTask.Delay(TimeSpan.FromSeconds(0.08f), cancellationToken: this.GetCancellationTokenOnDestroy());

        if (!_isDead && _meshRenderer != null)
        {
            _meshRenderer.material = _originalMaterial;
        }
    }

    private void AttackCar()
    {
        if (_isDead) return;

        if (_targetCar != null && _targetCar.TryGetComponent<IDamageable>(out var vehicle))
        {
            vehicle.TakeDamage(_damage);
        }
        Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        RunAnim(false);
        if (_pool != null)
        {
            _pool.Release(this);
        }
        else {
            gameObject.SetActive(false);
        } 
    }

    private void RunAnim(bool activate)
    {
        animator.SetBool("isRun", activate);
    }
}