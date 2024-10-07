using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
	public bool oneTime;

	private bool activated;

	public float delay;

	public GameObject[] toActivate;

	public GameObject[] toDisActivate;

	private void OnTriggerEnter(Collider other)
	{
		if (!activated && other.gameObject.tag == "Player")
		{
			if (oneTime)
			{
				activated = true;
			}
			Invoke("Activate", delay);
		}
	}

	private void Activate()
	{
		GameObject[] array = toDisActivate;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		GameObject[] array2 = toActivate;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.SetActive(true);
		}
	}
}
