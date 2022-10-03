using System;
using System.Collections.Generic;
using Butter;
using Random = Butter.Random;

namespace LD51
{

	public class GameManager : GameObject
	{
		public float EnemySpawnTimer;
		public float LevelChangeTimer = 10.0f;

		Prefab m_EnemyPrefab;
		Prefab m_LevelGeoPrefab;

		List<GameObject> m_Enemies;
		List<GameObject> m_WorldObjects;

		float m_TimeToSpawnEnemy = 5.0f;

		bool m_ResetPressed = false;
		float m_ResetTimer = 1.0f;

		void Start()
		{
			m_EnemyPrefab = new Prefab("Assets/Prefabs/Enemy.prefab");
			m_LevelGeoPrefab = new Prefab("Assets/Prefabs/LevelGeo.prefab");

			m_Enemies = new List<GameObject>();
			m_WorldObjects = new List<GameObject>();

			ChangeLevel();
		}

		void Update(float deltaTime)
		{
			if (m_ResetPressed)
			{
				m_ResetTimer -= 1 * deltaTime;

				if (m_ResetTimer <= 0)
				{
					m_ResetPressed = false;
					m_ResetTimer = 1.0f;
				}
			}
			else
			{
				EnemySpawnTimer += 1 * deltaTime;

				if (EnemySpawnTimer >= m_TimeToSpawnEnemy)
				{
					SpawnEnemy();
					EnemySpawnTimer = 0;
				}

				LevelChangeTimer -= 1 * deltaTime;

				if (LevelChangeTimer <= 0)
				{
					ChangeLevel();
					LevelChangeTimer = 10;
					m_TimeToSpawnEnemy -= 0.5f;
				}

				if (Input.IsKeyDown(KeyCode.R) && !m_ResetPressed)
				{
					ResetGame();
					m_ResetPressed = true;
				}
			}
		}

		void SpawnEnemy()
		{
			Transform spawnPos = new Transform();
			float spawnX = 0.0f;
			float spawnY = 0.0f;
			while (spawnX > -8 && spawnX < 8)
			{
				spawnX = Random.Range(-10.0f, 11.0f);
			}

			while (spawnY > -5 && spawnY < 5)
			{
				spawnY = Random.Range(-10.0f, 11.0f);
			}

			spawnPos.Position = new Vector3(spawnX, spawnY, 0.0f);
			spawnPos.Scale = new Vector3(1.0f, 1.0f, 1.0f);
			GameObject go = Instantiate(m_EnemyPrefab, spawnPos);
			m_Enemies.Add(go);
		}

		void ChangeLevel()
		{
			if (m_WorldObjects.Count > 0)
			{
				for (int i = 0; i < m_WorldObjects.Count; i++)
				{
					Destroy(m_WorldObjects[i]);
				}

				m_WorldObjects.Clear();
			}

			for (int i = 0; i < Random.Range(3, 6); i++)
			{
				Transform spawnPos = new Transform();
				spawnPos.Position = new Vector3(Random.Range(-7, 8.0f), Random.Range(-4, 5.0f), 0.0f);

				int scaleX, scaleY;

				scaleX = (int)Random.Range(1.0f, 3.0f);
				scaleY = (int)Random.Range(1.0f, 3.0f);

				spawnPos.Scale = new Vector3(scaleX, scaleY, 1.0f);

				GameObject obj = Instantiate(m_LevelGeoPrefab, spawnPos);

				obj.GetComponent<SpriteRendererComponent>().SetTilingFactor(new Vector2(spawnPos.Scale.X, spawnPos.Scale.Y));

				m_WorldObjects.Add(obj);
			}
		}

		void ResetGame()
		{
			m_TimeToSpawnEnemy = 5.0f;


			if (m_Enemies.Count > 0)
			{
				for (int i = 0; i < m_Enemies.Count; i++)
				{
					m_Enemies[i].As<Enemy>().DamageEnemy(1000.0f, false);
				}

				m_Enemies.Clear();
			}

			ChangeLevel();

			PlayerController player = FindGameObjectByName("Player").As<PlayerController>();
			player.ResetPlayer();

		}
	}

}