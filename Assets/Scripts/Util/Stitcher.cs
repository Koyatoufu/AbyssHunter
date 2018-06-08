using UnityEngine;
using System.Collections.Generic;

public class Stitcher
{
    public GameObject Stitch(GameObject source,GameObject target)
    {
        TransformCatalog catalog = new TransformCatalog(target.transform);
        SkinnedMeshRenderer[] skinnedMeshes = source.GetComponentsInChildren<SkinnedMeshRenderer>();
        GameObject cloth = AddNewChild(source,target.transform);
        
        for(int i=0;i<skinnedMeshes.Length;i++)
        {
            SkinnedMeshRenderer renderer = AddSkinnedMeshRenderer(skinnedMeshes[i], cloth);
            renderer.bones = TranslateTransforms(skinnedMeshes[i].bones, catalog);
        }

        return cloth;
    }

    private GameObject AddNewChild(GameObject source, Transform parent)
    {
        GameObject newChild = new GameObject(source.name);
        newChild.transform.SetParent(parent);
        newChild.transform.localPosition = source.transform.localPosition;
        newChild.transform.localRotation = source.transform.localRotation;
        newChild.transform.localScale = source.transform.localScale;
        newChild.layer = LayerMask.NameToLayer("Player");
        return newChild;
    }

    private SkinnedMeshRenderer AddSkinnedMeshRenderer(SkinnedMeshRenderer renderer,GameObject parent)
    {
        GameObject skin = new GameObject(renderer.gameObject.name);
        skin.transform.parent = parent.transform;
        skin.layer = LayerMask.NameToLayer("Player");
        SkinnedMeshRenderer newSkin = skin.AddComponent<SkinnedMeshRenderer>();
        newSkin.sharedMesh = renderer.sharedMesh;
        newSkin.sharedMaterials = renderer.sharedMaterials;
        return newSkin;
    }

    private Transform[] TranslateTransforms(Transform[] source, TransformCatalog catalog)
    {
        Transform[] newTrans = new Transform[source.Length];

        for(int i=0;i<newTrans.Length;i++)
        {
            newTrans[i] = DictionaryExtensions.Find(catalog, source[i].name);
        }

        return newTrans;
    }

    private class TransformCatalog:Dictionary<string,Transform>
    {
        public TransformCatalog(Transform transform)
        {
            Catalog(transform);
        }

        private void Catalog(Transform transform)
        {
            if(ContainsKey(transform.name))
            {
                Remove(transform.name);
                Add(transform.name, transform);
            }
            else
            {
                Add(transform.name, transform);
            }

            foreach(Transform child in transform)
            {
                Catalog(child);
            }
        }
    }

    private class DictionaryExtensions
    {
        public static TValue Find<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key)
        {
            TValue value;
            source.TryGetValue(key, out value);
            return value;
        }
    }
}
