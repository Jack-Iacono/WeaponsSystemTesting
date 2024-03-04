using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotParticleController : MonoBehaviour
{
    private ParticleSystem part;

    public void Initialize()
    {
        part = GetComponent<ParticleSystem>();
    }
    public void StartParticle()
    {
        part.Play();
    }
    private void OnParticleSystemStopped()
    {
        // Disables itself
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
