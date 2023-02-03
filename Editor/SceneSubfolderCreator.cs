using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Springpunk.MigrationTools.Editor
{
    public class SceneSubfolderCreator : MigrationTool
    {
        private string subfolderToCreate = "New Folder";

        public SceneSubfolderCreator() : base("Scene Subfolder Creator") { }

        protected override void OnGUI()
        {
            string[] scenePaths = MigrationToolsWindow.Instance.SceneLoader.SelectedScenePaths;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Subfolder to create's name:");
            subfolderToCreate = GUILayout.TextField(subfolderToCreate);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Create subfolders"))
            {
                CreateCustomFolderPerScene(scenePaths, subfolderToCreate);
            }
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
    }
}