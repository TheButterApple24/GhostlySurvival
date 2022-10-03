using System;
using Butter;

namespace LD51
{
	public class Enemy : GameObject
	{
		public float Speed = 3.0f;
		public float Health = 100.0f;

		bool m_AwardPointOnDeath = true;
		GameObject m_PlayerObject;
		GameManager m_GameManager;
		PlayerController m_Player;
		AudioSourceComponent m_AudioSource;

		int m_EnemyIndex = -1;

		void Start()
		{
			m_PlayerObject = FindGameObjectByName("Player");
			m_GameManager = FindGameObjectByName("GameManager").As<GameManager>();
			m_Player = m_PlayerObject.As<PlayerController>();
			m_AudioSource = GetComponent<AudioSourceComponent>();

			m_EnemyIndex = m_GameManager.m_Enemies.Count;
		}

		void Update(float deltaTime)
		{
			if(Health <= 0)
			{
				if (m_AwardPointOnDeath)
					m_Player.IncrementPlayerScore();
				if(m_EnemyIndex < m_GameManager.m_Enemies.Count)
					m_GameManager.m_Enemies.RemoveAt(m_EnemyIndex);

				Destroy(this);
				return;
			}

			if (m_Player != null)
			{
				if (!m_Player.IsDead)
				{
					Vector2 currentPosition = new Vector2(Transform.Position.X, Transform.Position.Y);
					Vector2 targetPostion = new Vector2(m_PlayerObject.Transform.Position.X, m_PlayerObject.Transform.Position.Y);
					Vector2 newPos = Vector2.MoveTowards(currentPosition, targetPostion, Speed * deltaTime);

					Transform.Position = new Vector3(newPos.X, newPos.Y, 0.0f);
				}
			}
		}

		public void DamageEnemy(float damage, bool awardPoints)
		{
			m_AwardPointOnDeath = awardPoints;
			Health -= damage;
			m_AudioSource.PlaySound();
		}

		void OnCollisionBegin2D(ulong other)
		{
			GameObject otherObject = FindGameObjectByUUID(other);

			if(otherObject != null)
			{
				if(otherObject.Tag == "Player")
				{
					PlayerController player = otherObject.As<PlayerController>();

					player.DamagePlayer();
				}
			}
		}
	}

}