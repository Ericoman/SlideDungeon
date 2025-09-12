using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioComponent : MonoBehaviour
{


    public AudioSource audioSource;

    void Awake()
    {
        
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; 
    }

    // Método público para reproducir
    public void Play()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
            
            
        
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }



    }


    public void Stop()
    {
        audioSource.Stop();
    }
}
