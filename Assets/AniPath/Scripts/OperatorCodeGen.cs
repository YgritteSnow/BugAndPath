using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(OperatorLink))]
public class OperatorCodeGen : MonoBehaviour {
	public Dictionary<string, OperatorLink> m_checkedNode;
	public string genCode()
	{
		m_checkedNode = new Dictionary<string, OperatorLink>();
		string result = "";
		_genCodeInner(GetComponent<OperatorLink>(), ref result);
		Debug.Log("result:\n" + result);

		m_checkedNode = null;
		return result;
	}

	private void _genCodeInner(OperatorLink link, ref string res)
	{
		string[] src_names = new string[link.m_inputOperators.Length];
		for(int input_idx = 0; input_idx != link.m_inputOperators.Length; ++input_idx)
		{
			if (link.m_inputOperators[input_idx] != null)
			{
				if (!m_checkedNode.ContainsKey(link.m_inputOperators[input_idx].oper.name))
				{
					_genCodeInner(link.m_inputOperators[input_idx], ref res);
				}
				src_names[input_idx] = link.m_inputOperators[input_idx].oper.m_outputName;
			}
		}

		res += link.oper.GenStr(src_names);
	}
}
