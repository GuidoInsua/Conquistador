using TMPro;
using UnityEngine;

public class VidaJugador : MonoBehaviour
{
	[SerializeField] private int vidaMaxima = 10;
	[SerializeField] private TMP_Text textoVida;
	[SerializeField] private GameObject cnavasMuerte;

	private int vidaActual;

	void Awake()
	{
		vidaActual = vidaMaxima;
		actualizarTextoVida();
	}

	private void actualizarTextoVida()
	{
		textoVida.text = "HP " + vidaActual.ToString();
	}

	public void actualizarVida(int cantidad)
	{
		vidaActual += cantidad;
		vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
		actualizarTextoVida();

		if (vidaActual <= 0)
		{
			Destroy(gameObject);
			cnavasMuerte.SetActive(true);
		}
	}
}
