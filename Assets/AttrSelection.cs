using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttrSelection : MonoBehaviour
{
    [SerializeField] private GameObject isEmptyPanel;
    [SerializeField] private GameObject attributeNameDropdown;
    [SerializeField] private GameObject patternPanel;
    [SerializeField] private GameObject operatorDropdown;
    [SerializeField] private GameObject valueInputfield;
    
    public GameObject IsEmptyPanel => isEmptyPanel;
    public GameObject PatternPanel => patternPanel;
    public GameObject OperatorDropdown => operatorDropdown;
    public GameObject ValueInputfield => valueInputfield;

    public GameObject AttributeNameDropdown => attributeNameDropdown;
    
    

}
