using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Playe_Controller : NetworkBehaviour
{
	IInput input;
	Player_Movement movement;

	private void OnEnable()
	{
		if (IsClient && IsOwner)
        {
			input = GetComponent<IInput>();
			movement = GetComponent<Player_Movement>();
			input.OnMovementDirectionInput += movement.HandleMovementDirection;
			input.OnMovementInput += movement.HandleMovement;
        }
	}

	private void OnDisable()
	{
		if (IsClient && IsOwner)
		{
			input.OnMovementDirectionInput -= movement.HandleMovementDirection;
			input.OnMovementInput -= movement.HandleMovement;
		}
	}
}
