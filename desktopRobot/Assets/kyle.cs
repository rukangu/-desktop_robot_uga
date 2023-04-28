using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGXUnity;
public class kyle : MonoBehaviour
{
    [SerializeField] Constraint jointConstraint;
    public float pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        jointConstraint.GetController<LockController>().Position = pos;
    }
}
