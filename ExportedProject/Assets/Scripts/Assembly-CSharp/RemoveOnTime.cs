using UnityEngine;

public class RemoveOnTime : MonoBehaviour
{
	public int time;

	private void Start()
	{
		Invoke("Remove", time);
	}

	private void Remove()
	{
		Object.Destroy(base.gameObject);
	}
}
