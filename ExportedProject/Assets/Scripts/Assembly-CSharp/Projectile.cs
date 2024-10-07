using UnityEngine;

public class Projectile : MonoBehaviour
{
	private Rigidbody rb;

	public float speed;

	private AudioSource aud;

	public GameObject explosionEffect;

	public float explosionForce;

	public float explosionRadius;

	public int damage;

	public bool friendly;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		aud = GetComponent<AudioSource>();
		aud.pitch = Random.Range(1.8f, 2f);
		aud.Play();
	}

	private void FixedUpdate()
	{
		rb.velocity = base.transform.forward * speed;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!friendly && other.gameObject.tag == "Player")
		{
			NewMovement componentInParent = other.gameObject.GetComponentInParent<NewMovement>();
			Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
			componentInParent.GetHurt(damage);
			if (componentInParent.gc.onGround)
			{
				other.transform.position += Vector3.up;
			}
			other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, base.transform.position, 0f, 1f, ForceMode.Impulse);
			Object.Destroy(base.gameObject);
		}
		else if (friendly && (other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "EndLimb"))
		{
			EnemyIdentifier eid = other.gameObject.GetComponent<EnemyIdentifierIdentifier>().eid;
			eid.hitter = "projectile";
			eid.DeliverDamage(other.gameObject, 10000, base.transform.position, 5f);
			eid.Explode();
			Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
		}
		else if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor")
		{
			Object.Instantiate(explosionEffect, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
		}
	}
}
