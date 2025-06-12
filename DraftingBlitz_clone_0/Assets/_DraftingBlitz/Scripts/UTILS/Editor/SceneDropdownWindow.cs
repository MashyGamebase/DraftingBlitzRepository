using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.Overlays;
using UnityEngine;
using System.IO;
using System.Linq;

[Overlay(typeof(SceneView), "Scene Switcher", true)]
public class SceneDropdownWindow : Overlay
{
    DropdownField sceneDropdown;
    string[] scenePaths;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();

        RefreshSceneList();

        var sceneList = scenePaths.ToList();
        sceneDropdown = new DropdownField("Scenes", sceneList, sceneList[0]);

        sceneDropdown.RegisterValueChangedCallback(evt =>
        {
            var selected = evt.newValue;
            string fullPath = GetScenePathByName(selected);
            if (!string.IsNullOrEmpty(fullPath) &&
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(fullPath);
            }
        });

        root.Add(sceneDropdown);
        return root;
    }

    private void RefreshSceneList()
    {
        var scenes = EditorBuildSettings.scenes;
        scenePaths = new string[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenePaths[i] = Path.GetFileNameWithoutExtension(scenes[i].path);
        }
    }

    private string GetScenePathByName(string name)
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (Path.GetFileNameWithoutExtension(scene.path) == name)
                return scene.path;
        }
        return null;
    }
}