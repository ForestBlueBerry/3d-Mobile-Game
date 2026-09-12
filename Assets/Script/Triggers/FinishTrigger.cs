using UnityEngine;
using VContainer;

public class FinishTrigger : MonoBehaviour
{
    private GameStateService _gameStateService;
    private Collider _triggerCollider;

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
    }

    [Inject]
    public void Construct(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    private void Start()
    {
        if (_gameStateService != null)
        {
            _gameStateService.OnLevelPrepared += ResetTrigger;
        }
    }

    private void OnDestroy()
    {
        if (_gameStateService != null)
        {
            _gameStateService.OnLevelPrepared -= ResetTrigger;
        }
    }

    private void ResetTrigger()
    {
        if (_triggerCollider != null)
        {
            _triggerCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggerCollider != null && !_triggerCollider.enabled) return;

        if (other.GetComponentInParent<ControllerVechicle>() != null)
        {
            if (_triggerCollider != null)
            {
                _triggerCollider.enabled = false;
            }
            _gameStateService.OnLevelFinished(true);
        }
    }
}