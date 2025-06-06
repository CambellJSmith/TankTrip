using UnityEditor;
using UnityEngine;
using System.Linq;


[CustomEditor(typeof(SoldierSounds))]
public class SoldierSoundsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get a reference to the SoldierSounds component
        SoldierSounds soldierSounds = (SoldierSounds)target;

        // Automatically fill the DeathLines and PanicLines arrays
        FillAudioClips(soldierSounds);

        // Draw the default inspector UI for SoldierSounds
        DrawDefaultInspector();
    }

    // This method fills the arrays with audio clips from the asset paths
    private void FillAudioClips(SoldierSounds soldierSounds)
    {
        // Get all the AudioClip assets in the specified folder paths
        soldierSounds.DeathLines = AssetDatabase.FindAssets("t:AudioClip", new[] { soldierSounds.deathLinesPath })
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => path.StartsWith(soldierSounds.deathLinesPath))
            .Select(path => AssetDatabase.LoadAssetAtPath<AudioClip>(path))
            .ToArray();

        soldierSounds.PanicLines = AssetDatabase.FindAssets("t:AudioClip", new[] { soldierSounds.panicLinesPath })
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => path.StartsWith(soldierSounds.panicLinesPath))
            .Select(path => AssetDatabase.LoadAssetAtPath<AudioClip>(path))
            .ToArray();
    }
}
