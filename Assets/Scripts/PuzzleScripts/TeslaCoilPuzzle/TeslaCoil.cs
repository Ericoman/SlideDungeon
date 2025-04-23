using System;
using UnityEngine;

public class TeslaCoil : MonoBehaviour
{
    public bool isActive = false;
    
    public GameObject thunderDetector;
    
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

    private void OnTriggerEnter(Collider other)
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
            isActive = false;
        }
    }
}
