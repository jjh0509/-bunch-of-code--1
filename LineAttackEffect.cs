using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAttackEffect : MonoBehaviour
{
    LineRenderer line;
    ParticleSystem particle;

    [HideInInspector] public float length;
    [Header("Settings")]
    private float widthlerp;
    public float TimeToDestroy;
    public float lifeTime;
    public float fadetime;
    public float fadespeed;
    public float disappearSpeed;
    public float particleRate;
    public float particleRadius;

    private float fadedeltatime;

    public bool faded;

    // Start is called before the first frame update
    private void Start()
    {
        faded = false;
        widthlerp = 0.1f;
        fadedeltatime = 0;
        lifeTime = 0;
        line = GetComponent<LineRenderer>();
        particle = GetComponentInChildren<ParticleSystem>();

        var newshape = particle.shape;
        newshape.scale = new Vector3(particleRadius, length, particleRadius);

        var newemmision = particle.emission;

        ParticleSystem.Burst newburst = new ParticleSystem.Burst(0, (short)(length * particleRate), (short)(length * particleRate * 2));
        newemmision.SetBurst(0,newburst);
        particle.transform.localPosition = new Vector3(0, 0, length / 2);

        particle.Play();
    }

    private void Update()
    {
        if (!faded)
        {
            widthlerp = Mathf.Lerp(widthlerp, line.endWidth, fadespeed);
            line.SetWidth(widthlerp, widthlerp);
            fadedeltatime += Time.deltaTime;
            if (fadedeltatime > fadetime)
            {
                faded = true;
            }
        }
        else
        {
            lifeTime += Time.deltaTime;
            widthlerp = Mathf.Lerp(widthlerp, 0, disappearSpeed);
            line.SetWidth(widthlerp, widthlerp);
            if(lifeTime > TimeToDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
