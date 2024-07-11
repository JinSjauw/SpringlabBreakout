using System;
using UnityEngine;

[RequireComponent(typeof(SpringComponent))]
public class Wall : MonoBehaviour
{
    private SpringComponent wallSpringComponent;
    private Vector3 startScale;
    
    private void Awake()
    {
        wallSpringComponent = GetComponent<SpringComponent>();
        wallSpringComponent.SetEquilibriumPosition(1);
        startScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = new Vector3(startScale.x * wallSpringComponent.SpringValue, startScale.y, startScale.z);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Ball"))
        {
            wallSpringComponent.Nudge(0.75f);
        }
    }
}
