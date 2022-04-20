using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public GameObject parent;
    public List<GameObject> children;
    public List<GameObject> neighbours; // The real question is how the fuck do we populate this one
    
    // This may be temporary, but lets have information for certain virtual shapes? E.g. grids. Note that this current
    // impl. limits you to one grid per shape. more flexible to just have grid component, no?
    private List<float> gridRows;
    private List<float> gridCols;

    [SerializeField] private Vector2 sizeExent;
    public Vector2 SizeExent => sizeExent;

    public List<float> GridRows
    {
        get => gridRows;
        set => gridRows = value;
    }

    public List<float> GridCols
    {
        get => gridCols;
        set => gridCols = value;
    }

    public void Start()
    {
        children ??= new List<GameObject>();
        neighbours ??= new List<GameObject>();
        gridRows = new List<float>();
        gridCols = new List<float>();
        
    }

    // Can't really be called in Start() because this object might not yet know who is its own parent
    public void SetupSizeExtent()
    {
        // Basically, if there is a Mesh on this object, use that. If not, use the parent. 
        var mesh = GetComponent<Mesh>();
        if (mesh == null)
        {
            var parentExtent = parent.GetComponent<Shape>().sizeExent;
            sizeExent = parentExtent;
        }
        else
        {
            throw new NotImplementedException("Implement the size extent thing!!");
        }
    }
    
    
}
