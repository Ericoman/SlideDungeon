using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class TriggerEndPuzzle : MonoBehaviour
{
    [SerializeField] GameObject EndCorridor;
    [SerializeField] GameObject[] WallsToDisappear;
    [SerializeField] GameObject[] roomTorches;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            foreach (GameObject torch in roomTorches)
            {
                GameObject fire = torch.transform.Find("Fire").gameObject;
                if (fire) fire.SetActive(true);
            }
            foreach (GameObject wall in WallsToDisappear)
            {
                wall.SetActive(false);
            }
            if (GetComponent<AudioComponent>())
            {
                GetComponent<AudioComponent>().Play();
            }
            EndCorridor.SetActive(true);
        }
        
    }
}
