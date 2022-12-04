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
        if (skateboard.GetComponent<PlayerMovement4>().push && PlayerMovement4.MainCamOn)
        {
            GetComponent<Animator>().Play("Pushing");
        }



        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Riding"))
        {
            PlayerMovement4.riding = true;
            if (skateboard.GetComponent<PlayerMovement4>().olliePressed)
            {
                GetComponent<Animator>().Play("Ollie");
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
