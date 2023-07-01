using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdpoint : MonoBehaviour
{
   public float lifeSpan = 5f;

   void Start()
   {
      Destroy(gameObject, lifeSpan);
   }
}
