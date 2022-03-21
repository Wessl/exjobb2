using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Selex : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_Dropdown groupSDropdown;

    [SerializeField] private Button addTopoSButton;

    [SerializeField] private Button addGroupSButton;
    [SerializeField] private Button addAttrSButton;
    
    private float usedPanelHeight;                              // Use this to scale the height dynamically
    private float maxPanelHeight;
    private RectTransform selexPanel;
    // Start is called before the first frame update
    void Start()
    {
        selexPanel = GetComponent<RectTransform>();
        usedPanelHeight = addGroupSButton.GetComponent<RectTransform>().sizeDelta.y;
        maxPanelHeight = selexPanel.sizeDelta.y;
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
        AddPanelHeight(addTopoSButton.GetComponent<RectTransform>().sizeDelta.y);
        // Move down addTopoSButton parent to allow for more buttons...
        Destroy(addTopoSButton);
    }

    public void AddGroupSButton()
    {
        // You should get no choice, just a dropdown of group selector options
        var newDropdown = Instantiate(groupSDropdown, addGroupSButton.transform.position, Quaternion.identity);
        newDropdown.transform.SetParent(this.transform, true);
        newDropdown.GetComponent<RectTransform>().localScale = Vector3.one;
        newDropdown.gameObject.SetActive(true);
       
        // Move down both buttons
        var height = addGroupSButton.GetComponent<RectTransform>().sizeDelta.y;
        addGroupSButton.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -height);
        AddPanelHeight(height);
    }

    private void AddPanelHeight(float height)
    {
        usedPanelHeight += height;
        if (usedPanelHeight > maxPanelHeight)
        {
            selexPanel.sizeDelta += new Vector2(0, 100);
            maxPanelHeight = selexPanel.sizeDelta.y;
        }
    }

    public void AddAttrSButton()
    {
        // You should get choice of two buttons, one to pick label attribute and one to pick list attribute thing
        // (do that here)
        // Move down both buttons
        var height = addAttrSButton.GetComponent<RectTransform>().sizeDelta.y;
        addAttrSButton.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -height);
        AddPanelHeight(height);
    }
}
