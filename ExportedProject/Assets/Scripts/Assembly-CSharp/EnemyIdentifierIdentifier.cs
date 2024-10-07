using UnityEngine;

public class EnemyIdentifierIdentifier : MonoBehaviour
{
	public EnemyIdentifier eid;

	private void Start()
	{
		eid = GetComponentInParent<EnemyIdentifier>();
	}
}
