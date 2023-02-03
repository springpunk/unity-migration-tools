using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
//using TMPro;

namespace Springpunk.MigrationTools.Editor
{
    public class MigrationTools : EditorWindow
    {
        public enum MigrationToolMode
        {
            None = 0,
            CreateSubfolder = 1,
            OpenAndSaveScenes = 2,
            CustomTools = 99,
        }

        private static Action customTools = null;
        public static Action CustomTools
        {
            set => customTools = value;
        }

        private List<string> scenePaths = new List<string>();

        private MigrationToolMode mode = MigrationToolMode.None;
        private bool showScenes = false;
        private string subfolderToCreate = "New Folder";

        private void Awake()
        {
            scenePaths = new List<string>();
        }

        [MenuItem("Window/Springpunk/Migration Tools")]
        public static void ShowWindow()
        {
            MigrationTools mt = GetWindow<MigrationTools>();
            mt.titleContent = new GUIContent("Migration Tools");
        }


        private void OnGUI()
        {
            GUIStyle headStyle = new();
            headStyle.fontSize = 30;
            headStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            headStyle.margin = new RectOffset(5, 5, 5, 10);

            GUILayout.Label("Migration Tools", headStyle);
            DrawLine();
            GUILayout.Label("Scene loading:");
            DrawSceneSelection();
            DrawLine();
            mode = (MigrationToolMode)EditorGUILayout.EnumPopup("Tool:", mode);
            GUILayout.Space(10);

            switch (mode)
            {
                case MigrationToolMode.CreateSubfolder:
                    DrawFolderCreationTool();
                    break;
                case MigrationToolMode.OpenAndSaveScenes:
                    DrawOpenAllScenesTool();
                    break;
                case MigrationToolMode.CustomTools:
                    DrawCustomTools();
                    break;
                case MigrationToolMode.None:
                default:
                    GUILayout.Label("No tools selected...");
                    break;
            }

            DrawLine();

            //if (GUILayout.Button("Edit TMPs")) {
            //    TMP_Text[] ts = GameObject.FindObjectsOfType<TMP_Text>();
            //    Debug.Log(ts.Length);

            //    foreach (TMP_Text t in ts) t.SetText(t.text + " EDITED");
            //    for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
            //        EditorSceneManager.MarkAllScenesDirty();
            //    }
            //}
        }


        private void DrawLine(float height = 1, float spacing = 10f)
        {
            GUILayout.Space(spacing);
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f));
            GUILayout.Space(spacing);
        }

        private void DrawSceneSelection()
        {
            if (GUILayout.Button("Get All Scenes"))
            {
                scenePaths = GetAllScenes();
            }

            if (scenePaths.Count < 1) return;

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Selected scenes: {scenePaths.Count}");
            showScenes = GUILayout.Toggle(showScenes, "Show:");
            GUILayout.EndHorizontal();
            if (showScenes)
            {
                int i = 0;
                foreach (string scene in scenePaths)
                {
                    i++;
                    GUILayout.Label($"{i} - {scene}");
                }
            }

        }

        private void DrawFolderCreationTool()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Subfolder to create's name:");
            subfolderToCreate = GUILayout.TextField(subfolderToCreate);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Create subfolders"))
            {
                CreateCustomFolderPerScene(scenePaths.ToArray(), subfolderToCreate);
            }
        }

        private void DrawOpenAllScenesTool()
        {
            if (GUILayout.Button("Open all loaded scenes"))
            {
                OpenScenes(scenePaths.ToArray());
            }
            if (GUILayout.Button("Save all open scenes"))
            {
                EditorSceneManager.SaveOpenScenes();
            }
            if (GUILayout.Button("Mark all scenes as dirty"))
            {
                for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    EditorSceneManager.MarkAllScenesDirty();
                }
            }

        }

        private void DrawCustomTools()
        {
            if (customTools == null)
            {
                GUILayout.Label("No custom tools available.");
                GUILayout.Label("Assign your custom tools using MigrationTools.CustomTools");
                return;

            }
            customTools.Invoke();
        }

        private static List<string> GetAllScenes()
        {
            string[] allScenes = AssetDatabase.FindAssets("t:Scene");
            List<string> scenePaths = new List<string>();
            allScenes.ToList().ForEach((string guid) =>
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                scenePaths.Add(path);
            });

            return scenePaths;
        }

        private static void CreateSceneFolders(string[] scenePaths)
        {

            List<(string, string)> sceneFoldersToCreate = new List<(string, string)>();
            foreach (string path in scenePaths)
            {

                // Get Scene Directory and Name
                int lastSlashIdx = path.LastIndexOf('/');
                string sceneDirectory = path.Substring(0, lastSlashIdx);
                string sceneName = path.Substring(lastSlashIdx + 1).Replace(".unity", "");

                // Mark scenes to create a folder for
                string sceneFolder = $"{sceneDirectory}/{sceneName}";
                if (!AssetDatabase.IsValidFolder(sceneFolder))
                {
                    (string, string) t = (sceneDirectory, sceneName);
                    sceneFoldersToCreate.Add(t);
                }
            }
            if (sceneFoldersToCreate.Count < 1) return;

            Debug.Log($"Creating {sceneFoldersToCreate.Count} missing scene folders.");
            foreach ((string dir, string name) in sceneFoldersToCreate)
                AssetDatabase.CreateFolder(dir, name);
        }

        private static void CreateCustomFolderPerScene(string[] scenePaths, string customFolderName)
        {
            CreateSceneFolders(scenePaths);

            int suffixLength = ".unity".Length;

            List<string> scenesWithMissingSubfolder = new List<string>();
            foreach (string path in scenePaths)
            {
                // Mark scenes to create a folder for
                string sceneFolder = path.Substring(0, path.Length - suffixLength);
                string customFolder = sceneFolder + "/" + customFolderName;
                if (!AssetDatabase.IsValidFolder(customFolder))
                {
                    scenesWithMissingSubfolder.Add(sceneFolder);
                }
            }
            if (scenesWithMissingSubfolder.Count < 1) return;

            Debug.Log($"Creating {customFolderName} subfolder for {scenesWithMissingSubfolder.Count} scenes.");
            foreach (string sceneFolder in scenesWithMissingSubfolder)
                AssetDatabase.CreateFolder(sceneFolder, customFolderName);
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