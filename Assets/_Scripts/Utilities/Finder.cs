using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finder
{

    public static GameObject FindGameObjectByTag(string tag)
    {
        var scene = SceneManager.GetActiveScene();
        var allGameObjects = scene.GetRootGameObjects();
        foreach (var child in allGameObjects)
        {
            foreach (Transform grandchild in child.transform)
            {
                if (grandchild.tag.CompareTo(tag) == 0)
                {
                    return grandchild.gameObject;
                }
            }
        }
        return null;
    }
    public static List<GameObject> FindGameObjectsByTag(string tag)
    {
        List<GameObject> list = new();
        var scene = SceneManager.GetActiveScene();
        var allGameObjects = scene.GetRootGameObjects();
        foreach (var child in allGameObjects)
        {
            var childrenByTag = FindGameObjectsByTag(tag, child.transform);
            list.AddRange(childrenByTag);
        }

        return list;
    }

    public static List<GameObject> FindGameObjectsByTag(string tag, Transform branch)
    {
        List<GameObject> list = new();
        foreach (Transform child in branch)
        {
            if (child.tag == tag)
            {
                list.Add(child.gameObject);
            }

            if (child.childCount > 0)
            {
                var grandchildren = FindGameObjectsByTag(tag, child);
                list.AddRange(grandchildren);
            }
        }

        return list;
    }

}
