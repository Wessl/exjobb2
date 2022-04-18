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
    public void functionCaller(int val)
    {
        switch (val)
        {
            case 0:
                // This is just the default
                break;  
            case 1:
                selectIsEmpty();
                break;
            case 2:
                selectPattern();
                break;
            case 3:
                selectIsOdd();
                break;
            case 4:
                selectIsEven();
                break;
            case 5:
                selectLabel();
                break;
            case 6:
                selectRowIdx();
                break;
            case 7:
                selectColIdx();
                break;
            case 8:
                selectIdx();
                break;
            default:
                Debug.Log("eyo boss something broke");
                break;
        }
    }
    public void selectIsEmpty()
    {
        // Whenever this gets called it should send up the selected information to 
        // whatever state object it is that contains every selection
        isEmptyPanel.SetActive(true);
        operatorDropdown.SetActive(false);
        valueInputfield.SetActive(false);
    }

    public void CloseIsEmpty()
    {
        isEmptyPanel.SetActive(false);
    }

    public void selectPattern()
    {
        
    }

    public void ClosePattern()
    {
        patternPanel.SetActive(false);    
    }

    public void selectIsOdd()
    {
        
    }

    public void selectIsEven()
    {
        Debug.Log("selected iseven");
    }

    public void selectLabel()
    {
        
    }

    public void selectRowIdx()
    {
        
    }

    public void selectColIdx()
    {
        
    }

    public void selectIdx()
    {
        
    }
    
    

}
