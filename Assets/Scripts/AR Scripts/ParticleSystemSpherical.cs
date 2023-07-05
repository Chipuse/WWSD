using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleSystemSpherical : MonoBehaviour
{
    public ParticleSystem pSystem;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ParticleGO(Random.value, Random.Range(-1f, 1f));
    }

    

    private void ParticleGO(float yaw, float pitch)
    {
        var posModifier = new Vector3(Mathf.Sin(yaw * Mathf.PI) * Mathf.Cos(pitch * Mathf.PI), Mathf.Sin(pitch * Mathf.PI), Mathf.Cos(yaw * Mathf.PI) * Mathf.Cos(pitch * Mathf.PI));
        pSystem.Emit(new ParticleSystem.EmitParams() { position = Random.insideUnitSphere, startColor = Random.ColorHSV(1f, 1f, 1f, 1f, 0.5f, 1f) }, 1);
    }
}
