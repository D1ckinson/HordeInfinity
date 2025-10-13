using System;
using TMPro;
using UnityEngine;

public class DropdownTest : MonoBehaviour
{
    public TMP_Dropdown Dropdown;

    private void Start()
    {
        Method();
    }

    public void Method()
    {
        Dropdown.onValueChanged.AddListener(Skr);
    }

    private void Skr(int arg0)
    {
        Debug.Log(arg0);
    }
}
