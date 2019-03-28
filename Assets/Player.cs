using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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

	// The sensitivity of camera movement
	[SerializeField]
	float		m_MouseSensitivity = 1;
	// Whether to invert the y-axis of the camera or not
	[SerializeField]
	bool		m_MouseInvertY = true;

	// The current velocity of the player object
	Vector3		m_Velocity;
	// The current camera rotation values
	float		m_CameraPitch;
	float		m_CameraYaw;

    // Start is called before the first frame update
    void Start ()
    {
		// Lock the mouse cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

    // Update is called once per frame
    void Update ()
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

		// This is where we modify the current velocity of the player object.
		// The Vector3.Lerp function is used to smoothly change the velocity over time, based on either the acceleration or deceleration value.
		// We determine if we are accelerating or decelerating by checking if the input vector is almost zero.
		// The reason we are not doing "if (input.magnitude == 0f)" is because of floating point precision.
		if (input.magnitude < Mathf.Epsilon)
			m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, m_Deceleration * Time.deltaTime);
		else
			m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, m_Acceleration * Time.deltaTime);

		// Finally, we apply the movement to the player object position.
		// We need to take into account the amount of time between Updates, which is why we multiple with Time.deltaTime
		// to get the unit m/s, which is the unit of speed.
		transform.position += m_Velocity * Time.deltaTime;
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
}
