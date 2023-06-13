using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    public float WaveHeight = 3.0f;
    public float WaveDuration = 3.0f;

    public AudioSource WaveEmitter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Animate the waves
        float time = Time.time;
        float oceanHeight = Mathf.Sin((time / WaveDuration) * 2.0f * Mathf.PI) * WaveHeight;
        transform.position = new Vector3(0.0f, oceanHeight, 0.0f);

        // Position the wave audio below the camera
        if (WaveEmitter != null)
        {
            var listener = GameObject.FindObjectOfType<AudioListener>();
            var listenerPos = listener.transform.position;
            WaveEmitter.transform.position = new Vector3(listenerPos.x, oceanHeight, listenerPos.z);
        }
    }
}
