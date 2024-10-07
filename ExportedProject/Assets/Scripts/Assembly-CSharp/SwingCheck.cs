using UnityEngine;

public class SwingCheck : MonoBehaviour
{
	private EnemyIdentifier eid;

	private NewMovement nmo;

	private EnemyType type;

	private ZombieMelee zombie;

	private void Start()
	{
		eid = GetComponentInParent<EnemyIdentifier>();
		type = eid.type;
	}

	private void OnTriggerStay(Collider other)
	{
		if (!(other.gameObject.tag == "Player") || type != 0)
		{
			return;
		}
		if (zombie == null)
		{
			zombie = GetComponentInParent<ZombieMelee>();
		}
		if (zombie.coolDown == 0f)
		{
			zombie.Swing();
		}
		if (zombie.damaging && zombie.zmb.player.gameObject.layer != 15)
		{
			zombie.damaging = false;
			if (nmo == null)
			{
				nmo = zombie.zmb.player.GetComponent<NewMovement>();
			}
			nmo.GetHurt(25);
		}
	}
}
