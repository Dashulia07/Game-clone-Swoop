using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public FlightController flightController;
    public AudioSource audioSource;
    public float aroundMapAngle;
    public float height;
    public float destroyDistance = 0.2f;
    public bool destroyOnCollision = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && destroyOnCollision)
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!flightController.isAlive) { return; }
        if (aroundMapAngle - flightController.aroundMapAngle < -destroyDistance)
        {
            Destroy(this.gameObject);
        }
    }
}