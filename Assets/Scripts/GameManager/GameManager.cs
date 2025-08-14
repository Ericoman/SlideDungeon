using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public float moveDuration = 1.0f;

    public float ShowingFirstMovementDuration = 3.0f;

    public bool movingCamera = false;

    public Camera maincamera;
    public Camera endCamera;

    public Camera cameraZoomOut;
    public bool puzzleMode = false;

    private Coroutine currentMoveCoroutine;

    public Transform maincameraRightPosition;

    public Vector3 savedCamPosition;
    public Quaternion savedCamRotation;

    public Material m_highlight;

    public Vector2Int initialTilePositionUI;

    public Vector2Int TilePositionUI;

    public GridManager gm;

    public Tileable currentTileable=null;

    public Material savedMaterialTileable;

    public Tileable selectedTileable = null;

    public bool movingHL = false;

    public bool ShowingFirstmovement = false;

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

        TilePositionUI = initialTilePositionUI;

        // Opcional: Persistir entre escenas
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (puzzleMode)
        {

        }
    }

    public void SetSelectedTileable()
    {
        if (selectedTileable == null)
        {
            if (currentTileable != null)
            {
                selectedTileable = currentTileable;
                selectedTileable.GetComponent<Outline>().OutlineColor = Color.blue;
            }
            
        }
        else
        {
            selectedTileable.GetComponent<Outline>().OutlineColor = Color.green;
            selectedTileable = null;
            
        }
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
        //HighlightTileable();

    }

    public void ResetMainCamera(Transform start, Vector3 destination)
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

    public void HighlightTileable()
    {
        if (gm.GetTile(TilePositionUI) != null)
        {
            currentTileable = gm.GetTile(TilePositionUI);
            //savedMaterialTileable = currentTileable.GetComponentInChildren<MeshRenderer>().material;
            //MeshRenderer[] renderers = currentTileable.GetComponentsInChildren<MeshRenderer>();

            //foreach (MeshRenderer renderer in renderers)
            //{
            //    renderer.material = m_highlight;
            //}
            currentTileable.GetComponent<Outline>().enabled = true;
        }
    }

    public void ResetHighlight()
    {
        if (gm.GetTile(TilePositionUI) != null)
        {

            //MeshRenderer[] renderers = currentTileable.GetComponentsInChildren<MeshRenderer>();

            //foreach (MeshRenderer renderer in renderers)
            //{
            //    renderer.material = savedMaterialTileable;
            //}

            currentTileable.GetComponent<Outline>().enabled = false;

            //currentTileable = null;
            //savedMaterialTileable = null;
        }
        TilePositionUI = initialTilePositionUI;
    }

    public void SetNextTileable(float x, float y)
    {

        int maxTries = 1; // Nunca más iteraciones que tiles existen
        int tries = 0;
        if (!movingHL)
        {

            if (currentTileable != null)
            {
                //MeshRenderer[] renderers = currentTileable.GetComponentsInChildren<MeshRenderer>();

                //foreach (MeshRenderer renderer in renderers)
                //{
                //    renderer.material = savedMaterialTileable;
                //}
                currentTileable.GetComponent<Outline>().enabled = false;
            }

            while ((gm.GetTile(TilePositionUI) == currentTileable || gm.GetTile(TilePositionUI) == null)
       && tries < maxTries)
            {
                tries++;

                if (x > 0)
                    TilePositionUI.x += 1;
                else if (x < 0)
                    TilePositionUI.x -= 1;

                if (y > 0)
                    TilePositionUI.y += 1;
                else if (y < 0)
                    TilePositionUI.y -= 1;

                // Ajuste de límites (wrap-around)
                if (TilePositionUI.x >= gm.Width)
                    TilePositionUI.x = 0;
                else if (TilePositionUI.x < 0)
                    TilePositionUI.x = gm.Width - 1;

                if (TilePositionUI.y >= gm.Height)
                    TilePositionUI.y = 0;
                else if (TilePositionUI.y < 0)
                    TilePositionUI.y = gm.Height - 1;
            }



            StartCoroutine(MoveHighlight());
        }
        
    }

    IEnumerator MoveHighlight()
    {
        movingHL = true;
        HighlightTileable();
        yield return new WaitForSeconds(0.2f);
        movingHL = false;
    }


    public bool CheckCurrentTileable(Vector2Int position, float x, float y)
    {
        if (gm.GetTile(position) == currentTileable)
        {
            if (x > 0)
            {
                TilePositionUI.x += 1;
            }
            else if (x < 0)
            {
                TilePositionUI.x -= 1;
            }
            if (y > 0)
            {
                TilePositionUI.y += 1;
            }
            else if (y < 0)
            {
                TilePositionUI.y -= 1;
            }

            if (TilePositionUI.x >= gm.Width)
            {
                TilePositionUI.x = 0;
            }
            else if (TilePositionUI.x < 0)
            {
                TilePositionUI.x = gm.Width - 1;
            }

            if (TilePositionUI.y >= gm.Height)
            {
                TilePositionUI.y = 0;
            }
            else if (TilePositionUI.y < 0)
            {
                TilePositionUI.y = gm.Height - 1;
            }

            HighlightTileable();
            StartCoroutine(MoveHighlight());
            return false;
        }
        else
        {
            return true;
        }

            
        
        
    }

    public void ShowingFirstRoomMovement()
    {
        StartCoroutine(ShowingFirstRoomMovementCoroutine());
    }

    IEnumerator ShowingFirstRoomMovementCoroutine()
    {
        ShowingFirstmovement = true;
        maincamera.GetComponent<Camera>().enabled = false;
        cameraZoomOut.GetComponent<Camera>().enabled = true;
        yield return new WaitForSeconds(ShowingFirstMovementDuration);
        ShowingFirstmovement = false;
        maincamera.GetComponent<Camera>().enabled = true;
        cameraZoomOut.GetComponent<Camera>().enabled = false;
    }
}
