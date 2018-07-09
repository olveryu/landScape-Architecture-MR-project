// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

[RequireComponent(typeof(Renderer))]
public class Keywords : MonoBehaviour, ISpeechHandler {

    public void key(string key) {
        switch (key.ToLower()) {
            case "use gravity":
                if (gameObject.GetComponent<Rigidbody>() == null) {
                    gameObject.AddComponent<Rigidbody>();
                }
                break;
            case "cancel gravity":
                if (gameObject.GetComponent<Rigidbody>() != null) {
                    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                    Destroy(rb);
                }
                break;
        }
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData) {
        key(eventData.RecognizedText);
    }
}
