using System;
using System.Collections.Generic;
using System.Linq;
using Rooms;
using Rooms.Conditions;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.Editor
{
    public class RoomDataGeneratorEditorWindow : EditorWindow
    {
        [MenuItem("Tools/Room Data Generator")]
        public static void ShowWindow()
        {
           EditorWindow window = EditorWindow.GetWindow(typeof(RoomDataGeneratorEditorWindow));
           window.titleContent = new GUIContent("Room Data Generator");
           window.Show();
        }

        private List<ObjectField> objectFields = new List<ObjectField>();
        private void CreateGUI()
        {
            // Title
            rootVisualElement.Add(new Label("MyData Generator Tool")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    fontSize = 14
                }
            });
            
            //Fields
            IntegerField initialIdField = new IntegerField("Initial ID"){value = 1};
            IntegerField finalIdField = new IntegerField("Final ID"){value = 1};
            
            TextField folderField = new TextField("Folder Path") { value = "Assets/ScriptableObjects/Rooms/" };
            
            // Button
            Button generateButton = new Button(() =>
            {
                List<RoomConditionSO> roomConditions = new List<RoomConditionSO>();
                foreach (ObjectField objectField in objectFields)
                {
                    if (objectField.objectType == typeof(RoomConditionSO))
                    {
                        roomConditions.Add(objectField.value as RoomConditionSO);
                    }
                }
                GenerateAssets(initialIdField.value,finalIdField.value, folderField.value,roomConditions.ToArray());
            })
            {
                text = "Generate Assets"
            };

            // Add to root
            rootVisualElement.Add(initialIdField);
            rootVisualElement.Add(finalIdField);
            rootVisualElement.Add(folderField);
            
            //Conditions
            VisualElement container = new VisualElement();
            rootVisualElement.Add(container);

            // Button to add new ObjectField
            Button addButton = new Button(() =>
            {
                AddObjectField<RoomConditionSO>(container);
            })
            {
                text = "Add Condition"
            };
            rootVisualElement.Add(addButton);
            
            initialIdField.RegisterValueChangedCallback(evt =>
            {
                int newValue = evt.newValue;

                if (newValue <= 0)
                {
                    initialIdField.value = newValue = 1;
                }
                
                if (newValue > finalIdField.value)
                {
                    finalIdField.value = newValue;
                }
            });

            finalIdField.RegisterValueChangedCallback(evt =>
            {
                int newValue = evt.newValue;
                if (newValue <= 0)
                {
                    finalIdField.value = newValue = 1;
                }
                
                if (newValue < initialIdField.value)
                {
                    initialIdField.value = newValue;
                }
            });
            
            rootVisualElement.Add(generateButton);
        }

        private void AddObjectField<T>(VisualElement container)
        {
            ObjectField objectField = new ObjectField();
            objectField.objectType = typeof(T);

            Button removeButton = new Button(() =>
            {
                container.Remove(objectField.parent);
                objectFields.Remove(objectField);
            })
            {
                text = "Remove"
            };

            VisualElement row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            row.Add(objectField);
            row.Add(removeButton);
            
            container.Add(row);
            objectFields.Add(objectField);
        }
        private void GenerateAssets(int initialId, int finalId, string folderPath, RoomConditionSO[] conditions)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets", folderPath.Replace("Assets/", ""));
            }

            for (int i = initialId; i <= finalId; i++)
            {
                string path = $"{folderPath}/RoomData_{i}.asset";

                RoomDataSO existingAsset = AssetDatabase.LoadAssetAtPath<RoomDataSO>(path);
                if (existingAsset != null)
                {
                    // Show Replace/Skip dialog
                    bool replace = EditorUtility.DisplayDialog(
                        "Asset Already Exists",
                        $"The asset '{path}' already exists.\nDo you want to replace it?",
                        "Replace",
                        "Skip"
                    );

                    if (!replace)
                    {
                        Debug.Log($"Skipped: {path}");
                        continue;
                    }

                    AssetDatabase.DeleteAsset(path);
                }

                // Create new asset
                RoomDataSO asset = ScriptableObject.CreateInstance<RoomDataSO>();
                asset.id = i;
                asset.conditions = conditions;
                //asset.conditions = ;

                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($"Created/Replaced: {path}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Generation finished. Created/Updated from {initialId} to {finalId} assets.");
        }
    }
}
