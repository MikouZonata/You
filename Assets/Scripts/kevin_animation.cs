using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kevin_animation : MonoBehaviour
{
    private Vector3 origine;
    public float snelheid, amplitude;
    // Update is called once per frame
    private void Awake()
    {
        origine = transform.localPosition;
    }
    void Update()
    {
        transform.localPosition = origine + new Vector3(0, amplitude * Mathf.Sin(Time.time * snelheid), 0);
    }
}
