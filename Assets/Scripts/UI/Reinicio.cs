using UnityEngine;
using UnityEngine.SceneManagement;

public class Reinicio : MonoBehaviour
{
	public void ReiniciarEscena()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void salirDelJuego()
	{
		Application.Quit();
	}

	public void abrirMenu()
	{
		SceneManager.LoadScene("Menu");
	}
}
