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
		if (m_pos.Count > 1) // 当数量超过1的时候，需要重新计算上一个节点的位置
		{
			Vector2 last_left_cross, last_right_cross;
			GetLineCross(m_pos[m_pos.Count - 2], m_pos[m_pos.Count - 1], pos, m_width, out last_left_cross, out last_right_cross);
			m_vertices_left[m_pos.Count - 1] = last_left_cross;
			m_vertices_right[m_pos.Count - 1] = last_right_cross;
		}

		if (m_pos.Count == 0) // 第一个加进去的点，不计算其左右
		{
			m_pos.Add(pos);
			m_vertices_left.Add(Vector2.zero);
			m_vertices_right.Add(Vector2.zero);
		}
		else
		{
			Vector2 cur_left_cross, cur_right_cross = Vector2.zero;
			GetLineLeftRight(m_pos[m_pos.Count - 1], pos, m_width, out cur_left_cross, out cur_right_cross);
			if(m_pos.Count == 1) // 第二个加进去的点，重新计算第一个点的左右
			{
				m_vertices_left[0] = cur_left_cross + (m_pos[0] - pos);
				m_vertices_right[0] = cur_right_cross + (m_pos[0] - pos);
			}
			m_pos.Add(pos);
			m_vertices_left.Add(cur_left_cross);
			m_vertices_right.Add(cur_right_cross);
		}
	}

	private void GetLineLeftRight(Vector2 last_point, Vector2 cur_point, float width, out Vector2 left_cross, out Vector2 right_cross)
	{
		Vector2 next_vec = cur_point - last_point;
		next_vec.Normalize();
		Vector2 left_vec = new Vector2(-next_vec.y, next_vec.x) * width;
		left_cross = left_vec + cur_point;
		right_cross = cur_point - left_vec;
	}

	private void GetLineCross(Vector2 last_point, Vector2 cur_point, Vector2 next_point, float width, out Vector2 left_cross, out Vector2 right_cross)
	{
		Vector2 last_vec = last_point - cur_point;
		last_vec.Normalize();
		Vector2 next_vec = next_point - cur_point;
		next_vec.Normalize();
		Vector2 left_vec, right_vec = default(Vector2);
		if((last_vec + next_vec).sqrMagnitude < 0.001f)
		{
			left_vec = new Vector2(-next_vec.y, next_vec.x) * width;
			right_vec = -left_vec;
		}
		else
		{
			left_vec = next_vec - last_vec;
			float dot_m2 = Vector2.Dot(last_vec, next_vec);
			float sin = Mathf.Sqrt((1 - dot_m2) / 2);
			left_vec.Set(-left_vec.y, left_vec.x);
			left_vec.Normalize();
			left_vec *= width / sin;
			right_vec = -left_vec;
		}
		left_cross = cur_point + left_vec;
		right_cross = cur_point + right_vec;
	}
}
