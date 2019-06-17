using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTreeOffsets : MonoBehaviour
{

    Terrain terrain;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();

        foreach (TreePrototype tree in terrain.terrainData.treePrototypes)
        {
            foreach (Material material in tree.prefab.gameObject.GetComponent<Renderer>().materials)
            {
                material.SetFloat("_Offset", 1);
            }       //tree.prefab.gameObject.GetComponent<Renderer>().materials
        }
    }
}
