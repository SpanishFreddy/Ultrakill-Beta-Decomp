using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
	private AudioSource aud;

	private Transform bubble;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
		aud.pitch = Random.Range(0.9f, 1.1f);
		aud.Play();
		bubble = base.transform.GetChild(0);
	}

	private void Update()
	{
		if (bubble.localScale.x > 0f)
		{
			bubble.localScale -= Vector3.one * Time.deltaTime;
		}
		else
		{
			bubble.localScale = Vector3.zero;
		}
	}
}
