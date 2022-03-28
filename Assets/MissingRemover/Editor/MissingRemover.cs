using UnityEngine;
using UnityEditor;
using System;

namespace RiClothes {
    public class MissingRemover: MonoBehaviour {
        [MenuItem("RiClothes/MissingRemover")]
        static void Run() {
            EditorWindow.GetWindow<MissingRemoverGUI> (true, "MissingRemoverGUI");
        }
    }

    public class MissingRemoverGUI: EditorWindow {
        GameObject avatar = null;
        void OnGUI() {
            avatar = EditorGUILayout.ObjectField("アバター(Avatar)", avatar, typeof(GameObject), true) as GameObject;

            GUILayout.Space(16);

            if(GUILayout.Button("実行(Run)")) {
                if(avatar != null) {
                    try {
                        PrefabUtility.UnpackPrefabInstance(avatar, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                    } catch (ArgumentException) {
                    }

                    RemoveMissingScript(avatar);
                    EditorApplication.ExecuteMenuItem("Edit/Play");
                    Close();
                }
            }
        }

        void RemoveMissingScript(GameObject gameObject) {
            Component[] components = gameObject.GetComponents<Component>();
            int count = 0;
            for(int i = 0;i < components.Length; i++) {
                Component component = components[i];
                if(component == null) {
                    SerializedObject sObject = new SerializedObject(gameObject);
                    SerializedProperty property = sObject.FindProperty("m_Component");
                    property.DeleteArrayElementAtIndex(i - count);
                    count++;
                    sObject.ApplyModifiedProperties();
                }
            }

            if(gameObject.transform.childCount > 0) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    RemoveMissingScript(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    
}