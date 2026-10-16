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
using UnityEngine.Rendering.Universal;

public class FaseBoss : MonoBehaviour
{
    [Tooltip("Desactivado en la escena, arriba de la pantalla. Aparece al terminar la transición.")]
    [SerializeField] private JefeCangrejo jefe;
    [Tooltip("Pausa después de matar al último alien, antes de que empiece el zoom.")]
    [SerializeField] private float esperaInicial = 1.5f;

    [Header("Cámara")]
    [SerializeField] private Camera camara;
    [Tooltip("Tamaño ortográfico durante la pelea. Más grande = más lejos.")]
    [SerializeField] private float tamanoCamaraBoss = 7f;
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

        // El Pixel Perfect Camera pisa el tamaño de la cámara cada frame y solo acerca en pasos enteros: sin apagarlo no hay zoom.
        if (camara.TryGetComponent(out PixelPerfectCamera pixelPerfect))
            pixelPerfect.enabled = false;

        float tamanoInicial = camara.orthographicSize;
        for (float t = 0f; t < duracionZoom; t += Time.deltaTime)
        {
            camara.orthographicSize = Mathf.Lerp(tamanoInicial, tamanoCamaraBoss, Mathf.SmoothStep(0f, 1f, t / duracionZoom));
            yield return null;
        }

        camara.orthographicSize = tamanoCamaraBoss;
        bordes.Recalcular();
        jefe.Aparecer(alVencerJefe);
    }
}
