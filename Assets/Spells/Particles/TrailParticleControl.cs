using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailParticleControl : MonoBehaviour
{
    public SpriteRenderer Spr;
    public Rigidbody2D RB;
    public Color particleColor = new Color(1f, 1f, 1f, 1f);
    public Color color;

    public float fadeTimeDelay = 0.05f;
    public float colorAlphaStep = 0.05f;
    public float gravityAcceleration = -1f;

    public bool HasGravity = true;

    private void Awake()
    {
        Spr = this.gameObject.GetComponent<SpriteRenderer>();
        RB = this.gameObject.GetComponent<Rigidbody2D>();
    }


    private void OnEnable()
    {
        Spr.color = particleColor;
        RB.gravityScale = gravityAcceleration;
    }


    private void FixedUpdate()
    {
        if (Spr.color.a < colorAlphaStep)
        {
            this.gameObject.SetActive(false);

            color = Spr.color;
            color.a = 1f;
            Spr.color = color;
        }
    }


    public IEnumerator FadeTimer()
    {
        while(this.gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(fadeTimeDelay);
            color = Spr.color;
            color.a -= colorAlphaStep;
            Spr.color = color;
        }
    }
}
