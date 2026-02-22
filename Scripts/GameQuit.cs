using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameQuit
{

    public static void ExitGame()
    {
        Debug.Log("Exit");
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // 如果是打包后的应用（Windows/Mac/Android/iOS）
        Application.Quit();
    #endif
    }
}
