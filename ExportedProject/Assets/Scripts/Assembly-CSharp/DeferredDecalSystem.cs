using System.Collections.Generic;

public class DeferredDecalSystem
{
	private static DeferredDecalSystem m_Instance;

	internal HashSet<Decal> m_DecalsDiffuse = new HashSet<Decal>();

	internal HashSet<Decal> m_DecalsNormals = new HashSet<Decal>();

	internal HashSet<Decal> m_DecalsBoth = new HashSet<Decal>();

	public static DeferredDecalSystem instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new DeferredDecalSystem();
			}
			return m_Instance;
		}
	}

	public void AddDecal(Decal d)
	{
		RemoveDecal(d);
		if (d.m_Kind == Decal.Kind.DiffuseOnly)
		{
			m_DecalsDiffuse.Add(d);
		}
		if (d.m_Kind == Decal.Kind.NormalsOnly)
		{
			m_DecalsNormals.Add(d);
		}
		if (d.m_Kind == Decal.Kind.Both)
		{
			m_DecalsBoth.Add(d);
		}
	}

	public void RemoveDecal(Decal d)
	{
		m_DecalsDiffuse.Remove(d);
		m_DecalsNormals.Remove(d);
		m_DecalsBoth.Remove(d);
	}
}
