using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ControllerVechicle _vehicle;
    [SerializeField] private Transform[] _wheels;

    [Header("Settings")]
    [SerializeField] private float _wheelRadius = 0.35f;
    [SerializeField] private Vector3 _rotationAxis = Vector3.right; 

    private void Awake()
    {
        if (_vehicle == null)
        {
            _vehicle = GetComponent<ControllerVechicle>();
        }
    }

    private void Update()
    {
        if (_vehicle == null || _vehicle.IsDead || _wheels == null) return;

        float speed = _vehicle.CurrentSpeed;
        if (speed <= 0.01f) return;

        float rotationDegrees = (speed / _wheelRadius) * Mathf.Rad2Deg * Time.deltaTime;

        for (int i = 0; i < _wheels.Length; i++)
        {
            if (_wheels[i] != null)
            {
                _wheels[i].Rotate(_rotationAxis, rotationDegrees, Space.Self);
            }
        }
    }
}