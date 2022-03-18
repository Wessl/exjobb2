using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Selex : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    [SerializeField] private Button addTopoSButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddTopoSButton()
    {
        var newDropdown = Instantiate(dropdown, addTopoSButton.transform.position, Quaternion.identity);
        newDropdown.transform.SetParent(this.transform, true);
        newDropdown.GetComponent<RectTransform>().localScale = Vector3.one;
        newDropdown.gameObject.SetActive(true);
        // Move down addTopoSButton parent to allow for more buttons...
        var addTopoSHeight = addTopoSButton.GetComponent<RectTransform>().sizeDelta.y;
        Debug.Log(addTopoSHeight);
        addTopoSButton.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -addTopoSHeight);
    }
}
