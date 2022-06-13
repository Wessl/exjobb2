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
        
        // Instantiate Selex cell
        InstantiateSelexCell();
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

    private void InstantiateSelexCell()
    {
        var newCell = Instantiate(possibleRules[0], transform.position, Quaternion.identity);// -1 cuz default option 
        newCell.transform.SetParent(gameObject.transform.parent, false);
        newCell.transform.SetSiblingIndex(this.transform.GetSiblingIndex());                                // Put into correct hierarchy position
        
        Destroy(this.gameObject);
    }
}
