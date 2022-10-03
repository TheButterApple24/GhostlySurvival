using System;
using Butter;

namespace LD51
{
	public class Projectile : GameObject
	{
		public float m_ProjectileDamage = 50.0f;
		public float m_Speed = 50.0f;

		Rigidbody2DComponent m_RigidBody2D;
		Prefab m_ExplosionPrefab;
		Vector2 m_FireDirection;

		bool m_Fired = false;

		void Start()
		{
			m_RigidBody2D = GetComponent<Rigidbody2DComponent>();

			m_FireDirection = Input.GetMouseWorldPosition();

			m_FireDirection.X -= Transform.Position.X;
			m_FireDirection.Y -= Transform.Position.Y;

			m_FireDirection.Normalize();

			m_FireDirection.X *= m_Speed;
			m_FireDirection.Y *= m_Speed;

			m_ExplosionPrefab = new Prefab("Assets/Prefabs/Explosion.prefab");
		}

		void Update(float deltaTime)
		{
			
		}

		void FixedUpdate(float fixedDeltaTime)
		{
			if(m_Fired == false)
			{
				m_RigidBody2D.ApplyForceToCenter(m_FireDirection);
				m_Fired = true;
			}
		}

        public void OnTriggerBegin2D(ulong other)
        {
            GameObject otherObject = FindGameObjectByUUID(other);

            if (otherObject != null)
            {
                if (otherObject.GetComponent<TagComponent>().GetTag() == "Enemy")
                {
                    Enemy enemy = otherObject.As<Enemy>();
                    if (enemy != null)
                    {
                        Instantiate(m_ExplosionPrefab);
                        enemy.DamageEnemy(m_ProjectileDamage, true);
                        Destroy(this);
                    }
                }
                else if (otherObject.GetComponent<TagComponent>().GetTag() == "Projectile")
                {
                    // Do nothing
                }
                else if (otherObject.GetComponent<TagComponent>().GetTag() != "Player")
                {
                    Instantiate(m_ExplosionPrefab);
                    Destroy(this);
                }
            }

        }

		public void SetDamage(float damage)
		{
			m_ProjectileDamage = damage;
		}
	}

}