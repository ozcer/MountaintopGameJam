using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public Player player;

    public void Save(float xPos, float yPos)
    {
        PlayerPrefs.SetFloat("X", xPos);
        PlayerPrefs.SetFloat("Y", yPos);
    }

    public void Load()
    {
        var xPos = PlayerPrefs.GetFloat("X");
        var yPos = PlayerPrefs.GetFloat("Y");
        var pos = new Vector2(xPos, yPos);
        player.transform.position = pos;
    }
}
