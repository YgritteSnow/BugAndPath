using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkNodeData
{
	public LinkNodeData(OperatorLink o, GameObject a, bool isin, int ii, int oi)
	{
		obj = o;
		isInput = isin;
		anchorObj = a;
		_inputIdx = ii;
		_outputIdx = oi;
	}
	public OperatorLink obj;
	public GameObject anchorObj;
	public bool isInput;
	private int _inputIdx;
	private int _outputIdx;

	public int inputIdx { get
		{
			if(!isInput)
			{
				Debug.LogError("not input!!");
			}
			return _inputIdx;
		} }
	public int outputIdx { get
		{
			if(isInput)
			{
				Debug.LogError("is input!!");
			}
			return _outputIdx;
		} }
}

public class LinkData
{
	public LinkData(LinkLine l, OperatorLink i, int ii, OperatorLink o)
	{
		lineObj = l;
		inputObj = i;
		inputObjIdx = ii;
		outputObj = o;
	}
	public LinkLine lineObj;
	public OperatorLink inputObj;
	public int inputObjIdx;
	public OperatorLink outputObj;
}

public class LinkLineMgr : MonoBehaviour {
	public GameObject m_lineSample;
	public GameObject m_bgObj;

	static private LinkLineMgr m_instance = null;
	static public LinkLineMgr Instance { get { return m_instance; } }
	
	private LinkNodeData m_matchingObj;
	private LinkLine m_curLine;

	private List<LinkData> m_links;

	void Awake()
	{
		m_instance = this;
		m_links = new List<LinkData>();

		UIEventListener.Get(m_bgObj).onClick = OnCancenAll;
	}

	// 创建一条线
	public void StartMatch(LinkNodeData src)
	{
		m_matchingObj = src;
		m_curLine = CreateLine();
		if (src.isInput)
		{
			m_curLine.SetNode(null, src.anchorObj);
		}
		else
		{
			m_curLine.SetNode(src.anchorObj, null);
		}
	}

	// 检查是否正在连接
	public bool IsMatching()
	{
		return m_matchingObj != null;
	}

	// 检查这个节点是否可以用来连接
	public bool CanMatch(LinkNodeData dst)
	{
		if(!m_matchingObj.isInput ^ dst.isInput)
		{
			Debug.Log("input is same!");
			return false;
		}

		if(IsCauseCicle(dst))
		{
			Debug.Log("cause circle!");
			return false;
		}

		return true;
	}

	// 检查是否会造成循环
	public bool IsCauseCicle(LinkNodeData node)
	{
		if(node.obj == m_matchingObj.obj)
		{
			return true;
		}

		Dictionary<OperatorLink, bool> checked_pool = new Dictionary<OperatorLink, bool>();
		checked_pool[node.obj] = true;
		checked_pool[m_matchingObj.obj] = true;

		OperatorLink input_node = node.isInput ? node.obj : m_matchingObj.obj;
		OperatorLink output_node = node.isInput ? m_matchingObj.obj : node.obj;

		Stack<OperatorLink> to_check_stack = new Stack<OperatorLink>();
		to_check_stack.Push(output_node);
		while (to_check_stack.Count != 0)
		{
			OperatorLink cur_link = to_check_stack.Pop();
			foreach (OperatorLink child_link in cur_link.m_inputOperators)
			{
				if (child_link != null)
				{
					if (child_link == input_node)
					{
						return true;
					}
					if (!checked_pool.ContainsKey(child_link))
					{
						checked_pool[child_link] = true;
						to_check_stack.Push(child_link);
					}
				}
			}
		}

		return false;
	}
	
	// 决定当前的线
	public void SettleMatch(LinkNodeData dst)
	{
		if(!CanMatch(dst))
		{
			return;
		}

		OperatorLink input_node = (dst.isInput ? dst : m_matchingObj).obj;
		OperatorLink output_node = (!dst.isInput ? dst : m_matchingObj).obj;
		GameObject input_anchor = (dst.isInput ? dst : m_matchingObj).anchorObj;
		GameObject output_anchor = (!dst.isInput ? dst : m_matchingObj).anchorObj;
		int input_idx = dst.isInput ? dst.inputIdx : m_matchingObj.inputIdx;
		
		input_node.AddInputLink(output_node, input_idx);
		output_node.AddOutputLink(input_node, input_idx);

		m_links.Add(new LinkData(m_curLine, input_node, input_idx, output_node));
		m_curLine.SetNode(output_anchor, input_anchor);

		m_matchingObj = null;
		m_curLine = null;

		OnLinkChange();
	}

