using UnityEngine;

public class GoreSplatter : MonoBehaviour
{
	private Rigidbody rb;

	private Vector3 direction;

	private float force;

	private bool goreOver;

	private Vector3 defaultScale;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		defaultScale = base.transform.localScale;
		direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		force = Random.Range(5, 20);
		rb.AddForce(direction * force, ForceMode.VelocityChange);
		Invoke("ReadyToStopGore", 5f);
	}

	private void FixedUpdate()
	{
		if (goreOver && rb.velocity.y > -0.1f && rb.velocity.y < 0.1f)
		{
			StopGore();
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == 8)
		{
			base.transform.parent = other.gameObject.GetComponentInParent<GoreZone>().goreZone;
			goreOver = true;
		}
	}

	private void ReadyToStopGore()
	{
		if (!goreOver)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void StopGore()
	{
		Object.Destroy(rb);
		Object.Destroy(base.gameObject.GetComponent<Collider>());
		Object.Destroy(this);
	}
}
