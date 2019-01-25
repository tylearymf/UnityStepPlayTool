using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sample
{
    [MenuItem("Tools/Test")]
    static void A()
    {
        EditorInputDialog.Display("test", string.Empty, pValue =>
        {
            Debug.Log(pValue);
        }, pOkCloseView: false);
    }
}
