using UnityEngine;

public class LightOff : MonoBehaviour
{
	private Light light;

	private AudioSource[] aud;

	public GameObject otherLamp;

	private Light otherLight;

	public float oLIntensity;

	private void Awake()
	{
		aud = GetComponentsInChildren<AudioSource>();
		light = GetComponentInChildren<Light>();
		otherLight = otherLamp.GetComponent<Light>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			AudioSource[] array = aud;
			foreach (AudioSource audioSource in array)
			{
				audioSource.Play();
			}
			if (light != null)
			{
				light.enabled = false;
			}
			if (otherLight != null)
			{
				otherLight.intensity = oLIntensity;
			}
		}
	}
}
