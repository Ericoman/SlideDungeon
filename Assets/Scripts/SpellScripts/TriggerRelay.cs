using UnityEngine;

public class TriggerRelay : MonoBehaviour
{
    private System.Action<Collider> onTriggerEnter;

    public void Setup(System.Action<Collider> triggerEnterCallback)
    {
        onTriggerEnter = triggerEnterCallback;
    }

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }
}
