using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    [CustomEditor(typeof(RoomBuilder))]
    public class RoomBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RoomBuilder builder = (RoomBuilder)target;
            Color originalColor = GUI.backgroundColor;

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate Walls and Floor"))
            {
                builder.GenerateRoom();

                // Mark scene dirty so you can save changes
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                }
            }
            GUI.backgroundColor = originalColor;
            
            if (GUILayout.Button("Generate only Walls"))
            {
                builder.GenerateWalls();

                // Mark scene dirty so you can save changes
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                }
            }
            
            if (GUILayout.Button("Generate only Floor"))
            {
                builder.GenerateFloors();

                // Mark scene dirty so you can save changes
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                }
            }
            
            if (GUILayout.Button("Clear Walls and Floor"))
            {
                builder.ClearRoomExceptCustom();

                // Mark scene dirty so you can save changes
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                }
            }
            
            if (GUILayout.Button("Clear only Walls"))
            {
                builder.ClearWalls();

                // Mark scene dirty so you can save changes
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                }
            }
            if (GUILayout.Button("Clear only Floor"))
            {
                builder.ClearFloors();

                // Mark scene dirty so you can save changes
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                }
            }
            
            
            GUI.backgroundColor = Color.red;
            
            if (GUILayout.Button("Clear everything"))
            {
                if (EditorUtility.DisplayDialog(
                        "Confirm deletion",
                        "This will delete everything inside the room. Continue?",
                        "Yes",
                        "No"))
                {
                    builder.ClearRoom();
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(builder.gameObject.scene);
                }
            }
            
            GUI.backgroundColor = originalColor;
        }
    }
}
