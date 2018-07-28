﻿using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CreateModel : MonoBehaviour {

    /*
    public Camera MRCamera;
    public GameObject ScenceContent;
    public BoundingBox BoxPrefabs;
    public AppBar barPrefabs;
    public AudioClip clip;
    public Material scaleMaterial;
    public Material rotateMaterial;
    public Material interactMaterial;
    public Shader hololenShader;
    public GameObject newGameObject;
    */
    public Dropdown[] DrowDownMenu;
    private int index;



    // model that user going to instaniate
    public GameObject[] models;

    public void Start() {
        try {
            PopulateDrowDown();
        }
        catch (Exception e) {
            Debug.Log(e);
        }
    }

    public void PopulateDrowDown() {
        List<string> dropOptions = new List<string>();
        for (int i = 0; i < models.Length; i++) {
            dropOptions.Add(models[i].name);
        }
        for (int i = 0; i < DrowDownMenu.Length; i++) {
            DrowDownMenu[i].AddOptions(dropOptions);
        }
    }

    /*
    public void Create(int menu) {
        if (DrowDownMenu[menu].value == 0) {
            return;
        }
        else {
            InstantiateModel((DrowDownMenu[menu].value) - 1);
        }
    }
    */

    /*
    public void InstantiateModel(int index) {
        //instantiate models
        Vector3 position = MRCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, 2.0f));
        newGameObject = Instantiate(models[index], position, models[index].transform.rotation);

        // GazeResponder
        newGameObject.transform.parent = ScenceContent.transform;
        newGameObject.AddComponent<GazeResponder>();

        // MeshRenderer
        if (newGameObject.GetComponent<MeshRenderer>() == null) {
            newGameObject.AddComponent<MeshRenderer>();
        }

        // change shader
        ChangeShader(newGameObject);

        // add coilder
        if (!CheckColliderExist(newGameObject)){
            AddCollider(newGameObject);
        }

        // TapResponder
        newGameObject.AddComponent<TapResponder>();

        // AudioSource
        AudioSource source = newGameObject.AddComponent<AudioSource>();
        source.clip = clip;

        // keywords
        newGameObject.AddComponent<Keywords>();

        // TwoHandManipulatable
        TwoHandManipulatable twoHand = newGameObject.AddComponent<TwoHandManipulatable>();
        twoHand.ManipulationMode = ManipulationMode.RotateAndScale;
        twoHand.BoundingBoxPrefab = BoxPrefabs;

        // BoundingBoxRig
        BoundingBoxRig box = newGameObject.AddComponent<BoundingBoxRig>();
        box.ScaleHandleMaterial = scaleMaterial;
        box.RotateHandleMaterial = rotateMaterial;
        box.InteractingMaterial = interactMaterial;
        box.BoundingBoxPrefab = BoxPrefabs;
        box.appBarPrefab = barPrefabs;
        box.scaleRate = 3.0f;
        box.maxScale = 100.0f;

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
            try{
                mc.convex = true;
            }catch(Exception e){
                Debug.Log(e);
            }
        }
        if (obj.GetComponent<BoxCollider>() == null && obj.GetComponent<SphereCollider>() == null
            && obj.GetComponent<CapsuleCollider>() == null && obj.GetComponent<WheelCollider>() == null 
            && obj.GetComponent<TerrainCollider>() == null && obj.GetComponent<MeshCollider>() == null) {
            return false;
        }else{
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

    // changed shader to hololens standard
    private void ChangeShader(GameObject obj) {
        if (obj.GetComponent<MeshRenderer>() == null) {
            return;
        }
        else {
            obj.GetComponent<MeshRenderer>().material.shader = hololenShader;
            for (int i = 0; i < obj.transform.childCount; i++) {
                GameObject child = obj.transform.GetChild(i).gameObject;
                ChangeShader(child);
            }
        }
    }
    */
}