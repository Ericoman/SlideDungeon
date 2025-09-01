using Assets.Scripts.Interactions;
using UnityEngine;
using static RotateInteract;

public class PushPullInteract : MonoBehaviour, IInteractableHeld
{

    public Transform[] positions = new Transform[4];
    public float playerSpeed = 3f;

    public void InteractHeld(GameObject interactor)
    {
        if (interactor.tag == "Player")
        {
            Interactor interactorComponent = interactor.gameObject.GetComponentInChildren<Interactor>();
            if (interactorComponent)
            {
                SetPlayerTransform(interactor);

                interactorComponent.SetGrabbedObject(gameObject);
                gameObject.transform.SetParent(interactor.transform);
            }
            PlayerMovement playerMovement = interactor.GetComponent<PlayerMovement>();
            if (playerMovement) 
            {
                float yRot = interactor.transform.rotation.eulerAngles.y;
                if (Mathf.Abs(yRot - 90f) <= 1f || Mathf.Abs(yRot - 270f) <= 1f) 
                { 
                    //bloquear eje Z
                    playerMovement.SetBlockZ(true);
                }
                else 
                {
                    //bloquear eje X
                    playerMovement.SetBlockX(true);
                }
                playerMovement.SetMoveSpeed(playerSpeed);
            }
        }
    }

    void SetPlayerTransform(GameObject interactor) 
    {
     
        int indexPosition = 0;
        float distanciaMinima = Vector3.Distance(interactor.transform.position, positions[0].position);

        for (int i = 1; i < positions.Length; i++)
        {
            float distancia = Vector3.Distance(interactor.transform.position, positions[i].position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                indexPosition = i;
            }
        }

        //interactor.transform.position = positions[indexPosition];
        interactor.transform.position = new Vector3(positions[indexPosition].position.x, interactor.transform.position.y, positions[indexPosition].position.z);
        interactor.transform.rotation = positions[indexPosition].rotation;

    }
}
