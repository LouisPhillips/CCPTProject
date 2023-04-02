using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Animator animator = null;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    private GameObject skateboard;

    public Transform forcePoint;
    private void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        skateboard = GameObject.FindGameObjectWithTag("Skateboard");
        ToggleRagdoll(false);
    }

    public void Fall()
    {
        ToggleRagdoll(true);
        transform.parent = null;
        skateboard.GetComponent<PlayerMovement4>().respawning = true;
        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.AddExplosionForce(1 * skateboard.GetComponent<PlayerMovement4>().getSpeed, forcePoint.position, 5f, 0f, ForceMode.Impulse);
        }
    }

    public void ToggleRagdoll(bool state)
    {
        animator.enabled = !state;

        foreach (Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }

        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = state;
        }

    }
}
