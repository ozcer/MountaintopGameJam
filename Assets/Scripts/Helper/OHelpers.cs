using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;



public static class OHelpers
{
    public static Vector2 Change(this Vector2 org, object x = null, object y = null, object z = null)
    {
        return new Vector2(
            (x == null ? org.x: (float) x), 
            (y == null ? org.y: (float) y));
    }

    public static Vector3 Change(this Vector3 org, object x = null, object y = null, object z = null)
    {
        return new Vector3(
            (x == null ? org.x: (float) x), 
            (y == null ? org.y: (float) y),
            (z == null ? org.z: (float) z));
    }

    /// <summary>
    /// Return new vector with component changed relatively
    /// </summary>
    /// <param name="org">original vector</param>
    /// <param name="x">x value to change relatively</param>
    /// <param name="y">y value to change relatively</param>
    /// <param name="z">z value to change relatively</param>
    /// <returns></returns>
    public static Vector3 DChange(this Vector3 org, object x = null, object y = null, object z = null)
    {
        return new Vector3(
            (x == null ? org.x: org.x + (float) x), 
            (y == null ? org.y: org.y + (float) y),
            (z == null ? org.z: org.z +(float) z));
    }

    // Return color with component changed
    public static Color Change(this Color org, object r = null, object g = null, object b = null, object a = null)
    {
        return new Color(
            r == null ? org.r : (float) r,
            g == null ? org.g : (float) g,
            b == null ? org.b : (float) b,
            a == null ? org.a : (float) a);
    }
    
    // Random color
    public static Color RandomColor()
    {
        return new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f));
    }
    

    public static T GetRandom<T>(this List<T> og)
    {
        return og[UnityEngine.Random.Range(0, og.Count)];
    }
    
    // Get unique random elements from list
    public static List<T> GetRandom<T>(this List<T> og, int count)
    {
        List<T> result = new();
        List<int> indexes = Enumerable.Range(0, og.Count).ToList();
        indexes.Shuffle();
        Debug.Log(indexes);
        for (int i = 0; i < count; i++)
        {
            int index = indexes[i];
            result.Add(og[index]);
        }

        return result;
    }
    
    public static List<int> ShuffledIndices(int count)
    {
        List<int> result = Enumerable.Range(0, count).ToList();
        result.Shuffle();
        return result;
    }

    public static Bounds CombinedBounds(this Transform og)
    {
        Bounds combinedBounds = new Bounds(og.position, Vector3.one);
        Renderer[] colliders = og.GetComponentsInChildren<Renderer>();
        foreach(Renderer collider in colliders) {
            combinedBounds.Encapsulate(collider.bounds);
        }

        return combinedBounds;
    }

    public static bool IsWithin(this int self, int left, int right)
    {
        return self >= left && self <= right;
    }

    public static List<Transform> GetChildren(this Transform self)
    {
        List<Transform> children = new();
        foreach (Transform child in self)
        {
            children.Add(child);
        }

        return children;
    }
    
    public static void ShuffleChildren(this Transform self)
    {
        List<Transform> children = self.GetChildren();
        children.Shuffle();
        foreach (Transform child in children)
        {
            child.SetAsLastSibling();
        }
    }

    public static void DestroyAllChildren(this Transform self)
    {
        foreach (Transform child in self)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    public static float Sign(this float self)
    {
        return self >= 0 ? 1 : -1;
    }

    public static void DrawGizmosArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
    
        Gizmos.DrawRay(pos, direction);
   
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }

    public static IEnumerator ExecuteWithDelay(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        var random = new System.Random();
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = random.Next(n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }

    public static List<T> ShuffledCopy<T>(this IList<T> list)
    {
        List<T> copied = new List<T>(list);
        copied.Shuffle();
        return copied;
    }
    
    public static void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static IEnumerator DelayedAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
    
}

