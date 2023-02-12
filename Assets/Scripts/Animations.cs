using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private GameObject skateboard;
    private GameObject skateboardMotions;
    void Start()
    {
        skateboard = GameObject.FindGameObjectWithTag("Skateboard");
        skateboardMotions = GameObject.FindGameObjectWithTag("SkateboardMotions");
    }

    // Update is called once per frame
    void Update()
    {
        if (skateboard.GetComponent<PlayerMovement4>().push && PlayerMovement4.mainCamOn)
        {
            GetComponent<Animator>().Play("Pushing");
        }



        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Riding"))
        {
            PlayerMovement4.riding = true;
            if (skateboard.GetComponent<PlayerMovement4>().olliePressed)
            {
                GetComponent<Animator>().Play("Ollie");
                skateboardMotions.GetComponent<Animator>().Play("Ollie");
            }
        }
        else
        {
            PlayerMovement4.riding = false;
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Ollie"))
        {
            PlayerMovement4.ollie = true;
        }
        else
        {
            PlayerMovement4.ollie = false;
        }
    }
}
