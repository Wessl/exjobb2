using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    public List<GameObject> currentSelection;

    [SerializeField] private GameObject root;
    // Start is called before the first frame update
    void Start()
    {
        currentSelection = new List<GameObject>();
        var rootChildren = root.GetComponentsInChildren<Transform>();
        foreach (var rootChild in rootChildren)
        {
            currentSelection.Add(rootChild.gameObject);
            Debug.Log("rootchild: " + rootChild.gameObject);
        }
    }

    
}
