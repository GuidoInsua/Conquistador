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
// Desc: Mueve la nave según el input y aplica el boost de velocidad.

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
			rigidBody = GetComponent<Rigidbody2D>();

		if (animacion == null)
			animacion = GetComponent<AnimacionJugador>();

		PlayerInput playerInput = GetComponent<PlayerInput>();
		moveAction = playerInput != null ? playerInput.actions?.FindAction("Move", throwIfNotFound: false) : null;

		if (rigidBody == null || animacion == null || moveAction == null)
		{
			Debug.LogError("MovimientoJugador: falta el Rigidbody2D, AnimacionJugador o la acción 'Move' del PlayerInput.");
			enabled = false;
		}
	}

	void FixedUpdate()
	{
		if (Time.time >= boostEndTime && speedMultiplier != 1f)
			speedMultiplier = 1f;

		Vector2 input = moveAction.ReadValue<Vector2>();
		animacion.ActualizarDireccion(input);

		// Velocidad reducida mientras la nave cambia de inclinación.
		float multiplicadorInclinacion = animacion.EstaInclinando ? transitionSpeedMultiplier : 1f;

		rigidBody.linearVelocity = input * speed * speedMultiplier * multiplicadorInclinacion;
	}

	public void ApplySpeedBoost(float boostPercentage, float duration)
	{
		speedMultiplier = 1f + boostPercentage;
		boostEndTime = Time.time + duration;
	}
}
