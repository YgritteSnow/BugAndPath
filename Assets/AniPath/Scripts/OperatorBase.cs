using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 基类运算
/// </summary>
public class OperatorBase : MonoBehaviour {
	public float[] m_inputDefaultValue;
	public string m_funcPattern;

	[HideInInspector]
	public string m_outputName;
	[HideInInspector]
	public int inputCount { get { return m_inputDefaultValue.Length; } }

	static private int _genid;
	static private int GenId() { return ++_genid; }
	static private string GenParamName(string name) { return name + GenId().ToString(); }

	private string _operName = "base_";
	public string operName { get { return _operName; } }

	private string[] m_inputNames;

	void Awake()
	{
		SetInputCount();
	}

	protected void SetInputCount()
	{
		_operName = GenParamName("base_");

		int count = inputCount;
		m_inputNames = new string[count];
		for (int i = 0; i != count; ++i)
		{
			m_inputNames[i] = _operName + "input_" + i;
		}
		
		m_outputName = _operName + "output";
	}

	public virtual string GenStr(string[] src_names)
	{
		string result = "";
		for(int i = 0; i != src_names.Length; ++i)
		{
			if(src_names[i] != null)
			{
				result += "local " + m_inputNames[i] + " = " + src_names[i] + "\n";
			}
			else
			{
				float default_value = (m_inputDefaultValue != null && m_inputDefaultValue.Length >= i) ? m_inputDefaultValue[i] : 0;
				result += "local " + m_inputNames[i] + " = " + default_value + "\n";
			}
		}

		result += "local " + m_outputName + " = " + string.Format(m_funcPattern, m_inputNames) + "\n";
		return result;
	}
}
