using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Springpunk.MigrationTools.Editor
{
    public class SceneOpener : MigrationTool
    {
        public SceneOpener() : base("Scene Opener") {}

        protected override void OnGUI()
        {
            string[] scenePaths = MigrationToolsWindow.Instance.SceneLoader.SelectedScenePaths;

            GUILayout.Label("Scene opening:");
            if (GUILayout.Button("Open all loaded scenes")) OpenScenes(scenePaths);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Mark all scenes as dirty")) EditorSceneManager.MarkAllScenesDirty();
            if (GUILayout.Button("Save all open scenes")) EditorSceneManager.SaveOpenScenes();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("Mark scenes as dirty and Save"))
            {
                for (int i = 0; i < EditorSceneManager.sceneCount; i++) EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
            }
        }

        private static void OpenScenes(string[] scenesToOpen)
        {
            EditorUtility.DisplayProgressBar("Opening scenes...", "Please wait...", 0f);
            int counter = 0;
            foreach (string path in scenesToOpen)
            {
                counter++;
                EditorUtility.DisplayProgressBar("Opening scenes...", path, ((float)counter / scenesToOpen.Length));
                EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}