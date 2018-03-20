using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 跟随并绘制点
/// </summary>
public class FollowDrawTarget : MonoBehaviour
{
	public const int m_buffer_size = 3;
	public Transform m_targetObj = null;
	public float m_posEpsilon = 0.001f;
	public float m_rotEpsilon = 0.001f;
	public float m_lerp_param = 0.8f;
	public DrawLines m_dr;

	public bool m_isDrawing = false;
	Vector3[] m_pos_vec;
	int m_pos_idx;

	private void Awake()
	{
		m_pos_vec = new Vector3[m_buffer_size];
		m_pos_idx = 0;

		if(m_dr == null)
		{
			m_dr = GetComponent<DrawLines>();
		}
	}

	void Update()
	{
		if (m_targetObj == null)
		{
			return;
		}

		if (m_isDrawing)
		{
			RecordPath();
		}
	}

	public void ClearRecord()
	{
		m_isDrawing = false;
		m_pos_vec = new Vector3[m_buffer_size];
		m_pos_idx = 0;

		m_dr.ClearDraw();
	}

	public void StartRecord()
	{
		m_isDrawing = true;
	}

	public void PauseRecord()
	{
		m_isDrawing = false;
	}

	public void RecordPath()
	{
		Vector3 new_pos = GetTargetPos();
		if (!NeedRecord(new_pos))
		{
			return;
		}

		// 计算当前buffer中的平均值
		Vector3 avg = Vector3.zero;
		int avg_count = 0;
		for(int i = 0; i != m_buffer_size; ++i)
		{
			if (m_pos_vec[(i + m_pos_idx) % m_buffer_size] != null)
			{
				avg += m_pos_vec[(i + m_pos_idx) % m_buffer_size];
				++avg_count;
			}
		}
		avg /= avg_count;

		// 绘制平均值
		Vector3 to_draw_pos = Vector3.Lerp(avg, new_pos, m_lerp_param);
		DrawAt(to_draw_pos);

		// 记录实际值
		m_pos_idx = (m_pos_idx + 1) % m_buffer_size;
		m_pos_vec[m_pos_idx] = new_pos;
	}

	public Vector3 GetTargetPos()
	{
		return m_targetObj.transform.position;
	}

	// 如果新加入的点距离很近而且转角不大，那么可能可以不去记录它
	public bool NeedRecord(Vector3 new_pos)
	{
		if (m_buffer_size < 3 || m_pos_vec[(m_pos_idx - 1 + m_buffer_size) % m_buffer_size] == null)
		{
			return true;
		}

		Vector3 last_pos = m_pos_vec[(m_pos_idx - 1 + m_buffer_size) % m_buffer_size];
		Vector3 cur_pos = m_pos_vec[(m_pos_idx - 0 + m_buffer_size) % m_buffer_size];
		if ((cur_pos - new_pos).sqrMagnitude > m_posEpsilon)
		{
			return true;
		}

		Vector3 next_dir = new_pos - cur_pos;
		next_dir.Normalize();
		Vector3 last_dir = cur_pos - last_pos;
		last_dir.Normalize();
		if (1 - Vector3.Dot(next_dir, last_dir) > m_rotEpsilon)
		{
			return true;
		}
		return false;
	}

	public void DrawAt(Vector3 new_pos)
	{
		m_dr.DrawAt(new_pos);
	}
}