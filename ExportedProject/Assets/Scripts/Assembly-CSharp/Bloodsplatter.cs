using System.Collections.Generic;
using UnityEngine;

public class Bloodsplatter : MonoBehaviour
{
	public ParticleSystem part;

	public List<ParticleCollisionEvent> collisionEvents;

	public GameObject bloodStain;

	private GameObject bldstn;

	private int i;

	private AudioSource[] bloodStainAud;

	private AudioSource aud;

	public Material[] materials;

	private MeshRenderer mr;

	private NewMovement nmov;

	public int hpAmount;

	private SphereCollider col;

	public bool hpOnParticleCollision;

	private GoreZone gz;

	private void Start()
	{
		part = GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
		aud = GetComponent<AudioSource>();
		if (aud != null)
		{
			aud.pitch = Random.Range(0.75f, 1.5f);
			aud.Play();
		}
		col = GetComponent<SphereCollider>();
		Invoke("DestroyCollider", 0.2f);
		if (hpOnParticleCollision)
		{
			Invoke("Destroy", 3f);
		}
		else
		{
			Invoke("Destroy", 3f);
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor")
		{
			int num = part.GetCollisionEvents(other, collisionEvents);
			bldstn = Object.Instantiate(bloodStain, collisionEvents[0].intersection, base.transform.rotation, base.transform);
			bldstn.transform.forward = collisionEvents[0].normal;
			mr = bldstn.GetComponent<MeshRenderer>();
			mr.material = materials[Random.Range(0, materials.Length - 1)];
			bldstn.transform.Rotate(Vector3.forward * Random.Range(0, 359), Space.Self);
			Vector3 localScale = bldstn.transform.localScale;
			if (gz == null)
			{
				gz = other.GetComponentInParent<GoreZone>();
			}
			bldstn.transform.parent = gz.goreZone;
		}
		else if (hpOnParticleCollision && other.gameObject.tag == "Player")
		{
			if (nmov == null)
			{
				nmov = other.GetComponent<NewMovement>();
			}
			nmov.GetHealth(5, true);
			MonoBehaviour.print("Get Health!");
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Object.Destroy(col);
			if (nmov == null)
			{
				nmov = other.GetComponent<NewMovement>();
			}
			nmov.GetHealth(hpAmount, false);
		}
	}

	private void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	private void DestroyCollider()
	{
		if (col != null)
		{
			Object.Destroy(col);
		}
	}
}
