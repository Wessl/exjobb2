using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Selex : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    [SerializeField] private Button addTopoSButton;

    [SerializeField] private Button addGroupSButton;
    [SerializeField] private Button addAttrSButton;
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
        Destroy(addTopoSButton);
    }

    public void AddGroupSButton()
    {
        // Move down both buttons
        var height = addGroupSButton.GetComponent<RectTransform>().sizeDelta.y;
        addGroupSButton.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -height);
        // You should get choice of two buttons, one to pick label attribute and one to pick list attribute thing
    }

    public void AddAttrSButton()
    {
        // Move down both buttons
        var height = addAttrSButton.GetComponent<RectTransform>().sizeDelta.y;
        addAttrSButton.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -height);
        // You should get no choice, just a dropdown of group selector options
        Instantiate()
    }
}
