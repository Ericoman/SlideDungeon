using System;
using System.Collections.Generic;
using System.Linq;
using Rooms;
using Rooms.Conditions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.Search.ObjectField;

namespace Tools.Editor
{
    public class RoomManagerUpdaterEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Room Manager Updater")]
        public static void ShowWindow()
        {
           EditorWindow window = EditorWindow.GetWindow(typeof(RoomManagerUpdaterEditorWindow));
           window.titleContent = new GUIContent("Room Manager Updater");
           window.Show();
        }

        [SerializeField]
        private List<GameObject> roomPrefabs = new List<GameObject>();
        
        private void CreateGUI()
        {
            // Title
            rootVisualElement.Add(new Label("Room Manager Updater Tool")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    fontSize = 14
                }
            });
            
            //Fields
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty prefabArrayProp = serializedObject.FindProperty("roomPrefabs");
            PropertyField prefabListField = new PropertyField(prefabArrayProp, "Prefabs");
            prefabListField.Bind(serializedObject); // bind serialized object to UI
            
            // Button
            Button changeButton = new Button(() =>
            {
                UpdateRoomManagers();
            })
            {
                text = "Update RoomManagers"
            };

            // Add to root
            rootVisualElement.Add(prefabListField);
            
            rootVisualElement.Add(changeButton);
        }

        private void UpdateRoomManagers()
        {
            foreach (var prefab in roomPrefabs)
            {
                if (prefab == null) continue;

                string path = AssetDatabase.GetAssetPath(prefab);
                GameObject prefabContents = PrefabUtility.LoadPrefabContents(path);

                if (prefabContents.GetComponent<Rooms.RoomManager>() == null)
                {
                    RoomManager oldRoomManager = prefabContents.GetComponentInChildren<RoomManager>();
                    if (oldRoomManager != null)
                    {
                        Rooms.RoomManager newRoomManager = oldRoomManager.gameObject.AddComponent<Rooms.RoomManager>();
                        newRoomManager.SetValuesFromOldRoomManager(oldRoomManager);
                        if (PrefabUtility.IsPartOfPrefabInstance(oldRoomManager.gameObject))
                        {
                            PrefabUtility.UnpackPrefabInstance(oldRoomManager.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                        }
                        DestroyImmediate(oldRoomManager);
                    }
                    else
                    {
                        prefabContents.AddComponent<Rooms.RoomManager>();
                    }
                    Debug.Log($"Updated Room Manager on {prefab.name}");
                }

                PrefabUtility.SaveAsPrefabAsset(prefabContents, path);
                PrefabUtility.UnloadPrefabContents(prefabContents);
            }
        }
    }
}
