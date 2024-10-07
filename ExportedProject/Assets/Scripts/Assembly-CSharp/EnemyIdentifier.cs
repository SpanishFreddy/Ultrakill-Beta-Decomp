using UnityEngine;

public class EnemyIdentifier : MonoBehaviour
{
	public EnemyType type;

	public Zombie zombie;

	public SpiderBody spider;

	public RaycastHit rhit;

	public string hitter;

	private void Start()
	{
		if (type == EnemyType.Zombie)
		{
			zombie = GetComponent<Zombie>();
		}
		else if (type == EnemyType.Spider)
		{
			spider = GetComponent<SpiderBody>();
		}
	}

	public void DeliverDamage(GameObject target, int bulletForce, Vector3 hurtPoint, float multiplier)
	{
		if (type == EnemyType.Zombie)
		{
			if (zombie == null)
			{
				zombie = GetComponent<Zombie>();
			}
			zombie.GetHurt(target, bulletForce, hurtPoint, multiplier);
		}
		if (type == EnemyType.Spider)
		{
			if (spider == null)
			{
				spider = GetComponent<SpiderBody>();
			}
			spider.GetHurt(target, bulletForce, rhit, multiplier);
		}
	}

	public void InstaKill()
	{
		if (type == EnemyType.Zombie)
		{
			if (zombie == null)
			{
				zombie = GetComponent<Zombie>();
			}
			zombie.GoLimp();
		}
	}

	public void Explode()
	{
		if (type != 0)
		{
			return;
		}
		if (zombie == null)
		{
			zombie = GetComponent<Zombie>();
		}
		if (!zombie.limp)
		{
			return;
		}
		Transform[] componentsInChildren = zombie.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.gameObject.tag == "Limb")
			{
				Object.Destroy(transform.GetComponent<CharacterJoint>());
				transform.transform.parent = null;
			}
			else if (transform.gameObject.tag == "Head")
			{
				zombie.GetHurt(transform.gameObject, 1000, transform.position, 2f);
			}
		}
	}
}
