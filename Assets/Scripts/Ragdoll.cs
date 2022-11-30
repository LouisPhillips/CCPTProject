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

    public void Die()
    {
        ToggleRagdoll(true);
        transform.parent = null;

        foreach(Rigidbody rb in ragdollBodies)
        {
            skateboard.GetComponent<PlayerMovement4>().respawning = true;
            rb.AddExplosionForce(1 * skateboard.GetComponent<PlayerMovement4>().getSpeed, forcePoint.position, 5f, 0f, ForceMode.Impulse);
        }

        //add explosion force based on speed
    }

    public void ToggleRagdoll(bool state)
    {
        animator.enabled = !state;

        foreach(Rigidbody rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }

        foreach(Collider collider in ragdollColliders)
        {
            collider.enabled = state;
        }

    }
}
