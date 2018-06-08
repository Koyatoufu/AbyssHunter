using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public static class AssetCreator
{
    public static void CreateAsset<T>(string subPath) where T:ScriptableObject
    {
        string path = "Assets/Resources/Scriptable/"+subPath;
        string fullName = path + typeof(T) + ".asset";

        T tAsset = ScriptableObject.CreateInstance<T>();

        if (AssetDatabase.LoadAssetAtPath(fullName, typeof(GameObject)))
        {
            Debug.Log(typeof(T).ToString() + " already created!");
        }
        else
        {
            AssetDatabase.CreateAsset(tAsset, fullName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = tAsset;
        }
    }

    public static void SaveCharcterPrefab(GameObject source, string name)
    {
        string path = "Assets/Resources/Prefabs/Player/Set/";
        string fullName = path + name + ".prefab";

        if (AssetDatabase.LoadAssetAtPath(fullName, typeof(GameObject)))
        {
            Object prefab = PrefabUtility.CreatePrefab(fullName, source);
            PrefabUtility.ReplacePrefab(source, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }
        else
        {
            AssetDatabase.CreateAsset(source, fullName);
        }
    }

#region ItemScritpable
    [MenuItem("Assets/Create/Custom/Item/Weapon")]
    public static void CreateWeapon()
    {
        CreateAsset<CWeapon>("Item/Weapon/");
    }
    [MenuItem("Assets/Create/Custom/Item/Wearable")]
    public static void CreateWearable()
    {
        CreateAsset<CWearable>("Item/Wearable");
    }
    [MenuItem("Assets/Create/Custom/Item/UseItem")]
    public static void CreateUseItem()
    {
        CreateAsset<CUseItem>("Item/UseItem/");
    }
    [MenuItem("Assets/Create/Custom/Item/Spell")]
    public static void CreateSpellItem()
    {
        CreateAsset<CSpellItem>("Item/Spell/");
    }
#endregion

    [MenuItem("Assets/Create/Custom/Action")]
    public static void CreateAction()
    {
        CreateAsset<CAction>("Action/");
    }

    [MenuItem("Assets/Create/Custom/Unit")]
    public static void CreateUnitScript()
    {
        CreateAsset<CUnitData>("Unit/");
    }
}
#endif