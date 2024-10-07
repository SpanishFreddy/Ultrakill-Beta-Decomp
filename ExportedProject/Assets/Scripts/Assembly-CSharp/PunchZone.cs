using UnityEngine;

public class PunchZone : MonoBehaviour
{
	public bool active;

	private AudioSource aud;

	private void Start()
	{
		aud = GetComponent<AudioSource>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (active)
		{
			if (other.gameObject.layer == 8)
			{
				Debug.Log("bep");
				aud.Play();
				active = false;
			}
			else if (other.gameObject.tag == "Enemy")
			{
				active = false;
				other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid.DeliverDamage(other.gameObject, 10000, other.transform.position, 1f);
			}
		}
	}
}
