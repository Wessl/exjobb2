using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public GameObject parent;
    public List<GameObject> children;
    public List<GameObject> neighbours; // The real question is how the fuck do we populate this one

    private void Start()
    {
        children ??= new List<GameObject>();
        neighbours ??= new List<GameObject>();
    }
}
