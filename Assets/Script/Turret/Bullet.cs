using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 45f;
    [SerializeField] private float _lifeTime = 2f;
    [SerializeField] private float _damage = 10f;

    private IObjectPool<Bullet> _pool;
    private float _timer;
    private bool _isReleased;

    public void SetPool(IObjectPool<Bullet> pool)
    {
        _pool = pool;
    }

    private void OnEnable()
    {
        _timer = _lifeTime;
        _isReleased = false;
    }

    private void Update()
    {
        if (_isReleased) return;

        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            ReleaseToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isReleased) return;

        if (other.TryGetComponent<IDamageable>(out var enemy))
        {
            enemy.TakeDamage(_damage);
            ReleaseToPool();
            return;
        }

        if (other.isTrigger) return;

        ReleaseToPool();
    }

    private void ReleaseToPool()
    {
        if (_isReleased) return;
        _isReleased = true;

        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}