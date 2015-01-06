using UnityEngine;
using System.Collections;

public class CursorChanger : MonoBehaviour {

	public Texture2D m_activeTexture, m_inactiveTexture;
	public Vector2 m_offset;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0))
		{
			Cursor.SetCursor(m_activeTexture, m_offset, CursorMode.ForceSoftware);
		}
		else
		{
			Cursor.SetCursor(m_inactiveTexture, m_offset, CursorMode.ForceSoftware);
		}
	}
}
