using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(OperatorBase))]
public class OperatorLink : MonoBehaviour {
	public GameObject m_inputUISample;
	
	public OperatorLink[] m_inputOperators;
	public List<OperatorLink> m_outputOperators;
	public List<int> m_outputOperatorIdx;

	private OperatorBase m_oper;
	public OperatorBase oper { get { return m_oper; } }
	private void Awake()
	{
		m_oper = GetComponent<OperatorBase>();
		Debug.Log("OperatorLink.Awake" + m_oper.inputCount);

		m_inputOperators = new OperatorLink[m_oper.inputCount];
		m_outputOperators = new List<OperatorLink>();
		m_outputOperatorIdx = new List<int>();

		UITable inputUITable = transform.Find("Input/Table").GetComponent<UITable>();
		List<Transform> children = inputUITable.GetChildList();
		for(int i = children.Count; i < m_oper.inputCount; ++i)
		{
			GameObject child = NGUITools.AddChild(inputUITable.gameObject, m_inputUISample);
			child.SetActive(true);
		}

		children = inputUITable.GetChildList();
		for (int i = 0; i != children.Count; ++i)
		{
			if (i < m_oper.inputCount)
			{
				children[i].gameObject.SetActive(true);
				children[i].gameObject.name = i.ToString();
				UIEventListener.Get(children[i].gameObject).onClick = OnClickInputChild;
			}
			else
			{
				children[i].gameObject.SetActive(false);
			}
		}
		inputUITable.Reposition();

		Transform outputNode = transform.Find("Output");
		if (outputNode != null)
		{
			UIEventListener.Get(outputNode.gameObject).onClick = OnClickOutput;
		}
	}

	public void AddInputLink(OperatorLink other_out, int in_idx)
	{
		m_inputOperators[in_idx] = other_out;
	}
	public void AddOutputLink(OperatorLink other_in, int in_idx)
	{
		m_outputOperators.Add(other_in);
		m_outputOperatorIdx.Add(in_idx);
	}

	public void RemoveInputLink(int idx)
	{
		m_inputOperators[idx] = null;
	}
	public void RemoveOutputLink(OperatorLink in_node, int in_idx)
	{
		for(int i = 0; i != m_outputOperators.Count; ++i)
		{
			if(m_outputOperators[i] == in_node && m_outputOperatorIdx[i] == in_idx)
			{
				m_outputOperatorIdx.RemoveAt(i);
				m_outputOperators.RemoveAt(i);
				return;
			}
		}
	}

	public OperatorLink GetInputNode(int idx)
	{
		return m_inputOperators[idx];
	}
	public OperatorLink GetOutputNode(int idx)
	{
		return m_outputOperators[idx];
	}
	public int GetOutputNodeInIdx(int idx)
	{
		return m_outputOperatorIdx[idx];
	}

	public GameObject FindOutputAnchor()
	{
		return transform.Find("Output").gameObject;
	}
	public GameObject FindInputAnchor(int idx)
	{
		return transform.Find("Input/Table/" + idx).gameObject;
	}

	void OnClickInputChild(GameObject sender)
	{
		int child_idx = int.Parse(sender.name);
		Debug.Log("click input child: " + sender);
		LinkNodeData cur_data = new LinkNodeData(this, sender, true, child_idx, 0);
		if (!LinkLineMgr.Instance.HasOccupy(cur_data))
		{
			LinkLineMgr.Instance.SettleOrCreateMatch(cur_data);
		}
		else if(LinkLineMgr.Instance.IsMatching())
		{
			return;
		}
		else
		{
			LinkLineMgr.Instance.ChangeLink(cur_data);
		}
	}

	void OnClickOutput(GameObject sender)
	{
		Debug.Log("click output child" + sender);
		LinkNodeData cur_data = new LinkNodeData(this, sender, false, -1, m_outputOperators.Count);
		LinkLineMgr.Instance.SettleOrCreateMatch(cur_data);
	}
}
