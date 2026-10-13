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
// Desc: Dispara las balas del jugador con la acción Shoot, con un tiempo mínimo entre disparos.

using UnityEngine;
using UnityEngine.InputSystem;

public class DispararJugador : MonoBehaviour
{
	[SerializeField] Transform posicionArma;
	[SerializeField] Bala balaPrefab;
	[SerializeField] float tiempoEntreDisparos = 0.15f;

	[Header("Disparo doble (power-up)")]
	[SerializeField] float separacionDisparoDoble = 0.4f;

	private float proximoDisparo;

	// Mejoras temporales que dan los power-ups
	private float finBalasPotenciadas;
	private float multiplicadorAncho = 1f;
	private float multiplicadorDano = 1f;
	private float finDisparoDoble;
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

		if (Time.time < finDisparoDoble)
		{
			Vector3 mitadSeparacion = Vector3.right * (separacionDisparoDoble / 2f);
			CrearBala(posicionArma.position - mitadSeparacion);
			CrearBala(posicionArma.position + mitadSeparacion);
		}
		else
		{
			CrearBala(posicionArma.position);
		}
	}

	private void CrearBala(Vector3 posicion)
	{
		Bala bala = Instantiate(balaPrefab, posicion, Quaternion.identity);

		if (Time.time < finBalasPotenciadas)
			bala.Potenciar(multiplicadorDano, multiplicadorAncho);
	}

	public void ActivarBalasPotenciadas(float multAncho, float multDano, float duracion)
	{
		multiplicadorAncho = multAncho;
		multiplicadorDano = multDano;
		finBalasPotenciadas = Time.time + duracion;
	}

	public void ActivarDisparoDoble(float duracion)
	{
		finDisparoDoble = Time.time + duracion;
	}
}

