using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Editor script to automatically add the ZENJECT_PRESENT define symbol
/// </summary>
public class SetZenjectDefineSymbol : Editor
{
    [InitializeOnLoadMethod]
    private static void AddZenjectDefineSymbol()
    {
        const string DEFINE_SYMBOL = "ZENJECT_PRESENT";
        
        // Get all build targets
        BuildTargetGroup[] targetGroups = System.Enum.GetValues(typeof(BuildTargetGroup))
            .Cast<BuildTargetGroup>()
            .Where(x => x != BuildTargetGroup.Unknown && !IsObsolete(x))
            .ToArray();
            
        foreach (BuildTargetGroup targetGroup in targetGroups)
        {
            // Get existing defines
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> definesList = defines.Split(';').ToList();
            
            // Add Zenject define if it doesn't exist
            if (!definesList.Contains(DEFINE_SYMBOL))
            {
                definesList.Add(DEFINE_SYMBOL);
                string newDefines = string.Join(";", definesList);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newDefines);
                Debug.Log($"Added {DEFINE_SYMBOL} define for {targetGroup}");
            }
        }
    }
    
    // Helper to check for obsolete build targets
    private static bool IsObsolete(BuildTargetGroup group)
    {
        var attrs = typeof(BuildTargetGroup).GetField(group.ToString())
            .GetCustomAttributes(typeof(System.ObsoleteAttribute), false);
        return attrs != null && attrs.Length > 0;
    }
}
