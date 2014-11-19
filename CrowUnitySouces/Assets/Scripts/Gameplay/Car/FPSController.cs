using UnityEngine;
using System.Collections;

[AddComponentMenu("Gameplay/FPSController")]
public class FPSController : MonoBehaviour 
{

	#region Members

	// Constants
	public float _moveSpeed;
	public float _rotateSpeed;
	public float _jumpForce;
	public float _dashDistance;
	public float _dashSpeed;
	public float _dashCooldown;

	//reticle
	public GameObject _reticle;
	public float _reticlegrowth;
	public float _reticleSize = 0.01f;

	// Refs
	public GameObject _camera;
	public GameObject _weapon;


	// Movement members
	private int m_forward = 0;
	private int m_right = 0;
	private bool m_canJump = false;
    //private float m_weaponRotation = 0;
	private Timer m_rotateTimer;
	private float m_yOffset = -0.75f;
	private float m_feetRadius = 0.4f;
	private float m_jumpCd = 1f;
	private Timer m_jumpTimer;


	//movement audio
    //private float m_deltaTimeWalk = 0.32f;
	private float m_lastTimeWalk = 0f;
	private bool m_isWalking = false;

	enum Direction
	{
		NONE,
		FORWARD,
		BACKWARD,
		LEFT,
		RIGHT
	}

	// Dash
	private Direction m_lastDirection; 
	private Timer m_doubleKeyTimer; 
	private Timer m_dashTimer;
	private Vector3 m_dashDirection;
	private float m_dashDistanceRun = 0f;


	//reticle
	private Timer m_reticleTimer;
	private float m_growthCooldown;

	#endregion

	#region MonoBehaviour

	void Start ()
	{
		//move and camera controls
		KeyBinder.Instance.DefineActions("Forward", new KeyActionConfig(KeyType.Movement, 0, Forward, () => {m_forward -= 1;}));
		KeyBinder.Instance.DefineActions("Backward", new KeyActionConfig(KeyType.Movement, 1, Backward, () => {m_forward += 1;}));
		KeyBinder.Instance.DefineActions("StrafeLeft", new KeyActionConfig(KeyType.Movement, 2, StrafeLeft, () => {m_right += 1;}));
		KeyBinder.Instance.DefineActions("StrafeRight", new KeyActionConfig(KeyType.Movement, 3, StrafeRight, () => {m_right -= 1;}));
		KeyBinder.Instance.DefineActions("Jump/Dash", new KeyActionConfig(KeyType.Movement, 4, () => {if(m_canJump){Jump();}}, null));
        //KeyBinder.Instance.DefineActions("MouseX", new AxisActionConfig(KeyType.Head, 0, MouseX));
        //KeyBinder.Instance.DefineActions("MouseY", new AxisActionConfig(KeyType.Head, 0, MouseY));

		m_doubleKeyTimer = new Timer();
		m_dashTimer = new Timer();
		m_reticleTimer = new Timer();
		m_jumpTimer = new Timer(m_jumpCd);
	}

	// applying movement
	void FixedUpdate ()
	{
		if(m_dashDirection != Vector3.zero)
		{
			transform.position += m_dashDirection * (_moveSpeed * 4 * Time.deltaTime);
			m_dashDistanceRun += _moveSpeed * 4 * Time.deltaTime;
			if(m_dashDistanceRun >= _dashDistance)
			{
				m_dashTimer.Reset(_dashCooldown);
				StopDash();
			}
			return;
		}

		if(m_forward != 0)
		{
			transform.position += transform.forward * (m_forward * _moveSpeed * Time.deltaTime);
		}

		if(m_right != 0)
		{
            transform.position += transform.right * (m_right * _moveSpeed * Time.deltaTime);

		}

		if(m_forward != 0 || m_right != 0)
		{

			if(!m_isWalking && m_canJump)
			{
				m_isWalking = true;
			}

		}else{
			if(m_isWalking && m_canJump)
			{
				m_lastTimeWalk = 0f;
				m_isWalking = false;
			}
		}


	}

