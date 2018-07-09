// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// This class implements IFocusable to respond to gaze changes.
/// It highlights the object being gazed at.
/// </summary>
public class GazeResponder : MonoBehaviour, IFocusable {
    private Material[] defaultMaterials;

    private void Start() {
        defaultMaterials = GetComponent<Renderer>().materials;
    }

    public void OnFocusEnter() {
        try {
            for (int i = 0; i < defaultMaterials.Length; i++) {
                // Highlight the material when gaze enters using the shader property.
                defaultMaterials[i].SetFloat("_Gloss", 10.0f);
            }
        }
        catch {
            Debug.Log("gaze fail");
        }
    }

    public void OnFocusExit() {
        for (int i = 0; i < defaultMaterials.Length; i++) {
            // Remove highlight on material when gaze exits.
            defaultMaterials[i].SetFloat("_Gloss", 1.0f);
        }
    }

    private void OnDestroy() {
        foreach (var material in defaultMaterials) {
            Destroy(material);
        }
    }
}