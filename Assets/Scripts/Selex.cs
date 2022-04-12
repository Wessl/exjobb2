using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Selex : MonoBehaviour
{
    [SerializeField] private GameObject masterPrefab;
    [SerializeField] private GameObject actionCell;
    
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_Dropdown groupSDropdown;

    [SerializeField] private Button addTopoSButton;

    [SerializeField] private Button addGroupSButton;
    [SerializeField] private Button addAttrSButton;

    [SerializeField] private GameObject parentCell;

    private List<GameObject> allTopologySelections = new List<GameObject>();
    public List<GameObject> AllTopologySelections => allTopologySelections;
    private List<GameObject> allAttributeSelections = new List<GameObject>();
    public List<GameObject> AllAttributeSelections => allAttributeSelections;
    private List<GameObject> allGroupsSelections = new List<GameObject>();
    public List<GameObject> AllGroupsSelections => allGroupsSelections;

    public GameObject ParentCell
    {
        get => parentCell;
        set => parentCell = value;
    }

    private float usedPanelHeight;                              // Use this to scale the height dynamically
    private float maxPanelHeight;
    private RectTransform selexPanel;
    private List<Selex> connectedChildren;
    [SerializeField] private GameObject connectionImage;
    [SerializeField] private GameObject attrSelectorsParent;
    
    // Start is called before the first frame update
    void Start()
    {
        selexPanel = GetComponent<RectTransform>();
        usedPanelHeight = addGroupSButton.GetComponent<RectTransform>().sizeDelta.y;
        maxPanelHeight = selexPanel.sizeDelta.y;
        connectedChildren = new List<Selex>();
    }

    public void AddActionCell()
    {
        var newActionCell = Instantiate(actionCell);
        newActionCell.transform.SetParent(this.transform.parent, true);
        newActionCell.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);  // Put into correct hierarchy position ?
        newActionCell.GetComponent<Action>().selectionParent = this.gameObject;
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
        
        allTopologySelections.Add(newDropdown.gameObject);
    }

    // Execute this when clicking on the AddGroupS button
    public void AddGroupSButton()
    {
        // You should get no choice, just a dropdown of group selector options
        var newDropdown = Instantiate(groupSDropdown, addGroupSButton.transform.position, Quaternion.identity);
        newDropdown.transform.SetParent(this.transform, true);
        newDropdown.GetComponent<RectTransform>().localScale = Vector3.one;
        newDropdown.gameObject.SetActive(true);
        // This is to make space for the next possible selection
        MoveDownAttrAndGroupSelectors();
        
        allGroupsSelections.Add(newDropdown.gameObject);
    }

    private void AddPanelHeight(float height)
    {
        usedPanelHeight += height;
        if (usedPanelHeight > (maxPanelHeight))
        {
            selexPanel.sizeDelta += new Vector2(0, height*4);
            maxPanelHeight = selexPanel.sizeDelta.y;
            addAttrSButton.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, height*2);
        }
    }

    public void AddAttrSButton()
    {
        // You should get choice of two buttons, one to pick label attribute and one to pick list attribute thing
        EnableAttrSelectionOptions();
        MoveDownAttrAndGroupSelectors();
    }
    
    private void EnableAttrSelectionOptions()
    {
        var newAttrSelectorParent = Instantiate(attrSelectorsParent, attrSelectorsParent.transform.position, Quaternion.identity);
        newAttrSelectorParent.transform.SetParent(this.transform, true);
        newAttrSelectorParent.GetComponent<RectTransform>().localScale = Vector3.one;
        newAttrSelectorParent.gameObject.SetActive(true);
        
        allAttributeSelections.Add(newAttrSelectorParent.gameObject);

    }

    /*
     * Since it's possible to chain multiple selection expressions at a time, enable ability to add brand new selex panel,
     * connected to this one 
     */
    public void AddNewLineOfSelex()
    {
        var masterPrefab = Instantiate(this.masterPrefab).GetComponent<MasterPrefab>();
        var newPanel = Instantiate(masterPrefab.GetSelexCellPrefab(), addAttrSButton.transform.position, Quaternion.identity);
        newPanel.transform.SetParent(this.transform.parent, true);
        newPanel.GetComponent<RectTransform>().localScale = Vector3.one;
        newPanel.transform.SetSiblingIndex(this.transform.GetSiblingIndex()+1);                                // Put into correct hierarchy position
        // Hook up
        connectedChildren.Add(newPanel.GetComponent<Selex>());
        this.gameObject.GetComponent<Selex>().EnableConnectionToChild(newPanel);
        
    }

    private void MoveDownAttrAndGroupSelectors()
    {
        // Move down both buttons
        var height = addAttrSButton.GetComponent<RectTransform>().sizeDelta.y;
        // Parent contains both addAttrSButton and addGroupSButton
        addAttrSButton.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -height);
        // AND move down the invisible "default" parent to the attribute selectors
        attrSelectorsParent.transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -height);
        AddPanelHeight(height);
    }


    public void EnableConnectionToChild(GameObject childSelexPanel)
    {
        connectionImage.SetActive(true);
        childSelexPanel.GetComponent<Selex>().ParentCell = this.gameObject;
        Debug.Log("now hooking up selex panel with id " + childSelexPanel.GetInstanceID() + " as a child to the gameobject with id " + gameObject.GetInstanceID());
    }
    
    
}
