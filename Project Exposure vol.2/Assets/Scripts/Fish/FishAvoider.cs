using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAvoider : FishBehaviourParent
{
    private void Start()
    {
        SingleTons.FishManager.AddGlobalFishAvoider(this);
        SingleTons.FishManager.AddAvoidableCreature(_thisCreatureType, this);
        Debug.Log("Added " + gameObject.name + " to " + _thisCreatureType);
    }
}
