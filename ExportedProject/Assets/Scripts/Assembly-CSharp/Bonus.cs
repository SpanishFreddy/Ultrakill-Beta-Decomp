using UnityEngine;

public class Bonus : MonoBehaviour
{
	private Vector3 cRotation;

	public GameObject breakEffect;

	private void Start()
	{
		cRotation = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
	}

	private void Update()
	{
		base.transform.Rotate(cRotation * Time.deltaTime * 5f);
	}

	public void Break()
	{
		Invoke("BeginBreak", 0.02f);
	}

	private void BeginBreak()
	{
		Object.Instantiate(breakEffect, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}
}
