using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPlayer : MonoBehaviour
{
	// Reference pointing to the pivot which the camera rotates around
	[SerializeField]
	Transform	m_CameraPivot;

	// The movement speed of the player in m/s
	[SerializeField]
	float		m_MovementSpeed = 5;
	// How fast the player movement accelerates
	[SerializeField]
	float		m_Acceleration = 4;
	// How fast the player movement comes to a stop
	[SerializeField]
	float		m_Deceleration = 2;
	// The vertical velocity of a jump
	[SerializeField]
	float		m_JumpVelocity = 5;

	// The sensitivity of camera movement
	[SerializeField]
	float		m_MouseSensitivity = 1;
	// Whether to invert the y-axis of the camera or not
	[SerializeField]
	bool		m_MouseInvertY = true;

	// The rigidbody component present on the player object
	Rigidbody	m_Rigidbody;
	// The current velocity of the player object
	Vector3		m_Velocity;
	// The current camera rotation values
	float		m_CameraPitch;
	float		m_CameraYaw;
	// The current number of collisions with other objects. Used to determine if the player object is grounded or not.
	int			m_CollisionCounter;
	// Track if the player should jump next FixedUpdate
	bool		m_ShouldJump;

    // Start is called before the first frame update
    void Start ()
    {
		// Get the reference to the rigidbody on the same gameobject using the GetComponent function.
		m_Rigidbody = GetComponent<Rigidbody>();

		// Lock the mouse cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

	// Update is called once per frame
	void Update ()
	{
		// We need to check and store this here, as the FixedUpdate might "miss" some inputs otherwise,
		// since Update runs more often.
		if (Input.GetKey(KeyCode.Space))
			m_ShouldJump = true;
	}

	// FixedUpdate is called once per phsyics frame
	void FixedUpdate ()
    {
        UpdateMovement();
    }

	// LateUpdate is also called once per frame, but after all regular Update functions
	void LateUpdate ()
	{
		UpdateCamera();
	}

	// This function updates the position of the player object, based on keyboard input
	void UpdateMovement ()
	{
		// If we are currently colliding with at least one object, we regard that as being "on the ground"
		bool isGrounded = m_CollisionCounter > 0;

		// This is the "input vector" which will determine how the player object should move
		Vector3 input = Vector3.zero;

		// To make the movement relative to the camera orientation, we grab the camera's directional vectors
		Vector3 right = m_CameraPivot.right;
		Vector3 forward = m_CameraPivot.forward;

		// We then "flatten" these vectors, to prevent the player object from moving vertically.
		right.y = 0;
		forward.y = 0;

		// We also have to normalize them, as flattening them with the method above will make them shorter.
		right.Normalize();		
		forward.Normalize();

		// Player wants to move to the right
		if (Input.GetKey(KeyCode.D))
			input += right;
		// Player wants to move to the left
		if (Input.GetKey(KeyCode.A))
			input += -right;
		// Player wants to move forward
		if (Input.GetKey(KeyCode.W))
			input += forward;
		// Player wants to move backwards
		if (Input.GetKey(KeyCode.S))
			input += -forward;

		// If the player is pressing multiple buttons to move in a diagonal direction,
		// it is likely that the input vector's length is larger than 1.
		// If it is, we need to normalize it, to prevent the player object from moving faster diagonally.
		if (input.magnitude > 1f)
			input.Normalize();

		// The new velocity the player object should move in is our input direction * the player movement speed
		Vector3 newVelocity = input * m_MovementSpeed;

		// Get the current velocity of the rigidbody
		Vector3 velocity = m_Rigidbody.velocity;

		// If we have input from the player and are grounded, modify the velocity vector
		if (input.magnitude > Mathf.Epsilon && isGrounded)
		{
			// Modify the velocity vector
			velocity = Vector3.Lerp(velocity, newVelocity, m_Acceleration * Time.deltaTime);
		}

		// We overwrite the y-value of the new velocity vector with the value from the physics simulation, regardless of if there is input or not.
		// This way, the player object will not stop falling if the player moves while in the air.
		velocity.y = m_Rigidbody.velocity.y;

		// If we are grounded and spacebar is pressed, override the vertical velocity with the jump velocity
		if (isGrounded && m_ShouldJump)
			velocity.y = m_JumpVelocity;

		// Regardless if we jumped or not, we reset this bool since a FixedUpdate now has passed.
		m_ShouldJump = false;

		// Apply our changes to the rigidbody velocity by setting the value to our new vector.
		m_Rigidbody.velocity = velocity;
	}

	// This function updates the camera rotation, based on mouse input
	void UpdateCamera ()
	{
		// Horizontal rotation
		m_CameraYaw += Input.GetAxis("Mouse X") * m_MouseSensitivity;

		// Vertical rotation
		if (m_MouseInvertY)
			m_CameraPitch -= Input.GetAxis("Mouse Y") * m_MouseSensitivity;
		else
			m_CameraPitch += Input.GetAxis("Mouse Y") * m_MouseSensitivity;

		// Prevent "over-rotating" the camera
		m_CameraPitch = Mathf.Clamp(m_CameraPitch, -89, 89);

		// Create a Quaternion rotation from Euler angles, based on our pitch and yaw values
		m_CameraPivot.rotation = Quaternion.Euler(m_CameraPitch, m_CameraYaw, 0);
	}

	// Called whenever another object starts colliding with this object
	void OnCollisionEnter (Collision collision)
	{
		// Increment our collision counter
		m_CollisionCounter++;
	}

	// Called whenever another object stops colliding with this object
	void OnCollisionExit (Collision collision)
	{
		// Decrement our collision counter
		m_CollisionCounter--;
	}
}
