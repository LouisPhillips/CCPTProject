using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceChange : MonoBehaviour
{
    private GameObject player;
    public enum enumState { Concrete, Grass, Dirt , Custom};
    public enumState state;

    bool onSurface;

    public Material concreteMaterial;
    public Material grassMaterial;
    public Material dirtMaterial;
    public Material customMaterial;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");


    }

    private void Update()
    {
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
            case enumState.Custom:
                {
                    transform.GetComponent<MeshRenderer>().material = customMaterial;
                        return;
                }
        }

    }
}
