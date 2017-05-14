using UnityEngine;
using System.Collections;
using System;

public class fusion_tweener : MonoBehaviour {
    ParticleEmitter[] emitters;
    ParticleAnimator[] animators;
    ParticleSystem[] systems;
	// Use this for initialization
	void Start () {
        emitters = GetComponentsInChildren<ParticleEmitter>();
        animators = GetComponentsInChildren<ParticleAnimator>();
        systems = GetComponentsInChildren<ParticleSystem>();
        start_time = Program.TimePassed();
        foreach (ParticleSystem system in systems)
        {
            system.startSpeed *= scaleFactor;
            system.startSize *= scaleFactor;
            system.gravityModifier *= scaleFactor;
        }
        //apply scaling to emitters
        foreach (ParticleEmitter emitter in emitters)
        {
            emitter.minSize *= scaleFactor;
            emitter.maxSize *= scaleFactor;
            emitter.worldVelocity *= scaleFactor;
            emitter.localVelocity *= scaleFactor;
            emitter.rndVelocity *= scaleFactor;
        }

        //apply scaling to animators
        foreach (ParticleAnimator animator in animators)
        {
            animator.force *= scaleFactor;
            animator.rndForce *= scaleFactor;
        }
	}
    int step = 1;
    float scaleFactor = 0.1f;
    int start_time = 0;
	// Update is called once per frame
    void Update()
    {
        if (Program.TimePassed() - start_time > 0)
        {
            step = 1;
        }
        if (Program.TimePassed() - start_time > 1500)
        {
            step = 2;
        }
        if (Program.TimePassed() - start_time > 3000)
        {
            step = 3;
        }
        if (Program.TimePassed() - start_time > 3500)
        {
            step = 4;
        }

        if (step == 1)
        {
            scaleFactor = 1 + Time.deltaTime*1.5f;
        }
        if (step == 2)
        {
            scaleFactor = 1f;
        }
        if (step == 3)
        {
            scaleFactor = 0.2f;
        }
        if (step == 4)
        {
            Destroy(gameObject);
            return;
        }
        foreach (ParticleSystem system in systems)
        {
            system.startSpeed *= scaleFactor;
            system.startSize *= scaleFactor;
            system.gravityModifier *= scaleFactor;
        }
        //apply scaling to emitters
        foreach (ParticleEmitter emitter in emitters)
        {
            emitter.minSize *= scaleFactor;
            emitter.maxSize *= scaleFactor;
            emitter.worldVelocity *= scaleFactor;
            emitter.localVelocity *= scaleFactor;
            emitter.rndVelocity *= scaleFactor;
        }

        //apply scaling to animators
        foreach (ParticleAnimator animator in animators)
        {
            animator.force *= scaleFactor;
            animator.rndForce *= scaleFactor;
        }
    }
}
