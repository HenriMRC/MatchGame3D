using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class EditorBehaviour
{
    private const string EDITOR_OPEN_SCENE = "EditorOpenScene";

    static EditorBehaviour()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        EditorApplication.quitting += Quitting;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange mode)
    {
        switch (mode)
        {
            case PlayModeStateChange.ExitingEditMode:
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorApplication.isPlaying = false;
                    return;
                }

                string initScene = SceneUtility.GetScenePathByBuildIndex(0);

                if (EditorSceneManager.sceneCount == 1 && EditorSceneManager.GetSceneAt(0).path == initScene)
                    break;

                string[] scenes = new string[EditorSceneManager.sceneCount];
                for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    Scene scene = EditorSceneManager.GetSceneAt(i);
                    scenes[i] = scene.path;
                }

                string json = JsonConvert.SerializeObject(scenes);
                EditorPrefs.SetString(EDITOR_OPEN_SCENE, json);
                //Debug.Log("Close: " + json);

                EditorSceneManager.OpenScene(initScene, OpenSceneMode.Single);
                break;
            case PlayModeStateChange.EnteredEditMode:
                if (!EditorPrefs.HasKey(EDITOR_OPEN_SCENE))
                    break;

                string pref = EditorPrefs.GetString(EDITOR_OPEN_SCENE);
                EditorPrefs.DeleteKey(EDITOR_OPEN_SCENE);
                if (string.IsNullOrWhiteSpace(pref))
                    break;

                //Debug.Log("Load: " + pref);

                string[] allScenes = JsonConvert.DeserializeObject<string[]>(pref);
                if (allScenes.Length == 0)
                    break;

                EditorSceneManager.OpenScene(allScenes[0], OpenSceneMode.Single);

                for (int i = 1; i < allScenes.Length; i++)
                    EditorSceneManager.OpenScene(allScenes[i], OpenSceneMode.Additive);
                break;
        }
    }

    private static void Quitting()
    {
        EditorPrefs.DeleteKey(EDITOR_OPEN_SCENE);
    }
}