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
// Desc: Aplica el volumen de música elegido en opciones sobre el volumen propio de su AudioSource.

using UnityEngine;

public class VolumenMusica : MonoBehaviour
{
    private AudioSource fuente;
    private float volumenBase;

    private void Awake()
    {
        fuente = GetComponent<AudioSource>();
        if (fuente == null)
        {
            Debug.LogError("VolumenMusica: falta el AudioSource.");
            enabled = false;
            return;
        }

        // El volumen del Inspector es la mezcla de esa canción; opciones lo escala.
        volumenBase = fuente.volume;
    }

    private void OnEnable()
    {
        VolumenAudio.Cambio += Aplicar;
        Aplicar();
    }

    private void OnDisable() => VolumenAudio.Cambio -= Aplicar;

    private void Aplicar() => fuente.volume = volumenBase * VolumenAudio.Musica;
}
