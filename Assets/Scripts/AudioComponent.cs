using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioComponent : MonoBehaviour
{


    public AudioClip clip;

    private AudioSource audioSource;

    void Awake()
    {
        
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; 
    }

    // M�todo p�blico para reproducir
    public void Play()
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No se asign� un audio en " + gameObject.name);
        }
    }

    
    public void Stop()
    {
        audioSource.Stop();
    }
}
