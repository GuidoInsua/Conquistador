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
	[SerializeField] Sprite spritePotenciada; // Se usa cuando la bala viene del power-up rojo

	private new Rigidbody2D rigidbody2D;
	private bool colisiono = false;

	// Lo llama DispararJugador justo después de crear la bala, antes del primer frame.
	public void Potenciar(float multiplicadorDano, float multiplicadorAncho)
	{
		dano *= multiplicadorDano;

		Vector3 escala = transform.localScale;
		escala.x *= multiplicadorAncho;
		transform.localScale = escala;

		if (spritePotenciada != null && TryGetComponent(out SpriteRenderer sr))
			sr.sprite = spritePotenciada;
	}

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
