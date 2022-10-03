using System;
using Butter;

namespace LD51
{
	public class Explosion : GameObject
	{
		public float LifeTimer = 0.0f;
		AudioSourceComponent m_AudioSource;

		void Start()
		{
			m_AudioSource = GetComponent<AudioSourceComponent>();
			m_AudioSource.PlaySound();
		}

		void Update(float deltaTime)
		{
			LifeTimer += 1 * deltaTime;

			if (LifeTimer >= 2)
				Destroy(this);
		}
	}

}