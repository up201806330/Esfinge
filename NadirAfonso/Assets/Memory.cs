using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    MemoryPickups father;
    // Start is called before the first frame update
    void Start()
    {
        father = GetComponentInParent<MemoryPickups>();
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("HERERER");
        father.memories[father.currentMem].SetActive(false);
        father.Activate(++father.currentMem);
    }
}
