using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触发型运算
/// </summary>
public class OperatorTrigger : OperatorBase {

	public override string GenStr(string[] src_names)
	{
		string result = "";
		for (int i = 0; i != src_names.Length; ++i)
		{
			if (src_names[i] != null)
			{
				result += "return " + src_names[i] + "\n";
			}
			else
			{
				float default_value = (m_inputDefaultValue != null && m_inputDefaultValue.Length >= i) ? m_inputDefaultValue[i] : 0;
				result += "return " + default_value + "\n";
			}
		}
		return result;
	}
}
