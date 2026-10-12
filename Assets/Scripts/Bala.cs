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
		if (SalidoDePatalla())
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

	private bool SalidoDePatalla()
	{
		Camera mainCamera = Camera.main;
		if (mainCamera == null)
			return false;

		Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

		return screenPos.x < -50 || screenPos.x > Screen.width + 50 ||
			   screenPos.y < -50 || screenPos.y > Screen.height + 50;
	}

	public float GetDano()
	{
		return dano;
	}
}
