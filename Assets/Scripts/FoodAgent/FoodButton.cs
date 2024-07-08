using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodButton : MonoBehaviour
{
    private bool _canUseButton = true;
    
    public bool CanUseButton()
    {
        return _canUseButton;
    }
    
    public void UseButton()
    {
        _canUseButton = false;
    }
}
