using UnityEngine;
using UnityEditor;
using System.Reflection;

public static class TestVerticalSlice
{
    static void Trigger(string ipName)
    {
        var go = GameObject.Find(ipName);
        if (go == null)
        {
            Debug.LogError($"InteractionPoint '{ipName}' not found");
            return;
        }
        var ip = go.GetComponent<InteractionPoint>();
        var method = typeof(InteractionPoint).GetMethod("Activate", BindingFlags.NonPublic | BindingFlags.Instance);
        try
        {
            method.Invoke(ip, null);
        }
        catch (TargetInvocationException ex)
        {
            Debug.LogError($"Trigger failed: {ex.InnerException}");
            return;
        }

        var beat = GameManager.Instance != null ? GameManager.Instance.CurrentBeat.ToString() : "GameManager missing";
        Debug.Log($"Triggered {ipName}. GameManager.CurrentBeat is now: {beat}");
    }

    public static void TriggerWakeUp() => Trigger("IP_WakeUp");
    public static void TriggerPackageArrives() => Trigger("IP_PackageArrives");
    public static void TriggerAssembleRocket() => Trigger("IP_AssembleRocket");
    public static void TriggerFireRocket() => Trigger("IP_FireRocket");

    public static void ReportState()
    {
        string[] names = { "Player", "PackageBlock", "RocketNose", "RocketBody", "RocketFin" };
        foreach (var n in names)
        {
            var go = GameObject.Find(n);
            if (go != null)
                Debug.Log($"{n} position: {go.transform.position}");
        }
        if (GameManager.Instance != null)
            Debug.Log($"CurrentBeat: {GameManager.Instance.CurrentBeat}");
    }
}
