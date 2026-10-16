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
// Desc: Bala del alien: baja y le quita vida al jugador cuando lo toca.

using UnityEngine;

public class BalaEnemiga : MonoBehaviour
{
	[SerializeField] private float velocidad = 5f;
	// Todo golpe enemigo le saca una ficha al jugador. Es const para que ningún valor viejo del Inspector lo cambie.
	private const int danoAlJugador = 1;

	[Header("Balanceo (copo de nieve)")]
	[SerializeField] private float amplitudBalanceo = 0f; // Velocidad lateral máxima; 0 = cae derecho
	[SerializeField] private float frecuenciaBalanceo = 3f;

	private float faseBalanceo;

	// Reparte las balas en abanico, de -anguloCono/2 a +anguloCono/2 alrededor de la rotación dada.
	public static void DispararAbanico(GameObject prefab, Vector3 posicion, Quaternion rotacion, int cantidad, float anguloCono)
	{
		for (int i = 0; i < cantidad; i++)
			Instantiate(prefab, posicion, rotacion * Quaternion.Euler(0f, 0f, AnguloAbanico(i, cantidad, anguloCono)));
	}

	public static float AnguloAbanico(int indice, int cantidad, float anguloCono) =>
		cantidad > 1 ? Mathf.Lerp(-anguloCono / 2f, anguloCono / 2f, indice / (float)(cantidad - 1)) : 0f;

	private void Start()
	{
		faseBalanceo = Random.value * Mathf.PI * 2f;
	}

	private void Update()
	{
		float lateral = Mathf.Cos(Time.time * frecuenciaBalanceo + faseBalanceo) * amplitudBalanceo;
		transform.Translate(new Vector2(lateral, -velocidad) * Time.deltaTime);

		if (Pantalla.EstaFuera(transform.position))
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			VidaJugador vidaJugador = collision.GetComponent<VidaJugador>();
			if (vidaJugador != null)
			{
				vidaJugador.actualizarVida(-danoAlJugador);
			}

			Destroy(gameObject);
		}
	}
}
