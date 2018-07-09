// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using HoloToolkit.Unity.UX;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// This class implements IInputClickHandler to handle the tap gesture.
/// It increases the scale of the object when tapped.
/// </summary>
public class TapResponder : MonoBehaviour, IInputClickHandler {
    private AudioSource myAudioSource;
    private AppBar appBarInstance;
    public void Start() {
        myAudioSource = GetComponent<AudioSource>();
        appBarInstance = GetComponent<BoundingBoxRig>().appBarInstance;
    }

    public void Update() {
        appBarInstance = GetComponent<BoundingBoxRig>().appBarInstance;
    }
    public void OnInputClicked(InputClickedEventData eventData) {
        appBarInstance.IsVisible = true;
        myAudioSource.Play();
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }
}
