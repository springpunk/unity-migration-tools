using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Springpunk.MigrationTools.Editor
{
    public class MigrationTool
    {
        protected string name = "Tool";
        public string Name {
            get => name;
            protected set => name = value;
        }

        public MigrationTool(string name) {
            this.name = name;
        }

        public void Draw() => OnGUI();

        protected virtual void OnGUI() {
            GUILayout.Space(10);
            GUILayout.Label("You tool goes here...");
            GUILayout.Space(10);
        }
    }
}