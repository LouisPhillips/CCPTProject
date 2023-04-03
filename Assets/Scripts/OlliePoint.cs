using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlliePoint : MonoBehaviour
{
    public GameObject characterPos;

    private void Update()
    {
        transform.position = characterPos.transform.position;
    }
}
