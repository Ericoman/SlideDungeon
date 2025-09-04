using System;
using UnityEngine;

namespace SavingSystem
{
    [RequireComponent(typeof(Collider))]
    public class AutoSavePoint : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.SaveState();
            }
        }
    }
}
