using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesList : MonoBehaviour
{
    /*
     * Custom inspector component that will contain all the rules to implement
     */
    [SerializeField] private List<RuleListWrapper> rulesList;
    
    
}

[System.Serializable]
public class RuleListWrapper
{
    public List<Rule> ruleList;
}
