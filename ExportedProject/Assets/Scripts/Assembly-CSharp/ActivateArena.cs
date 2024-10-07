using UnityEngine;

public class ActivateArena : MonoBehaviour
{
	private bool activated;

	public float delay;

	public Door[] doors;

	public GameObject[] enemies;

	private int currentEnemy;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.gameObject.tag == "Player"))
		{
			return;
		}
		activated = true;
		if (doors.Length > 0)
		{
			Door[] array = doors;
			foreach (Door door in array)
			{
				door.Lock();
			}
		}
		if (enemies.Length > 0)
		{
			Invoke("SpawnEnemy", delay);
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void SpawnEnemy()
	{
		enemies[currentEnemy].SetActive(true);
		currentEnemy++;
		if (currentEnemy < enemies.Length)
		{
			Invoke("SpawnEnemy", 0.1f);
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
