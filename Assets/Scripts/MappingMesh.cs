using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MappingMesh : MonoBehaviour {
    public Material deptmesh;
    public Material spatialUnderstandingMesh;
    public GameObject tempMesh;

    public void ChangeDept() {
        for (int i = 0; i < tempMesh.transform.childCount; i++) {
            if (tempMesh.transform.GetChild(i).GetComponent<MeshRenderer>()) {
                tempMesh.transform.GetChild(i).GetComponent<MeshRenderer>().material = deptmesh;
            }
        }
    }

    public void ChangeSpatial() {
        for (int i = 0; i < tempMesh.transform.childCount; i++) {
            if (tempMesh.transform.GetChild(i).GetComponent<MeshRenderer>()) {
                tempMesh.transform.GetChild(i).GetComponent<MeshRenderer>().material = spatialUnderstandingMesh;
            }
        }
    }
}
