using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReddotUI : MonoBehaviour, IPointerClickHandler
{
    public string path;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text=transform.GetChild(0).GetComponent<Text>();
        TreeNode node = RedDotManager.Instance.AddListener(path, ReddotCallback);
    }

    private void ReddotCallback(int obj)
    {
        text.text= obj.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        RedDotManager.Instance.Update();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int value = RedDotManager.Instance.GetValue(path);
        if (eventData.button== PointerEventData.InputButton.Left)
        {
            RedDotManager.Instance.ChangeValue(path, value + 1);
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            RedDotManager.Instance.ChangeValue(path, Mathf.Clamp(value - 1, 0,value));
        }
    }
}
