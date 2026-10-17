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
// Desc: En pantallas más angostas que 16:9 sube la altura de la cámara para que siempre entre todo el ancho del juego.

using UnityEngine;

public class CamaraAjustada : MonoBehaviour
{
    [Tooltip("Ancho de mundo que siempre se tiene que ver. 22.4 es lo que entra en 16:9 con altura 6.3.")]
    [SerializeField] private float anchoMinimo = 22.4f;

    // En Awake, antes de que las paredes y el spawner lean el tamaño de la cámara.
    private void Awake()
    {
        Camera camara = GetComponent<Camera>();
        if (camara == null)
        {
            Debug.LogError("CamaraAjustada: falta la Camera.");
            enabled = false;
            return;
        }

        camara.orthographicSize = Altura(camara.orthographicSize, anchoMinimo, camara.aspect);
    }

    // En 16:9 o más ancho devuelve la altura de siempre; más angosto, la sube lo justo para el ancho mínimo.
    public static float Altura(float alturaBase, float anchoMinimo, float aspecto)
    {
        // Altura que hace falta para que entre el ancho mínimo con esta forma de pantalla.
        float alturaParaElAncho = anchoMinimo / 2f / aspecto;

        // Se queda con la más grande de las dos.
        return Mathf.Max(alturaBase, alturaParaElAncho);
    }
}
