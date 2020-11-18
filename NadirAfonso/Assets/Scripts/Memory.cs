using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    MemoryPickups controller;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<MemoryPickups>();
    }

    private void OnTriggerEnter(Collider other) {
        controller.memories[controller.currentMem].SetActive(false);
        controller.Activate(++controller.currentMem);
    }

}
