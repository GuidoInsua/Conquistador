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

	private void Update()
	{
		transform.Translate(Vector2.down * velocidad * Time.deltaTime);

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
