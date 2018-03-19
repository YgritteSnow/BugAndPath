using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLua;

public class AntMove : MonoBehaviour {
	public float forward_speed = 0.0f;
	public float rotate_speed = 0.0f;

	public float total_time = 1;

	private LuaState L = null;
	private LuaFunction lua_func_rotation = null;
	private LuaFunction lua_func_position = null;
	public OperatorCodeGen rotation_code = null;
	public OperatorCodeGen position_code = null;

	void Update ()
	{
		if(lua_func_rotation == null || lua_func_position == null)
		{
			return;
		}
		float time = Time.fixedTime % total_time;
		rotate_speed = float.Parse(lua_func_rotation.call(new object[1] { time }).ToString());
		forward_speed = float.Parse(lua_func_position.call(new object[1] { time }).ToString());

		float delta_time = Time.deltaTime;

		Vector3 old_position = transform.position;
		Vector3 new_position = old_position + forward_speed * delta_time * transform.TransformDirection(Vector3.up);
		new_position.z = -1;

		Quaternion old_rotation = transform.rotation;
		Quaternion new_rotation = old_rotation * Quaternion.Euler(new Vector3(0, 0, delta_time * rotate_speed));

		transform.SetPositionAndRotation(new_position, new_rotation);
	}

	static private AntMove _instance = null;
	static public AntMove Instance { get { return _instance; } }
	private void Awake()
	{
		_instance = this;
		L = new LuaState();
	}

	public void ResetFunc()
	{
		string rot_str = rotation_code.genCode();
		string pos_str = position_code.genCode();
		L.doString("function RotationFunc(time) \n" + rot_str + "\n end");
		L.doString("function PositionFunc(time) \n" + pos_str + "\n end");
		lua_func_rotation = L.getFunction("RotationFunc");
		lua_func_position = L.getFunction("PositionFunc");
	}
}
