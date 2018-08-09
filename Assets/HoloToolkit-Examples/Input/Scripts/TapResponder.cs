// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.SharingWithUNET;
using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// This class implements IInputClickHandler to handle the tap gesture.
    /// It increases the scale of the object when tapped.
    /// </summary>
    public class TapResponder : MonoBehaviour, IInputClickHandler
    {
        private void Start() {
            // The bullet's transform should be in local space to the Shared Anchor.
            // Make the shared anchor the parent, but we don't want the transform to try
            // to 'preserve' the position, so we set false in SetParent.
            transform.SetParent(SharedCollection.Instance.transform, false);

            // The rigid body has a velocity that needs to be transformed into 
            // the shared coordinate system.
            //Rigidbody rb = GetComponentInChildren<Rigidbody>();
            //rb.velocity = transform.parent.TransformDirection(rb.velocity);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            // Increase the scale of the object just as a response.
            gameObject.transform.localScale += 0.05f * gameObject.transform.localScale;

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}