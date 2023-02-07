using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Springpunk.MigrationTools.Editor
{
    internal class SceneLoadItem {
        public string scenePath;
        public bool select;

        public SceneLoadItem(string scenePath, bool select = true) {
            this.scenePath = scenePath;
            this.select = select;
        }
    }

    public class SceneLoader : MigrationTool
    {
        private List<SceneLoadItem> sceneLoadItems = new List<SceneLoadItem>();
        public string[] SelectedScenePaths {
            get {
                
                string[] paths = sceneLoadItems
                    .Where((item) => item.select).ToList()
                    .ConvertAll((item) => item.scenePath).ToArray();
                if (paths.Length < 1) return new string[0];
                return paths;
            }
        }

        private bool showScenes = false;
        private Vector2 scrollProgress = Vector2.zero;

        public SceneLoader() : base("Scene Loader") { }

        protected override void OnGUI()
        {
            GUILayout.Label("Scene loading:");
            if (GUILayout.Button("Get All Scenes"))
            {
                LoadAllScenes();
            }

            if (sceneLoadItems.Count < 1) {
                GUILayout.Label("No scenes loaded in the tool...");
                return;
            }

            int selectedCount = CountSelectedScenes();
            string sceneCountLabel = $"Selected scenes: {selectedCount}/{sceneLoadItems.Count}";
            showScenes = EditorGUILayout.BeginFoldoutHeaderGroup(showScenes, sceneCountLabel);
            if (showScenes)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Select all"))
                    sceneLoadItems.ForEach((item) => item.select = true);
                if(GUILayout.Button("Select none"))
                    sceneLoadItems.ForEach((item) => item.select = false);
                GUILayout.EndHorizontal();
                scrollProgress = GUILayout.BeginScrollView(scrollProgress, GUILayout.MaxHeight(400));
                for (int i = 0; i < sceneLoadItems.Count; i++)
                {
                    GUILayout.BeginHorizontal("box");
                    sceneLoadItems[i].select = GUILayout.Toggle(sceneLoadItems[i].select, sceneLoadItems[i].scenePath);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                MigrationToolsWindow.DrawLine();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void LoadAllScenes()
        {
            if (sceneLoadItems == null) sceneLoadItems = new List<SceneLoadItem>();
            else sceneLoadItems.Clear();

            string[] allScenes = AssetDatabase.FindAssets("t:Scene");
            allScenes.ToList().ForEach((string guid) =>
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneLoadItem item = new SceneLoadItem(path);
                sceneLoadItems.Add(item);
            });
        }

        private int CountSelectedScenes() {
            int c = 0;
            for (int i = 0; i < sceneLoadItems.Count; i++) {
                if (sceneLoadItems[i].select) c++;
            }
            return c;
        }
    }
}