using System;
using UnityEngine;

public class AlarmTrigger : MonoBehaviour
{
    public event Action Enabled;
    public event Action Disabled;

    private void OnTriggerEnter(Collider other)
    {
        if (IsRogue(other))
            Enabled?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsRogue(other))
            Disabled?.Invoke();
    }

    private bool IsRogue(Collider other)
    {
        return other.gameObject.TryGetComponent<Rogue>(out _);
    }
}
