using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using VContainer;

public class GameStateService : MonoBehaviour
{
    public event Action OnLevelPrepared;

    private IInputService _inputService;
    private ControllerVechicle _vehicle;
    private ControlAutoturret _turret;
    private EnemySpawner _enemySpawner;
    private UIOverlayView _uiOverlayView;

    private bool _isLevelEnded;

    [Inject]
    public void Constructor(
        IInputService inputService,
        ControllerVechicle vehicle,
        ControlAutoturret turret,
        EnemySpawner enemySpawner,
        UIOverlayView uiOverlayView) 
    {
        _inputService = inputService;
        _vehicle = vehicle;
        _turret = turret;
        _enemySpawner = enemySpawner;
        _uiOverlayView = uiOverlayView;
    }

    private void Start()
    {
        if (_vehicle != null)
        {
            _vehicle.OnHpChanged += OnHpChanged;
        }

        CancellationToken token = this.GetCancellationTokenOnDestroy();
        RunGameLoopAsync(token).Forget();
    }

    private void OnDestroy()
    {
        if (_vehicle != null)
        {
            _vehicle.OnHpChanged -= OnHpChanged;
        }
    }

    public async UniTaskVoid RunGameLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            PrepareLevel();

            await UniTask.WaitUntil(() => _inputService.IsTapped, cancellationToken: token);

            Launch();

            await UniTask.WaitUntil(() => _isLevelEnded, cancellationToken: token);

            await UniTask.Yield(PlayerLoopTiming.Update, token);

            await UniTask.WaitUntil(() => _inputService.IsTapped, cancellationToken: token);
        }
    }

    private void PrepareLevel()
    {
        _isLevelEnded = false;

        _vehicle.ResetVehicle();
        _turret.ActivateTurret(false);
        _turret.ReturnRotate(false);

        _enemySpawner.SpawnEnemiesForLevel();
        _uiOverlayView.ShowStartScreen();

        OnLevelPrepared?.Invoke();
    }

    private void Launch()
    {
        _vehicle.ActivateRide(true);
        _turret.ActivateTurret(true);
        _uiOverlayView.ShowGameplay();
    }

    public void OnLevelFinished(bool loseorwin)
    {
        if (_isLevelEnded) return;

        _isLevelEnded = true;

        _vehicle.ActivateRide(false);
        _turret.ActivateTurret(false);
        _turret.ReturnRotate(false);
        _enemySpawner.ClearActiveEnemies();
        WinOrLose(loseorwin);
    }

    private void WinOrLose(bool isWin)
    {
        if (isWin)
        {
            _uiOverlayView.ShowWin();
        }
        else
        {
            _uiOverlayView.ShowLose();
        }
    }

    private void OnHpChanged(float currentHp, float maxHp)
    {
        _uiOverlayView.UpdateHP(currentHp, maxHp);

        if (currentHp <= 0 && !_isLevelEnded)
        {
            OnLevelFinished(false);
        }
    }
}