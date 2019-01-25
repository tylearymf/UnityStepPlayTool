using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StepPlayTool
{
    static float mStepTime = 0.02F;
    static EditorCoroutine mEditorCoroutine;

    [MenuItem("Tools/逐帧播放/取消逐帧播放", true, 0)]
    static bool CancelStepPlayCondition()
    {
        return mEditorCoroutine != null;
    }

    [MenuItem("Tools/逐帧播放/取消逐帧播放", false, 0)]
    static void CancelStepPlay()
    {
        CancelStepPlayLogic();
    }

    [MenuItem("Tools/逐帧播放/逐帧播放设置", true, 0)]
    static bool StepPlayCondition()
    {
        return EditorApplication.isPlaying;
    }

    [MenuItem("Tools/逐帧播放/逐帧播放设置", false, 0)]
    static void StepPlay()
    {
        EditorInputDialog.Display("请输入延迟时间(>=0)", mStepTime.ToString(), pText =>
        {
            var tDelayTime = 0F;
            if (float.TryParse(pText, out tDelayTime) && tDelayTime >= 0)
            {
                StepPlayLogic(tDelayTime);
            }
            else Debug.LogError("输入有误");
        }, () =>
        {
            CancelStepPlay();
        }, false, "重新播放延迟", "取消延迟", false, false);
    }

    static void StepPlayLogic(float pTime)
    {
        if (!EditorApplication.isPlaying) return;
        CancelStepPlayLogic();

        mStepTime = pTime;
        EditorApplication.isPaused = true;
        mEditorCoroutine = Step();
    }

    static void CancelStepPlayLogic()
    {
        EditorApplication.isPaused = false;
        EditorCoroutine.StopCoroutine(mEditorCoroutine);
        mEditorCoroutine = null;
    }

    static EditorCoroutine Step()
    {
        if (!EditorApplication.isPlaying) return null;
        return EditorCoroutine.StartCoroutine(new EditorWaitForSeconds(mStepTime, () =>
        {
            if (EditorApplication.isPaused && EditorApplication.isPlaying)
            {
                EditorApplication.Step();
                mEditorCoroutine = Step();
            }
        }));
    }
}
