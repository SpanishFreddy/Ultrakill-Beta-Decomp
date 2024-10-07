using UnityEngine;

public class Flicker : MonoBehaviour
{
	private Light light;

	public float delay;

	private AudioSource aud;

	public float intensity;

	public bool onlyOnce;

	private void Awake()
	{
		light = GetComponent<Light>();
		aud = GetComponent<AudioSource>();
		Invoke("Flickering", delay);
	}

	private void Flickering()
	{
		if (light.intensity == 0f)
		{
			light.intensity = intensity;
			if (aud != null)
			{
				aud.Play();
			}
		}
		else
		{
			light.intensity = 0f;
		}
		if (!onlyOnce)
		{
			Invoke("Flickering", delay);
		}
	}
}
