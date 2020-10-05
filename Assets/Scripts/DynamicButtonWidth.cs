using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicButtonWidth : MonoBehaviour
{
    public float extraWidth = 20.0f;
    public float textOffset = 10.0f;

    private bool widthInit = false;
    
    public void InitWidth()
    {
        if (widthInit)
            return;
        
        RectTransform textTransform = GetComponentInChildren<TMP_Text>().gameObject.GetComponent<RectTransform>();
        float width = LayoutUtility.GetPreferredWidth(textTransform) + extraWidth;
        textTransform.localPosition += new Vector3(textOffset, 0 ,0);

        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        size.x = width;
        GetComponent<RectTransform>().sizeDelta = size;

        widthInit = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitWidth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
