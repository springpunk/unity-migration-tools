using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
//using TMPro;

namespace Springpunk.MigrationTools.Editor
{
    public class MigrationToolsWindow : EditorWindow
    {
        public enum MigrationToolMode
        {
            None = 0,
            CreateSubfolder = 1,
            OpenAndSaveScenes = 2,
            CustomTools = 99,
        }

        private static MigrationToolsWindow instance = null;
        public static MigrationToolsWindow Instance {
            get {
                if (instance == null) ShowWindow();
                return instance;
            }
            private set => instance = value;
        }

        private static List<MigrationTool> customTools = null;
        private static List<MigrationTool> CustomTools {
            get {
                if (customTools == null)
                    customTools = new List<MigrationTool>();
                return customTools;
            }
        }

        private MigrationToolMode mode = MigrationToolMode.None;
        private int selectedCustomTool = 0;

        // Tools
        private SceneLoader sceneLoader = new SceneLoader();
        public SceneLoader SceneLoader => sceneLoader;
        private SceneSubfolderCreator subfolderCreator = new SceneSubfolderCreator();
        private SceneOpener sceneOpener = new SceneOpener();


        [MenuItem("Window/Springpunk/Migration Tools")]
        public static void ShowWindow()
        {
            MigrationToolsWindow window = GetWindow<MigrationToolsWindow>();
            window.titleContent = new GUIContent("Springpunk Migration Tools");
            Instance = window;
        }


        private void OnGUI()
        {
            GUIStyle headStyle = new();
            headStyle.fontSize = 30;
            headStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            headStyle.margin = new RectOffset(5, 5, 5, 10);


            GUILayout.Label("Migration Tools", new GUIStyle {
                fontSize = 30,
                normal = new GUIStyleState
                {
                    textColor = new Color(0.9f, 0.9f, 0.9f),
                },
                margin = new RectOffset(5, 5, 5, 10),
        }
            );

            DrawLine();
            sceneLoader.Draw();
            DrawLine();

            mode = (MigrationToolMode)EditorGUILayout.EnumPopup("Mode:", mode);
            DrawLine();

            switch (mode)
            {
                case MigrationToolMode.CreateSubfolder:
                    subfolderCreator.Draw();
                    break;
                case MigrationToolMode.OpenAndSaveScenes:
                    sceneOpener.Draw();
                    break;
                case MigrationToolMode.CustomTools:
                    DrawCustomTools();
                    break;
                case MigrationToolMode.None:
                default:
                    GUILayout.Label("No tools selected...");
                    break;
            }
        }


        public static void DrawLine(float height = 1, float spacing = 10f)
        {
            GUILayout.Space(spacing);
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f));
            GUILayout.Space(spacing);
        }

        private void DrawCustomTools()
        {
            if (CustomTools.Count < 1)
            {
                GUILayout.Label("No custom tools available.");
                GUILayout.Label($"Use MigrationToolsWindow.RegisterCustomTool(MigrationTool) to register one...");
                return;
            }

            string[] options = CustomTools.ConvertAll((tool) => tool.Name).ToArray();
            selectedCustomTool = EditorGUILayout.Popup("Custom tool:", selectedCustomTool, options);
            DrawLine();

            CustomTools[selectedCustomTool].Draw();
        }

        public static void RegisterCustomTool(MigrationTool tool) {
            if (tool == null) return;
            CustomTools.Add(tool);
        }

        //public static void RegisterCustomTool<T>() where T : MigrationTool
        //{
        //    T tool = new();
        //    if (tool == null) return;
        //    CustomTools.Add(tool);
        //}
    }
}