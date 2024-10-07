using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class DeferredDecalRenderer : MonoBehaviour
{
	public Mesh m_CubeMesh;

	private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>();

	public void OnDisable()
	{
		foreach (KeyValuePair<Camera, CommandBuffer> camera in m_Cameras)
		{
			if ((bool)camera.Key)
			{
				camera.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, camera.Value);
			}
		}
	}

	public void OnWillRenderObject()
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled)
		{
			OnDisable();
			return;
		}
		Camera current = Camera.current;
		if (!current)
		{
			return;
		}
		CommandBuffer commandBuffer = null;
		if (m_Cameras.ContainsKey(current))
		{
			commandBuffer = m_Cameras[current];
			commandBuffer.Clear();
		}
		else
		{
			commandBuffer = new CommandBuffer();
			commandBuffer.name = "Deferred decals";
			m_Cameras[current] = commandBuffer;
			current.AddCommandBuffer(CameraEvent.BeforeLighting, commandBuffer);
		}
		DeferredDecalSystem instance = DeferredDecalSystem.instance;
		int num = Shader.PropertyToID("_NormalsCopy");
		commandBuffer.GetTemporaryRT(num, -1, -1);
		commandBuffer.Blit(BuiltinRenderTextureType.GBuffer2, num);
		commandBuffer.SetRenderTarget(BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal item in instance.m_DecalsDiffuse)
		{
			commandBuffer.DrawMesh(m_CubeMesh, item.transform.localToWorldMatrix, item.m_Material);
		}
		commandBuffer.SetRenderTarget(BuiltinRenderTextureType.GBuffer2, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal decalsNormal in instance.m_DecalsNormals)
		{
			commandBuffer.DrawMesh(m_CubeMesh, decalsNormal.transform.localToWorldMatrix, decalsNormal.m_Material);
		}
		RenderTargetIdentifier[] colors = new RenderTargetIdentifier[2]
		{
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.GBuffer2
		};
		commandBuffer.SetRenderTarget(colors, BuiltinRenderTextureType.CameraTarget);
		foreach (Decal item2 in instance.m_DecalsBoth)
		{
			commandBuffer.DrawMesh(m_CubeMesh, item2.transform.localToWorldMatrix, item2.m_Material);
		}
		commandBuffer.ReleaseTemporaryRT(num);
	}
}
