using UnityEngine;
using UnityEngine.AI;

public class Glass : MonoBehaviour
{
	public bool broken;

	private Transform[] glasses;

	public GameObject shatterParticle;

	public AudioClip scream;

	private StyleHUD shud;

	private int kills;

	public void Shatter()
	{
		glasses = base.transform.GetComponentsInChildren<Transform>();
		Transform[] array = glasses;
		foreach (Transform transform in array)
		{
			if (transform.gameObject != base.gameObject)
			{
				Object.Destroy(transform.gameObject);
			}
		}
		base.gameObject.layer = 17;
		broken = true;
		if (base.gameObject.tag == "GlassFloor")
		{
			Invoke("BecomeObstacle", 0.1f);
		}
		Object.Instantiate(shatterParticle, base.transform);
	}

	private void OnTriggerStay(Collider other)
	{
		if (broken && other.gameObject.tag == "Enemy")
		{
			if (shud == null)
			{
				shud = GameObject.FindWithTag("StyleHUD").GetComponent<StyleHUD>();
			}
			kills++;
			if (kills < 3)
			{
				shud.AddPoints(50, "ENVIROKILL");
			}
			else
			{
				shud.AddPoints(50 / (kills / 2), "ENVIROKILL");
			}
			EnemyIdentifier component = other.gameObject.GetComponent<EnemyIdentifier>();
			component.InstaKill();
			AudioSource component2 = other.GetComponent<AudioSource>();
			component2.clip = scream;
			component2.volume = 1f;
			component2.priority = 78;
			component2.pitch = Random.Range(0.8f, 1.2f);
			component2.Play();
		}
	}

	private void BecomeObstacle()
	{
		NavMeshObstacle component = GetComponent<NavMeshObstacle>();
		component.enabled = true;
	}
}
