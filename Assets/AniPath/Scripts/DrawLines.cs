using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines : MonoBehaviour
{
	string propertyName = "_MainTex";
	public Texture2D brushTexture;

	public int width = 512;
	public int height = 512;

	public Material mat;
	public Texture2D maskTexture;
	
	int brushWidth;
	int brushHeight;
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

		brushWidth = brushTexture.width;
		brushHeight = brushTexture.height;

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
		int mask_left = (int)new_pos.x - brushWidth / 2;
		int mask_top = (int)new_pos.y - brushHeight / 2;
		int mask_right = mask_left + brushWidth;
		int mask_bottom = mask_top + brushHeight;
		int clipped_mask_left = Mathf.Max(0, Mathf.Min(width, mask_left));
		int clipped_mask_top = Mathf.Max(0, Mathf.Min(height, mask_top));
		int clipped_mask_right = Mathf.Max(0, Mathf.Min(width, mask_right));
		int clipped_mask_bottom = Mathf.Max(0, Mathf.Min(height, mask_bottom));
		if(clipped_mask_left == clipped_mask_right || clipped_mask_bottom == clipped_mask_top)
		{
			return;
		}
		Color[] colors = brushTexture.GetPixels(clipped_mask_left - mask_left, clipped_mask_top - mask_top
			, clipped_mask_right - clipped_mask_left, clipped_mask_bottom - clipped_mask_top);
		maskTexture.SetPixels(clipped_mask_left, clipped_mask_top
			, clipped_mask_right - clipped_mask_left, clipped_mask_bottom - clipped_mask_top, colors);
		maskTexture.Apply();
	}

	public void DrawLine(Vector3 new_pos)
	{
	}
}
