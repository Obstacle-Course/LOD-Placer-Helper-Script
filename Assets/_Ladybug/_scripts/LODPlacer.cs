#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// This script creates a custom wizard in the Unity Editor to automate the process
// of setting up Level of Detail (LOD) groups for selected GameObjects.

public class LODPlacer : ScriptableWizard
{
    // Size threshold for LOD transitions
    public float LodSize = 0.20f;

    // Enumeration for different LOD modes
    public enum Mode
    {
        None,
        CrossFade,
        SpeedTree
    }

    // Selected LOD mode
    public Mode LodMode = Mode.CrossFade;

    // Array of GameObjects to apply LOD to
    public Transform[] LodObjects;

    // Menu item to create the LODPlacer wizard
    [MenuItem("LadyBug/PlaceLOD")]
    static void CreateWizard()
    {
        // Display the wizard and set the selected GameObjects
        var replaceGameObjects = ScriptableWizard.DisplayWizard<LODPlacer>("Place Lod", "GO");
        replaceGameObjects.LodObjects = Selection.transforms;
    }

    // Called when the "GO" button in the wizard is clicked
    void OnWizardCreate()
    {
        // Iterate through selected GameObjects
        foreach (Transform go in LodObjects)
        {
            // Get all MeshRenderers in the GameObject hierarchy
            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();

            // Check if there are any MeshRenderers
            if (renderers == null || renderers.Length == 0)
            {
                // Skip GameObjects without MeshRenderers
                return;
            }

            // Remove existing LODGroup component if it exists
            if (go.gameObject.GetComponent<LODGroup>())
            {
                Debug.LogError("Destroy go");
                DestroyImmediate(go.gameObject.GetComponent<LODGroup>());
            }

            // Add a new LODGroup component to the GameObject
            LODGroup temp = go.gameObject.AddComponent<LODGroup>();

            // Create a single LOD with the specified size threshold and MeshRenderers
            LOD[] lod = new LOD[1];
            lod[0] = new LOD(LodSize, renderers);

            // Set the fade mode based on the selected LOD mode
            if (LodMode == Mode.None)
                temp.fadeMode = LODFadeMode.None;
            if (LodMode == Mode.CrossFade)
                temp.fadeMode = LODFadeMode.CrossFade;
            if (LodMode == Mode.SpeedTree)
                temp.fadeMode = LODFadeMode.SpeedTree;

            // Set the LODs for the LODGroup
            temp.SetLODs(lod);
        }
    }
}
#endif
