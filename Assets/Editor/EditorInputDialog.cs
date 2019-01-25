using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorInputDialog : EditorWindow
{
    public string okContent { private set; get; }
    public string cancelCoentent { private set; get; }
    public bool needFocus { set; get; }
    public string text { private set; get; }
    public bool okCloseView { private set; get; }
    public bool CancelCloseView { private set; get; }

    [SerializeField]
    SerializableDelegate mSerializeActionActionString;
    [SerializeField]
    SerializableDelegate mSerializeActionAction;
    Action<string> okCallBack
    {
        set
        {
            mSerializeActionActionString = new SerializableDelegate(value);
        }
        get
        {
            return mSerializeActionActionString == null ? null : mSerializeActionActionString.value as Action<string>;
        }
    }
    Action cancelCallBack
    {
        set
        {
            mSerializeActionAction = new SerializableDelegate(value);
        }
        get
        {
            return mSerializeActionAction == null ? null : mSerializeActionAction.value as Action;
        }
    }

    static public EditorInputDialog Display(string pTitle, string pText, Action<string> pOkCallBack, Action pCancelCallBack = null,
        bool pNeedFocus = false, string pOkContent = "Ok", string pCancelContent = "Cancel", bool pOkCloseView = true, bool pCancelCloseView = true)
    {
        var tView = GetWindow<EditorInputDialog>(true);
        tView.title = pTitle;
        tView.text = pText;
        tView.okCallBack = pOkCallBack;
        tView.cancelCallBack = pCancelCallBack;
        tView.needFocus = pNeedFocus;
        tView.okContent = pOkContent;
        tView.cancelCoentent = pCancelContent;
        tView.okCloseView = pOkCloseView;
        tView.CancelCloseView = pCancelCloseView;
        tView.minSize = new Vector2(300, 45);
        tView.maxSize = new Vector2(300, 45);
        tView.ShowUtility();
        return tView;
    }

    void OnGUI()
    {
        text = EditorGUILayout.TextField(text);
        using (var s = new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button(okContent, GUILayout.Width(150)))
            {
                if (okCallBack != null) okCallBack(text);
                if (okCloseView) Close();
            }
            if (GUILayout.Button(cancelCoentent))
            {
                if (cancelCallBack != null) cancelCallBack();
                if (CancelCloseView) Close();
            }
        }
    }

    void OnLostFocus()
    {
        if (needFocus) Focus();
    }
}
