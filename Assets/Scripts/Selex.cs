using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Selex : MonoBehaviour
{
    [SerializeField] private GameObject masterPrefab;
    [SerializeField] private GameObject actionCell;
    
    [SerializeField] private TMP_Dropdown topoSDropdown;
    [SerializeField] private TMP_Dropdown groupSDropdown;

    [SerializeField] private Button addTopoSButton;

    [SerializeField] private Button addGroupSButton;
    [SerializeField] private Button addAttrSButton;

    [SerializeField] private GameObject parentSelexCell;

    [SerializeField]private List<GameObject> allTopologySelections = new List<GameObject>();
    public List<GameObject> AllTopologySelections => allTopologySelections;
    [SerializeField]private List<GameObject> allAttributeSelections = new List<GameObject>();
    public List<GameObject> AllAttributeSelections => allAttributeSelections;
    [SerializeField]private List<GameObject> allGroupsSelections = new List<GameObject>();
    public List<GameObject> AllGroupsSelections => allGroupsSelections;

    public GameObject ParentCell
    {
        get => parentSelexCell;
        set => parentSelexCell = value;
    }

    private float usedPanelHeight;                              // Use this to scale the height dynamically
    private float maxPanelHeight;
    private RectTransform selexPanel;
    private List<Selex> connectedChildren;
    [SerializeField] private GameObject connectionImage;
    [SerializeField] private GameObject attrSelectorsParent;
    private int groupNattrBtnsAdded = 0;
    
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
        var newDropdown = Instantiate(topoSDropdown, addTopoSButton.transform.position, Quaternion.identity);
        newDropdown.transform.SetParent(this.transform, true);
        newDropdown.GetComponent<RectTransform>().localScale = Vector3.one;
        newDropdown.gameObject.SetActive(true);
        newDropdown.transform.SetSiblingIndex(addTopoSButton.transform.GetSiblingIndex());
        // AddPanelHeight(addTopoSButton.GetComponent<RectTransform>().sizeDelta.y);
        // Move down addTopoSButton parent to allow for more buttons...
        addTopoSButton.gameObject.SetActive(false);
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
        var groupSBtnHeight = addGroupSButton.GetComponent<RectTransform>().sizeDelta.y;
        groupNattrBtnsAdded++;
        newDropdown.transform.position += new Vector3(0, - groupSBtnHeight * groupNattrBtnsAdded, 0);
        // This is to make space for the next possible selection
        // MoveDownAttrAndGroupSelectors();
        AddPanelHeight(groupSBtnHeight);
        
        allGroupsSelections.Add(newDropdown.gameObject);
    }

    private void AddPanelHeight(float height)
    {
        usedPanelHeight += height;
        if (usedPanelHeight > (maxPanelHeight))
        {
            selexPanel.sizeDelta += new Vector2(0, height);
            maxPanelHeight = selexPanel.sizeDelta.y;
            addAttrSButton.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, height*2);
        }
    }

    public void AddAttrSButton()
    {
        var newAttrSelectorParent = Instantiate(attrSelectorsParent, attrSelectorsParent.transform.position, Quaternion.identity);
        newAttrSelectorParent.transform.SetParent(this.transform, true);
        newAttrSelectorParent.GetComponent<RectTransform>().localScale = Vector3.one;
        newAttrSelectorParent.gameObject.SetActive(true);
        var attrSBtnHeight = addGroupSButton.GetComponent<RectTransform>().sizeDelta.y;
        groupNattrBtnsAdded++;
        newAttrSelectorParent.transform.position += new Vector3(0, -attrSBtnHeight * groupNattrBtnsAdded, 0);
        
        AddPanelHeight(attrSBtnHeight);
        
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
        newPanel.gameObject.GetComponent<Selex>().ConnectChildToThis(gameObject);
        
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


    // This will only be called by a parent onto a child when child is created
    public void ConnectChildToThis(GameObject parentSelexPanel)
    {
        connectionImage.SetActive(true);
        gameObject.GetComponent<Selex>().ParentCell = parentSelexPanel;
        Debug.Log("now hooking up selex panel with id " + parentSelexPanel.GetInstanceID() + " as a child to the gameobject with id " + gameObject.GetInstanceID());
    }
    
    
}
