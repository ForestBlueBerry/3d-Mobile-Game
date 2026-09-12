using UnityEngine;

public class ExhaustSmoke : MonoBehaviour
{
    [SerializeField] private ControllerVechicle _vehicle;
    [SerializeField] private ParticleSystem _smokeParticles;

    private void Update()
    {
        if (_vehicle == null || _smokeParticles == null) return;

        bool shouldEmit = !_vehicle.IsDead && _vehicle.CurrentSpeed > 0.1f;

        var emission = _smokeParticles.emission;
        emission.enabled = shouldEmit;
    }
}