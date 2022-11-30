using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private GameObject skateboard;
    void Start()
    {
        skateboard = GameObject.FindGameObjectWithTag("Skateboard");
    }

    // Update is called once per frame
    void Update()
    {
        if (skateboard.GetComponent<PlayerMovement4>().push)
        {
            Debug.Log("pushed");
            GetComponent<Animator>().SetTrigger("Push");
        }
    }
}
