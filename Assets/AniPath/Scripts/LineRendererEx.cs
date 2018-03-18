using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererEx : MonoBehaviour {
	public List<Vector2> m_pos;
	public Mesh m_mesh;
	public float m_width;

	// 缓存Mesh顶点数据
	private List<Vector3> m_vertices_left;
	private List<Vector3> m_vertices_right;
	private void Start()
	{
		m_pos = new List<Vector2>();
		m_mesh = new Mesh();
	}

	public void ClearPoint()
	{
		m_pos.Clear();
		ClearMesh();
	}

	public void AddPoint(Vector2 point)
	{
		m_pos.Add(point);
		AddLineMeshPos(point);
	}

	private void ClearMesh()
	{
		m_mesh = new Mesh();
		m_vertices_left.Clear();
		m_vertices_right.Clear();
	}

	private void AddLineMeshPos(Vector2 pos)
	{

	}

	private void GetLineCross(Vector2 last_point, Vector2 cur_point, Vector2 next_point, out Vector2 left_cross, out Vector2 right_cross)
	{
		left_cross = 
	}
}
