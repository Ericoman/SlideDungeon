using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoil : MonoBehaviour
{
    public bool isActive = false;
    
    public GameObject thunderDetector;
    
    public float activeDuration = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thunderDetector.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (isActive)
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
        isActive = true;
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
            isActive = true;
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
        if (isActive)
        {
            isActive = false;
        }
    }
    
}
