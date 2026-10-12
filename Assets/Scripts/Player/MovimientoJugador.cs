using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class MovimientoJugador : MonoBehaviour
{
	private enum MovementState
	{
		Idle,
		Left,
		Right,
		Up,
		UpLeft,
		UpRight
	}

	private enum TransitionState
	{
		Left,
		Right,
		UpLeft,
		UpRight
	}

	private enum HorizontalSide
	{
		None,
		Left,
		Right
	}

	[System.Serializable]
	private class SpriteMapping
	{
		public MovementState state;
		public Sprite sprite;
	}

	[System.Serializable]
	private class TransitionSpriteMapping
	{
		public TransitionState state;
		public Sprite sprite;
	}

	[Header("Componentes")]
	[SerializeField] Rigidbody2D rigidBody;
	[SerializeField] SpriteRenderer spriteRenderer;

	[Header("Movimiento")]
	[SerializeField] float speed = 3f;

	[Header("Sprites normales")]
	[SerializeField] SpriteMapping[] spriteMappings = new SpriteMapping[6];

	[Header("Sprites intermedios")]
	[SerializeField]
	TransitionSpriteMapping[] transitionSpriteMappings = new TransitionSpriteMapping[4];

	[Header("Animación de transición")]
	[SerializeField] float transitionDuration = 0.08f;

	[SerializeField, Range(0.1f, 1f)]
	float transitionSpeedMultiplier = 0.75f;

	[Header("Pausa en Idle al cambiar de lado")]
	[SerializeField] float idleTransitionDuration = 0.05f;

	private Vector2 movementInput;

	private MovementState currentState = MovementState.Idle;
	private MovementState previousState = MovementState.Idle;

	private Dictionary<MovementState, Sprite> stateToSprite;
	private Dictionary<TransitionState, Sprite> transitionToSprite;

	private Coroutine transitionCoroutine;

	private bool isTransitioning = false;

	// Boost de velocidad
	private float speedMultiplier = 1f;
	private float boostEndTime = 0f;

	void Awake()
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();

			if (spriteRenderer == null)
			{
				Debug.LogError(
					"No SpriteRenderer encontrado en este GameObject."
				);

				enabled = false;
				return;
			}
		}

		if (rigidBody == null)
		{
			rigidBody = GetComponent<Rigidbody2D>();

			if (rigidBody == null)
			{
				Debug.LogError(
					"No Rigidbody2D encontrado en este GameObject."
				);

				enabled = false;
				return;
			}
		}

		InitializeSprites();
	}


	void FixedUpdate()
	{
		// Comprobar boost
		if (Time.time >= boostEndTime && speedMultiplier != 1f)
		{
			speedMultiplier = 1f;
		}

		// Determinar estado según el input
		UpdateState();

		// Detectar cambios de estado
		if (currentState != previousState)
		{
			HandleStateChange();

			previousState = currentState;
		}

		// Velocidad normal o reducida durante la transición
		float currentSpeedMultiplier =
			isTransitioning
				? transitionSpeedMultiplier
				: 1f;

		rigidBody.linearVelocity =
			movementInput *
			speed *
			speedMultiplier *
			currentSpeedMultiplier;
	}


	void OnMove(InputValue value)
	{
		movementInput = value.Get<Vector2>();
	}


	private void UpdateState()
	{
		bool isMovingLeft = movementInput.x < -0.5f;
		bool isMovingRight = movementInput.x > 0.5f;

		// Hacia abajo solo se ve la inclinación lateral: S solo es Idle, S+A es Left, S+D es Right.
		if (movementInput.y > 0.5f)
		{
			currentState = isMovingLeft ? MovementState.UpLeft
				: isMovingRight ? MovementState.UpRight
				: MovementState.Up;
		}
		else
		{
			currentState = isMovingLeft ? MovementState.Left
				: isMovingRight ? MovementState.Right
				: MovementState.Idle;
		}
	}


	private void HandleStateChange()
	{
		// Cancelar transición anterior
		if (transitionCoroutine != null)
		{
			StopCoroutine(transitionCoroutine);
			transitionCoroutine = null;
		}

		isTransitioning = false;

		// Si no hay transición visual necesaria
		if (!NeedsTransition(previousState, currentState))
		{
			UpdateNormalSprite();
			return;
		}

		transitionCoroutine =
			StartCoroutine(
				PlayTransition(previousState, currentState)
			);
	}


	private bool NeedsTransition(
		MovementState from,
		MovementState to
	)
	{
		// Hay transición visual si alguno de los dos estados tiene inclinación lateral
		return IsLateralState(from) || IsLateralState(to);
	}


	private IEnumerator PlayTransition(
		MovementState from,
		MovementState to
	)
	{
		isTransitioning = true;

		HorizontalSide fromSide = GetHorizontalSide(from);
		HorizontalSide toSide = GetHorizontalSide(to);

		if (fromSide != HorizontalSide.None)
		{
			// Ej:
			// Left -> leftIntermedio
			// UpLeft -> upLeftIntermedio
			SetTransitionSprite(from);

			yield return new WaitForSeconds(
				transitionDuration
			);
		}

		bool changesSide =
			fromSide != HorizontalSide.None &&
			toSide != HorizontalSide.None &&
			fromSide != toSide;

		if (changesSide)
		{
			// Ej:
			// Left -> Idle
			SetNormalSprite(MovementState.Idle);

			yield return new WaitForSeconds(
				idleTransitionDuration
			);
		}
		else if (
			fromSide != HorizontalSide.None &&
			toSide == HorizontalSide.None
		)
		{
			// Ej:
			// Left -> Idle
			// UpLeft -> Up

			SetNormalSprite(MovementState.Idle);

			yield return new WaitForSeconds(
				idleTransitionDuration
			);
		}


		if (toSide != HorizontalSide.None)
		{
			// Ej:
			// Idle -> leftIntermedio
			// Idle -> upRightIntermedio

			SetTransitionSprite(to);

			yield return new WaitForSeconds(
				transitionDuration
			);
		}


		if (currentState == to)
		{
			SetNormalSprite(to);
		}

		isTransitioning = false;
		transitionCoroutine = null;
	}


	private bool IsLateralState(MovementState state)
	{
		return
			state == MovementState.Left ||
			state == MovementState.Right ||
			state == MovementState.UpLeft ||
			state == MovementState.UpRight;
	}


	private HorizontalSide GetHorizontalSide(
		MovementState state
	)
	{
		switch (state)
		{
			case MovementState.Left:
			case MovementState.UpLeft:
				return HorizontalSide.Left;

			case MovementState.Right:
			case MovementState.UpRight:
				return HorizontalSide.Right;

			default:
				return HorizontalSide.None;
		}
	}


	private TransitionState GetTransitionState(
		MovementState state
	)
	{
		switch (state)
		{
			case MovementState.Left:
				return TransitionState.Left;

			case MovementState.Right:
				return TransitionState.Right;

			case MovementState.UpLeft:
				return TransitionState.UpLeft;

			case MovementState.UpRight:
				return TransitionState.UpRight;

			default:
				return TransitionState.Left;
		}
	}


	private void SetNormalSprite(
		MovementState state
	)
	{
		if (spriteRenderer == null)
			return;

		if (
			stateToSprite.TryGetValue(
				state,
				out Sprite sprite
			)
		)
		{
			if (sprite != null)
			{
				spriteRenderer.sprite = sprite;
			}
		}
	}


	private void UpdateNormalSprite()
	{
		SetNormalSprite(currentState);
	}


	private void SetTransitionSprite(
		MovementState state
	)
	{
		if (spriteRenderer == null)
			return;

		TransitionState transitionState =
			GetTransitionState(state);

		if (
			transitionToSprite.TryGetValue(
				transitionState,
				out Sprite sprite
			)
		)
		{
			if (sprite != null)
			{
				spriteRenderer.sprite = sprite;
			}
		}
	}


	private void InitializeSprites()
	{
		stateToSprite =
			new Dictionary<MovementState, Sprite>();

		foreach (SpriteMapping mapping in spriteMappings)
		{
			if (
				mapping != null &&
				mapping.sprite != null
			)
			{
				stateToSprite[mapping.state] =
					mapping.sprite;
			}
		}


		transitionToSprite =
			new Dictionary<TransitionState, Sprite>();

		foreach (
			TransitionSpriteMapping mapping
			in transitionSpriteMappings
		)
		{
			if (
				mapping != null &&
				mapping.sprite != null
			)
			{
				transitionToSprite[mapping.state] =
					mapping.sprite;
			}
		}
	}


	public void ApplySpeedBoost(
		float boostPercentage,
		float duration
	)
	{
		speedMultiplier =
			1f + boostPercentage;

		boostEndTime =
			Time.time + duration;
	}
}