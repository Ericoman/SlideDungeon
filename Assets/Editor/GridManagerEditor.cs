using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Reference to the target script
        GridManager gridManager = (GridManager)target;

        // Add a button to the Inspector
        if (GUILayout.Button("Clear all tiles"))
        {
            gridManager.ClearGrid();
        }
    }
}

