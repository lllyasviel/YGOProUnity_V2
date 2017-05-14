using UnityEngine;
using System.Collections;

public class ParticleScale : MonoBehaviour {
	

	public float scaleStep = 0.1f;

	private ParticleSystem _particle = null;



	public void UpdateScale(float mod) {

		if(particle == null) {
			return;
		}

		particle.startSize = particle.startSize + particle.startSize * mod;
		particle.startSpeed = particle.startSpeed + particle.startSpeed * mod;
		
	}


	public void ReduceScale(float mod) {
		
		if(particle == null) {
			return;
		}


		particle.startSize = particle.startSize - particle.startSize * mod;
		particle.startSpeed = particle.startSpeed - particle.startSpeed * mod;
		
	}


	public ParticleSystem particle {
		get {
			if(_particle == null) {
				_particle = GetComponent<ParticleSystem>();
			}
			
			return _particle;
		}
	}
}
