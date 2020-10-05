using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenBackpackButton : MonoBehaviour
{
    static private OpenBackpackButton instance;
    
    void Awake()
    {
        instance = this;
    }

    static public OpenBackpackButton Instance()
    {
        return instance;
    }
    
    public void OnOpenBackpack()
    {
        DungeonGenerator.Instance().OpenBackpack();
    }
    
    public void DisableButton()
    {
        GetComponent<Button>().interactable = false;
    }

    public void EnableButton()
    {
        GetComponent<Button>().interactable = true;
    }
}
