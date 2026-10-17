// ##########################################################################
// ## o ########## o ##                                                     #
// ### \ ######## / ###                                                     #
// #### \ ###### / ####       CONQUISTADOR                                  #
// ##### :-'""'-: #####       ------------                                  #
// ## .-'  ____  `-. ##       Autor:   Guido Insua                          #
// # ( (  (_()_)  ) ) #       Mail:    insua.guido@gmail.com                #
// ## `-.   ^^   .-' ##       Materia: Programacion 1 2do Cuatri 2026       #
// ##### `._==_.' #####                                                     #
// ###### __)(___ #####                                                     #
// ##########################################################################
// Desc: Mueve la nave según el input, más lento mientras se inclina, y aplica el boost de velocidad.

using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
	[Header("Componentes")]
	[SerializeField] Rigidbody2D rigidBody;
	[SerializeField] AnimacionJugador animacion;

	[Header("Movimiento")]
	[SerializeField] float speed = 3f;

	[SerializeField, Range(0.1f, 1f)]
	float transitionSpeedMultiplier = 0.75f;

	private InputAction moveAction;

	// Boost de velocidad
	private float speedMultiplier = 1f;
	private float boostEndTime = 0f;

	void Awake()
	{
		if (rigidBody == null)
		{
			rigidBody = GetComponent<Rigidbody2D>();
		}

		if (animacion == null)
		{
			animacion = GetComponent<AnimacionJugador>();
		}

		PlayerInput playerInput = GetComponent<PlayerInput>();
		// Busca la acción "Move" del Input System. Si falta algo queda en null, y abajo se avisa.
		// throwIfNotFound: false hace que devuelva null en vez de tirar un error si no la encuentra.
		moveAction = null;
		if (playerInput != null && playerInput.actions != null)
		{
			moveAction = playerInput.actions.FindAction("Move", throwIfNotFound: false);
		}

		if (rigidBody == null || animacion == null || moveAction == null)
		{
			Debug.LogError("MovimientoJugador: falta el Rigidbody2D, AnimacionJugador o la acción 'Move' del PlayerInput.");
			enabled = false;
		}
	}

	void FixedUpdate()
	{
		if (Time.time >= boostEndTime && speedMultiplier != 1f)
		{
			speedMultiplier = 1f;
		}

		Vector2 input = moveAction.ReadValue<Vector2>();
		animacion.ActualizarDireccion(input);

		// Velocidad reducida mientras la nave cambia de inclinación.
		float multiplicadorInclinacion = 1f;
		if (animacion.EstaInclinando)
		{
			multiplicadorInclinacion = transitionSpeedMultiplier;
		}

		rigidBody.linearVelocity = input * speed * speedMultiplier * multiplicadorInclinacion;
	}

	public void ApplySpeedBoost(float boostPercentage, float duration)
	{
		speedMultiplier = 1f + boostPercentage;
		boostEndTime = Time.time + duration;
	}
}
