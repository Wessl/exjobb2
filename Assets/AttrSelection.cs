using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttrSelection : MonoBehaviour
{
    public void functionCaller(int val)
    {
        switch (val)
        {
            case 0:
                selectIsEmpty();
                break;
            case 1:
                selectPattern();
                break;
            case 2:
                selectIsOdd();
                break;
            case 3:
                selectIsEven();
                break;
            case 4:
                selectLabel();
                break;
            case 5:
                selectRowIdx();
                break;
            case 6:
                selectColIdx();
                break;
            case 7:
                selectIdx();
                break;
            default:
                Debug.Log("eyo boss something broke");
                break;
        }
    }
    public void selectIsEmpty()
    {
        
    }

    public void selectPattern()
    {
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
