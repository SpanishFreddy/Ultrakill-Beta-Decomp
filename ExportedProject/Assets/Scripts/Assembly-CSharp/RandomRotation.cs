using UnityEngine;

public class RandomRotation : MonoBehaviour
{
	public Material[] materials;

	private MeshRenderer mr;

	private void Awake()
	{
		mr = GetComponent<MeshRenderer>();
		mr.material = materials[Random.Range(0, materials.Length - 1)];
		base.transform.Rotate(Vector3.forward * Random.Range(0, 359), Space.Self);
	}

	private void Update()
	{
	}
}
