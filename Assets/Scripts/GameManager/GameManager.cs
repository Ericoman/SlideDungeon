using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public float moveDuration = 1.0f;

    public bool movingCamera = false;

    public Camera maincamera;
    public Camera endCamera;
    public bool puzzleMode = false;

    private Coroutine currentMoveCoroutine;

    public Transform maincameraRightPosition;

    Vector3 savedCamPosition;
    Quaternion savedCamRotation;

    private void Awake()
    {
        // Si ya hay una instancia y no somos nosotros, nos destruimos
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Establecemos la instancia
        Instance = this;

        // Opcional: Persistir entre escenas
        DontDestroyOnLoad(gameObject);
    }


    

    public void MoveCameraToLocation(Transform start, Transform destination)
    {
        StopAllCoroutines();
        movingCamera = true;
        savedCamPosition = maincamera.transform.position;
        savedCamRotation = maincamera.transform.rotation;
        if (currentMoveCoroutine != null)
            StopCoroutine(currentMoveCoroutine);

        currentMoveCoroutine = StartCoroutine(MoveCameraCoroutine(start.position, start.rotation, destination.position, destination.rotation));
        movingCamera = false;
        puzzleMode = true;
    }

    IEnumerator MoveCameraCoroutine(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot)
    {
        float elapsed = 0f;

        

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;

            // Interpolación de posición
            maincamera.transform.position = Vector3.Lerp(startPos, endPos, t);

            // Interpolación de rotación
            maincamera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Asegurar posición y rotación final exactas
        
        maincamera.transform.position = endPos;
        maincamera.transform.rotation = endRot;
    }

    public void ResetMainCamera(Transform start, Transform destination)
    {
        GameManager.Instance.movingCamera = true;
        GameManager.Instance.puzzleMode = false;
        if (currentMoveCoroutine != null)
            StopCoroutine(currentMoveCoroutine);

        currentMoveCoroutine = StartCoroutine(MoveCameraCoroutine(
            maincamera.transform.position,
            maincamera.transform.rotation,
            savedCamPosition,
            savedCamRotation
        ));
        GameManager.Instance.movingCamera = false;
    }
}
