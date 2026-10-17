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
// Desc: Reproduce los efectos de sonido sin que el mismo clip se apile muchas veces a la vez.

using System.Collections.Generic;
using UnityEngine;

// Va en un objeto de la escena con un AudioSource 2D. La música usa su propio AudioSource.
public class EfectosSonido : MonoBehaviour
{
    [Tooltip("Tiempo mínimo entre dos veces el mismo clip. Si diez balas pegan en el mismo frame suena una sola, en vez de sumarse y saturar.")]
    [SerializeField] private float separacionMinima = 0.05f;

    private static EfectosSonido instancia;
    private AudioSource fuente;
    private readonly Dictionary<AudioClip, float> ultimaVez = new Dictionary<AudioClip, float>();

    private void Awake()
    {
        fuente = GetComponent<AudioSource>();
        if (fuente == null)
        {
            Debug.LogError("EfectosSonido: falta el AudioSource.");
            enabled = false;
            return;
        }

        instancia = this;
    }

    // Sin EfectosSonido en la escena (o sin clip asignado) no suena nada.
    public static void Reproducir(AudioClip clip)
    {
        if (clip == null || instancia == null || !instancia.enabled)
            return;

        if (instancia.Anotar(clip, Time.unscaledTime))
            instancia.fuente.PlayOneShot(clip, VolumenAudio.Efectos);
    }

    // Devuelve si el clip puede sonar ahora y, si puede, guarda la hora.
    private bool Anotar(AudioClip clip, float ahora)
    {
        if (ultimaVez.TryGetValue(clip, out float ultima) && ahora - ultima < separacionMinima)
            return false;

        ultimaVez[clip] = ahora;
        return true;
    }
}
