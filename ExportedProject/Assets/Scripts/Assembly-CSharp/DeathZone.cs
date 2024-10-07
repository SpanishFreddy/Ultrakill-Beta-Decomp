using UnityEngine;

public class DeathZone : MonoBehaviour
{
	private NewMovement pm;

	private AudioSource aud;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			aud = GetComponent<AudioSource>();
			aud.Play();
			pm = other.GetComponent<NewMovement>();
			pm.GetHurt(999);
			base.enabled = false;
		}
	}
}
