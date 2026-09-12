using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _carTransform;
    [SerializeField] private Transform _finishTransform;
    [SerializeField] private Slider _slider;

    private float _startZ;
    private float _totalDistance;

    private void Start()
    {
        if (_carTransform == null || _finishTransform == null || _slider == null) return;

        _startZ = _carTransform.position.z;
        _totalDistance = _finishTransform.position.z - _startZ;

        _slider.minValue = 0f;
        _slider.maxValue = 1f;
        _slider.value = 0f;
    }

    private void Update()
    {
        if (_carTransform == null || _totalDistance <= 0f) return;
        float currentDistance = _carTransform.position.z - _startZ;
        float progress = Mathf.Clamp01(currentDistance / _totalDistance);
        _slider.value = progress;
    }
}