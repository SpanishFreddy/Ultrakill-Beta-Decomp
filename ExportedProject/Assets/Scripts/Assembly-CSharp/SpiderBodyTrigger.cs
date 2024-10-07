using UnityEngine;

public class SpiderBodyTrigger : MonoBehaviour
{
	private SpiderBody spbody;

	private void Start()
	{
		spbody = base.transform.parent.GetComponentInChildren<SpiderBody>();
	}

	private void Update()
	{
		if (spbody != null)
		{
			base.transform.position = spbody.transform.position;
			base.transform.rotation = spbody.transform.rotation;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Head" || other.gameObject.tag == "Body" || other.gameObject.tag == "Limb" || other.gameObject.tag == "LimbEnd")
		{
			Debug.Log("FUCKING CUNT + " + other.gameObject);
			spbody.TriggerHit(other);
		}
	}
}
