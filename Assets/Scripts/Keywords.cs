// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

[RequireComponent(typeof(Renderer))]
public class Keywords : MonoBehaviour, ISpeechHandler {

    public void key(string key) {
        Rigidbody rb = GetComponent<Rigidbody>();
        switch (key.ToLower()) {
            case "use gravity":
                rb.isKinematic = false;
                break;
            case "cancel gravity":
                rb.isKinematic = true;
                break;
        }
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData) {
        key(eventData.RecognizedText);
    }
}
