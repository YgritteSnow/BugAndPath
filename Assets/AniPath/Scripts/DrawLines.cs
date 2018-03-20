using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines : MonoBehaviour
{
	string propertyName = "_Blend_Texture";
	public Texture2D brushTexture;

	public int width = 512;
	public int height = 512;

	Material mat;
	Texture2D maskTexture;
	
	public int brushWidth;
	public int brushHeight;
	int lastCenterX = -1;
	int lastCenterY = -1;

	bool hasStarted = false;

	void Awake()
	{
		if (GetComponent<Renderer>() != null)
		{
			mat = GetComponent<Renderer>().material;
		}

		Debug.Log("DrawLine: shader name = " + mat.shader.name);
		maskTexture = new Texture2D(width, height, TextureFormat.Alpha8, false);
		maskTexture.wrapMode = TextureWrapMode.Clamp;
		mat.SetTexture(propertyName, maskTexture);

		ClearDraw();
	}

	Color[] PureColors(int width, int height, Color color)
	{
		Color[] result = new Color[width * height];
		for (int i = 0; i < result.Length; i++)
		{
			result[i] = color;
		}
		return result;
	}

	public void ClearDraw()
	{
		maskTexture.SetPixels(PureColors(maskTexture.width, maskTexture.height, Color.black));
		maskTexture.Apply();
		hasStarted = false;
	}

	public void DrawAt(Vector3 new_pos)
	{
		if(hasStarted)
		{
			DrawLine(new_pos);
		}
		else
		{
			DrawDot(new_pos);
		}
		//hasStarted = true;
	}

	public void DrawDot(Vector3 new_pos)
	{
		Color[] colors = brushTexture.GetPixels();
		maskTexture.SetPixels((int)new_pos.x - brushWidth / 2, (int)new_pos.y - brushHeight / 2, brushWidth, brushHeight, colors);
	}

	public void DrawLine(Vector3 new_pos)
	{
	}
}
