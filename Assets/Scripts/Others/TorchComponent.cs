using UnityEngine;

public class TorchComponent : MonoBehaviour
{
    [SerializeField] 
    private GameObject fire;

    public void LightFire(bool enable)
    {
        fire.SetActive(enable);
    }
}
