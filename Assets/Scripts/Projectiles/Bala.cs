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
// Desc: Bala del jugador: sube y daña al enemigo que toca.

using UnityEngine;

public class Bala : MonoBehaviour
{
	[SerializeField] float velocidad = 10f;
	[SerializeField] float dano = 10f;

	private new Rigidbody2D rigidbody2D;
	private bool colisiono = false;

	void Start()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();

		if (rigidbody2D == null)
		{
			Debug.LogError("Bala requiere un Rigidbody2D component");
			Destroy(gameObject);
			return;
		}

		// Disparar hacia arriba (Y positivo)
		rigidbody2D.linearVelocity = Vector2.up * velocidad;
	}

	void Update()
	{
		// Solo verificar si salió de pantalla
		if (Pantalla.EstaFuera(transform.position))
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (colisiono)
			return;

		if (collision.CompareTag("Enemy"))
		{
			colisiono = true;

			IDamageable damageable = collision.GetComponent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(dano);
			}

			Destroy(gameObject);
		}
	}

}
