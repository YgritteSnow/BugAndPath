using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 跟随并绘制点
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class FollowDrawObj : MonoBehaviour {
	public Transform m_targetObj = null;
	public float m_posEpsilon = 0.001f;
	public float m_rotEpsilon = 0.001f;

	LineRenderer m_lineRenderer = null;
	List<Vector3> m_pos = new List<Vector3>();
	public bool m_isDrawing = false;
	private void Awake()
	{
		m_lineRenderer = GetComponent<LineRenderer>();
		m_lineRenderer.SetPositions(new Vector3[0]);
	}

	void Update () {
		if(m_targetObj == null)
		{
			return;
		}

		if(m_isDrawing)
		{
			RecordPath();
		}
	}

	public void ClearRecord()
	{
		m_isDrawing = false;
		m_pos = new List<Vector3>();
		m_lineRenderer.SetPositions(new Vector3[0]);
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
		if(!NeedRecord(new_pos))
		{
			return;
		}

		m_pos.Add(new_pos);
		Vector3[] pos_arr = new Vector3[m_pos.Count];
		m_pos.CopyTo(pos_arr);
		Debug.Log("Record:" + m_pos.Count + ", " + new_pos);

		for (int i = 0; i != 5; ++i)
		{
			m_lineRenderer.SetPosition(i, Vector3.one * Time.time);
		}
	}

	public Vector3 GetTargetPos()
	{
		return m_targetObj.transform.position;
	}

	public bool NeedRecord(Vector3 new_pos)
	{
		if (m_pos.Count > 1) // 如果新加入的点距离很近而且转角不大，那么可能可以不去记录它
		{
			Vector3 last_pos = m_pos[m_pos.Count - 2];
			Vector3 cur_pos = m_pos[m_pos.Count - 1];
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
			//Debug.Log("too near:" + (cur_pos - new_pos).sqrMagnitude + ", " + (1 - Vector3.Dot(next_dir, last_dir)));
			return false;
		}
		else
		{
			return true;
		}
	}
}
