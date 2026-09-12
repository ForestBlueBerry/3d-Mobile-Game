using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class ControlAutoturret : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float speedRotation = 10f;

    [Header("Shooting Settings")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private float _downwardAngle = 3f;

    private IInputService _inputService;
    private IObjectPool<Bullet> _bulletPool;
    private float _currentangle;
    private bool activate;
    private CancellationTokenSource _shootCts;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    private void Awake()
    {
        _bulletPool = new ObjectPool<Bullet>(
           createFunc: () =>
           {
               Quaternion startRot = _firePoint != null ? _firePoint.rotation * Quaternion.Euler(_downwardAngle, 0f, 0f) : Quaternion.identity;
               Vector3 startPos = _firePoint != null ? _firePoint.position : Vector3.zero;
               var bullet = Instantiate(_bulletPrefab, startPos, startRot);
               bullet.gameObject.SetActive(false);

               bullet.SetPool(_bulletPool);
               return bullet;
           },
            actionOnGet: b =>
            {
                if (_firePoint != null)
                {
                    Quaternion bulletRotation = _firePoint.rotation * Quaternion.Euler(_downwardAngle, 0f, 0f);
                    b.transform.SetPositionAndRotation(_firePoint.position, bulletRotation);
                }
                b.gameObject.SetActive(true);
            },
            actionOnRelease: b => b.gameObject.SetActive(false),
            actionOnDestroy: b => Destroy(b.gameObject),
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    private void Start()
    {
        _currentangle = transform.localEulerAngles.y;
    }

    private void Update()
    {
        RotateTurret();
    }

    public void ActivateTurret(bool _activate)
    {
        if (activate == _activate) return;
        
        activate = _activate;

        _shootCts?.Cancel();
        _shootCts?.Dispose();
        _shootCts = null;

        if (activate)
        {
            _shootCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            ShootLoopAsync(_shootCts.Token).Forget();
        }
    }

    private void RotateTurret()
    {
        if (activate && _inputService != null && _inputService.IsPressed)
        {
            _currentangle += _inputService.TouchDelta.x * speedRotation * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0, _currentangle, 0);
        }
       
    }

    private async UniTaskVoid ShootLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && activate)
        {
            if (_bulletPrefab != null && _firePoint != null)
            {
                _bulletPool.Get(); 
            }

            bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(_fireRate), cancellationToken: token)
                .SuppressCancellationThrow();

            if (isCanceled) break;
        }
    }

    private void OnDestroy()
    {
        _shootCts?.Cancel();
        _shootCts?.Dispose();
    }

    public void ReturnRotate(bool _activate)
    {
        ActivateTurret(_activate);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}