	// 根据当前状态，决定是创建新的还是放下旧的
	public void SettleOrCreateMatch(LinkNodeData src)
	{
		if (LinkLineMgr.Instance.IsMatching())
		{
			if (LinkLineMgr.Instance.CanMatch(src))
			{
				LinkLineMgr.Instance.SettleMatch(src);
			}
			else
			{
				Debug.Log("Cannot match!");
			}
		}
		else
		{
			LinkLineMgr.Instance.StartMatch(src);
		}
	}

	// 放弃当前的线
	public void CancelMatch()
	{
		if(m_matchingObj == null)
		{
			return;
		}

		m_matchingObj = null;
		if(m_curLine != null)
		{
			GameObject.Destroy(m_curLine.gameObject);
			m_curLine = null;
		}
	}

	// 删除一个link
	public void RemoveLink(LinkNodeData node)
	{
		OperatorLink input_node, output_node;
		int input_idx;
		GetLinkData(node, out input_node, out input_idx, out output_node);
		output_node.RemoveOutputLink(input_node, input_idx);
		input_node.RemoveInputLink(input_idx);
		LinkLine line = RemoveLinkLine(input_node, input_idx, output_node);
		GameObject.Destroy(line.gameObject);

		OnLinkChange();
	}

	// 断开当前的线，并握住这根线的这一头
	public void ChangeLink(LinkNodeData node)
	{
		OperatorLink input_node, output_node;
		int input_idx;
		GameObject input_anchor, output_anchor;
		GetLinkData(node, out input_node, out input_idx, out output_node);
		output_node.RemoveOutputLink(input_node, input_idx);
		input_node.RemoveInputLink(input_idx);
		LinkLine line = RemoveLinkLine(input_node, input_idx, output_node);
		m_curLine = line;
		
		if (node.isInput)
		{
			output_anchor = output_node.FindOutputAnchor();
			line.SetNode(output_anchor, null);
			m_matchingObj = new LinkNodeData(output_node, output_anchor, false, -1, -1);
		}
		else
		{
			input_anchor = input_node.FindInputAnchor(input_idx);
			line.SetNode(null, input_anchor);
			m_matchingObj = new LinkNodeData(input_node, input_anchor, true, input_idx, -1);
		}

		OnLinkChange();
	}

	// 获取某个连接点当前的状态信息
	public void GetLinkData(LinkNodeData node, out OperatorLink input_node, out int input_idx, out OperatorLink output_node)
	{
		if(node.isInput)
		{
			input_node = node.obj;
			input_idx = node.inputIdx;
			output_node = input_node.GetInputNode(input_idx);
		}
		else
		{
			input_node = node.obj.GetOutputNode(node.outputIdx);
			input_idx = node.obj.GetOutputNodeInIdx(node.outputIdx);
			output_node = node.obj;
		}
	}

	// 删除并返回留下的line对象
	public LinkLine RemoveLinkLine(OperatorLink input_node, int input_idx, OperatorLink output_node)
	{
		for (int i = 0; i != m_links.Count; ++i)
		{
			if (m_links[i].inputObj == input_node && m_links[i].outputObj == output_node && m_links[i].inputObjIdx == input_idx)
			{
				LinkLine line = m_links[i].lineObj;
				m_links.RemoveAt(i);
				return line;
			}
		}
		return null;
	}

	// 判断当前节点是否已经有线了
	public bool HasOccupy(LinkNodeData node)
	{
		for (int i = 0; i != m_links.Count; ++i)
		{
			if ((node.isInput && m_links[i].inputObj == node.obj && m_links[i].inputObjIdx == node.inputIdx)
				|| (!node.isInput && m_links[i].outputObj == node.obj))
			{
				return true;
			}
		}
		return false;
	}

	private LinkLine CreateLine()
	{
		if(m_curLine == null)
		{
			GameObject new_line = GameObject.Instantiate(m_lineSample);
			new_line.transform.parent = this.transform;
			new_line.transform.localScale = Vector3.one;
			m_curLine = new_line.GetComponent<LinkLine>();
		}
		return m_curLine;
	}

	private void OnLinkChange()
	{
		AntMove.Instance.ResetFunc();
	}

	private void OnCancenAll(GameObject sender)
	{
		LinkLineMgr.Instance.CancelMatch();
	}
}
