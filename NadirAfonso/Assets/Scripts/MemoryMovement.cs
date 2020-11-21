using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryMovement : MonoBehaviour
{
    public Transform playerTransf;
    public float amplitude = 2f;
    public float speed = 2f;
    float y0;
    Vector3 s0;

    public float maxDistance = 50f;
    public float minSize = 0.5f;
    public float maxSize = 1.5f;

    private void Start() {
        y0 = transform.position.y;
        s0 = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newScale = transform.localScale;
        newScale = s0 * Mathf.Lerp(minSize, maxSize, Vector3.Distance(playerTransf.position, transform.position) / maxDistance);
        transform.localScale = newScale;

        Vector3 newPos = transform.position;
        newPos.y = y0 + amplitude * Mathf.Sin(speed * Time.time);
        transform.position = newPos;
    }
}
