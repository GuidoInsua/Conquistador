// ##########################################################################
// ## o ########## o ##                                                     #
// ### \ ######## / ###                                                     #
// #### \ ###### / ####       CONQUISTADOR                                  #
// ##### :-'""'-: #####       ------------                                  #
// ## .-'  ____  `-. ##       Autor:        Guido Insua                     #
// # ( (  (_()_)  ) ) #       Materia:      Programación 1                  #
// ## `-.   ^^   .-' ##       Cuatrimestre: Segundo cuatrimestre 2026       #
// ##### `._==_.' #####                                                     #
// ###### __)(___ #####                                                     #
// ##########################################################################

using UnityEngine;
using UnityEngine.InputSystem;

public class DispararJugador : MonoBehaviour
{
	[SerializeField] Transform posicionArma;
	[SerializeField] Bala balaPrefab;
	[SerializeField] float tiempoEntreDisparos = 0.15f;

	private float proximoDisparo;
	private PlayerInput playerInput;
	private InputAction shootAction;

	void Awake()
	{
		if (posicionArma == null)
		{
			posicionArma = transform.Find("arma");
		}

		playerInput = GetComponent<PlayerInput>();

		shootAction = playerInput != null ? playerInput.actions?.FindAction("Shoot", throwIfNotFound: false) : null;

		if (shootAction == null)
		{
			Debug.LogError("DispararJugador: falta el PlayerInput o la acción 'Shoot'.");
		}

		if (posicionArma == null || balaPrefab == null)
		{
			Debug.LogError("DispararJugador: falta asignar el hijo 'arma' o el prefab de la bala.");
			shootAction = null;
		}
	}

	void OnEnable()
	{
		if (shootAction != null)
		{
			shootAction.performed += OnShootPerformed;
		}
	}

	void OnDisable()
	{
		if (shootAction != null)
		{
			shootAction.performed -= OnShootPerformed;
		}
	}

	private void OnShootPerformed(InputAction.CallbackContext context)
	{
		Disparar();
	}

	private void Disparar()
	{
		if (Time.time < proximoDisparo)
			return;

		proximoDisparo = Time.time + tiempoEntreDisparos;
		Instantiate(balaPrefab, posicionArma.position, Quaternion.identity);
	}
}

