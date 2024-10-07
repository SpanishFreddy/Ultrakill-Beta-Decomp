using UnityEngine;

[ExecuteInEditMode]
public class Decal : MonoBehaviour
{
	public enum Kind
	{
		DiffuseOnly = 0,
		NormalsOnly = 1,
		Both = 2
	}

	public Kind m_Kind;

	public Material m_Material;

	public void OnEnable()
	{
		DeferredDecalSystem.instance.AddDecal(this);
	}

	public void Start()
	{
		DeferredDecalSystem.instance.AddDecal(this);
	}

	public void OnDisable()
	{
		DeferredDecalSystem.instance.RemoveDecal(this);
	}

	private void DrawGizmo(bool selected)
	{
		Color color = new Color(0f, 0.7f, 1f, 1f);
		color.a = ((!selected) ? 0.1f : 0.3f);
		Gizmos.color = color;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		color.a = ((!selected) ? 0.2f : 0.5f);
		Gizmos.color = color;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}

	public void OnDrawGizmos()
	{
		DrawGizmo(false);
	}

	public void OnDrawGizmosSelected()
	{
		DrawGizmo(true);
	}
}
