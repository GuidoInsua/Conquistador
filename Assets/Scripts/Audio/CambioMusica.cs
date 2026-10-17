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
// Desc: Cambia la música al activarse su objeto, como la pantalla de victoria o la de derrota.

using UnityEngine;

public class CambioMusica : MonoBehaviour
{
    [Tooltip("El AudioSource de la música de la escena.")]
    [SerializeField] private AudioSource musica;
    [SerializeField] private AudioClip clip;

    private void Awake()
    {
        if (musica == null || clip == null)
        {
            Debug.LogError("CambioMusica: falta el AudioSource de la música o el clip.");
            enabled = false;
        }
    }

    // Las pantallas empiezan apagadas: esto corre cuando se muestran.
    private void OnEnable()
    {
        musica.resource = clip;
        musica.Play();
    }
}
