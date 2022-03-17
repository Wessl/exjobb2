using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rule : MonoBehaviour
{
    [SerializeField] private GameObject[] possibleRules;
    [SerializeField] private TMP_Dropdown dropdown;
    public void AddNewRule()
    {
        // Make new rule
        var newRule = Instantiate(this.gameObject, transform.position, Quaternion.identity);
        newRule.transform.SetParent(gameObject.transform.parent, false);
        // Remove ability to click on same one to make new rule
        var button = GetComponentInChildren<Button>();
        Destroy(button.gameObject);
        // Show rule choice dropdown
        dropdown.gameObject.SetActive(true);
    }

    private void Start()
    {
        
        SetUpRuleChoice();
    }

    private void SetUpRuleChoice()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions( new List<string> { "Select option from dropdown..." }  );
        dropdown.AddOptions(possibleRules.Select(o => o.name).ToList());
    }

    public void SetDropdownVal(int val)
    {
        var newCell = Instantiate(possibleRules[val-1], transform.position, Quaternion.identity);// -1 cuz default option 
        newCell.transform.SetParent(gameObject.transform.parent, false);
        newCell.transform.SetSiblingIndex(this.transform.GetSiblingIndex());                                // Put into correct hierarchy position
        Destroy(this.gameObject);
    }
}
