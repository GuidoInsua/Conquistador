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
// Desc: Panel de opciones: un slider para la música y otro para los efectos.

using UnityEngine;
using UnityEngine.UI;

public class OpcionesAudio : MonoBehaviour
{
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderEfectos;

    private void Awake()
    {
        if (sliderMusica == null || sliderEfectos == null)
        {
            Debug.LogError("OpcionesAudio: falta asignar los sliders.");
            enabled = false;
            return;
        }

        // Cada vez que se mueve un slider, se llama al método que guarda ese volumen.
        sliderMusica.onValueChanged.AddListener(CambiarVolumenMusica);
        sliderEfectos.onValueChanged.AddListener(CambiarVolumenEfectos);
    }

    private void CambiarVolumenMusica(float valor)
    {
        VolumenAudio.Musica = valor;
    }

    private void CambiarVolumenEfectos(float valor)
    {
        VolumenAudio.Efectos = valor;
    }

    // Cada vez que se abre muestra lo guardado.
    private void OnEnable()
    {
        if (!enabled)
        {
            return;
        }

        sliderMusica.SetValueWithoutNotify(VolumenAudio.Musica);
        sliderEfectos.SetValueWithoutNotify(VolumenAudio.Efectos);
    }
}
