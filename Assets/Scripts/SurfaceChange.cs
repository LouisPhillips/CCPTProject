using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceChange : MonoBehaviour
{
    private GameObject player;
    public enum enumState { Concrete, Grass, Dirt };
    public enumState state;

    bool onSurface;

    public Material concreteMaterial;
    public Material grassMaterial;
    public Material dirtMaterial;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        switch (state)
        {
            case enumState.Concrete:
                {
                    transform.GetComponent<MeshRenderer>().material = concreteMaterial;
                    return;
                }
            case enumState.Dirt:
                {
                    transform.GetComponent<MeshRenderer>().material = dirtMaterial;
                    return;
                }
            case enumState.Grass:
                {
                    transform.GetComponent<MeshRenderer>().material = grassMaterial;
                    return;
                }
        }
    }

    private void Update()
    {

        
    }
}
