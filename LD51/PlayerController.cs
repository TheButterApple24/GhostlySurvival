using System;
using Butter;

namespace LD51
{
	public class PlayerController : GameObject
	{
		public float Speed = 250.0f;
		public float Health = 100.0f;
		public float Score = 0.0f;
		public bool IsDead = false;

		Rigidbody2DComponent m_Rb;
		AudioSourceComponent m_AudioSource;
		MultiSpriteRendererComponent m_Sprite;
		Vector2 m_Inputs = Vector2.Zero;
		Prefab m_ProjectilePrefab;
		bool m_CanFire = true;

		bool m_ResettingGame = false;

		bool m_IsHit = false;
		float m_HitTimer = 2.0f;

		// Weapon
		float m_CooldownTimer = 0.0f;
		float m_CooldownTime = 0.3f;

		// UI
		TextRendererComponent m_ScoreText;
		TextRendererComponent m_HealthText;

		void Start()
		{
			m_Rb = GetComponent<Rigidbody2DComponent>();
			m_AudioSource = GetComponent<AudioSourceComponent>();
			m_Sprite = GetComponent<MultiSpriteRendererComponent>();

			m_ProjectilePrefab = new Prefab("Assets/Prefabs/Projectile.prefab");
		}

		void Update(float deltaTime)
		{
			m_Inputs = Vector2.Zero;

			if (m_ScoreText == null)
			{
				m_ScoreText = FindGameObjectByName("TimeText").GetComponent<TextRendererComponent>();
				m_HealthText = FindGameObjectByName("HealthText").GetComponent<TextRendererComponent>();
				m_ScoreText.SetText("Score: " + Score);
				m_HealthText.SetText("Health: " + Health);
			}

			if (Health <= 0.0f && !IsDead)
			{
				IsDead = true;
				m_Sprite.SetTextureIndex(1);
			}

			if (m_IsHit)
			{
				m_HitTimer -= deltaTime * 1;

				if (m_HitTimer <= 0.0f)
				{
					m_IsHit = false;
					m_HitTimer = 2.0f;
				}
			}

			if (m_ResettingGame)
			{
				m_Rb.SetBodyType(Rigidbody2DComponent.BodyType.DYNAMIC);
				m_ResettingGame = false;
				IsDead = false;
			}

			if (m_CanFire == false)
				m_CooldownTimer += 1 * deltaTime;

			if (m_CooldownTimer > m_CooldownTime)
			{
				m_CanFire = true;
				m_CooldownTimer = 0;
			}

			if (!IsDead)
			{
				if (Input.IsKeyDown(KeyCode.W))
				{
					m_Inputs.Y = 1;
				}
				else if (Input.IsKeyDown(KeyCode.S))
				{
					m_Inputs.Y = -1;
				}

				if (Input.IsKeyDown(KeyCode.D))
				{
					m_Inputs.X = 1;
				}
				else if (Input.IsKeyDown(KeyCode.A))
				{
					m_Inputs.X = -1;
				}

				if (Input.IsMouseButtonDown(MouseCode.ButtonLeft))
				{
					Transform spawnTransform = new Transform();
					spawnTransform.Position = Transform.Position;
					if (FireWeapon(spawnTransform))
						m_AudioSource.PlaySound();
				}
			}
		}

		void FixedUpdate(float fixedDeltaTime)
		{
			m_Rb.ApplyForceToCenter(m_Inputs * fixedDeltaTime * Speed);
		}

		public void DamagePlayer()
		{
			if (!m_IsHit)
			{
				m_IsHit = true;
				Health -= 5.0f;
				m_HealthText.SetText("Health: " + Health);
			}
		}

		public void IncrementPlayerScore()
		{
			Score += 1.0f;
			m_ScoreText.SetText("Score: " + Score);
		}

		public bool FireWeapon(Transform transform)
		{
			if (m_CanFire)
			{
				GameObject proj = Instantiate(m_ProjectilePrefab, transform);

				if (proj != null)
				{
					proj.As<Projectile>().SetDamage(50.0f);
					m_CanFire = false;
					return true;

				}

				return false;
			}

			return false;
		}

		public void ResetPlayer()
		{
			Health = 100.0f;

			m_Rb.SetBodyType(Rigidbody2DComponent.BodyType.KINEMATIC);
			Transform.Position = new Vector3(0, 0, 0);

			m_ResettingGame = true;

			Score = 0;

			m_ScoreText.SetText("Score: " + Score);
			m_HealthText.SetText("Health: " + Health);

			m_Sprite.SetTextureIndex(0);
		}
	}

}