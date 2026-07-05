using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class BuildGroundScene
{
    public static void Execute()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var cam = Camera.main;
        float orthoSize = 6f;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = orthoSize;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.backgroundColor = new Color(0.55f, 0.75f, 0.95f); // open blue sky
        }

        float viewHeight = orthoSize * 2f; // 12
        float bottomY = -orthoSize; // -6

        // Ground: thin sliver across the bottom, ~1/10th of the screen height
        float groundHeight = viewHeight / 10f;
        float groundWidth = 60f;
        var ground = CreateBlock("Ground", new Vector3(0f, bottomY + groundHeight / 2f, 0f),
            new Color(0.36f, 0.56f, 0.3f), new Vector2(groundWidth, groundHeight));
        float groundTopY = bottomY + groundHeight;

        // House: sits on the ground at the left side of the screen, ~1/8th of the screen height
        float houseHeight = viewHeight / 8f;
        float houseWidth = houseHeight * 1.2f;
        float houseX = -6f;
        CreateBlock("House", new Vector3(houseX, groundTopY + houseHeight / 2f, 0f),
            new Color(0.7f, 0.4f, 0.3f), new Vector2(houseWidth, houseHeight));

        // Player: a box that walks left/right along the ground
        float playerSize = 0.8f;
        var player = CreateBlock("Player", new Vector3(0f, groundTopY + playerSize / 2f, 0f),
            new Color(0.9f, 0.9f, 0.2f), Vector2.one * playerSize);
        player.tag = "Player";
        var col = player.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        player.AddComponent<GroundPlayerController>();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/GroundScene.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scene.path, true) };

        Debug.Log("GroundScene built and saved.");
    }

    static GameObject CreateBlock(string name, Vector3 position, Color color, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.position = position;
        var block = go.AddComponent<PlaceholderBlock>();
        block.color = color;
        block.size = size;
        return go;
    }
}
