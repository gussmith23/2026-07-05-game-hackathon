using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class BuildVerticalSlice
{
    public static void Execute()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Camera setup for 2D
        var cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.backgroundColor = new Color(0.55f, 0.7f, 0.85f);
        }

        // GameManager
        var gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();

        // Player
        var player = CreateBlock("Player", new Vector3(-8f, 0f, 0f), new Color(0.9f, 0.9f, 0.2f), Vector2.one * 0.8f);
        player.tag = "Player";
        var playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.size = Vector2.one;
        player.AddComponent<PlayerController>();

        // Package block: starts above screen, drops to the package interaction point
        var packageStart = new Vector3(-3f, 6f, 0f);
        var packageEnd = new Vector3(-3f, 0f, 0f);
        var package = CreateBlock("PackageBlock", packageStart, new Color(0.65f, 0.45f, 0.25f), Vector2.one * 0.6f);
        var packageMove = package.AddComponent<MoveOnActivate>();
        packageMove.block = package.GetComponent<PlaceholderBlock>();
        packageMove.targetPosition = packageEnd;
        packageMove.duration = 1.2f;

        // Rocket parts: start scattered near the assembly point, converge into a stack, then launch together
        Vector3 assembleCenter = new Vector3(0f, 0f, 0f);
        var partDefs = new (string name, Color color, Vector3 scatterOffset, Vector3 assembledOffset)[]
        {
            ("RocketNose", new Color(0.85f, 0.2f, 0.2f), new Vector3(-1.5f, -2f, 0f), new Vector3(0f, 1.1f, 0f)),
            ("RocketBody", new Color(0.8f, 0.8f, 0.8f), new Vector3(1.5f, -2f, 0f), new Vector3(0f, 0.4f, 0f)),
            ("RocketFin",  new Color(0.2f, 0.4f, 0.85f), new Vector3(0f, -3f, 0f),   new Vector3(0f, -0.3f, 0f)),
        };

        var assembleMoves = new MoveOnActivate[partDefs.Length];
        var launchMoves = new MoveOnActivate[partDefs.Length];

        for (int i = 0; i < partDefs.Length; i++)
        {
            var def = partDefs[i];
            var startPos = assembleCenter + def.scatterOffset;
            var assembledPos = assembleCenter + def.assembledOffset;
            var launchPos = assembledPos + new Vector3(0f, 14f, 0f);

            var part = CreateBlock(def.name, startPos, def.color, Vector2.one * 0.5f);
            var block = part.GetComponent<PlaceholderBlock>();

            var assembleMove = part.AddComponent<MoveOnActivate>();
            assembleMove.block = block;
            assembleMove.targetPosition = assembledPos;
            assembleMove.duration = 0.8f;
            assembleMoves[i] = assembleMove;

            var launchMove = part.AddComponent<MoveOnActivate>();
            launchMove.block = block;
            launchMove.targetPosition = launchPos;
            launchMove.duration = 1.5f;
            launchMoves[i] = launchMove;
        }

        // Interaction points
        CreateInteractionPoint("IP_WakeUp", new Vector3(-6f, 0f, 0f), Beat.WakeUp, null);
        CreateInteractionPoint("IP_PackageArrives", new Vector3(-3f, 0f, 0f), Beat.PackageArrives, new[] { packageMove });
        CreateInteractionPoint("IP_AssembleRocket", new Vector3(0f, 0f, 0f), Beat.AssembleRocket, assembleMoves);
        CreateInteractionPoint("IP_FireRocket", new Vector3(3f, 0f, 0f), Beat.FireRocket, launchMoves);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/VerticalSlice.unity");
        EditorBuildSettingsScene[] scenes = { new EditorBuildSettingsScene(scene.path, true) };
        EditorBuildSettings.scenes = scenes;

        Debug.Log("VerticalSlice scene built and saved.");
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

    static void CreateInteractionPoint(string name, Vector3 position, Beat beat, MoveOnActivate[] actions)
    {
        var go = new GameObject(name);
        go.transform.position = position;
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2f, 2f);

        var promptGO = new GameObject("Prompt");
        promptGO.transform.SetParent(go.transform);
        promptGO.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        var tm = promptGO.AddComponent<TextMesh>();
        tm.text = "Press X";
        tm.characterSize = 0.2f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.black;
        promptGO.SetActive(false);

        var ip = go.AddComponent<InteractionPoint>();
        ip.beat = beat;
        ip.actionsToPlay = actions;
        ip.promptRoot = promptGO;
    }
}
