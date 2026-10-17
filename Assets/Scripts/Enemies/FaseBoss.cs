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
// Desc: Transición a la pelea con el boss: aleja la cámara, cambia la música y lo hace aparecer.

using System.Collections;
using UnityEngine;

public class FaseBoss : MonoBehaviour
{
    [Tooltip("Desactivado en la escena, arriba de la pantalla. Aparece al terminar la transición.")]
    [SerializeField] private JefeCangrejo jefe;
    [Tooltip("Pausa después de matar al último alien, antes de que empiece el zoom.")]
    [SerializeField] private float esperaInicial = 1.5f;

    [Header("Cámara")]
    [SerializeField] private Camera camara;
    [Tooltip("Cuánto se aleja la cámara respecto de su tamaño normal. 1.15 = 15 % más lejos.")]
    [SerializeField] private float factorZoomBoss = 1.15f;
    [SerializeField] private float duracionZoom = 2f;
    [Tooltip("Se recalculan al terminar el zoom para que el jugador use el espacio nuevo.")]
    [SerializeField] private BordesMundo bordes;

    [Header("Música")]
    [SerializeField] private AudioSource musica;
    [Tooltip("Si queda vacío, sigue la música de siempre.")]
    [SerializeField] private AudioClip musicaBoss;

    private bool isIniciada;
    private System.Action alVencerJefe;

    private void Awake()
    {
        if (camara == null)
            camara = Camera.main;

        if (camara == null || bordes == null || jefe == null)
        {
            Debug.LogError("FaseBoss: falta asignar la cámara, los bordes o el jefe.");
            enabled = false;
        }
    }

    public void Iniciar(System.Action alVencerJefe)
    {
        if (isIniciada || !enabled)
            return;

        isIniciada = true;
        this.alVencerJefe = alVencerJefe;
        StartCoroutine(Transicion());
    }

    private IEnumerator Transicion()
    {
        yield return new WaitForSeconds(esperaInicial);

        if (musica != null && musicaBoss != null)
        {
            musica.resource = musicaBoss;
            musica.Play();
        }

        float tamanoInicial = camara.orthographicSize;
        float tamanoBoss = tamanoInicial * factorZoomBoss;
        for (float t = 0f; t < duracionZoom; t += Time.deltaTime)
        {
            camara.orthographicSize = Mathf.Lerp(tamanoInicial, tamanoBoss, Mathf.SmoothStep(0f, 1f, t / duracionZoom));
            yield return null;
        }

        camara.orthographicSize = tamanoBoss;
        bordes.Recalcular();
        jefe.Aparecer(alVencerJefe);
    }
}
