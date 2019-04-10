#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

/// <summary>
/// Prevents script compilation and reload while in play mode.
/// The editor will show a the spinning reload icon if there are unapplied changes but will not actually
/// apply them until playmode is exited.
/// Note: Script compile errors will not be shown while in play mode.
/// Derived from the instructions here:
/// https://support.unity3d.com/hc/en-us/articles/210452343-How-to-stop-automatic-assembly-compilation-from-script
/// </summary>
[InitializeOnLoad]
public class UnityExtensions
{
    static UnityExtensions()
    {
        EditorApplication.playModeStateChanged
            += onPlayModeStateChanged;
    }

    static void onPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        switch (stateChange)
        {
            case (PlayModeStateChange.EnteredPlayMode):
            {
                EditorApplication.LockReloadAssemblies();
                break;
            }
            case (PlayModeStateChange.ExitingPlayMode):
            {
                EditorApplication.UnlockReloadAssemblies();
                break;
            }
        }
    }

}

#endif
