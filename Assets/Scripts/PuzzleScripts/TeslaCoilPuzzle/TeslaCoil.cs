using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TeslaCoil : MonoBehaviour
{
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            Activated?.Invoke(_isActive);
        }
    }

    private bool _isActive = false;
    
    public GameObject thunderDetector;
    
    public float activeDuration = 5f;

    public event Action<bool> Activated;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thunderDetector.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (IsActive)
        {
            thunderDetector.SetActive(true);
        }
        else
        {
            thunderDetector.SetActive(false);
        }
        
    }

    public void Activate()
    {
        IsActive = true;
        StartCoroutine(DeactivateAfterTime(activeDuration)); 
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ThunderLink"))
        {
            Activate();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ThunderLink"))
        {
            IsActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("ThunderLink"))
        {
            Activate();
        }
    }

    private IEnumerator DeactivateAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Only deactivate if it is still active
        if (IsActive)
        {
            IsActive = false;
        }
    }
    
}
