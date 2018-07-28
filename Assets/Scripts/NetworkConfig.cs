using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using HoloToolkit.Unity.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkConfig : MonoBehaviour {

    public CreateModel tutorial;
    public CreateModel student;
    public BoundingBox BoxPrefabs;
    public AppBar barPrefabs;
    public AudioClip clip;
    public Material scaleMaterial;
    public Material rotateMaterial;
    public Material interactMaterial;
    // Use this for initialization

    public void Start() {
        NetworkManager nw = GetComponent<NetworkManager>();
        config(tutorial.models, nw);
        config(student.models, nw);
    }

    private void config(GameObject[] models, NetworkManager nw) {
        foreach (GameObject prefab in models) {
            if (prefab) {
                if (!prefab.GetComponent<NetworkIdentity>()) {
                    // add network identity
                    NetworkIdentity ni = prefab.AddComponent<NetworkIdentity>();
                    ni.localPlayerAuthority = true;
                }
                // add network tranform

                if (!prefab.GetComponent<NetworkTransform>()) {
                    NetworkTransform nt = prefab.AddComponent<NetworkTransform>();
                    nt.transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
                }

                /*
                // add Authority
                if (!prefab.GetComponent<Authority>()) {
                    prefab.AddComponent<Authority>();
                }
                */

                // add rigidbody
                Rigidbody rb = prefab.GetComponent<Rigidbody>();
                if (rb) {
                    rb.useGravity = true;
                    rb.isKinematic = true;
                }
                else {
                    rb = prefab.AddComponent<Rigidbody>();
                    rb.useGravity = true;
                    rb.isKinematic = true;
                }

                // GazeResponder
                if (!prefab.GetComponent<GazeResponder>()) {
                    prefab.AddComponent<GazeResponder>();
                }

                // MeshRenderer
                if (prefab.GetComponent<MeshRenderer>() == null) {
                    prefab.AddComponent<MeshRenderer>();
                }

                // add coilder
                if (!CheckColliderExist(prefab)) {
                    AddCollider(prefab);
                }

                // TapResponder
                if (!prefab.GetComponent<TapResponder>()) {
                    prefab.AddComponent<TapResponder>();
                }

                // AudioSource
                if (!prefab.GetComponent<AudioSource>()) {
                    AudioSource source = prefab.AddComponent<AudioSource>();
                    source.clip = clip;
                }


                // keywords
                if (!prefab.GetComponent<Keywords>()) {
                    prefab.AddComponent<Keywords>();
                }

                // TwoHandManipulatable
                if (!prefab.GetComponent<TwoHandManipulatable>()) {
                    TwoHandManipulatable twoHand = prefab.AddComponent<TwoHandManipulatable>();
                    twoHand.ManipulationMode = ManipulationMode.RotateAndScale;
                    twoHand.BoundingBoxPrefab = BoxPrefabs;
                }

                // BoundingBoxRig
                if (!prefab.GetComponent<BoundingBoxRig>()) {
                    BoundingBoxRig box = prefab.AddComponent<BoundingBoxRig>();
                    box.ScaleHandleMaterial = scaleMaterial;
                    box.RotateHandleMaterial = rotateMaterial;
                    box.InteractingMaterial = interactMaterial;
                    box.BoundingBoxPrefab = BoxPrefabs;
                    box.appBarPrefab = barPrefabs;
                    box.scaleRate = 3.0f;
                    box.maxScale = 100.0f;
                }

            }
            nw.spawnPrefabs.Add(prefab);
            ClientScene.RegisterPrefab(prefab);
        }
    }



    // check checkColliderExist in parent and it's child gameobject
    private bool CheckColliderExist(GameObject obj) {
        if (HasCollider(obj)) {
            return true;
        }
        else {
            for (int i = 0; i < obj.transform.childCount; i++) {
                GameObject child = obj.transform.GetChild(i).gameObject;
                if (CheckColliderExist(child)) {
                    return CheckColliderExist(child);
                }
            }
            return false;
        }
    }

    // check single gameobject has collider
    private bool HasCollider(GameObject obj) {
        MeshCollider mc = obj.GetComponent<MeshCollider>();
        if (mc) {
            try {
                mc.convex = true;
            }
            catch (Exception e) {
                Debug.Log(e);
            }
        }
        if (obj.GetComponent<BoxCollider>() == null && obj.GetComponent<SphereCollider>() == null
            && obj.GetComponent<CapsuleCollider>() == null && obj.GetComponent<WheelCollider>() == null
            && obj.GetComponent<TerrainCollider>() == null && obj.GetComponent<MeshCollider>() == null) {
            return false;
        }
        else {
            return true;
        }
    }

    // add collider to the most most inner gameobject
    private void AddCollider(GameObject obj) {
        if (obj.transform.childCount == 0) {
            obj.AddComponent<BoxCollider>();
        }
        else {
            for (int i = 0; i < obj.transform.childCount; i++) {
                GameObject child = obj.transform.GetChild(i).gameObject;
                AddCollider(child);
            }
        }
    }
}