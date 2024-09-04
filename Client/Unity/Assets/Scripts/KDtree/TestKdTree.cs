using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestKdTree : MonoBehaviour {

    public List<EntityUnit> list_EntityUnity = new List<EntityUnit>();

    // Use this for initialization
    void Start () {
        KdTree<EntityUnit> testKdTrees = new KdTree<EntityUnit>();
        testKdTrees.AddAll(list_EntityUnity);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
