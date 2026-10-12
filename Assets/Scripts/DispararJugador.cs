using UnityEngine;
using UnityEngine.InputSystem;

public class DispararJugador : MonoBehaviour
{
	[SerializeField] Transform posicionArma;
	[SerializeField] Bala balaPrefab;

	private PlayerInput playerInput;
	private InputAction shootAction;

	void Awake()
	{
		if (posicionArma == null)
		{
			posicionArma = transform.Find("arma");
		}

		playerInput = GetComponent<PlayerInput>();

		shootAction = playerInput.actions["Shoot"];
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
		Bala nuevaBala = Instantiate(balaPrefab, posicionArma.position, Quaternion.identity);
	}
}

