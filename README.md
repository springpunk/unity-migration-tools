# Migration Tools for Unity

Editor tools to facilitate some steps in migrating/updating to new systems

![Screenshot](images/screenshot.png)

# Default functionalities

- Project scene selection
- Scene folder and subfolder creation
- Opening scenes, marking them as dirty and saving them
- Custom tool creation

# Creating a custom tool

Check the [example] to see how to implement custom tools

```cs
using UnityEngine.Editor;
using Springpunk.MigrationTools.Editor;

[InitializeOnLoad]
public class MyCustomTool : MigrationTool
{
    // Automatically register the tool for use on editor load
    static MyCustomTool()
    {
        MyCustomTool example = new MyCustomTool();
        MigrationToolsWindow.RegisterCustomTool(example);
    }

    public MyCustomTool() : base("Example Migration Tool") { }

    protected override void OnGUI()
    {
        // Editor GUI code goes here...
    }
}

```