	void Update()
	{
		if(_reticle != null)
		{
			float size = Mathf.Lerp(_reticleSize + _reticlegrowth, _reticleSize, 1 - m_reticleTimer.CurrentNormalized);
			_reticle.transform.localScale = Vector3.one * size;
		}

		if(IsGrounding())
		{
			if(m_jumpTimer.IsElapsedLoop)
			{
				Grounding ();
				StopDash();
				rigidbody.velocity = Vector3.zero;
			}
		}else{
			m_canJump = false;
		}

		if(m_isWalking && m_canJump)
		{
			m_lastTimeWalk += Time.deltaTime;
		}
	}

	#endregion


	#region Movement

	void Forward()
	{
		if(m_forward <1)
			m_forward += 1;

		if(m_lastDirection == Direction.FORWARD && !m_doubleKeyTimer.IsElapsedLoop && m_dashDirection == Vector3.zero && !m_canJump)
		{
			StartDash(transform.forward);
		}

		m_lastDirection = Direction.FORWARD;
		m_doubleKeyTimer.Reset(0.3f);
	}

	void Backward()
	{
		if(m_forward > -1)
			m_forward -= 1;

		if(m_lastDirection == Direction.BACKWARD && !m_doubleKeyTimer.IsElapsedLoop && m_dashDirection == Vector3.zero && !m_canJump)
		{
			StartDash(-transform.forward);
		}

		m_lastDirection = Direction.BACKWARD;
		m_doubleKeyTimer.Reset(0.3f);
	}

	void StrafeLeft()
	{
		if(m_right > -1)
			m_right -= 1;

		if(m_lastDirection == Direction.LEFT && !m_doubleKeyTimer.IsElapsedLoop && m_dashDirection == Vector3.zero && !m_canJump)
		{
			StartDash(-transform.right);
		}
		
		m_lastDirection = Direction.LEFT;
		m_doubleKeyTimer.Reset(0.3f);
	}

	void StrafeRight()
	{
		if(m_right < 1)
			m_right += 1;

		if(m_lastDirection == Direction.RIGHT && !m_doubleKeyTimer.IsElapsedLoop && m_dashDirection == Vector3.zero && !m_canJump)
		{
			StartDash(transform.right);
		}
		
		m_lastDirection = Direction.RIGHT;
		m_doubleKeyTimer.Reset(0.3f);
	}

	void StartDash(Vector3 direction)
	{
		if(m_dashTimer.IsElapsedLoop || _dashCooldown == 0)
		{
			m_dashDirection = direction;
			m_dashDistanceRun = 0f;
			rigidbody.velocity = Vector3.zero;
			rigidbody.useGravity = false;
		}
	}

	void StopDash()
	{
		rigidbody.useGravity = true;
		m_dashDirection = Vector3.zero;
	}

	void Jump()
	{
		rigidbody.AddForce(Vector3.up * _jumpForce);
		m_jumpTimer.Reset(m_jumpCd);
		m_canJump = false;
	}

	void Grounding()
	{
		m_canJump = true;
	}

	#endregion

	#region Camera

	// rotating camera left and right
	void MouseX(float value)
	{
		transform.Rotate(Vector3.up, (value *_rotateSpeed));
	}

	//rotating camera up and down
	void MouseY(float value)
	{
		Vector3 currEuler = _camera.transform.localRotation.eulerAngles;
		currEuler.x += (-value * _rotateSpeed);

		if( currEuler.x > 90 && currEuler.x < 180 )
		{
			currEuler.x = 90f;
		}

		if( currEuler.x > 180 && currEuler.x < 270)
		{
			currEuler.x = 270;
		}
			_camera.transform.localRotation = Quaternion.Euler(currEuler);		
		//_camera.transform.Rotate(Vector3.right, (-value * _rotateSpeed));
	}

	public virtual void OnDrawGizmos()
    {
		Gizmos.color = Color.blue;
		
		Gizmos.DrawWireSphere(transform.position + Vector3.up * m_yOffset, m_feetRadius);
	}

    #endregion

    #region AudioWalk

    // verifying if we can jump again
    bool IsGrounding()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position + Vector3.up * m_yOffset, m_feetRadius, ~(1 << LayerMask.NameToLayer("Player")));

        return cols.Length > 0;
    }

	#endregion
}
