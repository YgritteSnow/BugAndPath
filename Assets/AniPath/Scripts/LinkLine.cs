using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LinkNodeBase
{
	Vector3 GetPos();
}

public class LinkNodeMouse : LinkNodeBase
{
	public Vector3 GetPos()
	{
		if (UICamera.currentCamera != null)
		{
			Vector3 pos = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = -1;
			return pos;
		}
		else
		{
			return Vector3.zero;
		}
	}

	public static LinkNodeMouse identity = new LinkNodeMouse();
}

public class LinkNodeObject : LinkNodeBase
{
	GameObject _obj;
	public LinkNodeObject(GameObject obj)
	{
		_obj = obj;
	}

	public Vector3 GetPos()
	{
		Vector3 obj_pos = _obj.GetComponent<UIWidget>().worldCenter;
		obj_pos.z = -1;
		return obj_pos;
	}
}

public class LinkLine : MonoBehaviour {
	public int steps = 10;
	public LineRenderer lr;

	public GameObject bgn_obj;
	public GameObject end_obj;

	LinkNodeBase bgn_node;
	LinkNodeBase end_node;

	GameObject m_bgnPoint;
	GameObject m_endPoint;

	void Start()
	{
		m_bgnPoint = transform.Find("bgn_node").gameObject;
		m_endPoint = transform.Find("end_node").gameObject;
	}

	public void SetNode(GameObject bgn, GameObject end)
	{
		bgn_obj = bgn;
		end_obj = end;
		bgn_node = GetLinkNode(bgn);
		end_node = GetLinkNode(end);
	}

	LinkNodeBase GetLinkNode(GameObject obj)
	{
		if (obj == null)
		{
			return LinkNodeMouse.identity;
		}
		else
		{
			return new LinkNodeObject(obj);
		}
	}

	public void Reposition()
	{
		LateUpdate();
	}

	void LateUpdate()
	{
		if (bgn_node == null || end_node == null)
		{
			return;
		}
		Vector3 bgn_pos = bgn_node.GetPos();
		Vector3 end_pos = end_node.GetPos();
		lr.SetPositions(new Vector3[2] { bgn_pos, end_pos });

		m_bgnPoint.transform.position = bgn_pos;
		m_endPoint.transform.position = end_pos;
	}
}
