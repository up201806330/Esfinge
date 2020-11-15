using System.Collections;
using Cinemachine;
using UnityEngine;

public class MemoryPickups : MonoBehaviour
{
    public GameObject[] memories;
    public GameObject playerCam;
    public float timeout = 5f;

    public int currentMem = 0;
    
    private void Start() {
        foreach (GameObject mem in memories) mem.SetActive(false);
        memories[0].SetActive(true);
        StartCoroutine(FirstMemTimeout());
    }

    public void onGameStart() { // Called by main menu
        
    }
    
    private IEnumerator FirstMemTimeout() {
        yield return new WaitForSeconds(timeout);
        memories[0].SetActive(false);
        Activate(currentMem = 1);
    }

    public void Activate(int memoryIndex) {
        if (memoryIndex >= memories.Length) return;
        // Sound effect

        memories[memoryIndex].SetActive(true);

        if (memoryIndex == 1) {
            StartCoroutine(SwitchCameras(memoryIndex));
            // Switch to track 1
        }
        // Switch music
    }

    private IEnumerator SwitchCameras(int memoryIndex) {
        if ((memoryIndex < memories.Length && memoryIndex >= 0)) {
            playerCam.SetActive(false); memories[memoryIndex].GetComponentInChildren<CinemachineVirtualCamera>().enabled = false;
            yield return new WaitForSeconds(3);
            memories[memoryIndex].GetComponentInChildren<CinemachineVirtualCamera>().enabled = true; playerCam.SetActive(true);
        }
    }

}
