using UnityEngine;

[ExecuteInEditMode]
public class cPrecision : MonoBehaviour
{
	private Material material;

	public int colorPrecision = 64;

	public bool usePalette;

	public Texture2D palette;

	private void Awake()
	{
		material = new Material(Shader.Find("Hidden/cPrecision"));
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (usePalette)
		{
			material.SetFloat("_usePalette", 1f);
		}
		else
		{
			material.SetFloat("_usePalette", 0f);
		}
		material.SetFloat("_Colors", colorPrecision);
		material.SetTexture("_Palette", palette);
		Graphics.Blit(source, destination, material);
	}
}
