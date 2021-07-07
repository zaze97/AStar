using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   private int moveRange = 5;

   public void GetMovePath(System.Action<Path> OnPathSerchOkCallBack)
   {
      var moveGSore = this.moveRange * 1000 * 3;
      //var  SerchPath=MoveRangConStantPath
   }
}
