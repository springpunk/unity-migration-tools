using UnityEngine;
using UnityEditor;

using Springpunk.MigrationTools.Editor;

namespace Springpunk.MigrationTools.Example
{
    [InitializeOnLoad]
    public class ExampleMigrationTool : MigrationTool
    {
        static ExampleMigrationTool()
        {
            // Use this to register a custom tool

            //ExampleMigrationTool example = new ExampleMigrationTool();
            //MigrationToolsWindow.RegisterCustomTool(example);
        }

        public ExampleMigrationTool() : base("Example Migration Tool") { }

        protected override void OnGUI()
        {
            int guess = Random.Range(0, 9);
            GUILayout.Label("Guess the button:");
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            for (int i = 0; i < 9; i += 3)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < 3; j++)
                {
                    if (GUILayout.Button("Pick me"))
                    {
                        if (guess == i + j)
                        {
                            Debug.Log("You guessed correctly!");
                        }
                        else
                        {
                            Debug.Log("Better luck next time...");
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}