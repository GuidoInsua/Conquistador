using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
	public void salirDelJuego()
	{
		Application.Quit();
	}

	public void abrirJuego()
	{
		SceneManager.LoadScene("Juego");
	}
}
